namespace PMS.Query
{
    /// <summary>
    /// Represents SQL's "field LIKE value"
    /// </summary>
    public class LikeClause : ValueClause
    {
        /// <summary>
        /// Construct with field and LIKE value
        /// </summary>
        /// <param name="field">database field</param>
        /// <param name="value">object which field should be LIKE</param>
        public LikeClause(string field, object value) : 
            base(field, value, "LIKE")
        {
        }
    }
}
