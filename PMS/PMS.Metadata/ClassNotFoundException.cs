using System;
using System.Collections;

namespace PMS.Metadata
{    
    public class ClassNotFoundException : System.ApplicationException
    {
        public ClassNotFoundException(string msg) : base(msg) {}

        public ClassNotFoundException(string msg, Exception e) : base(msg, e) {}
    }
}
