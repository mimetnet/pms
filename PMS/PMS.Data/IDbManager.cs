using System;
using System.Data;

namespace PMS.Data
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
    }
}
