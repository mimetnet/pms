using System;
using System.Data;

namespace PMS.Data.Pool
{
    interface IConnectionPool
    {
        void Open();
        void Close();
        
        IDbConnection GetConnection();

        void ReturnConnection(IDbConnection conn);

        void DestroyConnection(IDbConnection conn);
    }
}
