// See https://aka.ms/new-console-template for more information

using System.Runtime.CompilerServices;
using System.CodeDom;
using VisualFA;
using System.Security.Cryptography.X509Certificates;

var opts = new FADotGraphOptions();

opts.HideAcceptSymbolIds = false;
FA commentBlock = FA.Parse(@"\/\*", 0);
FA commentBlockEnd = FA.Parse(@"\*\/");
FA commentLine = FA.Parse(@"\/\/[^\n]*", 1);
FA wspace = FA.Parse("[ \\t\\r\\n]+", 2);
FA ident = FA.Parse("[A-Za-z_][A-Za-z0-9_]*", 3);
FA num= FA.Parse(@"(0|-?([1-9][0-9]*))((\.[0-9]+[Ee]\-?[1-9][0-9]*)?|\.[0-9]+)", 4);
FA op = FA.Parse(@"[\-\+\*\/\=]",5);
FA[] tokens = new FA[] { 
	commentBlock, 
	commentLine, 
	wspace, 
	ident, 
	num,
	op
};
string[] syms = new string[] {
	"commentBlock",
	"commentLine",
	"wspace",
	"ident",
	"num",
	"op"
};
// our tokens will be minimized by ToLexer
// we must minimize our block ends ourselves.
FA[] blockEnds = new FA[] { 
	commentBlockEnd.ToMinimizedDfa()
};
// ToLexer will minimize its tokens and create
// a DFA lexer by default
FA lexer = FA.ToLexer(tokens,true);

//lexer = lexer.ToDfa();
// NOTE: never call ToMinimizedDfa() on a lexer machine
// as it will lose its distinct accept states
// ToDfa() is okay, and ToMinimizedDfa() is
// usually okay on states other than the root.

// create an expanded NFA
// small bug in rendering the movement through
// this expression (NFA) w/ Graphviz. Not easy to fix
var sexp = "(ba[rz])*foofoofoo";
FA testFa = FA.Parse(sexp, 0) ;
testFa.RenderToFile(@"..\..\..\testFa.png");
Console.WriteLine("var nfa = FA.Parse(@\"{0}\");",sexp);
Console.WriteLine("nfa.ToString(\"e\") = @\"{0}\"", testFa.ToString("e"));
Console.WriteLine("nfa.ToString(\"r\") = @\"{0}\"", testFa.ToString("r"));

var mdfa = testFa.ToMinimizedDfa();
mdfa.RenderToFile(@"..\..\..\mdfa.png");
testFa.SetIds();
Console.WriteLine(testFa.ToString());
var fexp = mdfa.ToString("e");
Console.WriteLine(fexp);
var mdfa2 = FA.Parse(fexp).ToMinimizedDfa();
mdfa2.RenderToFile(@"..\..\..\mdfa2.png");

Console.WriteLine("q0 trans count {0}", testFa.Transitions.Count);
// we're going to show the
// subset construction in
// the graph. ToMinimuzedDfa()
// doesn't preserve that.
// So use ToDfa();
FA dfa = testFa.ToDfa();
FADotGraphOptions dgo = new FADotGraphOptions();
// the image is wide for this 
// website. Let's make it a 
// little less wide by making
// it top to bottom instead of
// left to right
dgo.Vertical = true;
// this expression does not use
// blockEnds. If we did, we'd
// put the block end array here
dgo.BlockEnds = null;
// let's show the NFA together
// with the DFA
dgo.DebugShowNfa = true;
// and so we give it
// the source NFA to use
dgo.DebugSourceNfa = testFa;
// we don't need to show the accept 
// symbols. That's for lexers
dgo.HideAcceptSymbolIds = true;
// this is also for lexers
// it takes a string[] of names
// that map to the accept symbol
// of the same index in the array
// it works like blockEnds in 
// terms of how it associates
dgo.AcceptSymbolNames = null;
// let's do something fun
// we can graph movement
// through the machine by providing
// an input string. 
dgo.DebugString = "fo";
// finally, render it.
dfa.RenderToFile(@"..\..\..\dfa_subset.jpg", dgo);
dgo.BlockEnds = blockEnds;
dgo.DebugString = null;
dgo.DebugSourceNfa = null;
dgo.DebugShowNfa = false;
dgo.AcceptSymbolNames = syms;
dgo.HideAcceptSymbolIds = false;
lexer.RenderToFile(@"..\..\..\lexer_dfa.jpg", dgo);


