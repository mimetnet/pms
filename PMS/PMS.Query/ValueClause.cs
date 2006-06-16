namespace PMS.Query
{
    public abstract class ValueClause : IClause
    {
        private string Field;
        private object Value;
        private string Operator;

        internal ValueClause(string field, object value, string op)
        {
            Field    = field;
            Value    = value;
            Operator = op;
        }

        public override string ToString()
        {
            return (Field + " " + Operator + " " + Value);
        }
    }
}
