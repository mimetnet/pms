namespace PMS.Query
{
    /// <summary>
    /// Represents SQL's field BETWEEN x AND y
    /// </summary>
    public sealed class BetweenClause : RangeClause
    {
        /// <summary>
        /// Construct with field and min, max values
        /// </summary>
        /// <param name="field">database field</param>
        /// <param name="val1">min value of field</param>
        /// <param name="val2">max value of field</param>
        public BetweenClause(string field, object val1, object val2) :
            base(field, "BETWEEN", val1, "AND", val2)
        {
        }
    }
}
