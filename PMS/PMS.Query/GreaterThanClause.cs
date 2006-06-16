namespace PMS.Query
{
    public class GreaterThanClause : ValueClause
    {
        public GreaterThanClause(string field, object value) :
            base(field, value, ">")
        {
        }
    }
}
