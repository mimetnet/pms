using System;
using System.Data;

namespace PMS.DataAccess
{
    internal interface IDbManager
    {
        IDbCommand GetCommand(string sql, AccessMode mode);
        IDbCommand GetCommand(AccessMode mode);

        void Start();
        void Stop();

        bool BeginTransaction();
        bool RollbackTransaction();
        bool CommitTransaction();
    }
}
