# Visual FA
Visual FA a Unicode enabled DFA regular expression engine

There's a series of articles on it starting here:
https://www.codeproject.com/Articles/5375797/Visual-FA-Part-1-Understanding-Finite-Automata

### License Notice

Portions of this code in the file FA.cs were derived from code created by other authors, and similarly licensed under MIT
- Fare, Copyright (C) 2013 Nikos Baxevanis http://github.com/moodmosaic/Fare/ itself ported from 
- dk.brics.automaton, Copyright (c) 2001-2011 Anders Moeller http://www.brics.dk/automaton/

### Build Tools Notice

The two executables in the root folder are necessary for the build process.
They are safe decompilable .NET assemblies

CSBrick - https://github.com/codewitch-honey-crisis/CSBrick

DSlang - https://github.com/codewitch-honey-crisis/Deslang


### Performance 

Microsoft Regex "Lexer": [■■■■■■■■■■] 100% Found 220000 matches in 33ms

Microsoft Regex compiled "Lexer": [■■■■■■■■■■] 100% Found 220000 matches in 20ms

FAStringRunner (proto): [■■■■■■■■■■] 100% Found 220000 matches in 7ms

FATextReaderRunner: (proto) [■■■■■■■■■■] 100% Found 220000 matches in 12ms

FAStringDfaTableRunner: [■■■■■■■■■■] 100% Found 220000 matches in 10ms

FATextReaderDfaTableRunner: [■■■■■■■■■■] 100% Found 220000 matches in 14ms

FAStringStateRunner (NFA): [■■■■■■■■■■] 100% Found 220000 matches in 144ms

FAStringStateRunner (Compact NFA): [■■■■■■■■■■] 100% Found 220000 matches in 42ms

FATextReaderStateRunner (Compact NFA): [■■■■■■■■■■] 100% Found 220000 matches in 47ms

FAStringStateRunner (DFA): [■■■■■■■■■■] 100% Found 220000 matches in 11ms

FATextReaderStateRunner (DFA): [■■■■■■■■■■] 100% Found 220000 matches in 15ms

FAStringRunner (Compiled): [■■■■■■■■■■] 100% Found 220000 matches in 7ms

FATextReaderRunner (Compiled): [■■■■■■■■■■] 100% Found 220000 matches in 12ms


### Use

```cs
string exp = @"[A-Z_a-z][A-Z_a-z0-9]*|0|\-?[1-9][0-9]*";
string text = "the quick brown fox jumped over the -10 lazy #@!*$ dog";
// lex a string
foreach (FAMatch match in FA.Parse(exp).Run(text))
{
	Console.WriteLine("{0} at {1}", match.Value, match.Position);
}
// *or* parse it into AST
RegexExpression ast = RegexExpression.Parse(exp);
// visit the AST
ast.Visit((parent, expr) => { Console.WriteLine(expr.GetType().Name +" "+ expr); return true; });
// turn it into a state machine
// (with expanded epsilons)
FA nfa = ast.ToFA(0,false);

// compact the expanded
// epsilons (if desired)
nfa.Compact();

// turn it into a DFA
FA dfa = nfa.ToDfa();

// *or* turn it into an optimized DFA
FA mdfa = nfa.ToMinimizedDfa();

// FARunner has MatchNext()/Reset()
// and IEnumerable<FAMatch>

// If you reference FA.Compiler:

FARunner compiledStr = mdfa.CompileString();
compiledStr.Set(text);
//or
FARunner compiledRdr = mdfa.CompileTextReader();
compiledRdr.Set(text)
// to lex (as above)
foreach(FAMatch match in <compiledStr/compiledRdr>) {
	Console.WriteLine("{0}:{1}",match.Position,match.Value);
}


// for table DFA

int[] table = mdfa.ToArray();

// Note: You can create table NFAs as well
// but the following functions require a 
// DFA table

// to lex (as above)

foreach(FAMatch match in FA.Run(text,table)) {
	Console.WriteLine("{0}:{1}",match.Position,match.Value);
}

// to recreate an FA from an array
// (works with NFAs too)
FA newFA = FA.FromArray(table);

// on FAs if you have GraphViz installed
// from https://graphviz.org and in
// your PATH:

// can be dot/jpg/png/svg and more
newFA.RenderToFile("my_image.png");

using(Stream stream = nfa.RenderToStream("jpg")) {
	...
}

// If you reference FA.Generator and the CodeDom
// you can generate source code, potentially
// dependency free (when GenerateSharedCode is true)

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
	opts.ClassName = "MyLexer";
	opts.Dependencies = FAGeneratorDependencies.GenerateSharedCode;
	opts.GenerateTables = true;
	var runner = lexer.Generate(new FA[] { null, null, null, commentEnd }, opts);
	var cprov = new CSharpCodeProvider();
	var copt = new CodeGeneratorOptions();
	cprov.GenerateCodeFromCompileUnit(runner, sw, copt);
}

```

### Projects

- VisualFA - The main project (.NET 6)
- VisualFA.DNF - The main project (.NET Framework 4.8)
- VisualFA.Compiler - The compiler (.NET 6)
- VisualFA.Compiler.DNF - The compiler (.NET Framework 4.8)
- VisualFA.Generator - The code generator (.NET 6)
- VisualFA.Generator.DNF - The code generator (.NET Framework 4.8)
- LexGen - A command line tool for generating regex lexer source code (.NET 6)
- LexGen.DNF - The same tool (.NET Framework 4.8)
- FsmExplorer - A visual app for stepping through regex state machines (.NET Framework 4.8, Windows only)
- Benchmarks - Runs several benchmarks (.NET 7)
- GeneratorDemo - Demonstrates source code generation
- GeneratorDemoVB - Same as above but for VB.NET instead of C#
- SimpleStuff - Various demonstrations
- ArticleImages - Code I used to generate codeproject.com content
