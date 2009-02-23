namespace PMS.Query
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public interface IClause
    {
        string Name { get; }
        bool IsCondition { get; }

        string ToString();
        
        IList<IDataParameter> CreateParameters(CreateParameterDelegate callback);
    }
}
