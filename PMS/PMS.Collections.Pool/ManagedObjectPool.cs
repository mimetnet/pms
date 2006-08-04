using System;
using System.Collections;
using System.Security.Principal;
using System.Reflection;
using System.Threading;

namespace PMS.Collections.Pool
{
    public class ManagedObjectPool : MarshalByRefObject, IObjectPool
    {
        protected static readonly log4net.ILog log =
            log4net.LogManager.GetLogger("PMS.Collections.Pool.ManagedObjectPool");

        private int min = 0;
        private int max = 0;
        private int index = -1;
        private string cleanup;
        private IList pool;
        private object ilock = 0;
        private Type type = null;

        #region Constructors
        
        public ManagedObjectPool(Type type, int min, int max, string sFree)
        {
            this.type = type;
            this.min = min;
            this.max = max;
            this.cleanup = sFree;
            this.pool = new ArrayList();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ManagedObjectPool()
        {
            Close();
        } 
        #endregion

        #region Abstract
        /// <summary>
        /// Add new instance of Type to pool
        /// </summary>
        /// <returns>status</returns>
        protected virtual bool Add()
        {
            Item obj = null;

            lock (ilock) {
                pool.Add((obj = new Item(Activator.CreateInstance(type))));
            }

            if (log.IsDebugEnabled)
                log.DebugFormat("ManagedObjectPool.Add(new {0}())", obj.Object.GetType());

            return true;
        }

        /// <summary>
        /// Borrow next object in pool
        /// </summary>
        /// <returns>next object in pool</returns>
        public virtual object Borrow()
        {
            lock (ilock) {
                index = (index != (this.Actual - 1)) ? index + 1 : 0;

                if (((Item)pool[index]).Available == true) {
                    ((Item)pool[index]).Available = false;

                    if (log.IsDebugEnabled)
                        log.DebugFormat("ManagedObjectPool.Borrow(OID={0} TID={1})",
                                        pool[index].GetHashCode(),
                                        Thread.CurrentThread.ManagedThreadId);

                    return ((Item)pool[index]).Object;
                }

                // nothing left, so add one more and recurse
                this.Add();
                return this.Borrow();
            }

            //throw new PoolEmptyException("Max Reached and No Objects Left");
        }

        /// <summary>
        /// Return object to beginning of pool
        /// </summary>
        /// <param name="obj">object to return</param>
        /// <returns>status</returns>
        public virtual bool Return(object obj)
        {
            int code = obj.GetHashCode();
            //Console.WriteLine("BEFORE");
            //this.Debug();

            lock (ilock) {
                for (int x = 0; x < pool.Count; x++) {
                    if (((Item)pool[x]).Object.GetHashCode() == code) {
                        ((Item)pool[x]).Available = true;

                        if (log.IsDebugEnabled)
                            log.DebugFormat("ManagedObjectPool.Return(OID={0} TID={1})",
                                            pool[index].GetHashCode(),
                                            Thread.CurrentThread.ManagedThreadId);

                        //Console.WriteLine("AFTER");
                        //this.Debug();
                        return true;
                    }
                }
            }

            //Console.WriteLine("NOT FOUND");
            return false;
        }

        /// <summary>
        /// Remove object from pool
        /// </summary>
        /// <param name="obj">object to remove</param>
        /// <returns>status</returns>
        public virtual bool Remove(object obj)
        {
            int code = obj.GetHashCode();

            lock (ilock) {
                for (int x = 0; x < pool.Count; x++) {
                    if (((Item)pool[x]).Object.GetHashCode() == code) {
                        CleanObject(ref ((Item)pool[x]).Object);
                        pool.RemoveAt(x);

                        if (log.IsDebugEnabled)
                            log.Debug("ManagedObjectPool.Remove(" + pool[index].GetHashCode() + ")");

                        return true;
                    }
                }
            }

            return false;
        }

        public bool Open()
        {
            string l = String.Empty;

            lock (l) {
                for (int i = 0; i < this.Min; i++) {
                    this.Add();
                }
            }

            return true;
        }

        /// <summary>
        /// Close pool, clean object if there is a specified sFree method
        /// </summary>
        public void Close()
        {
            if (log.IsDebugEnabled)
                log.Debug("ManagedObjectPool.Close()");

            foreach (Item item in this.pool) {
                CleanObject(ref item.Object);
            }

            pool.Clear();
        }

        /// <summary>
        /// Objects to clean are passed by reference and closing 
        /// method is invoked via reflection.
        /// </summary>
        /// <param name="obj">Element by ref to clean</param>
        public void CleanObject(ref Object obj)
        {
            Type objType = null;
            MethodInfo methInfo = null;

            try {
                if (cleanup != null) {
                    objType = obj.GetType();
                    methInfo = objType.GetMethod(cleanup);
                    methInfo.Invoke(obj, null);

                    if (log.IsDebugEnabled)
                        log.Debug(objType + "." + this.cleanup + "()");
                }
            } finally {
                obj = null;
                objType = null;
                methInfo = null;
            }
        }

        /// <summary>
        /// Returns the number of elements within the pool
        /// </summary>
        public int Count
        {
            get { return pool.Count; }
        }

        /// <summary>
        /// Returns how small the pool can be
        /// </summary>
        public int Min
        {
            get { return min; }
            set { min = value; }
        }

        /// <summary>
        /// Returns the actual size of Pool GE min AND LE max
        /// </summary>
        public int Actual
        {
            get { return pool.Count; }
        }

        /// <summary>
        /// Returns how large the pool can be
        /// </summary>
        public int Max
        {
            get { return max; }
            set { max = value; }
        }

        /// <summary>
        /// Returns the number of available elements within the pool
        /// </summary>
        public int Available
        {
            get {
                int y = 0;
                for (int x = 0; x < pool.Count; x++)
                    if (((Item)pool[x]).Available == true)
                        y++;

                return y;
            }
        }

        public void Debug()
        {
            Console.WriteLine("<ManagedObjectPool>");
            foreach (Item item in pool) {
                Console.WriteLine("\t<Item Avail={0} Hash={1} TID={2} />", item.Available, item.GetHashCode(), System.Threading.Thread.CurrentThread.ManagedThreadId);
            }
            Console.WriteLine("</ManagedObjectPool>");
        }
        #endregion // Abstract

        /// <summary>
        /// Wraps an object in the Pool
        /// </summary>
        protected class Item
        {
            /// <summary>
            /// Contains the element in the pool
            /// </summary>
            public object Object = null;

            /// <summary>
            /// Is the element available
            /// </summary>
            public bool Available = false;

            /// <summary>
            /// Identity of the wrapped object
            /// </summary>
            public IPrincipal Principal = null;

            /// <summary>
            /// Create Item to wrap obj (Available by default)
            /// </summary>
            /// <param name="obj">Element to wrap</param>
            public Item(Object obj) : this(obj, true)
            {
            }

            /// <summary>
            /// Create Item to wrap with availability settings
            /// </summary>
            /// <param name="obj">Element to Wrap</param>
            /// <param name="avail">Is it available</param>
            public Item(Object obj, bool avail)
            {
                Object = obj;
                Available = avail;
            }
        } 
    }
}
