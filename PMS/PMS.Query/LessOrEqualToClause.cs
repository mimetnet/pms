namespace PMS.Query
{
    /// <summary>
    /// Represents SQL's "field LESS THAN EQUAL To value"
    /// </summary>
    public class LessOrEqualToClause : ValueClause
    {
        /// <summary>
        /// Construct with field and value
        /// </summary>
        /// <param name="field">database field</param>
        /// <param name="value">value to be less than or equal to</param>
        public LessOrEqualToClause(string field, object value) :
            base(field, value, "<=")
        {
        }
    }
}
