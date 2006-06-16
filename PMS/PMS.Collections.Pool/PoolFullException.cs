using System;
using System.Collections;

namespace PMS.Collections.Pool
{    
    public class PoolFullException : System.ApplicationException
    {
        public PoolFullException() : base("Pool is Full") {}

        public PoolFullException(string msg) : base(msg) {}

        public PoolFullException(string msg, Exception e) : base(msg, e) {}
    }
}
