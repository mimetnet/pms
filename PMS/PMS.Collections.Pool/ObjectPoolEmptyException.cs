using System;

namespace PMS.Collections.Pool
{    
    public sealed class ObjectPoolEmptyException : ApplicationException
    {
        public ObjectPoolEmptyException() : base("ObjectPool is empty!") {}

        public ObjectPoolEmptyException(string msg) : base(msg) {}

        public ObjectPoolEmptyException(string msg, Exception e) : base(msg, e) {}
    }
}
