using System;
using System.Collections;
using System.Reflection;

namespace PMS.Collections.Pool
{
    public abstract class AbstractObjectPool : MarshalByRefObject, IObjectPool
    {
        protected static readonly log4net.ILog log = 
            log4net.LogManager.GetLogger("PMS.Collections.Pool.IObjectPool");
        
        protected int max;
        protected int index = -1;
        protected string cleanup;
        protected ArrayList pool;

        #region Constructors
        /// <summary>
        /// Construct while setting max number of items in pool
        /// </summary>
        /// <param name="max">Max Items in pool</param>
        public AbstractObjectPool(int max)
            : this(max, null)
        {
        }

        /// <summary>
        /// Construct while setting string representing Close()-like method
        /// </summary>
        /// <param name="sFree">Method called via reflection when removing object from pool</param>
        public AbstractObjectPool(string sFree)
            : this(5, sFree)
        {
        }

        /// <summary>
        /// Construct while setting max items and Close()-like method
        /// </summary>
        /// <param name="max">Max items in pool</param>
        /// <param name="sFree">Method called via reflection when removing object from pool</param>
        public AbstractObjectPool(int max, string sFree)
        {
            this.max = max;
            this.cleanup = sFree;
            this.pool = new ArrayList();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~AbstractObjectPool()
        {
            Close();
        } 
        #endregion

        #region NotImplemented
        public virtual bool Add(object obj)
        {
            throw new NotImplementedException();
        }

        public virtual object Borrow()
        {
            throw new NotImplementedException();
        }

        public virtual bool Remove(object obj)
        {
            throw new NotImplementedException();
        }

        public virtual bool Return(object obj)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Abstract
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
            get
            {
                int y = 0;
                for (int x = 0; x < pool.Count; x++)
                    if (((Item)pool[x]).Available == true)
                        y++;

                return y;
            }
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
            /// Create Item to wrap obj (Available by default)
            /// </summary>
            /// <param name="obj">Element to wrap</param>
            public Item(Object obj)
                : this(obj, true)
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
