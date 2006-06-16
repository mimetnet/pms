using System;
using System.Collections;

namespace PMS.Collections.Pool
{

    /// <summary>
    /// Exception that is thrown in the case of an object being returned to the pool
    /// when it did not originate from the pool.
    /// </summary>
    public class ForeignObjectException : System.ApplicationException
    {
        public ForeignObjectException() : base("Foreign Object: object not from pool") {}

        public ForeignObjectException(string msg) : base(msg) {}
        
        public ForeignObjectException(string msg, Exception e) : base(msg, e) {}
    }
}
