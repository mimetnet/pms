using System;
using System.Data;
using System.Security.Principal;

namespace PMS.DataAccess
{
    internal interface IDbManager
    {
        IDbCommand GetCommand(string sql, AccessMode mode);
        IDbCommand GetCommand(AccessMode mode);
        void ReturnCommand(IDbCommand command);

        void Start();
        void Stop();

        bool BeginTransaction(IPrincipal principal);
        bool RollbackTransaction(IPrincipal principal);
        bool CommitTransaction(IPrincipal principal);
    }
}
