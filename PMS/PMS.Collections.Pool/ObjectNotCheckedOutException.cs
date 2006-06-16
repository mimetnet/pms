using System;
using System.Collections;

namespace PMS.Collections.Pool
{    
    public class ObjectNotCheckedOutException : System.ApplicationException
    {
        public ObjectNotCheckedOutException () : base("Object has not been checked" +
                                                      "out, it cannot be returned") {}

        public ObjectNotCheckedOutException (string msg) : base(msg) {}

        public ObjectNotCheckedOutException(string msg, Exception e) : base(msg, e) {}
    }
}
