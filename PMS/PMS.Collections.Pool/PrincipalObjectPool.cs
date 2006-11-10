using System;
using System.Collections;
using System.Security.Principal;
using System.Reflection;
using System.Threading;
using System.Timers;

namespace PMS.Collections.Pool
{
    public class PrincipalObjectPool : MarshalByRefObject, IObjectPool
    {
        protected static readonly log4net.ILog log =
            log4net.LogManager.GetLogger("PMS.Collections.Pool.PrincipalObjectPool");

        protected object ilock = 0;
		const int THRESHOLD = 400;
        private int min = 0;
        private int max = 0;
        private string cleanup;
        private ItemCollection pool = null;
        private Type type = null;
        private object[] typeParams;
		private event EventHandler ZombieHandler;
		private System.Timers.Timer zTimer = null;


        #region Constructors
        
        public PrincipalObjectPool(Type type, int min, int max, string sFree) :
			this(type, null, min, max, sFree)
        {
        }

        public PrincipalObjectPool(Type type, object[] typeParams, int min, int max, string sFree)
        {
            this.type = type;
            this.min = min;
            this.max = max;
            this.cleanup = sFree;
            this.pool = new ItemCollection();
            this.typeParams = typeParams;
			this.ZombieHandler += new EventHandler(ZombieMaster);

			this.zTimer = new System.Timers.Timer();
			this.zTimer.Enabled = true;
			this.zTimer.Interval = 60000;
			this.zTimer.AutoReset = true;
			this.zTimer.Elapsed += new ElapsedEventHandler(ZombieMaster);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~PrincipalObjectPool()
        {
            Close();
        } 
        #endregion

        #region Abstract

        /// <summary>
        /// Add new instance of Type to pool
        /// </summary>
        /// <returns>status</returns>
        protected virtual long Add()
        {
            Item obj = null;

            lock (ilock) {
                pool.Add((obj = new Item(Activator.CreateInstance(type, typeParams))));
            }

            if (log.IsDebugEnabled)
                log.DebugFormat("ManagedObjectPool.Add(new {0}())", obj.Object.GetType());

			return pool.Count;
        }

        /// <summary>
        /// Borrow next object in pool
        /// </summary>
        /// <returns>next object in pool</returns>
        public virtual object Borrow()
        {
            lock (ilock) {
                
                for (int x = 0; x < this.pool.Count ; x++) {

                    if (pool[x].Principal.Identity.Name == Thread.CurrentPrincipal.Identity.Name) {

                        if (log.IsDebugEnabled) {
                            log.DebugFormat("ManagedObjectPool.Borrow(OID={0} IDN={1})",
                                            pool[x].Object.GetHashCode(),
                                            Thread.CurrentPrincipal.Identity.Name);
						}

                        return pool[x].Checkout();
                    }
                }

                // nothing left, so add one more and recurse
                if (this.Add() > THRESHOLD) {
					this.ZombieHandler(this, EventArgs.Empty);
				}

                return this.Borrow();
            }

            //throw new PoolEmptyException("Max Reached and No Objects Left");
        }

		protected void ZombieMaster(object sender, EventArgs e)
		{
			ZombieKiller(30);
		}

		protected void ZombieMaster(object sender, ElapsedEventArgs e)
		{
			ZombieKiller(30);
		}

		protected void ZombieKiller(int minutes)
		{
			lock (ilock) {
				for (int x = 0; x < this.pool.Count ; x++) {
					if (pool[x].LastUsed.AddMinutes(minutes) <= DateTime.Now) {
						if (log.IsInfoEnabled) {
							log.InfoFormat("ManagedObjectPool.ZombieMaster(OID={0} D={1}) CNT={2}", pool[x].Object.GetHashCode(), pool[x].LastUsed, pool.Count);
						}
						CleanObject(ref pool[x].Object);
						pool.RemoveAt(x);
					}
				}
			}
		}

        /// <summary>
        /// Return object to beginning of pool
        /// </summary>
        /// <param name="obj">object to return</param>
        /// <returns>status</returns>
        public virtual bool Return(object obj)
        {
            lock (ilock) {
                for (int x = 0; x < pool.Count; x++) {

                    if (pool[x].Principal.Identity.Name == Thread.CurrentPrincipal.Identity.Name) {

                        if (log.IsDebugEnabled) {
                            log.DebugFormat("ManagedObjectPool.Return(OID={0} IDN={1})",
                                            pool[x].Object.GetHashCode(),
                                            Thread.CurrentPrincipal.Identity.Name);
						}

                        return true;
                    }
                }
            }

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
                    if (pool[x].Object.GetHashCode() == code) {
                        CleanObject(ref pool[x].Object);
                        pool.RemoveAt(x);
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

                    if (log.IsDebugEnabled) {
                        log.DebugFormat("{0}.{1}(OID={2})", objType, cleanup, obj.GetHashCode());
					}
                }
            } finally {
                obj = null;
                objType = null;
                methInfo = null;
            }
        }

        #region Properties
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
                for (int x = 0; x < pool.Count; x++) {
                    if (pool[x].Principal == null) {
                        y++;
					}
				}

                return y;
            }
        } 
        #endregion

        #endregion // Abstract
    }
}
