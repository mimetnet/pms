namespace PMS.Query
{
    public sealed class NotInClause : RangeClause
    {
        public NotInClause(string field, params object[] list) :
            base(field, "NOT IN", ", ", list)
        {
        }
    }
}
