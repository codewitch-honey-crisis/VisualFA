### Visual FA Source Generator for C#

This package provides tokenizer/lexer generation facilities using source generator technology (C#9/.NET 6 and above)

Documentation
Visual FA Basics https://www.codeproject.com/Articles/5375797/Visual-FA-Part-1-Understanding-Finite-Automata
Source Generator Docs https://www.codeproject.com/Articles/5376805/Visual-FA-Part-4-Generating-matchers-and-lexers-wi

Usage
```cs
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
partial class CalcRunner : FAStringDfaTableRunner
{

}
```
After which you can do
```cs
var exp = "the 10 quick brown #@%$! foxes jumped over 1.5 lazy dogs";
var runner = new CalcRunner();
runner.Set(exp);
foreach (var match in runner)
{
    Console.WriteLine(match);
}
```
That will print the following to the *stdout*:
```
[SymbolId: 3, Value: "the", Position: 0 (1, 1)]
[SymbolId: 2, Value: " ", Position: 3 (1, 4)]
[SymbolId: 4, Value: "10", Position: 4 (1, 5)]
[SymbolId: 2, Value: " ", Position: 6 (1, 7)]
[SymbolId: 3, Value: "quick", Position: 7 (1, 8)]
[SymbolId: 2, Value: " ", Position: 12 (1, 13)]
[SymbolId: 3, Value: "brown", Position: 13 (1, 14)]
[SymbolId: 2, Value: " ", Position: 18 (1, 19)]
[SymbolId: -1, Value: "#@", Position: 19 (1, 20)]
[SymbolId: 9, Value: "%", Position: 21 (1, 22)]
[SymbolId: -1, Value: "$!", Position: 22 (1, 23)]
[SymbolId: 2, Value: " ", Position: 24 (1, 25)]
[SymbolId: 3, Value: "foxes", Position: 25 (1, 26)]
[SymbolId: 2, Value: " ", Position: 30 (1, 31)]
[SymbolId: 3, Value: "jumped", Position: 31 (1, 32)]
[SymbolId: 2, Value: " ", Position: 37 (1, 38)]
[SymbolId: 3, Value: "over", Position: 38 (1, 39)]
[SymbolId: 2, Value: " ", Position: 42 (1, 43)]
[SymbolId: 4, Value: "1.5", Position: 43 (1, 44)]
[SymbolId: 2, Value: " ", Position: 46 (1, 47)]
[SymbolId: 3, Value: "lazy", Position: 47 (1, 48)]
[SymbolId: 2, Value: " ", Position: 51 (1, 52)]
[SymbolId: 3, Value: "dogs", Position: 52 (1, 53)]
```
The base class of your lexer can be one of several values each providing a consistent interface, but different capabilities or implementations:

1. `FAStringRunner` - a compiled tokenizer/lexer that works on strings
2. `FATextReaderRunner` - a compiled tokenizer/lexer that works on TextReaders
3. `FAStringDfaTableRunner` - a table driven tokenizer/lexer that works on strings
4. `FATextReaderDfaTableRunner` - a table driven tokenizer/lexer that works on strings

The other alternative is to use `[FARule(...}]` on a partial method. This will create the runner for you, and implement its class behind the scenes

```cs
partial class Lexers
{
    // Declare a lexer. Here we specify the rules and the type of lexer
    // as indicated by FARule attributes and the return type
    // shared dependency code is automatically generated as needed.
    // It won't be generated if your code references VisualFA.
    [FARule(@"\/\*",Symbol="commentBlock",BlockEnd=@"\*\/")]
    [FARule(@"\/\/[^\n]*", Symbol = "lineComment")]
    [FARule(@"[ \t\r\n]+", Symbol = "whiteSpace")]
    [FARule(@"[A-Za-z_][A-Za-z0-9_]*",Symbol="identifier")]
    [FARule(@"(0|([1-9][0-9]*))((\.[0-9]+[Ee]\-?[1-9][0-9]*)?|\.[0-9]+)",Symbol="number")]
    [FARule(@"\+",Symbol = "plus")]
    [FARule(@"\-", Symbol = "minus")]
    [FARule(@"\*", Symbol = "multiply")]
    [FARule(@"\/", Symbol = "divide")]
    [FARule(@"%", Symbol = "modulo")]
    internal static partial FATextReaderDfaTableRunner Calc(TextReader text);
}
```
After which you can do
```cs
var exp = "the 10 quick brown #@%$! foxes jumped over 1.5 lazy dogs";
foreach (var match in Lexers.Calc(new StringReader(exp)))
{
    Console.WriteLine(match);
}
```
The same output will be produced as before

See the documentation for more details.