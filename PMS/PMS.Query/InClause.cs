namespace PMS.Query
{
    public sealed class InClause : RangeClause
    {
        public InClause(string field, params object[] list) :
            base(field, "IN", ", ", list)
        {
        }
    }
}
