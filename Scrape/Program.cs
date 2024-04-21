using VisualFA;
var expr = FA.Parse(@"https?\://[^"";\)]+");
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