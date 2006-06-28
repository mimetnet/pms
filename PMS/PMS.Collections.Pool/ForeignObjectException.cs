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
        /// <summary>
        /// Construct
        /// </summary>
        public ForeignObjectException() : base("Foreign Object: object not from pool") {}

        /// <summary>
        /// Construct with Message
        /// </summary>
        /// <param name="msg">Message</param>
        public ForeignObjectException(string msg) : base(msg) {}

        /// <summary>
        /// Construct with Message and Child Exception
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="e">Child Exception</param>
        public ForeignObjectException(string msg, Exception e) : base(msg, e) {}
    }
}
