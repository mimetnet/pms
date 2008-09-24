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

        public static IPersistenceBroker CreateBroker()
        {
            return (real != null) ? real : (real = new PersistenceBroker());
        }

        public static IPersistenceBroker CreateProxiedBroker()
        {
			throw new NotImplementedException();
        }
    }
}
