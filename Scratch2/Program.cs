using System.Text;
using System.Text.Json.Nodes;


namespace Scratch2
{
    internal class Program
   {
		
		static void Main(string[] args)
        {
            using(var reader = new StreamReader(@"..\..\..\data.json"))
            {
                var obj = Json.JsonObject.Parse(reader);
                Console.WriteLine(((Json.JsonObject?)obj).ToString());
            }
		}
    }
}
