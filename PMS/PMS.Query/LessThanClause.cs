namespace PMS.Query
{
    /// <summary>
    /// Represents SQL's "field LESS value"
    /// </summary>
    public class LessThanClause : ValueClause
    {
        /// <summary>
        /// Construct with field and value
        /// </summary>
        /// <param name="field">database field</param>
        /// <param name="value">value to be less than</param>
        public LessThanClause(string field, object value) :
            base(field, value, "<")
        {
        }
    }
}
