//HintName: FARuleAttribute.g.cs
#nullable disable
namespace VisualFA
{
    [System.AttributeUsage(System.AttributeTargets.Method,AllowMultiple = true,Inherited = false)]
    class FARuleAttribute : System.Attribute
    {
        public FARuleAttribute() { }
        public FARuleAttribute(string expression)
        { 
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (expression.Length == 0) throw new ArgumentException("The expression must not be empty", nameof(expression));
            Expression = expression;
        }
        public string Expression { get; set; } = "";
        public string BlockEnd { get; set; } = null;
        public int Id { get; set; } = -1;
        public string Symbol { get; set; } = null;
    }
}