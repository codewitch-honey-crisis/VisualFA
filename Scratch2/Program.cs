using System.Text;

using VisualFA;

namespace Scratch2
{
    internal class Program
   {
		public static IEnumerable<FAMatch> Scrape(HttpClient client, string url, FA lexer, FA[]? blockEnds = null)
		{
			using (var msg = new HttpRequestMessage(HttpMethod.Get, url))
			{
				using (var resp = client.Send(msg))
				{
					using (var reader = new StreamReader(resp.Content.ReadAsStream()))
					{
						foreach (var match in lexer.Run(reader, blockEnds))
						{
							if (match.IsSuccess)
							{
								yield return match;
							}
						}
					}
				}
			}
			
		}
		static void Main()
        {
            var url = FA.Parse(@"https?\://[^\""\';\)]+");
			url.RenderToFile(@"..\..\..\url.jpg");
            var client = new HttpClient();
			foreach(var match in Scrape(client,"https://www.google.com", url))
			{
				Console.WriteLine(match.Value);
				
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
