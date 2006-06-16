namespace PMS.Query
{
    public class GreaterOrEqualToClause : ValueClause
    {
        public GreaterOrEqualToClause(string field, object value) :
            base(field, value, ">=")
        {
        }
    }
}
