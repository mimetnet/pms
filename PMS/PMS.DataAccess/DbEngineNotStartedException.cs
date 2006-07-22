using System;
using System.Runtime.Serialization;

namespace PMS.DataAccess
{
    public class DbEngineNotStartedException : Exception
    {
        public DbEngineNotStartedException()
            : base() { }

        public DbEngineNotStartedException(string msg)
            : base(msg) { }

        public DbEngineNotStartedException(string msg, Exception exception)
            : base(msg, exception) { }

        public DbEngineNotStartedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
