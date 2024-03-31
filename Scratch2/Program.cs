using VisualFA;
namespace Scratch2
{
    internal class Program
   {
        static void Main(string[] args)
        {
            var number = FA.Parse(@"-?(?:0|[1-9][0-9]*)(?:\.[0-9]+)?(?:[eE][+-]?[0-9]+)?");
            var boolean = FA.Parse("true|false");
            var @null = FA.Parse("null");
            var @string = FA.Parse(@"""([^\n""\\]|\\([btrnf""\\/]|(u[0-9A-Fa-f]{4})))*""");
            var dgo = new FADotGraphOptions();
            dgo.HideAcceptSymbolIds = true;
			@string = @string.ToMinimizedDfa();
			@string.RenderToFile(@"..\..\..\string.jpg", dgo);
		}
    }
}
