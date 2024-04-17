using Microsoft.CSharp;

using System.Diagnostics;
using VisualFA;

var keywords = "abstract|as|ascending|async|await|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|descending|do|double|dynamic|else|enum|equals|explicit|extern|event|false|finally|fixed|float|for|foreach|get|global|goto|if|implicit|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|partial|private|protected|public|readonly|ref|return|sbyte|sealed|set|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|var|virtual|void|volatile|while|yield";
var ident = "[A-Za-z_][0-9A-Za-z_]*";//"[[:IsLetter:]_][[:IsLetterOrDigit:]_]*";
var whitespace = "[ \r\n\t]+";// "[[:IsWhiteSpace:]]+";
var nfaIdent = FA.Parse(ident, 0);
var nfaKeywords = FA.Parse(keywords, 1);
var nfaWhitespace = FA.Parse(whitespace,2);
var nfa = FA.ToLexer(new FA[] { nfaIdent,nfaKeywords, nfaWhitespace }, false, true);
var sw = new Stopwatch();
Console.Write("Minimizing");
sw.Start();
var dfa = FA.ToLexer(new FA[] { nfaIdent, nfaKeywords, nfaWhitespace }, true, true, new Progress<int>((i) => { if (0 == (i % 10000)) Console.Write("."); }));
sw.Stop();
Console.WriteLine("done in {0} seconds", sw.Elapsed.TotalSeconds.ToString());
Console.WriteLine("final: " + dfa.ToString("e"));
Console.WriteLine("NFA has {0} states. DFA has {1} states", nfa.FillClosure().Count, dfa.FillClosure().Count);
dfa.RenderToFile(@"..\..\..\dfa.jpg");
var genopts = new FAGeneratorOptions()
{
	StringRunnerClassName = "TestRunner",
	TextReaderRunnerClassName = "TestTextRunner",
	Dependencies = FAGeneratorDependencies.UseRuntime,
	GenerateTables = false,
	GenerateStringRunner = true,
	Namespace = "",
	GenerateTextReaderRunner = true,
	UseSpans = true
};
var ccu = dfa.Generate(null, genopts);
var csharp = new CSharpCodeProvider();
using (var writer = new StreamWriter(@"..\..\..\TestRunner.cs"))
{
	var opts = new System.CodeDom.Compiler.CodeGeneratorOptions()
	{
		BlankLinesBetweenMembers = false,
		BracingStyle = "BLOCK",
		ElseOnClosing = true,
		IndentString = "    ",
		VerbatimOrder = true
	};
	csharp.GenerateCodeFromCompileUnit(ccu, writer, opts);
}
var testTextRunner = new TestTextRunner();
testTextRunner.Set(new StringReader("as case base"));
foreach (var m in testTextRunner)
{
	Console.WriteLine("{0}:{1} at {2}", m.SymbolId, m.Value, m.Position);
}
