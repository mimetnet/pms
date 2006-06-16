using System;
using System.Collections;

namespace PMS.Collections.Pool
{    
    public class ObjectNotInPoolException : System.ApplicationException
    {
        public ObjectNotInPoolException() : base("Object is not in Pool") {}

        public ObjectNotInPoolException(string msg) : base(msg) {}

        public ObjectNotInPoolException(string msg, Exception e) : base(msg, e) {}
    }
}
