using System;

namespace PMS.DataAccess
{
    /// <summary>
    /// Represents AccessMode of a database connection
    /// </summary>
    public enum AccessMode
    {
        /// <summary>
        /// Connection can be read from
        /// </summary>
        Read,

        /// <summary>
        /// Connection can be written to
        /// </summary>
        Write
    }
}
