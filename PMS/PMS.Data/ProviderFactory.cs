using System;
using System.Configuration;
using System.Data;

using PMS.Data.PostgreSql;

namespace PMS.Data
{
    internal sealed class ProviderFactory
    {
        private static string PROVIDER_KEY = "Umbrella.Data.Provider";

        public static IProvider Factory(Type type)
        {
            if (type == typeof(Npgsql.NpgsqlConnection))
                return PostgreSqlProvider.Instance;
            //else if (type == typeof(ByteFX.Data.MySqlClient.MySqlConnection))
            //    return MySqlProvider.Instance;
            
            throw new ProviderNotFoundException("'" + type + "' does not exist within the ADO framework");
        }
            
        public static IProvider Factory(string name)
        {
            if (name.Equals("PostgreSql"))
                return PostgreSqlProvider.Instance;
            
            throw new ProviderNotFoundException("'" + name + "' does not exist within the ADO framework");
        }

        public static IProvider Factory()
        {
#if NET_2_0
            string key = ConfigurationManager.AppSettings[PROVIDER_KEY];
#else
            string key = ConfigurationSettings.AppSettings[PROVIDER_KEY];
#endif
            
            if (key != null)
                return Factory(key);

            throw new ProviderDefaultException("Please specificy a default provider in a .config file before trying to use it");
        }
    }
}