string tolex = "/* example lex */" + Environment.NewLine +
	"var a = 2 + 2" + Environment.NewLine +
	"print a";

foreach(var token in lexer.Run(tolex,blockEnds)) {
	Console.WriteLine("{0}:{1} at position {2}",token.SymbolId,token.Value,token.Position);
}

var exp = "/* foo *//*baz*/ &%^ the quick /*bar */#(@*$//brown fox /* tricky */ jumped over the -10 $#(%*& lazy dog ^%$@@";
var commentStart = FA.Parse(@"\/\*", 0, false);
var commentEnd = FA.Parse(@"\*\/", 0, false);
commentLine = FA.Parse(@"\/\/[^\n]*", 1, false);
var lexer_nfa = FA.ToLexer(new FA[] { commentStart, commentLine }, false, false);
opts.BlockEnds = new FA[] { commentEnd.Clone() };
opts.AcceptSymbolNames = new string[] { "blockComment","lineComment" };
opts.Vertical = true;
lexer_nfa.RenderToFile(@"..\..\..\clexer_nfa.jpg", opts);
lexer_nfa.SetIds();
var lexer_cnfa = lexer_nfa.Clone();
lexer_cnfa.Compact();
foreach (var be in opts.BlockEnds)
{
	if (be != null)
	{
		be.Compact();
	}
}
lexer_cnfa.RenderToFile(@"..\..\..\clexer_compact_nfa.jpg", opts);
opts.BlockEnds = new FA[] { commentEnd.Clone() };
for (var i = 0; i < opts.BlockEnds.Length; i++)
{
	var be = opts.BlockEnds[i];
	if (be != null)
	{
		opts.BlockEnds[i] = be.ToDfa();
	}
}
var lexer_dfa = lexer_nfa.ToDfa();
lexer_dfa.RenderToFile(@"..\..\..\clexer_dfa.jpg", opts);
opts.BlockEnds = new FA[] { commentEnd.Clone() };
for (var i = 0; i < opts.BlockEnds.Length; i++)
{
	var be = opts.BlockEnds[i];
	if (be != null)
	{
		opts.BlockEnds[i] = be.ToMinimizedDfa();
	}
}
var lexer_mdfa = FA.ToLexer(new FA[] { commentStart, commentLine }, true);
lexer_mdfa.RenderToFile(@"..\..\..\clexer_minimized_dfa.jpg", opts);

var mdfa_table = lexer_mdfa.ToArray();
var nfa_runner = lexer_nfa.Run(exp, new FA[] { commentEnd.Clone() });
foreach (var m in nfa_runner)
{
	if (m.IsSuccess)
	{
		Console.WriteLine("{0} at {1}", m.Value, m.Position);
	}
}
var cec = commentEnd.Clone();
cec.Compact();
var cnfa_runner = lexer_cnfa.Run(new StringReader(exp), new FA[] {  cec });
var table_runner = FA.Run(exp, mdfa_table,new int[][] { commentEnd.ToMinimizedDfa().ToArray() });
foreach (var m in cnfa_runner)
{
	//Console.WriteLine("{0}:{1} at {2}", m.SymbolId, m.Value, m.Position);
}
//Console.WriteLine("------------------------");
var stringProto = new CommentFAStringRunner();
stringProto.Set(exp);
foreach (var m in stringProto)
{
//	Console.WriteLine("{0}:{1} at {2}", m.SymbolId, m.Value, m.Position);
}
var textProto = new CommentFATextRunner();
textProto.Set(new StringReader(exp));
foreach (var m in textProto)
{
	//Console.WriteLine("{0}:{1} at {2}", m.SymbolId, m.Value, m.Position);
}


var compiledStringRunner = lexer_mdfa.Run(exp, new FA[] { cec.ToMinimizedDfa() },true);
foreach (var m in compiledStringRunner)
{
	Console.WriteLine("{0}:{1} at {2}", m.SymbolId, m.Value, m.Position);
}
Console.WriteLine("-----------------");
var compiledTextRunner = lexer_mdfa.Run(new StringReader(exp), new FA[] { cec.ToMinimizedDfa() },true);
foreach (var m in compiledTextRunner)
{
	Console.WriteLine("{0}:{1} at {2}", m.SymbolId, m.Value, m.Position);
}

//var c = new CommentFAStringRunner();
