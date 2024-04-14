using System.Text;


namespace Scratch2
{
    internal class Program
   {
		
		static void Main(string[] args)
        {
            using(var reader = new StreamReader(@"..\..\..\data.json"))
            {
                dynamic? obj = Json.JsonObject.Parse(reader);
                Console.WriteLine(obj!.seasons[0].episodes[0].overview);
            }
		}
    }
}
