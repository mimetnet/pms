using System;

namespace PMS.Collections.Pool
{    
    /// <summary>
    /// Thrown with ManagedObjectPool cannot find anymore elements
    /// </summary>
    public class PoolEmptyException : ApplicationException
    {
        /// <summary>
        /// Construct
        /// </summary>
        public PoolEmptyException() : base("Pool is empty") {}
        
        /// <summary>
        /// Construct with Message
        /// </summary>
        /// <param name="msg">Message</param>
        public PoolEmptyException(string msg) : base(msg) {}
        
        /// <summary>
        /// Construct with Message and Child Exception
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="e">Child Exception</param>
        public PoolEmptyException(string msg, Exception e) : base(msg, e) {}
    }
}
