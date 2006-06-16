using System;
using System.Collections;

namespace PMS.Collections.Pool
{    
    public class PoolEmptyException : System.ApplicationException
    {
        public PoolEmptyException() : base("Pool is empty") {}
        
        public PoolEmptyException(string msg) : base(msg) {}
        
        public PoolEmptyException(string msg, Exception e) : base(msg, e) {}
    }
}
