using System;
using System.Runtime.Serialization;

namespace PMS.Metadata
{
    public class RepositoryException : Exception
    {
        public RepositoryException() : 
            base() {}

        public RepositoryException(string msg) : 
            base(msg) { }

        public RepositoryException(string msg, Exception exception) :
            base(msg, exception) { }

        public RepositoryException(SerializationInfo info, StreamingContext context) : 
            base(info,  context) {}
    }
}
