namespace PMS.Query
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public class AndClause : IClause
    {
        public bool IsCondition { 
            get { return false; } 
        }

        public override string ToString()
        {
            return " AND ";
        }

        public IList<IDataParameter> CreateParameters(CreateParameterDelegate callback)
        {
            throw new NotSupportedException("RawClause doesn't support parameters");
        }
    }
}
