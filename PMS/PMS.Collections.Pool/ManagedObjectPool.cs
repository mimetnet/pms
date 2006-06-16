using System;
using System.Collections;
using System.Reflection;

namespace PMS.Collections.Pool
{
    public class ManagedObjectPool
    {
        
        protected int min, max;
        protected int index = -1;
        protected string cleanup;
        protected ArrayList pool;

        public ManagedObjectPool(int _max)
        {
            max = _max;
            pool = new ArrayList();
        }

        public ManagedObjectPool(string user_cleanup)
        {
            min = 0;
            max = 5;
            cleanup = user_cleanup;
            pool = new ArrayList();
        }

        public ManagedObjectPool(int _max, string user_cleanup)
        {
            min = 0;
            max = _max;
            cleanup = user_cleanup;
            pool = new ArrayList();
        }
        
        ~ManagedObjectPool()
        {
            Close();
        }

        public bool Add(object obj)
        {
            pool.Add(new Item(obj, true));
            
            return true;
        }

        // index = (index != (max-1))? index + 1 : 0;

        public object Borrow()
        {
            index = (index != (max-1))? index + 1 : 0;

            Item item = (Item) pool[index];
            if (item.Available == true) {
                ((Item)pool[index]).Available = false;
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
                    return true;
                }
            }

            return false;
        }

        public void Close()
        {
            foreach (Item item in pool) {
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
