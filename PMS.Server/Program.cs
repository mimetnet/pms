using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace PMS.Server
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            Type type = null;
            TcpChannel chnl = new TcpChannel(8085);
            ChannelServices.RegisterChannel(chnl, false);

            type = typeof(PMS.Broker.PersistenceBroker);
            RemotingConfiguration.RegisterWellKnownServiceType(type, type.ToString(),
                                                               WellKnownObjectMode.Singleton);

            Console.WriteLine("Press [Enter] to halt...");
            Console.ReadLine();
        }
    }
}