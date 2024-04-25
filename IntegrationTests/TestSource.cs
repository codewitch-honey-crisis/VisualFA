using VisualFA;
namespace IntegrationTests
{
	//[FARule(@"\/\*", Symbol = "commentBlock", BlockEnd = @"\*\/")]
	[FARule("foo", Symbol = "bar")]
	partial class TestLexer : FAStringRunner
	{

	}


	[FARule(@"\/\*", Symbol = "commentBlock", BlockEnd = @"\*\/")]
    [FARule(@"\/\/[^\n]*", Symbol = "lineComment")]
    [FARule(@"[ \t\r\n]+", Symbol = "whiteSpace")]
    [FARule(@"[A-Za-z_][A-Za-z0-9_]*", Symbol = "identifier")]
    [FARule(@"(0|([1-9][0-9]*))((\.[0-9]+[Ee]\-?[1-9][0-9]*)?|\.[0-9]+)", Symbol = "number")]
    [FARule(@"\+", Symbol = "plus")]
    [FARule(@"\-", Symbol = "minus")]
    [FARule(@"\*", Symbol = "multiply")]
    [FARule(@"\/", Symbol = "divide")]
    [FARule(@"%", Symbol = "modulo")]
    [FARule("\"([^\\n\"\\\\]|\\\\([btrnf\"\\\\/]|(u[0-9A-Fa-f]{4})))*\"", Symbol = "string")]
    partial class FooLexer : FAStringRunner
    {

    }

    partial class TestSource
    {
        [FARule(@"\/\*", Symbol = "commentBlock", BlockEnd = @"\*\/")]
        [FARule(@"\/\/[^\n]*", Symbol = "lineComment")]
        [FARule(@"[ \t\r\n]+", Symbol = "whiteSpace")]
        [FARule(@"[A-Za-z_][A-Za-z0-9_]*", Symbol = "identifier")]
        [FARule(@"(0|([1-9][0-9]*))((\.[0-9]+[Ee]\-?[1-9][0-9]*)?|\.[0-9]+)", Symbol = "number")]
        [FARule(@"\+", Symbol = "plus")]
        [FARule(@"\-", Symbol = "minus")]
        [FARule(@"\*", Symbol = "multiply")]
        [FARule(@"\/", Symbol = "divide")]
        [FARule(@"%", Symbol = "modulo")]
        [FARule("\"([^\\n\"\\\\]|\\\\([btrnf\"\\\\/]|(u[0-9A-Fa-f]{4})))*\"", Symbol = "string")]
        internal static partial FATextReaderRunner CalcTextReaderRunner(TextReader text);
        [FARule(@"\/\*", Symbol = "commentBlock", BlockEnd = @"\*\/")]
        [FARule(@"\/\/[^\n]*", Symbol = "lineComment")]
        [FARule(@"[ \t\r\n]+", Symbol = "whiteSpace")]
        [FARule(@"[A-Za-z_][A-Za-z0-9_]*", Symbol = "identifier")]
        [FARule(@"(0|([1-9][0-9]*))((\.[0-9]+[Ee]\-?[1-9][0-9]*)?|\.[0-9]+)", Symbol = "number")]
        [FARule(@"\+", Symbol = "plus")]
        [FARule(@"\-", Symbol = "minus")]
        [FARule(@"\*", Symbol = "multiply")]
        [FARule(@"\/", Symbol = "divide")]
        [FARule(@"%", Symbol = "modulo")]
        [FARule("\"([^\\n\"\\\\]|\\\\([btrnf\"\\\\/]|(u[0-9A-Fa-f]{4})))*\"", Symbol = "string")]
        internal static partial FAStringRunner CalcStringRunner(string text);
        [FARule(@"\/\*", Symbol = "commentBlock", BlockEnd = @"\*\/")]
        [FARule(@"\/\/[^\n]*", Symbol = "lineComment")]
        [FARule(@"[ \t\r\n]+", Symbol = "whiteSpace")]
        [FARule(@"[A-Za-z_][A-Za-z0-9_]*", Symbol = "identifier")]
        [FARule(@"(0|([1-9][0-9]*))((\.[0-9]+[Ee]\-?[1-9][0-9]*)?|\.[0-9]+)", Symbol = "number")]
        [FARule(@"\+", Symbol = "plus")]
        [FARule(@"\-", Symbol = "minus")]
        [FARule(@"\*", Symbol = "multiply")]
        [FARule(@"\/", Symbol = "divide")]
        [FARule(@"%", Symbol = "modulo")]
        [FARule("\"([^\\n\"\\\\]|\\\\([btrnf\"\\\\/]|(u[0-9A-Fa-f]{4})))*\"", Symbol = "string")]
        internal static partial FATextReaderDfaTableRunner CalcTextReaderTableRunner(TextReader text);
        [FARule(@"\/\*", Symbol = "commentBlock", BlockEnd = @"\*\/")]
        [FARule(@"\/\/[^\n]*", Symbol = "lineComment")]
        [FARule(@"[ \t\r\n]+", Symbol = "whiteSpace")]
        [FARule(@"[A-Za-z_][A-Za-z0-9_]*", Symbol = "identifier")]
        [FARule(@"(0|([1-9][0-9]*))((\.[0-9]+[Ee]\-?[1-9][0-9]*)?|\.[0-9]+)", Symbol = "number")]
        [FARule(@"\+", Symbol = "plus")]
        [FARule(@"\-", Symbol = "minus")]
        [FARule(@"\*", Symbol = "multiply")]
        [FARule(@"\/", Symbol = "divide")]
        [FARule(@"%", Symbol = "modulo")]
        [FARule("\"([^\\n\"\\\\]|\\\\([btrnf\"\\\\/]|(u[0-9A-Fa-f]{4})))*\"", Symbol = "string")]
        internal static partial FAStringDfaTableRunner CalcStringTableRunner(string text);
    
