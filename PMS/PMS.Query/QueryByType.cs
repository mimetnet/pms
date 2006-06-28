using System;
using System.Collections;
using System.Reflection;
using System.Text;

using PMS.Data;
using PMS.Metadata;

namespace PMS.Query
{
    /// <summary>
    /// Create Query representing a class type
    /// </summary>
    public class QueryByType : QueryByObject
    {
        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="type">Type to query</param>
        public QueryByType(Type type) : base(Activator.CreateInstance(type))
        {
        }
    }
}
