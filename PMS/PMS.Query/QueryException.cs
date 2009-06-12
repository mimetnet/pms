using System;

namespace PMS.Query
{    
    public sealed class QueryException : ApplicationException
    {
        public QueryException() : base("Invalid Query") {}

        public QueryException(string msg) : base(msg) {}

        public QueryException(string msg, Exception e) : base(msg, e) {}
    }
}
