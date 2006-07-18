using System;

namespace PMS.Query
{
    public interface IQuery
    {
        object BaseObject { get; }

        Type Type { get; }

        SqlCommand Command { get; set; }

        string Selection { get; set; }

        string Table { get; }

        string Condition { get; }

        string Limit { get; }

        string OrderBy { get; }

        string InsertClause { get; }

        string UpdateClause { get; }

        string Insert();
        string Update();
        string Delete();
        string Select();
        string Count();

        bool IsValid { get; }
        Exception ValidationException { get; }

        Criteria Criteria { get; set;}
    }
}
