using System;

namespace PMS.Data
{    
    public class ProviderNotFoundException : System.ApplicationException
    {
        public ProviderNotFoundException() : base() {}

        public ProviderNotFoundException(string msg) : base(msg) {}

        public ProviderNotFoundException(string msg, Exception e) : base(msg, e) {}
    }
}
