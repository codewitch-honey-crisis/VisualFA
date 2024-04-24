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
		static void _RunIdents(FARunner runner,string[] idents)
		{
			for (int i = 0; i < idents.Length; i++)
			{
				if(runner is FAStringRunner)
				{
					((FAStringRunner)runner).Set(idents[i]);
				}
                else
                {
					((FATextReaderRunner)runner).Set(new StringReader(idents[i]));
                }
                Console.Write("\"");
				Console.Write(idents[i]);
				Console.Write("\" is ");
				if (!_IsMatch(runner))
				{
					Console.Write("not ");
				}
				Console.WriteLine("an identifier");
			}
		}
        static bool _IsMatch(FARunner runner)
        {
			return runner.NextMatch().IsSuccess && runner.NextMatch().SymbolId == -2;
		}
		static void Main()
        {
			var idents = new string[]
			{
				"foo",
				"",
				"baz",
				"1bar"
			};
			var tableRunner = new FATextReaderDfaTableRunner(_StateTable);
			_RunIdents(tableRunner, idents);
			var fa = FA.FromArray(_StateTable);
            fa.RenderToFile(@"..\..\..\ident.jpg");
			Console.WriteLine(fa.ToString("e"));
			var stateRunner = new FAStringStateRunner(fa);
            _RunIdents(stateRunner, idents);
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
