using System;
using System.Collections;
using System.Security.Principal;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;

namespace PMS.Collections.Pool
{
    public abstract class ObjectPool : MarshalByRefObject, IObjectPool
    {
		protected static readonly log4net.ILog log =
			            log4net.LogManager.GetLogger("PMS.Collections.Pool.ObjectPool");

		public const int THRESHOLD = 400;
		public const int MINIMUM = 10;

        private int min = 0;
        private int max = 0;
        private MethodInfo cleanup;
        private Type type = null;
        private object[] typeParams;

		protected event EventHandler ZombieHandler;
        protected object ilock = 0;
        protected ItemCollection pool = null;
		protected System.Timers.Timer zTimer = null;
		protected readonly bool verbose = false;


        #region Constructors
		public ObjectPool(Type type, string sFree) :
			this(type, null, MINIMUM, THRESHOLD, sFree, true)
		{
		}
        

        public ObjectPool(Type type, int min, int max, string sFree) :
			this(type, null, min, max, sFree, true)
        {
        }

        public ObjectPool(Type type, int min, int max, string sFree, bool zombie) :
			this(type, null, min, max, sFree, zombie)
		{
		}

        public ObjectPool(Type type, object[] typeParams, string sFree) :
			this(type, typeParams, MINIMUM, THRESHOLD, sFree, true)
		{
		}

        public ObjectPool(Type type, object[] typeParams, int min, int max, string sFree, bool zombie)
        {
            this.type = type;
            this.min = min;
            this.max = max;
            this.typeParams = typeParams;
            this.pool = new ItemCollection();
            this.cleanup = type.GetMethod(sFree);
			this.ZombieHandler += new EventHandler(ZombieMaster);

			this.zTimer = new System.Timers.Timer();
			this.zTimer.Enabled = zombie;
			this.zTimer.Interval = 120000;
			this.zTimer.AutoReset = true;
			this.zTimer.Elapsed += new ElapsedEventHandler(ZombieMaster);

			//verbose = (Environment.GetEnvironmentVariable("PMS_VERBOSE") != null)? true : false;
            verbose = true;
        }

        ~ObjectPool()
        {
            Close();
        } 
        #endregion

        #region Abstract

        protected virtual long Add()
        {
            Item obj = null;

            lock (ilock) {
				if (this.typeParams == null)
					this.pool.Add((obj = new Item(Activator.CreateInstance(type))));
				else
					this.pool.Add((obj = new Item(Activator.CreateInstance(type, typeParams))));
            }

            if (log.IsDebugEnabled)
                log.DebugFormat("ObjectPool.Add(new {0}())", obj.Object.GetType());

			return pool.Count;
        }

		[MethodImpl(MethodImplOptions.Synchronized)]
        public abstract object Borrow();

		[MethodImpl(MethodImplOptions.Synchronized)]
        public abstract bool Return(object obj);

		[MethodImpl(MethodImplOptions.Synchronized)]
        public virtual bool Remove(object obj)
		{
			int code = obj.GetHashCode();

			lock (ilock) {
				for (int x = 0; x < pool.Count; x++) {
					if (pool[x].Object.GetHashCode() == code) {
						CleanObject(ref pool[x].Object);
						pool.RemoveAt(x);
						return true;
					}
				}
			}

			return false;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
        public virtual bool Open()
        {
			for (int i = pool.Count; i < this.Min; i++) {
				this.Add();
			}

            return true;
        }

		[MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Close()
        {
            if (log.IsDebugEnabled)
                log.Debug("ManagedObjectPool.Close()");

            foreach (Item item in this.pool) {
                CleanObject(ref item.Object);
            }

            pool.Clear();
        }

		[MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void CleanObject(ref Object obj)
        {
            try {
                if (cleanup != null) {
					cleanup.Invoke(obj, null);

                    if (log.IsDebugEnabled) {
                        log.DebugFormat("{0}.{1}(OID={2})", type, cleanup, obj.GetHashCode());
					}
                }
            } finally {
                obj = null;
            }
        }

		[MethodImpl(MethodImplOptions.Synchronized)]
		protected void ZombieMaster(object sender, EventArgs e)
		{
			ZombieKiller(30);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		protected void ZombieMaster(object sender, ElapsedEventArgs e)
		{
			ZombieKiller(30);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		protected void ZombieKiller(int minutes)
		{
			lock (ilock) {
				for (int x = 0; x < this.pool.Count ; x++) {
					if (pool[x].LastUsed.AddMinutes(minutes) <= DateTime.Now) {
						CleanObject(ref pool[x].Object);
						pool.RemoveAt(x);
					}
				}
			}
		}

        #region Properties
        public int Count
        {
            get { return pool.Count; }
        }

        public int Min
        {
            get { return min; }
        }

        public int Max
        {
            get { return max; }
        }
        #endregion

        #endregion // Abstract
    }
}
