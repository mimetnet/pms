namespace PMS.Query
{
    public class LessThanClause : ValueClause
    {
        public LessThanClause(string field, object value) :
            base(field, value, "<")
        {
        }
    }
}
