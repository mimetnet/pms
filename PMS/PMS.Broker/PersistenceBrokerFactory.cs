using System;

namespace PMS.Broker
{
    public sealed class PersistenceBrokerFactory
    {
        private static IPersistenceBroker proxy = null;
        private static IPersistenceBroker real = null;

        private PersistenceBrokerFactory()
        {
        }

        /// <summary>
        /// Return Singleton
        /// </summary>
        public static IPersistenceBroker CreateBroker()
        {
            return (real != null) ? real : (real = new PersistenceBroker());
        }

        /// <summary>
        /// Return Proxied Singleton
        /// </summary>
        public static IPersistenceBroker CreateProxiedBroker()
        {
            return (proxy != null) ? proxy : (proxy = LoadProxy());
        }

        private static IPersistenceBroker LoadProxy()
        {
            return ((IPersistenceBroker) 
                Activator.GetObject(Type.GetType("PMS.Broker.PersistenceBroker, PMS"),
                    "tcp://localhost:5642/PMS.Broker.PersistenceBroker"));
        }
    }
}
