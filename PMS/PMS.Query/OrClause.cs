namespace PMS.Query
{
    /// <summary>
    /// Represents literal "OR" string in SQL
    /// </summary>
    public class OrClause : IClause
    {
        public override string ToString()
        {
            return " OR ";
        }
    }
}
