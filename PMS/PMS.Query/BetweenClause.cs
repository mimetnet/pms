namespace PMS.Query
{
    public sealed class BetweenClause : RangeClause
    {
        public BetweenClause(string field, params object[] list) :
            base(field, "BETWEEN", " AND ", false, list)
        {
        }
    }
}
