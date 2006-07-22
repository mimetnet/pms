using System;
using System.Runtime.Serialization;

namespace PMS.Metadata
{
    public class RepositoryNotFoundException : Exception
    {
        public RepositoryNotFoundException() : 
            base() {}

        public RepositoryNotFoundException(string msg) : 
            base(msg) { }

        public RepositoryNotFoundException(string msg, Exception exception) :
            base(msg, exception) { }

        public RepositoryNotFoundException(SerializationInfo info, StreamingContext context) : 
            base(info,  context) {}
    }
}
