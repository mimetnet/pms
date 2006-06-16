using System;
using System.Collections;
using System.Reflection;

namespace PMS.Collections.Pool
{
    public class ManagedObjectPool
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected int min, max;
        protected int index = -1;
        protected string cleanup;
        protected ArrayList pool;

        public ManagedObjectPool(int max) : this(max, null)
        {
        }

        public ManagedObjectPool(string sFree) : this(5, sFree)
        {
        }

        public ManagedObjectPool(int max, string sFree)
        {
            this.min = 0;
            this.max = max;
            this.cleanup = sFree;
            this.pool = new ArrayList();
        }
        
        ~ManagedObjectPool()
        {
            Close();
        }

        public bool Add(object obj)
        {
            pool.Add(new Item(obj, true));

            if (log.IsDebugEnabled)
                log.Debug("ManagedObjectPool.Add(new {" + obj.GetType() + "}())");
            
            return true;
        }

        public object Borrow()
        {
            index = (index != (max-1))? index + 1 : 0;

            Item item = (Item) pool[index];
            if (item.Available == true) {
                ((Item)pool[index]).Available = false;
                //Console.WriteLine("ManagedObjectPool.Borrow({0})", 
                //                  pool[index].GetHashCode());
                return item.Object;
            }

            throw new PoolEmptyException("Empty");
        }

        public bool Return(object obj)
        {
            int code = obj.GetHashCode();
            for (int x=0; x < pool.Count; x++) {
                if (((Item)pool[x]).Object.GetHashCode() == code) {
                    ((Item)pool[x]).Available = true;
                    //Console.WriteLine("ManagedObjectPool.Return({0})",
                    //                  pool[index].GetHashCode());
                    return true;
                }
            }

            return false;
        }

        public bool Remove(object obj)
        {
            int code = obj.GetHashCode();
            for (int x=0; x < pool.Count; x++) {
                if (((Item)pool[x]).Object.GetHashCode() == code) {
                    CleanObject(ref ((Item)pool[x]).Object);
                    pool.RemoveAt(x);
                    //Console.WriteLine("ManagedObjectPool.Remove({0})",
                    //                  pool[index].GetHashCode());
                    return true;
                }
            }

            return false;
        }

        public void Close()
        {
            //Console.WriteLine("ManagedObjectPool.Close()");

            foreach (Item item in this.pool) {
                CleanObject(ref item.Object);
            }
            pool.Clear();
        }         

        protected void CleanObject(ref Object obj)
        {
            try {
                if (cleanup != null) {
                    Type objType = obj.GetType();
                    MethodInfo methInfo = objType.GetMethod(cleanup);
                    methInfo.Invoke(obj, null);

                    //Console.WriteLine(objType + "." + this.cleanup + "()");
                }
            } finally {
                obj = null;
            }
        }

        public int Count {
            get { return pool.Count; }
        }

        public int Max {
            get { return max; }
            set { max = value; }
        }

        public int Available {
            get {
                int y=0;
                for (int x=0; x < pool.Count; x++)
                    if (((Item)pool[x]).Available == true)
                        y++;
                
                return y;    
            }
        }

        public void Stats()
        {
            Console.WriteLine("\nPool Availability (" + Available + ")");
            Console.WriteLine("======================");
            foreach (Item item in pool) {
                Console.WriteLine(item.Available);
            }
        }
        
        protected class Item
        {
            public object Object = null;
            public bool Available = false;

            public Item(Object obj, bool avail)
            {
                Object = obj;
                Available = avail;
            }
        }
    }
}
