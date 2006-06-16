namespace PMS.Query
{
    public class IsNotNullClause : ValueClause
    {
        public IsNotNullClause(string field) :
            base(field, "null", "is not")
        {
        }
    }
}
