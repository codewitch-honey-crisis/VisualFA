# Visual FA Compiler
The compiler for the Visual FA a Unicode enabled DFA regular expression engine

There's a series of articles on it starting here:
https://www.codeproject.com/Articles/5375797/Visual-FA-Part-1-Understanding-Finite-Automata


### Performance 

Microsoft Regex "Lexer": [■■■■■■■■■■] 100% Found 220000 matches in 33ms

Microsoft Regex compiled "Lexer": [■■■■■■■■■■] 100% Found 220000 matches in 20ms

FAStringRunner (Compiled): [■■■■■■■■■■] 100% Found 220000 matches in 7ms

FATextReaderRunner (Compiled): [■■■■■■■■■■] 100% Found 220000 matches in 12ms


### Use

```cs
string exp = @"[A-Z_a-z][A-Z_a-z0-9]*|0|\-?[1-9][0-9]*";
FA nfa = FA.Parse(exp);
// turn it into an optimized DFA
FA mdfa = nfa.ToMinimizedDfa();

// Make sure to reference VisualFA.Compiler:

FARunner compiledStr = mdfa.CompileString();
compiledStr.Set(text);
//or
FARunner compiledRdr = mdfa.CompileTextReader();
compiledRdr.Set(text)
// to lex (as above)
foreach(FAMatch match in <compiledStr/compiledRdr>) {
	Console.WriteLine("{0}:{1}",match.Position,match.Value);
}

