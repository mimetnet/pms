namespace PMS.Query
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    internal class RawClause : IClause
    {
        private string value;

        public RawClause(string s)
        {
            this.value = s;
        }

        public bool IsCondition { 
            get { return false; } 
        }

        public override string ToString()
        {
            return value;
        }

        public IList<IDataParameter> CreateParameters(CreateParameterDelegate callback)
        {
            throw new NotSupportedException("RawClause doesn't support parameters");
        }
    }
}
