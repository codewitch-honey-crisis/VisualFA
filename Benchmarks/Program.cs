﻿using VisualFA;

using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
const char _ProgressBlock = '■';
const string _ProgressBack = "\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b";
const int _Iterations = 10000;
const int _Divisor = (_Iterations / 100) <= 0 ? 1 : (_Iterations / 100);
StringBuilder _ProgressBuffer = new StringBuilder();

void _WriteProgressBar(int percent, bool update, TextWriter? output = null)
{
	if(output==null) output = Console.Out;
	_ProgressBuffer.Clear();
	if (update)
		_ProgressBuffer.Append(_ProgressBack);
	_ProgressBuffer.Append("[");
	var p = (int)((percent / 10f) + .5f);
	for (var i = 0; i < 10; ++i)
	{
		if (i >= p)
			_ProgressBuffer.Append(' ');
		else
			_ProgressBuffer.Append(_ProgressBlock);
	}
	_ProgressBuffer.Append(string.Format("] {0,3:##0}%", percent));
	output.Write(_ProgressBuffer.ToString());
}

int _RunBench(FARunner runner, string search, Stopwatch sw)
{
	var mc = 0;
	sw.Reset();
	_WriteProgressBar(0, false);
	for (int i = 0; i < _Iterations; ++i)
	{
		if (runner is FATextReaderRunner)
		{
			((FATextReaderRunner)runner).Set(new StringReader(search));
		} else
		{
			((FAStringRunner)runner).Set(search);
		}
		sw.Start();
		var match = runner.NextMatch();
		while (match.SymbolId != -2)
		{
			++mc;
			match = runner.NextMatch();
		}
		sw.Stop();
		_WriteProgressBar(i / _Divisor, true);
	}
	_WriteProgressBar(100, true);
	return mc;
}

#if DEBUG
Console.WriteLine("Running debug build. Results will be noticably slower.");
Console.WriteLine();
#endif

string[] exprs =
		{
			"[A-Za-z_][A-Za-z0-9_]*",
			"0|\\-?[1-9][0-9]*(\\.[0-9]+([Ee]\\-?[1-9][0-9]*)?)?",
			"[ \\t\\r\\n]+"
		};
int q = 0;
var fas = new FA[exprs.Length];
foreach(var ex in exprs)
{
	fas[q] = FA.Parse(ex, q,false);
	fas[q++].SetIds();
}
var lexerDfa = FA.ToLexer(fas);
lexerDfa.SetIds();
var lexerNfa = FA.ToLexer(fas,false,false);
lexerNfa.SetIds();
var lexerCNfa = lexerNfa.Clone();
lexerCNfa.Compact();
lexerCNfa.SetIds();
var dfaTable = lexerDfa.ToArray();
var stringTableRunner = new FAStringDfaTableRunner(dfaTable);
var textTableRunner = new FATextReaderDfaTableRunner(dfaTable);
var stringNfaRunner = new FAStringStateRunner(lexerNfa);
var stringCNfaRunner = new FAStringStateRunner(lexerCNfa);
var textCNfaRunner = new FATextReaderStateRunner(lexerCNfa);
var stringDfaRunner = new FAStringStateRunner(lexerDfa);
var textDfaRunner = new FATextReaderStateRunner(lexerDfa);
string search = "the quick brown fox jumped over the lazy dog 23.5 times ";

var sb = new StringBuilder();
var delim = "";
foreach (var ex in exprs)
{
	sb.Append(delim);
	sb.Append("(");
	sb.Append(ex);
	sb.Append(")");
	delim = "|";
}
var expr = sb.ToString();
Regex rx = new Regex(expr, RegexOptions.CultureInvariant);
Regex rxc = new Regex(expr, RegexOptions.Compiled | RegexOptions.CultureInvariant);
BenchFAStringRunner stringRunner = new BenchFAStringRunner();
BenchFATextReaderRunner textRunner = new BenchFATextReaderRunner();
var compiledStringRunner = lexerDfa.CompileString();
var compiledTextRunner = lexerDfa.CompileTextReader();
stringRunner.Set(search);
textRunner.Set(new StringReader(search));
compiledStringRunner.Set(search);
var pass = 0;
Console.WriteLine("Press any key to exit (will finish current pass)...");
while(!Console.KeyAvailable)
{
	++pass;
	Console.Write("Pass {0}", pass);
	Console.WriteLine("-----------------------------------------------------------------");
	var mc = 0;
	var m = rx.Match(search);
	while (m.Success)
	{
		m = m.NextMatch();
	}
	Stopwatch sw = new Stopwatch();
	Console.Write("Microsoft Regex \"Lexer\": ");
	_WriteProgressBar(0, false);
	for (int i = 0; i < _Iterations; ++i)
	{
		sw.Start();
		m = rx.Match(search);
		while (m.Success)
		{
			++mc;
			m = m.NextMatch();
		}
		sw.Stop();
		_WriteProgressBar(i / _Divisor, true);
	}
	_WriteProgressBar(100, true);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);
	
	mc = 0;
	m = rxc.Match(search);
	while (m.Success)
	{
		m = m.NextMatch();
	}
	sw.Reset();
	Console.Write("Microsoft Regex compiled \"Lexer\": ");
	_WriteProgressBar(0, false);
	for (int i = 0; i < _Iterations; ++i)
	{
		sw.Start();
		m = rxc.Match(search);
		while (m.Success)
		{
			++mc;
			m = m.NextMatch();
		}
		sw.Stop();
		_WriteProgressBar(i/_Divisor,true);
	}
	_WriteProgressBar(100, true);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);
	
	Console.Write("FAStringRunner (generated): ");
	_RunBench(stringRunner, search, sw);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);

	Console.Write("FATextReaderRunner: (generated) ");
	_RunBench(textRunner, search, sw);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);
	
	Console.Write("FAStringDfaTableRunner: ");
	_RunBench(stringTableRunner, search, sw);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);

	Console.Write("FATextReaderDfaTableRunner: ");
	_RunBench(textTableRunner, search, sw);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);
	
	Console.Write("FAStringStateRunner (NFA): ");
	_RunBench(stringNfaRunner, search, sw);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);
	
	Console.Write("FAStringStateRunner (Compact NFA): ");
	_RunBench(stringCNfaRunner, search, sw);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);
	
	Console.Write("FATextReaderStateRunner (Compact NFA): ");
	_RunBench(textCNfaRunner, search, sw);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);
	
	Console.Write("FAStringStateRunner (DFA): ");
	_RunBench(stringDfaRunner, search, sw);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);
	
	Console.Write("FATextReaderStateRunner (DFA): ");
	_RunBench(textDfaRunner, search, sw);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);
	
	Console.Write("FAStringRunner (Compiled): ");
	_RunBench(compiledStringRunner, search, sw);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);
	
	Console.Write("FATextReaderRunner (Compiled): ");
	_RunBench(compiledTextRunner, search, sw);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);
}
