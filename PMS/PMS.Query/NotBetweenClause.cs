namespace PMS.Query
{
    public sealed class NotBetweenClause : RangeClause
    {
        public NotBetweenClause(string field, params object[] list) : 
            base(field, "NOT BETWEEN", " AND ", list)
        {
        }
    }
}
