using System.Diagnostics.Metrics;
using System.Text;

using VisualFA;

namespace Scratch2
{
    internal class Program
   {
		static int[] _StateTable = new int[] {
	        -1, 1, 10, 3, 65, 90, 95, 95, 97, 122,
	        0, 1, 10, 4, 48, 57, 65, 90, 95, 95,
	        97, 122
        };
        static bool _IsMatch(FARunner runner)
        {
			return runner.NextMatch().IsSuccess && runner.NextMatch().SymbolId == -2;
		}
		static void Main()
        {
            var tableRunner = FA.Run("baz", _StateTable);
            if(_IsMatch(tableRunner))
            {
				Console.WriteLine("Matched identifier");
			}
			else
			{
				Console.WriteLine("Not an identifier");
			}
			tableRunner.Set("");
			if (_IsMatch(tableRunner))
			{
				Console.WriteLine("Matched identifier");
			}
			else
			{
				Console.WriteLine("Not an identifier");
			}

			var fa = FA.FromArray(_StateTable);
            fa.RenderToFile(@"..\..\..\ident.jpg");
			Console.WriteLine(fa.ToString("e"));
            var stateRunner = fa.Run("foo");
            if(_IsMatch(stateRunner))
            {
                Console.WriteLine("Matched identifier");
			}
			else
			{
				Console.WriteLine("Not an identifier");
			}
			stateRunner.Set("1bar");
            if (_IsMatch(stateRunner)) 
			{
				Console.WriteLine("Matched identifier");
			} else
            {
                Console.WriteLine("Not an identifier");
            }
		}
        static void Main2()
        {
            
            using(var reader = new StreamReader(@"..\..\..\data.json"))
            {
                dynamic? obj = Json.JsonObject.Parse(reader);
                Console.WriteLine(obj!.seasons[1].episodes[0].overview);
            }
		}
    }
}
