using System;
using System.Collections;
using System.Reflection;
using System.Text;

using PMS.Data;
using PMS.Metadata;

namespace PMS.Query
{
    [Serializable]
    public class QueryByType : QueryByObject
    {
        public QueryByType(Type type) : base(Activator.CreateInstance(type))
        {
        }

		protected new bool IsFieldSet(Field field)
		{
			return false;
		}

		protected new bool IsFieldSet(Field field, object value)
		{
			return false;
		}
    }
}
