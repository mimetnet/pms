using System;

namespace PMS.Query
{
    public sealed class QueryFactory
    {
        private QueryFactory()
        {
        }

        /// <summary>
        /// Returns QueryByObject(obj)
        /// </summary>
        public static IQuery ByObject(Object obj)
        {
            return ByObject(obj, null);
        }

        /// <summary>
        /// Returns Proxied QueryByObject(obj)
        /// </summary>
        public static IQuery ByObjectProxied(Object obj)
        {
            return ByProxiedObject(obj, null);
        }

        /// <summary>
        /// Returns QueryByObject(obj, criteria)
        /// </summary>
        public static IQuery ByObject(Object obj, Criteria criteria)
        {
            return new QueryByObject(obj, criteria);
        }

        /// <summary>
        /// Returns Proxied QueryByObject(obj, criteria)
        /// </summary>
        public static IQuery ByProxiedObject(Object obj, Criteria criteria)
        {
            AbstractQuery query = ((QueryByObject)
                Activator.GetObject(Type.GetType("PMS.Query.QueryByObject, PMS"),
                    "tcp://localhost:5642/PMS.Query.QueryByObject"));

            query.LoadMetaObject(obj, criteria);

            return query;
        }
    }
}

