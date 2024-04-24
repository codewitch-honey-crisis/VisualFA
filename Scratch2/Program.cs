using System;
using System.Diagnostics.Metrics;
using System.Text;

using VisualFA;

namespace Scratch2
{
    internal class Program
   {
		static void _PrintStates(IEnumerable<FA> states)
        {
            Console.Write("{ ");
            var delim = "";
			foreach (var fa in states)
			{
				// we use the Id from SetIds() here
				Console.Write(delim + "q" + fa.Id.ToString());
				delim = ", ";
			}
			Console.WriteLine(" }");
		}
		static void Main()
        {
            var expandedNfa = FA.Parse(@"-?(?:0|[1-9][0-9]*)(?:\.[0-9]+)?(?:[eE][+-]?[0-9]+)?",0,false);
			//expandedNfa.RenderToFile(@"..\..\..\num_xnfa.jpg", new FADotGraphOptions() { HideAcceptSymbolIds = true });
            // set the ids, essentially marking this specific state as the root of the machine
			expandedNfa.SetIds();
            // print the total states
			Console.WriteLine("Total states: {0}", expandedNfa.FillClosure().Count);
			// run some filters
			Console.Write("Accepting states: ");
            _PrintStates(expandedNfa.FillFind(FA.AcceptingFilter));
			Console.Write("Neutral states: ");
			_PrintStates(expandedNfa.FillFind(FA.NeutralFilter));
			Console.WriteLine("Compacting nfa");
			var compactNfa = expandedNfa.Clone();
			compactNfa.Compact();
			//compactNfa.RenderToFile(@"..\..\..\num_cnfa.jpg", new FADotGraphOptions() { HideAcceptSymbolIds = true });
			compactNfa.SetIds();
			Console.WriteLine("Total states: {0}", compactNfa.FillClosure().Count);
			Console.Write("Accepting states: ");
			_PrintStates(compactNfa.FillFind(FA.AcceptingFilter));
			Console.WriteLine("Making minimized DFA");
			var minDfa = compactNfa.ToMinimizedDfa();
			//minDfa.RenderToFile(@"..\..\..\num_mdfa.jpg", new FADotGraphOptions() { HideAcceptSymbolIds = true });
			minDfa.SetIds();
			Console.WriteLine("Total states: {0}", minDfa.FillClosure().Count);
			Console.Write("Accepting states: ");
			_PrintStates(minDfa.FillFind(FA.AcceptingFilter));

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
