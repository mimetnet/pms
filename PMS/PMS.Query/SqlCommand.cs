namespace PMS.Query
{
    /// <summary>
    /// Type of SQL to execute
    /// </summary>
    public enum SqlCommand
    {
        /// <summary>
        /// SELECT * FROM table
        /// </summary>
        Select,

        /// <summary>
        /// INSERT INTO table () VALUES ();
        /// </summary>
        Insert,

        /// <summary>
        /// UPDATE table SET x=y;
        /// </summary>
        Update,

        /// <summary>
        /// DELETE FROM table;
        /// </summary>
        Delete
    }
}
