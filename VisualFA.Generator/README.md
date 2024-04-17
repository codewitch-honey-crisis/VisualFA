# Visual FA Generator

Adds language independent source generation to Visual FA

Example:

```cs
var file = "MyLexer.cs";
using(var sw = new StreamWriter(file, false))
{					
	var ident = FA.Parse("[A-Z_a-z][0-9A-Z_a-z]*", 0, false);
	var num = FA.Parse("0|-?[1-9][0-9]*", 1, false);
	var ws = FA.Parse("[ ]+", 2, false);
	var commentStart = FA.Parse(@"\/\*", 3, false);
	var commentEnd = FA.Parse(@"\*\/", 3, false);
	var lexer = FA.ToLexer(new FA[] { ident, num, ws, commentStart }, true);
	var opts = new FAGeneratorOptions();
	opts.StringRunnerClassName = "MyLexer";
	opts.Dependencies = FAGeneratorDependencies.GenerateSharedCode;
	opts.GenerateTables = true;
	opts.Symbols = new string[] { "ident","num","ws","comment" };
	var runner = lexer.Generate(new FA[] { null, null, null, commentEnd }, opts);
	var cprov = new CSharpCodeProvider();
	var copt = new CodeGeneratorOptions();
	cprov.GenerateCodeFromCompileUnit(runner, sw, copt);
}

```