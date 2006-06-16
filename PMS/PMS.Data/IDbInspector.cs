using System;
using System.Data;

namespace PMS.Data
{
    public interface IDbInspector
    {
        IDbConnection Connection { get; set; }
        DataSet Database { get; }

        void Examine();
    }
}
