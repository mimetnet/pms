using System;
using System.Collections;

namespace PMS.Collections.Pool
{    
    /// <summary>
    /// Thrown when someone tries to return object never checkout out of pool
    /// </summary>
    public sealed class ObjectNotCheckedOutException : ApplicationException
    {
        /// <summary>
        /// Construct
        /// </summary>
        public ObjectNotCheckedOutException () : base("Object has not been checked" +
                                                      "out, it cannot be returned") {}
        /// <summary>
        /// Construct with Message
        /// </summary>
        /// <param name="msg">Message</param>
        public ObjectNotCheckedOutException (string msg) : base(msg) {}

        /// <summary>
        /// Construct with Message and Child Exception
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="e">Child Exception</param>
        public ObjectNotCheckedOutException(string msg, Exception e) : base(msg, e) {}
    }
}
