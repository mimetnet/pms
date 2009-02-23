namespace PMS.Query
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    internal class RawClause : IClause
    {
        private string value;
        private bool isCondition = false;

        public RawClause(string s) : this(s, false)
        {
        }

        public RawClause(string s, bool isCondition)
        {
            this.value = s;
            this.isCondition = isCondition;
        }

        public string Name {
            get { return null; }
        }

        public bool IsCondition { 
            get { return isCondition; } 
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
