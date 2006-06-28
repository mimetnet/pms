using System;
using System.Collections;
using System.Reflection;

namespace PMS.Collections.Pool
{
    public sealed class ManagedObjectPool
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private int max;
        private int index = -1;
        private string cleanup;
        private ArrayList pool;

        /// <summary>
        /// Construct while setting max number of items in pool
        /// </summary>
        /// <param name="max">Max Items in pool</param>
        public ManagedObjectPool(int max) : this(max, null)
        {
        }

        /// <summary>
        /// Construct while setting string representing Close()-like method
        /// </summary>
        /// <param name="sFree">Method called via reflection when removing object from pool</param>
        public ManagedObjectPool(string sFree) : this(5, sFree)
        {
        }

        /// <summary>
        /// Construct while setting max items and Close()-like method
        /// </summary>
        /// <param name="max">Max items in pool</param>
        /// <param name="sFree">Method called via reflection when removing object from pool</param>
        public ManagedObjectPool(int max, string sFree)
        {
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

        /// <summary>
        /// Add item to pool
        /// </summary>
        /// <param name="obj">object to add to pool</param>
        /// <returns>status</returns>
        public bool Add(object obj)
        {
            pool.Add(new Item(obj));

            if (log.IsDebugEnabled)
                log.Debug("ManagedObjectPool.Add(new {" + obj.GetType() + "}())");
            
            return true;
        }

        /// <summary>
        /// Borrow next object in pool
        /// </summary>
        /// <returns>next object in pool</returns>
        public object Borrow()
        {
            index = (index != (max-1))? index + 1 : 0;

            Item item = (Item) pool[index];
            if (item.Available == true) {
                ((Item)pool[index]).Available = false;

                if (log.IsDebugEnabled)
                    log.Debug("ManagedObjectPool.Borrow(" + pool[index].GetHashCode() + ")");

                return item.Object;
            }

            throw new PoolEmptyException("Empty");
        }

        /// <summary>
        /// Return object to beginning of pool
        /// </summary>
        /// <param name="obj">object to return</param>
        /// <returns>status</returns>
        public bool Return(object obj)
        {
            int code = obj.GetHashCode();
            for (int x=0; x < pool.Count; x++) {
                if (((Item)pool[x]).Object.GetHashCode() == code) {
                    ((Item)pool[x]).Available = true;

                    if (log.IsDebugEnabled)
                        log.Debug("ManagedObjectPool.Return(" + pool[index].GetHashCode() + ")");

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Remove object from pool
        /// </summary>
        /// <param name="obj">object to remove</param>
        /// <returns>status</returns>
        public bool Remove(object obj)
        {
            int code = obj.GetHashCode();
            for (int x=0; x < pool.Count; x++) {
                if (((Item)pool[x]).Object.GetHashCode() == code) {
                    CleanObject(ref ((Item)pool[x]).Object);
                    pool.RemoveAt(x);

                    if (log.IsDebugEnabled)
                        log.Debug("ManagedObjectPool.Remove(" + pool[index].GetHashCode() + ")");

                    return true;
                }
            }

            return false;
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
        /// Objects to clean are passed by reference and reflection is
        /// invoked 
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
        public int Count {
            get { return pool.Count; }
        }

        /// <summary>
        /// Returns how large the pool can be
        /// </summary>
        public int Max {
            get { return max; }
            set { max = value; }
        }

        /// <summary>
        /// Returns the number of available elements within the pool
        /// </summary>
        public int Available {
            get {
                int y=0;
                for (int x=0; x < pool.Count; x++)
                    if (((Item)pool[x]).Available == true)
                        y++;
                
                return y;    
            }
        }
        
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
