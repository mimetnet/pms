using System;
using System.Collections;
using System.Reflection;
using System.Text;

using PMS.Data;
using PMS.Metadata;

namespace PMS.Query
{
    public class QueryByType : QueryByObject
    {
        public QueryByType(Type type) : base(Activator.CreateInstance(type))
        {
        }
    }
}
