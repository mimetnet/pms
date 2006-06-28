namespace PMS.Query
{
    /// <summary>
    /// Represents range clauses
    /// </summary>
    public class RangeClause : IClause
    {
        private string field;
        private string oper;
        private string comparison;
        private object val1;
        private object val2;

        internal RangeClause(string field, string oper, object val1, string comparison, object val2)
        {
            this.field = field;
            this.comparison = comparison;
            this.oper = oper;
            this.val1 = val1;
            this.val2 = val2;
        }

        /// <summary>
        /// SQL representing Range
        /// </summary>
        /// <returns>SQL representing Range</returns>
        public override string ToString()
        {
            return (field + " " + oper + " " + val1 + " " + comparison + " " + val2);
        }
    }
}
