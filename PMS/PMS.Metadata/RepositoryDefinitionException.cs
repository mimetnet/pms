using System;
using System.Collections;

namespace PMS.Metadata
{    
    public class RepositoryDefinitionException : System.ApplicationException
    {
        public RepositoryDefinitionException(string msg) : base(msg) {}

        public RepositoryDefinitionException(string msg, Exception e) : base(msg, e) {}
    }
}
