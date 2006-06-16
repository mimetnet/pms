namespace PMS.Query
{
    public class IsNullClause : ValueClause
    {
        public IsNullClause(string field) :
            base(field, "null", "is")
        {
        }
    }
}
