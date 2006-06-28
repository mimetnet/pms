using System;

namespace PMS.Data
{    
    /// <summary>
    /// Thrown when PMS.Data.ProviderFactory cannot find specified IProvider
    /// </summary>
    public sealed class ProviderNotFoundException : ApplicationException
    {
        /// <summary>
        /// Construct
        /// </summary>
        public ProviderNotFoundException() : base() {}

        /// <summary>
        /// Construct with Message
        /// </summary>
        /// <param name="msg">Message</param>
        public ProviderNotFoundException(string msg) : base(msg) {}

        /// <summary>
        /// Construct with Message and Child Exception
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="e">Child Exception</param>
        public ProviderNotFoundException(string msg, Exception e) : base(msg, e) {}
    }
}
