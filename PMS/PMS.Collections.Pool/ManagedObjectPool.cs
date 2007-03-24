using System;
using System.Collections;
using System.Security.Principal;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PMS.Collections.Pool
{
    public class ManagedObjectPool : ObjectPool, IObjectPool
    {
		private int index = -1;

        public ManagedObjectPool(Type type, int min, int max, string sFree) :
			base(type, min, max, sFree, false)
        {
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ManagedObjectPool()
        {
            Close();
        } 

		[MethodImpl(MethodImplOptions.Synchronized)]
        public override object Borrow()
        {
			index = (index != (pool.Count - 1)) ? index + 1 : 0;

			if (pool[index].Available) {
				if (verbose && log.IsDebugEnabled) {
					log.DebugFormat("ManagedObjectPool.Borrow(OID={0} IDN={1})",
							pool[index].Object.GetHashCode(),
							Thread.CurrentPrincipal.Identity.Name);
					}

				return pool[index].Checkout();
			}

			// nothing left, so add one more and recurse
			if (this.Add() > THRESHOLD) {
				ZombieKiller(30);
			}

			return this.Borrow();
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
        public override bool Return(object obj)
		{
			int code = obj.GetHashCode();

			for (int x = 0; x < pool.Count; x++) {
				if (pool[x].Object.GetHashCode() == code) {
					pool[x].Available = true;

					if (verbose && log.IsDebugEnabled)
						log.DebugFormat("ManagedObjectPool.Return(OID={0} TID={1})",
								pool[x].GetHashCode(),
								Thread.CurrentThread.ManagedThreadId);

					return true;
				}
			}

			return false;
		}
	}
}