        public static bool CompareResults(FARunner runner, KeyValuePair<string, FAMatch[]> test)
        {
            var list = new List<FAMatch>(runner);
            return EqualsMatches(list, test.Value);
        }
        public static bool EqualsMatch(FAMatch lhs, FAMatch rhs)
        {
            if (lhs.SymbolId != rhs.SymbolId) return false;
            if (lhs.Value != rhs.Value) return false;
            if (lhs.Position != rhs.Position) return false;
            if (lhs.Line != rhs.Line) return false;
            if (lhs.Column != rhs.Column) return false;
            return true;
        }
        public static bool EqualsMatches(IList<FAMatch> lhs, IList<FAMatch> rhs)
        {
            if (object.ReferenceEquals(lhs, rhs)) return true;
            if (object.ReferenceEquals(lhs, null)) return false;
            if (object.ReferenceEquals(null, rhs)) return false;
            if (lhs.Count != rhs.Count) return false;
            for (int i = 0; i < lhs.Count; ++i)
            {
                if (!EqualsMatch(lhs[i], rhs[i])) return false;
            }
            return true;
        }
        public static readonly KeyValuePair<string, FAMatch[]> Test1 = new KeyValuePair<string, FAMatch[]>("the 10 quick brown #@%$! foxes jumped over 1.5 lazy dogs", new FAMatch[] {
                FAMatch.Create(3,"the",0,1,1),
                FAMatch.Create(2," ",3,1,4),
                FAMatch.Create(4,"10",4,1,5),
                FAMatch.Create(2," ",6,1,7),
                FAMatch.Create(3,"quick",7,1,8),
                FAMatch.Create(2," ",12,1,13),
                FAMatch.Create(3,"brown",13,1,14),
                FAMatch.Create(2," ",18,1,19),
                FAMatch.Create(-1,"#@",19,1,20),
                FAMatch.Create(9,"%",21,1,22),
                FAMatch.Create(-1,"$!",22,1,23),
                FAMatch.Create(2," ",24,1,25),
                FAMatch.Create(3,"foxes",25,1,26),
                FAMatch.Create(2," ",30,1,31),
                FAMatch.Create(3,"jumped",31,1,32),
                FAMatch.Create(2," ",37,1,38),
                FAMatch.Create(3,"over",38,1,39),
                FAMatch.Create(2," ",42,1,43),
                FAMatch.Create(4,"1.5",43,1,44),
                FAMatch.Create(2," ",46,1,47),
                FAMatch.Create(3,"lazy",47,1,48),
                FAMatch.Create(2," ",51,1,52),
                FAMatch.Create(3,"dogs",52,1,53)
            }
            );
    }
    
}
