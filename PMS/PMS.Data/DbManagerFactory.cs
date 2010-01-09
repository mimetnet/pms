using System;
using System.Collections.Generic;

using PMS.Metadata;

namespace PMS.Data
{
    public static class DbManagerFactory
    {
        private static Dictionary<Connection, IDbManager> managers = new Dictionary<Connection, IDbManager>(new ConnectionComparer());
        
        public static IDbManager Factory(Connection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            IDbManager value = null;

            lock (managers) {
                if (managers.TryGetValue(connection, out value))
                    return value;

                value = new DbManager(connection);
                
                if (value.Open()) {
                    managers.Add(connection, value);
                    return value;
                }
            }

            throw new Exception("Failed to open: " + connection.Value);
        }

        public static void Close()
        {
            lock (managers) {
                foreach (KeyValuePair<Connection, IDbManager> kv in managers) {
                    kv.Value.Close();
                }
                managers.Clear();
            }
        }

        private class ConnectionComparer : IEqualityComparer<Connection>
        {
            public bool Equals(Connection x, Connection y)
            {
                if (x == null || y == null)
                    return false;

                return (
                    (x.Provider.Name == y.Provider.Name) &&
                    (x.Maximum == y.Maximum) &&
                    (x.Minimum == y.Minimum) &&
                    (0 == StringComparer.InvariantCulture.Compare(x.Value, y.Value)));
            }

            public int GetHashCode(Connection obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
