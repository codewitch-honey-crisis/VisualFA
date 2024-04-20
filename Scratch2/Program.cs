using System.Text;

using VisualFA;

namespace Scratch2
{
    internal class Program
   {
		static void Main()
        {
            var nfa = FA.Parse("[^\r\n]*",0, false);
            nfa.RenderToFile(@"..\..\..\nfa.jpg");
            var dfa = nfa.ToMinimizedDfa();
			dfa.RenderToFile(@"..\..\..\dfa.jpg");

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
