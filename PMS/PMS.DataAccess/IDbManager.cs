using System;
using System.Data;
using System.Security.Principal;

namespace PMS.DataAccess
{
    public interface IDbManager
    {
        bool IsOpen { get; }

        IDbCommand GetCommand(string sql);
        void ReturnCommand(IDbCommand command);

        IDbConnection GetConnection();
        void ReturnConnection(IDbConnection connection);

        bool Open();
        void Close();

        void BeginTransaction();
        void RollbackTransaction();
        void CommitTransaction();
    }
}
