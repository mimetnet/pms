using System;

namespace PMS.Collections.Pool
{    
    /// <summary>
    /// Thrown when Object.GetHashCode() is not found in pool
    /// </summary>
    public sealed class ObjectNotInPoolException : ApplicationException
    {
        /// <summary>
        /// Construct
        /// </summary>
        public ObjectNotInPoolException() : base("Object is not in Pool") {}

        /// <summary>
        /// Construct with Message
        /// </summary>
        /// <param name="msg">Message</param>
        public ObjectNotInPoolException(string msg) : base(msg) {}

        /// <summary>
        /// Construct with Message and Child Exception
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="e">Child Exception</param>
        public ObjectNotInPoolException(string msg, Exception e) : base(msg, e) {}
    }
}
