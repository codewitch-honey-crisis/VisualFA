using System.Diagnostics;

using VisualFA;

var keywords = "abstract|as|ascending|async|await|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|descending|do|double|dynamic|else|enum|equals|explicit|extern|event|false|finally|fixed|float|for|foreach|get|global|goto|if|implicit|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|partial|private|protected|public|readonly|ref|return|sbyte|sealed|set|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|var|virtual|void|volatile|while|yield";
var ident = "[[:IsLetter:]_][[:IsLetterOrDigit:]_]*";
var whitespace = "[ \t\r\n]+";

var nfaKeywords = FA.Parse(keywords,0);
var nfaIdent = FA.Parse(ident, 1);
var nfaWhitespace= FA.Parse(whitespace,2);
var nfa = FA.ToLexer(new FA[] { nfaKeywords, nfaIdent, nfaWhitespace }, false, true);
var sw = new Stopwatch();
Console.Write(" minimizing");
sw.Start();
var dfa = FA.ToLexer(new FA[] { nfaKeywords, nfaIdent, nfaWhitespace }, true, true, new Progress<int>((i) => { if (0 == (i % 10000)) Console.Write("."); }));
sw.Stop();
Console.WriteLine("done in {0} seconds", sw.Elapsed.TotalSeconds.ToString());
Console.WriteLine("final: " + dfa.ToString("e"));
dfa.RenderToFile(@"..\..\..\lexer_dfa.png");
Console.WriteLine("NFA has {0} states. DFA has {1} states", nfa.FillClosure().Count, dfa.FillClosure().Count);
