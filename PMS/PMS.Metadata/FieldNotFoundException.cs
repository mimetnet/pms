using System;
using System.Collections;

namespace PMS.Metadata
{    
    public class FieldNotFoundException : System.ApplicationException
    {
        public FieldNotFoundException(string msg) : base(msg) {}

        public FieldNotFoundException(string msg, Exception e) : base(msg, e) {}
    }
}
