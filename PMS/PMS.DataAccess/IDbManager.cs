using System;
using System.Data;
using System.Security.Principal;

namespace PMS.DataAccess
{
    public interface IDbManager
    {
        IDbCommand GetCommand(string sql);
        void ReturnCommand(IDbCommand command);

        bool Start();
        void Stop();

        void BeginTransaction();
        void RollbackTransaction();
        void CommitTransaction();
    }
}
