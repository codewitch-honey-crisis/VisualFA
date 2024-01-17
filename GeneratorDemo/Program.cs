using VisualFA;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

// See https://aka.ms/new-console-template for more information
var exp = "/* foo *//*baz*/ &%^ the quick /*bar */#(@*$//brown fox /* tricky */ jumped over the -10 $#(%*& lazy dog ^%$@@";
var commentStart = FA.Parse(@"\/\*", 0, false);
var commentEnd = FA.Parse(@"\*\/", 0, false);
//var commentLine = FA.Parse(@"\/\/[^\n]*", 1, false);
var lexer = FA.ToLexer(new FA[] { commentStart /*, commentLine*/ }, true);
var dgo = new FADotGraphOptions();
dgo.BlockEnds = new FA[] { commentEnd.ToMinimizedDfa() };
dgo.AcceptSymbolNames = new string[] { "block", "line", "space" };
lexer.RenderToFile(@"..\..\..\lexer_dfa.jpg",dgo);
var gopts = new FAGeneratorOptions();
gopts.GenerateTables = false;
gopts.GenerateTextReaderRunner = false;
gopts.ClassName = "CommentRunner";
gopts.Dependencies = FAGeneratorDependencies.UseRuntime;
var ccu = lexer.Generate(new FA[] { commentEnd }, gopts);

CSharpCodeProvider cs = new CSharpCodeProvider();
var cgopts = new CodeGeneratorOptions();
cgopts.IndentString = "    ";
cgopts.BlankLinesBetweenMembers = false;
cgopts.VerbatimOrder = true;
using(var sw = new StreamWriter(@"..\..\..\CommentRunner.cs", false))
{
	cs.GenerateCodeFromCompileUnit(ccu,sw,cgopts);
}
Console.WriteLine("Hello, World!");
var stringRunner = lexer.Run(exp,new FA[] {commentEnd}) ;
foreach (var m in stringRunner)
{
	Console.WriteLine("{0}:{1} at {2}", m.SymbolId, m.Value, m.Position);
}
Console.WriteLine("---------------------");
var genRunner = new CommentRunner();
genRunner.Set(exp);
foreach (var m in genRunner)
{
	Console.WriteLine("{0}:{1} at {2}", m.SymbolId, m.Value, m.Position);
}

