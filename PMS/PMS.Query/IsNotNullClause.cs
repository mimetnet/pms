namespace PMS.Query
{
    public class IsNotNullClause : IsNullClause
    {
        public IsNotNullClause(string field) : base(field, "is not")
        {
        }
    }
}
