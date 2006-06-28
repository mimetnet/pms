using System;

namespace PMS.Data
{    
    public sealed class ProviderDefaultException : System.ApplicationException
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ProviderDefaultException() : base() {}

        /// <summary>
        /// Construct with message
        /// </summary>
        /// <param name="msg">Error Message</param>
        public ProviderDefaultException(string msg) : base(msg) {}

        /// <summary>
        /// Construct with message and child exception
        /// </summary>
        /// <param name="msg">Error Message</param>
        /// <param name="e">Child Exception</param>
        public ProviderDefaultException(string msg, Exception e) : base(msg, e) {}
    }
}
