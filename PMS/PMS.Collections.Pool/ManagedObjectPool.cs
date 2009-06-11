using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PMS.Collections.Pool
{
    public class ManagedObjectPool<T> : IObjectPool<T>
    {
        private int min = 0;
        private int max = 0;
        private MethodInfo cleanup;
        private Type type = null;
        private Object[] typeParams;
        private Queue<T> queue = new Queue<T>();
        private Semaphore poolGate = null;
        private Object lockObject = new Object();

        public ManagedObjectPool(Type type, int min, int max, string sFree) :
			this(type, null, min, max, sFree)
        {
        }

		public ManagedObjectPool(Type type, object[] typeParams, int min, int max, string sFree)
		{
            this.type = type;
            this.typeParams = typeParams;
            this.min = min;
            this.max = max;
            this.cleanup = type.GetMethod(sFree);
            this.poolGate = new Semaphore(max, max);
		}

        ~ManagedObjectPool()
        {
            this.Close();
        } 

        public int Minimum {
            get { return this.min; }
        }

        public int Maximum {
            get { return this.max; }
        }

        public int Count {
            get { return this.queue.Count; }
        }

        public T Borrow()
        {
            if (!this.poolGate.WaitOne((60 * 1000), false))
                throw new ApplicationException("Timer expired before borrow could work");

            lock (this.lockObject) {
                if (this.queue.Count == 0)
                    this.Add();
                //Console.WriteLine("Borrow: " + queue.Peek().GetHashCode());
                return queue.Dequeue();
            }
		}

        public bool Return(T obj)
		{
            if (obj == null)
                return false;

			lock (this.lockObject) {
                if (this.queue.Contains(obj))
                    throw new Exception("Return: queue already has object??");

                //Console.WriteLine("Return: " + obj.GetHashCode());
                this.queue.Enqueue(obj);

                this.poolGate.Release();
            }

            return true;
		}

        public bool Remove(T obj)
        {
            throw new NotImplementedException();
        }

        protected virtual long Add()
        {
			if (this.typeParams == null)
				this.queue.Enqueue((T)Activator.CreateInstance(type));
			else
				this.queue.Enqueue((T)Activator.CreateInstance(type, typeParams));

            Console.WriteLine("ManagedObjectPool.Add(new {0}())", type.Name);

            //if (queue.Count > THRESHOLD) {
			//    ZombieKiller(30);
		    //}

		    return queue.Count;
        }

        public bool Open()
        {
            lock (this.lockObject) {
                Console.WriteLine("ManagedObjectPool.Open()");

                for (int x=this.queue.Count; x<this.min; x++) {
                    this.Add();
                }

                return (this.queue.Count == this.min);
            }
        }

        public void Close()
        {
            lock (this.lockObject) {
                Console.WriteLine("ManagedObjectPool.Close({0})", this.queue.Count);
                while (this.queue.Count > 0) {
                    Object o = this.queue.Dequeue();
                    
                    if (cleanup != null) {
					    cleanup.Invoke(o, null);
                    }
                }
            }
        }
	}
}
