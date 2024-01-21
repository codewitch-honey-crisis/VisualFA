﻿using VisualFA;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Diagnostics.SymbolStore;

// See https://aka.ms/new-console-template for more information
var exp = "/* foo */\r\n/*baz*/\r\n&%^ the quick /*bar */#(@*$//brown fox /* tricky */ jumped over the -10 $#(%*& lazy dog ^%$@@";
var commentStart = FA.Parse(@"\/\*", 0, false);
var commentEnd = FA.Parse(@"\*\/", 0, false);
var commentLine = FA.Parse(@"\/\/[^\r\n]*", 1, false);
var whitespace = FA.Parse(@"[ \r\n\t]+",2,false);
var syms = new string[] { "commentBlock", "commentLine", "whitespace" };
var lexer = FA.ToLexer(new FA[] { commentStart , commentLine, whitespace }, true);
var dgo = new FADotGraphOptions();
dgo.BlockEnds = new FA[] { commentEnd.ToMinimizedDfa() };
dgo.AcceptSymbolNames = new string[] { "block", "line", "space" };
lexer.RenderToFile(@"..\..\..\lexer_dfa.jpg",dgo);
var gopts = new FAGeneratorOptions();
gopts.GenerateTables = false;
gopts.GenerateTextReaderRunner = false;
gopts.ClassName = "CommentRunner";
gopts.Dependencies = FAGeneratorDependencies.UseRuntime;
gopts.Symbols = syms;
var ccu = lexer.Generate(new FA[] { commentEnd }, gopts);

CSharpCodeProvider cs = new CSharpCodeProvider();
var cgopts = new CodeGeneratorOptions();
cgopts.IndentString = "    ";
cgopts.BlankLinesBetweenMembers = false;
cgopts.VerbatimOrder = true;
using (var sw = new StreamWriter(@"..\..\..\CommentRunner.cs", false))
{
	cs.GenerateCodeFromCompileUnit(ccu, sw, cgopts);
}
Console.WriteLine("Hello, World!");
var stringRunner = lexer.Run(exp,new FA[] {commentEnd}) ;
foreach (var m in stringRunner)
{
	Console.WriteLine("{0}:{1} at {2}, {3}:{4}", m.SymbolId, m.Value.Replace("\r", "\\r").Replace("\n","\\n"), m.Position, m.Line, m.Column);

}
Console.WriteLine("---------------------");
var textRunner = lexer.Run(new StringReader(exp), new FA[] { commentEnd });
foreach (var m in textRunner)
{
	Console.WriteLine("{0}:{1} at {2}, {3}:{4}", m.SymbolId, m.Value.Replace("\r", "\\r").Replace("\n", "\\n"), m.Position, m.Line, m.Column);

}
Console.WriteLine("---------------------");
var genRunner = new CommentRunner();

genRunner.Set(exp);
foreach (var m in genRunner)
{
	Console.WriteLine("{0}:{1} at {2}, {3}:{4}", m.SymbolId, m.Value.Replace("\r", "\\r").Replace("\n", "\\n"), m.Position, m.Line, m.Column);
}

