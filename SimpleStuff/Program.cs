// See https://aka.ms/new-console-template for more information

using VisualFA;

var exp = "/* foo *//*baz*/ &%^ the quick /*bar */#(@*$//brown fox /* tricky */ jumped over the -10 $#(%*& lazy dog ^%$@@";
var commentStart = FA.Parse(@"\/\*", 0, false);
var commentEnd = FA.Parse(@"\*\/", 0, false);
var commentLine = FA.Parse(@"\/\/[^\n]*", 1, false);
var lexer_nfa = FA.ToLexer(new FA[] { commentStart, commentLine }, false, false);
var opts = new FADotGraphOptions();
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
