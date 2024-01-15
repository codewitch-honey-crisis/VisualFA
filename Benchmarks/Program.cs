using VisualFA;

using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
const char _ProgressBlock = '■';
const string _ProgressBack = "\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b";
const int _Iterations = 10000;
const int _Divisor = (_Iterations / 100) <= 0 ? 1 : (_Iterations / 100);
const int _Times = 5;
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
for(int i = 0;i<3;++i)
{
	//search += search;
}

var sb = new StringBuilder();
var delim = "";
foreach (var ex in exprs)
{
	sb.Append(delim);
	sb.Append("(?:");
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

for (var time = 0; time < _Times; ++time)
{
	Console.Write("Pass {0} of {1} ", time + 1, _Times);
	Console.WriteLine("-----------------------------------------------------------------");
	int mc = 0;
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
	
	Console.Write("FAStringRunner (proto): ");
	mc = 0;
	sw.Reset();
	_WriteProgressBar(0, false);
	for (int i = 0; i < _Iterations; ++i)
	{
		stringRunner.Reset();
		sw.Start();
		var match = stringRunner.NextMatch();
		while (match.SymbolId != -2)
		{
			++mc;
			match = stringRunner.NextMatch();
		}
		sw.Stop();
		_WriteProgressBar(i/_Divisor, true);

	}
	_WriteProgressBar(100, true);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);

	Console.Write("FATextReaderRunner: (proto) ");
	mc = 0;
	sw.Reset();
	_WriteProgressBar(0, false);
	for (int i = 0; i < _Iterations; ++i)
	{
		// can't reset the text runner
		textRunner.Set(new StringReader(search));
		sw.Start();
		var match = textRunner.NextMatch();
		while (match.SymbolId != -2)
		{
			++mc;
			match = textRunner.NextMatch();
		}
		sw.Stop();
		_WriteProgressBar(i/_Divisor, true);
	}
	_WriteProgressBar(100,true);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);
	
	stringTableRunner.Set(search);
	Console.Write("FAStringDfaTableRunner: ");
	mc = 0;
	sw.Reset();
	_WriteProgressBar(0, false);
	for (int i = 0; i < _Iterations; ++i)
	{
		stringTableRunner.Reset();
		sw.Start();
		var match = stringTableRunner.NextMatch();
		while (match.SymbolId != -2)
		{
			++mc;
			match = stringTableRunner.NextMatch();
		}
		sw.Stop();
		_WriteProgressBar(i / _Divisor, true);
	}
	_WriteProgressBar(100,true);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);
	
	stringTableRunner.Set(search);
	Console.Write("FATextReaderDfaTableRunner: ");
	mc = 0;
	sw.Reset();
	_WriteProgressBar(0, false);
	for (int i = 0; i < _Iterations; ++i)
	{
		// reset is not supported for text reader
		textTableRunner.Set(new StringReader(search));
		sw.Start();
		var match = textTableRunner.NextMatch();
		while (match.SymbolId != -2)
		{
			++mc;
			match = textTableRunner.NextMatch();
		}
		sw.Stop();
		_WriteProgressBar(i/_Divisor,true);
	}
	_WriteProgressBar(100,true);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);
	stringNfaRunner.Set(search);
	Console.Write("FAStringStateRunner (NFA): ");
	mc = 0;
	sw.Reset();
	_WriteProgressBar(0, false);
	for (int i = 0; i < _Iterations; ++i)
	{
		stringNfaRunner.Reset();
		sw.Start();
		var match = stringNfaRunner.NextMatch();
		while (match.SymbolId != -2)
		{
			++mc;
			match = stringNfaRunner.NextMatch();
		}
		sw.Stop();
		_WriteProgressBar(i/_Divisor, true);
	}
	_WriteProgressBar(100,true);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);
	stringCNfaRunner.Set(search);
	Console.Write("FAStringStateRunner (Compact NFA): ");
	mc = 0;
	sw.Reset();
	_WriteProgressBar(0, false);
	for (int i = 0; i < _Iterations; ++i)
	{
		stringCNfaRunner.Reset();
		sw.Start();
		var match = stringCNfaRunner.NextMatch();
		while (match.SymbolId != -2)
		{
			++mc;
			match = stringCNfaRunner.NextMatch();
		}
		sw.Stop();
		_WriteProgressBar(i/_Divisor, true);
	}
	_WriteProgressBar(100,true);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);
	textCNfaRunner.Set(new StringReader(search));
	Console.Write("FATextReaderStateRunner (Compact NFA): ");
	mc = 0;
	sw.Reset();
	_WriteProgressBar(0, false);
	for (int i = 0; i < _Iterations; ++i)
	{
		// no reset
		textCNfaRunner.Set(new StringReader(search));
		sw.Start();
		var match = textCNfaRunner.NextMatch();
		while (match.SymbolId != -2)
		{
			++mc;
			match = textCNfaRunner.NextMatch();
		}
		sw.Stop();
		_WriteProgressBar(i/_Divisor, true);
	}
	_WriteProgressBar(100, true);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);
	stringDfaRunner.Set(search);
	Console.Write("FAStringStateRunner (DFA): ");
	mc = 0;
	sw.Reset();
	_WriteProgressBar(0, false);
	for (int i = 0; i < _Iterations; ++i)
	{
		stringDfaRunner.Reset();
		sw.Start();
		var match = stringDfaRunner.NextMatch();
		while (match.SymbolId != -2)
		{
			++mc;
			match = stringDfaRunner.NextMatch();
		}
		sw.Stop();
		_WriteProgressBar(i/_Divisor,true);
	}
	_WriteProgressBar(100,true);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);
	textDfaRunner.Set(new StringReader(search));
	Console.Write("FATextReaderStateRunner (DFA): ");
	mc = 0;
	sw.Reset();
	_WriteProgressBar(0, false);
	for (int i = 0; i < _Iterations; ++i)
	{
		// no reset
		textDfaRunner.Set(new StringReader(search));
		sw.Start();
		var match = textDfaRunner.NextMatch();
		while (match.SymbolId != -2)
		{
			++mc;
			match = textDfaRunner.NextMatch();
		}
		sw.Stop();
		_WriteProgressBar(i/_Divisor, true);
	}
	_WriteProgressBar(100,true);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);
	compiledStringRunner.Set(search);
	Console.Write("FAStringRunner (Compiled): ");
	mc = 0;
	sw.Reset();
	_WriteProgressBar(0, false);
	for (int i = 0; i < _Iterations; ++i)
	{
		compiledStringRunner.Reset();
		sw.Start();
		var match = compiledStringRunner.NextMatch();
		while (match.SymbolId != -2)
		{
			++mc;
			match = compiledStringRunner.NextMatch();
		}
		sw.Stop();
		_WriteProgressBar(i/_Divisor, true);
	}
	_WriteProgressBar(100,true);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);
	compiledTextRunner.Set(new StringReader(search));
	Console.Write("FATextReaderRunner (Compiled): ");
	mc = 0;
	sw.Reset();
	_WriteProgressBar(0, false);
	for (int i = 0; i < _Iterations; ++i)
	{
		// no reset
		compiledTextRunner.Set(new StringReader(search));
		sw.Start();
		var match = compiledTextRunner.NextMatch();
		while (match.SymbolId != -2)
		{
			++mc;
			match = compiledTextRunner.NextMatch();
		}
		sw.Stop();
		_WriteProgressBar(i/_Divisor, true);
	}
	_WriteProgressBar(100,true);
	Console.WriteLine(" Found {0} matches in {1}ms", mc, sw.ElapsedMilliseconds);
	
}
