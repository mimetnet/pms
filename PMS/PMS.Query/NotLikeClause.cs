namespace PMS.Query
{
    public class NotLikeClause : ValueClause
    {
        public NotLikeClause(string f, object v) : base(f, v, " NOT LIKE ") {}
    }
}
