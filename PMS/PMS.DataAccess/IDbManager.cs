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
        bool Start(Type type, string connectionString);
        void Stop();

        void BeginTransaction();
        void RollbackTransaction();
        void CommitTransaction();
    }
}
