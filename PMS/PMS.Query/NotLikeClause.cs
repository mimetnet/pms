namespace PMS.Query
{
    /// <summary>
    /// Represents SQL's field "NOT LIKE" value
    /// </summary>
    public class NotLikeClause : ValueClause
    {
        /// <summary>
        /// Construct SQL with field and value
        /// </summary>
        /// <param name="field">database field</param>
        /// <param name="value">value for field to not be like</param>
        public NotLikeClause(string field, object value) : 
            base(field, value, " NOT LIKE ") {}
    }
}
