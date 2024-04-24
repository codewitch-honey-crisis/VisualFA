using VisualFA;
int[] _StateTable = new int[] {
	-1, 1, 10, 3, 65, 90, 95, 95, 97, 122,
	0, 1, 10, 4, 48, 57, 65, 90, 95, 95,
	97, 122
};
static void _RunStrings(FAStringRunner runner, string[] strings)
{
	for (int i = 0; i < strings.Length; i++)
	{
		runner.Set(strings[i]);
		Console.Write("\"");
		Console.Write(strings[i]);
		Console.Write("\" is ");
		switch(_Match(runner))
		{
			case 0: Console.WriteLine("an email address"); break;
			case 1: Console.WriteLine("a phone number"); break;
			default:
				Console.WriteLine("not a valid input");
				break;
		}
		
	}
}
static int _Match(FARunner runner)
{
	var match = runner.NextMatch();
	if(match.IsSuccess && runner.NextMatch().SymbolId == FAMatch.EndOfInput)
	{
		return match.SymbolId;
	}
	return -1;
}
var idents = new string[]
			{
				"foo",
				"",
				"baz@bar.com",
				"(300) 555-1212"
			};
var email = FA.Parse(@"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])", 0);
var phone = FA.Parse(@"(?:\+[0-9]{1,2}[ ])?\(?[0-9]{3}\)?[ \.\-][0-9]{3}[ \.\-][0-9]{4}", 1);
var lexer = FA.ToLexer(new FA[] { email, phone });
var runner = new FAStringStateRunner(lexer);
_RunStrings(runner, idents);
