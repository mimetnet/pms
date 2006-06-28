using System;

namespace PMS.Collections.Pool
{    
    /// <summary>
    /// Thrown when Pool cannot have any more items added
    /// </summary>
    public sealed class PoolFullException : ApplicationException
    {
        /// <summary>
        /// Construct
        /// </summary>
        public PoolFullException() : base("Pool is Full") {}

        /// <summary>
        /// Construct with Message
        /// </summary>
        /// <param name="msg">Message</param>
        public PoolFullException(string msg) : base(msg) {}
        
        /// <summary>
        /// Construct with Message and Child Exception
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="e">Child Exception</param>
        public PoolFullException(string msg, Exception e) : base(msg, e) {}
    }
}
