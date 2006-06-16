namespace PMS.Query
{
    public class LessOrEqualToClause : ValueClause
    {
        public LessOrEqualToClause(string field, object value) :
            base(field, value, "<=")
        {
        }
    }
}
