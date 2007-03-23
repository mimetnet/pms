using System;
using System.Collections;
using System.Security.Principal;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;

namespace PMS.Collections.Pool
{
    public class PrincipalObjectPool : ObjectPool, IObjectPool
    {
        public PrincipalObjectPool(Type type, string sFree) : base(type, sFree)
		{
		}

        public PrincipalObjectPool(Type type, object[] typeParams, string sFree) : base(type, typeParams, sFree)
        {
        }

		public override bool Open()
		{
			return true;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
        public override object Borrow()
		{
			for (int x = 0; x < this.pool.Count ; x++) {

				if (pool[x].Principal.Identity.Name == Thread.CurrentPrincipal.Identity.Name) {

					if (verbose && log.IsDebugEnabled) {
						log.DebugFormat("ManagedObjectPool.Borrow(OID={0} IDN={1})",
								pool[x].Object.GetHashCode(),
								Thread.CurrentPrincipal.Identity.Name);
					}

					return pool[x].Checkout();
				}
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
			for (int x = 0; x < pool.Count; x++) {

				if (pool[x].Principal.Identity.Name == Thread.CurrentPrincipal.Identity.Name) {

					if (verbose && log.IsDebugEnabled) {
						log.DebugFormat("ManagedObjectPool.Return(OID={0} IDN={1})",
								pool[x].Object.GetHashCode(),
								Thread.CurrentPrincipal.Identity.Name);
					}

					return true;
				}
			}

            return false;
        }
    }
}
