using System;
using System.Data;

namespace PMS.Data
{
    public interface IDbInspector
    {
        /// <summary>
        /// Connection to Database to Inspect
        /// </summary>
        IDbConnection Connection { get; set; }

        /// <summary>
        /// Results of the database inspection
        /// </summary>
        DataSet Database { get; }

        /// <summary>
        /// Inspect database set via Connection
        /// </summary>
        void Examine();
    }
}
