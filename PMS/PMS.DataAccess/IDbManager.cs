using System;
using System.Data;
using System.Security.Principal;

namespace PMS.DataAccess
{
    internal interface IDbManager
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
