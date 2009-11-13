namespace PMS.Query
{
    public class LikeClause : ValueClause
    {
        public LikeClause(string field, object value) : 
            base(field, value, "LIKE")
        {
        }

        public LikeClause(string sqlFunction, string field, object value) : 
            base(sqlFunction + "(" + field + ")", value, "LIKE")
        {
        }
    }
}
