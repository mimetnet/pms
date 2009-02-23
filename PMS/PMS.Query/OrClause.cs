namespace PMS.Query
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public class OrClause : IClause
    {
        public string Name {
            get { return null; }
        }

        public bool IsCondition { 
            get { return false; } 
        }

        public override string ToString()
        {
            return " OR ";
        }

        public IList<IDataParameter> CreateParameters(CreateParameterDelegate callback)
        {
            throw new NotSupportedException("RawClause doesn't support parameters");
        }
    }
}
