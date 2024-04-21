using System.Diagnostics.Metrics;
using System.Text;

using VisualFA;

namespace Scratch2
{
    internal class Program
   {
		
		static void Main()
        {
            var expr = FA.Parse(@"https?\://[^\""\';\)]+");
			var client = new HttpClient();
			using (var msg = new HttpRequestMessage(HttpMethod.Get, "https://www.google.com"))
			{
				using (var resp = client.Send(msg))
				{
					using (var reader = new StreamReader(resp.Content.ReadAsStream()))
					{
						foreach (var match in expr.Run(reader))
						{
							if (match.IsSuccess)
							{
								Console.WriteLine(match.Value);
							}
						}
					}
				}
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
