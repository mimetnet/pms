using System;
using System.Data;

namespace PMS.DataAccess
{
    public interface IDbManager
    {
        IDbCommand GetCommand(string sql, AccessMode mode);
        IDbCommand GetCommand(AccessMode mode);
        void Start();
        void Stop();
    }
}
