using System;

namespace PMS.Data
{    
    public class ProviderDefaultException : System.ApplicationException
    {
        public ProviderDefaultException() : base() {}

        public ProviderDefaultException(string msg) : base(msg) {}

        public ProviderDefaultException(string msg, Exception e) : base(msg, e) {}
    }
}
