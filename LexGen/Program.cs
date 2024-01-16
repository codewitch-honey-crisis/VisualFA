using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using VisualFA;
using System.CodeDom.Compiler;

namespace LexGen
{
	public static class Program
	{
		static readonly string CodeBase = _GetCodeBase();
		static readonly string Filename = Path.GetFileName(CodeBase);
		static readonly string Name = _GetName();

		const string _ProgressTwirl = "-\\|/";
		const char _ProgressBlock = '■';
		const string _ProgressBack = "\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b";
		const string _ProgressBackOne = "\b";
		static readonly StringBuilder _ProgressBuffer = new StringBuilder();
		public static int Run(string[] args,TextReader stdin, TextWriter stdout, TextWriter stderr)
		{
			int result = -1;
			// app parameters
			string inputfile = null;
			string outputfile = null;
			string codeclass = null;
			string codelanguage = null;
			string codenamespace = null;
			string nfagraph = null;
			string dfagraph = null;
			bool ignorecase = false;
			bool noshared = false;
			bool ifstale = false;
			bool tables = false;
			bool runtime = false;
			bool staticprogress = false;
#if FALIB_SPANS
			bool nospans = false;
#endif
			bool textreader = false;
			bool vertical = false;
			int dpi = 0;
			// our working variables
			TextReader input = null;
			TextWriter output = null;
#if !DEBUG
			bool parsedArgs = false;
#endif
			try
			{
				if (0 == args.Length)
				{
					_PrintUsage(stderr);
					result = -1;
				}
				else if (args[0].StartsWith("/"))
				{
					throw new ArgumentException("Missing input file.");
				}
				else
				{
					// process the command line args
					inputfile = args[0];
					for (var i = 1; i < args.Length; ++i)
					{
						switch (args[i].ToLowerInvariant())
						{
							case "/output":
								if (args.Length - 1 == i) // check if we're at the end
									throw new ArgumentException(string.Format("The parameter \"{0}\" is missing an argument", args[i].Substring(1)));
								++i; // advance 
								outputfile = args[i];
								break;
							case "/class":
								if (args.Length - 1 == i) // check if we're at the end
									throw new ArgumentException(string.Format("The parameter \"{0}\" is missing an argument", args[i].Substring(1)));
								++i; // advance 
								codeclass = args[i];
								break;
							case "/language":
								if (args.Length - 1 == i) // check if we're at the end
									throw new ArgumentException(string.Format("The parameter \"{0}\" is missing an argument", args[i].Substring(1)));
								++i; // advance 
								codelanguage = args[i];
								break;
							case "/namespace":
								if (args.Length - 1 == i) // check if we're at the end
									throw new ArgumentException(string.Format("The parameter \"{0}\" is missing an argument", args[i].Substring(1)));
								++i; // advance 
								codenamespace = args[i];
								break;
							case "/tables":
								tables = true;
								break;
#if FALIB_SPANS
							case "/nospans":
								nospans = true;
								break;
#endif
							case "/textreader":
								textreader = true;
								break;
							case "/nfagraph":
								if (args.Length - 1 == i) // check if we're at the end
									throw new ArgumentException(string.Format("The parameter \"{0}\" is missing an argument", args[i].Substring(1)));
								++i; // advance 
								nfagraph = args[i];
								break;
							case "/dfagraph":
								if (args.Length - 1 == i) // check if we're at the end
									throw new ArgumentException(string.Format("The parameter \"{0}\" is missing an argument", args[i].Substring(1)));
								++i; // advance 
								dfagraph = args[i];
								break;
							case "/ignorecase":
								ignorecase = true;
								break;
							case "/noshared":
								noshared = true;
								break;
							case "/runtime":
								runtime= true;
								break;
							case "/ifstale":
								ifstale = true;
								break;
							case "/vertical":
								vertical = true;
								break;
							case "/staticprogress":
								staticprogress = true;
								break;
							case "/dpi":
								if (args.Length - 1 == i) // check if we're at the end
									throw new ArgumentException(string.Format("The parameter \"{0}\" is missing an argument", args[i].Substring(1)));
								++i; // advance 
								dpi = int.Parse(args[i], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
								break;
							default:
								throw new ArgumentException(string.Format("Unknown switch {0}", args[i]));
						}
						if (dpi != 0 && (dfagraph == null && nfagraph == null))
						{
							throw new ArgumentException("<dpi> was specified but no GraphViz graph was indicated.", "/dpi");
						}
						if (vertical && (dfagraph == null && nfagraph == null))
						{
							throw new ArgumentException("<vertical> was specified but no GraphViz graph was indicated.", "/dpi");
						}
#if FALIB_SPANS
						if (nospans && textreader)
						{
							throw new ArgumentException("<nospans> and <textreader> cannot both be specified since they conflict.");
						}
#endif
#if !DEBUG
					parsedArgs = true;
#endif

					}
					var dotopts = new FADotGraphOptions();
					if (dpi != 0)
					{
						dotopts.Dpi = dpi;
					}
					// now build it
					if (string.IsNullOrEmpty(codeclass))
					{
						// default we want it to be named after the code file
						// otherwise we'll use inputfile
						if (null != outputfile)
							codeclass = Path.GetFileNameWithoutExtension(outputfile);
						else
							codeclass = Path.GetFileNameWithoutExtension(inputfile);
					}
					if (string.IsNullOrEmpty(codelanguage))
					{
						if (!string.IsNullOrEmpty(outputfile))
						{
							codelanguage = Path.GetExtension(outputfile);
							if (codelanguage.StartsWith("."))
								codelanguage = codelanguage.Substring(1);
						}
						if (string.IsNullOrEmpty(codelanguage))
							codelanguage = "cs";
					}
					var stale = true;
					if (ifstale && null != outputfile)
					{
						stale = _IsStale(inputfile, outputfile);
						if (!stale)
							stale = _IsStale(CodeBase, outputfile);
					}
					if (!stale)
					{
						stderr.WriteLine("{0} skipped building {1} because it was not stale.", Name, outputfile);
					}
					else
					{
						if (null != outputfile)
							stderr.WriteLine("{0} is building file: {1}", Name, outputfile);
						else
							stderr.WriteLine("{0} is building tokenizer.", Name);
						input = new StreamReader(inputfile);
						var rules = new List<LexRule>();
						string line;
						while (null != (line = input.ReadLine()))
						{
							var lc = LexContext.Create(line);
							lc.TrySkipCCommentsAndWhiteSpace();
							if (-1 != lc.Current)
								rules.Add(LexRule.Parse(lc));
						}
						input.Close();
						input = null;
						LexRule.FillRuleIds(rules);
						rules.Sort(new Comparison<LexRule>((LexRule lhs, LexRule rhs) => {
							int cmp = lhs.Id.CompareTo(rhs.Id);
							return cmp;
						}));

						var symmap = new Dictionary<int, string>();
						for (int i = 0; i < rules.Count; ++i)
						{
							symmap.Add(rules[i].Id, rules[i].Expression);
						}
						FA[] lexerFas = _BuildLexerFAs(rules, ignorecase, inputfile, stderr);
						var symbolTable = _BuildSymbolTable(rules);
						var symids = new int[symbolTable.Length];
						for (var i = 0; i < symbolTable.Length; ++i)
							symids[i] = i;
						var blockEnds = _BuildBlockEnds(rules, ignorecase, inputfile);

						dotopts.BlockEnds = blockEnds;
						dotopts.AcceptSymbolNames = symbolTable;
						dotopts.Vertical = vertical;
						var lexer = new FA();
						foreach (var lfa in lexerFas)
						{
							lexer.AddEpsilon(lfa, false);
						}
						if (nfagraph != null)
						{
							/*foreach (var bfa in blockEnds)
							{
								if (bfa != null)
								{
									bfa.Compact();
								}
							}
							lexer.Compact();*/
							lexer.RenderToFile(nfagraph, dotopts);
						}
						var total = lexerFas.Length;
						foreach (var be in blockEnds)
						{
							if (be != null)
							{
								++total;
							}
						}
						var current = 0;
						stderr.Write("Converting to DFA ");
						if (!staticprogress)
						{
							_WriteProgressBar(0, false, stderr);
						}
						var dfaLexer = new FA();
						for (int i = 0; i < lexerFas.Length; ++i)
						{
							var mfa = lexerFas[i].ToMinimizedDfa();
							lexer.AddEpsilon(mfa);
							++current;
							if (!staticprogress)
							{
								_WriteProgressBar((int)(((double)current / (double)total) * 100.0d), true, stderr);
							}
							else
							{
								stderr.Write(".");
							}
						}
						for (int i = 0; i < blockEnds.Length; ++i)
						{
							var be = blockEnds[i];
							if (be != null)
							{
								blockEnds[i] = be.ToMinimizedDfa();
								++current;
								if (!staticprogress)
								{
									_WriteProgressBar((int)(((double)current / (double)total) * 100.0d), true, stderr);
								}
								else
								{
									stderr.Write(".");
								}
							}
						}
						if (!staticprogress)
						{
							_WriteProgressBar(100, true, stderr);
						}
						stderr.WriteLine(" Done!");
						stderr.Write("Finalizing lexer DFA ");
						var reporter = new Reporter(stderr, /*staticprogress*/true);
						lexer = lexer.ToDfa(reporter);
						stderr.WriteLine(" Done!");
						if (dfagraph != null)
						{
							lexer.RenderToFile(dfagraph, dotopts);
						}
						var genopts = new FAGeneratorOptions();
						if(runtime)
						{
							genopts.Dependencies = FAGeneratorDependencies.UseRuntime;
						} else if(noshared)
						{
							genopts.Dependencies = FAGeneratorDependencies.None;
						} else
						{
							genopts.Dependencies = FAGeneratorDependencies.GenerateSharedCode;
						}
						genopts.ClassName = codeclass;
						genopts.Namespace = codenamespace;
#if FALIB_SPANS
						genopts.UseSpans = !nospans;
#endif
						genopts.GenerateTextReaderMatcher = textreader;

						genopts.GenerateTables = tables;

						stderr.WriteLine("Generating code...");

						var ccu = lexer.Generate(blockEnds, genopts);
						//	_GenerateSymbolConstants(td, symmap, symbolTable);
						stderr.WriteLine();
						var prov = CodeDomProvider.CreateProvider(codelanguage);
						var opts = new CodeGeneratorOptions();
						opts.BlankLinesBetweenMembers = false;
						opts.VerbatimOrder = true;
						if (null == outputfile)
							output = stdout;
						else
						{
							// open the file and truncate it if necessary
							var stm = File.Open(outputfile, FileMode.Create);
							stm.SetLength(0);
							output = new StreamWriter(stm);
						}
						prov.GenerateCodeFromCompileUnit(ccu, output, opts);

						stderr.WriteLine();

					}
				}
				result = 0;
			}
			// we don't like to catch in debug mode
#if !DEBUG
			catch (Exception ex)
			{
				if (parsedArgs)
				{
					result = -1;
				}
				else
				{
					result = _ReportError(ex, stderr);
				}
			}
#endif
			finally
			{
				// close the input file if necessary
				if (null != input)
					input.Close();
				// close the output file if necessary
				if (null != outputfile && null != output)
					output.Close();
			}
			return result;
		}
#if !DEBUG
		// do our error handling here (release builds)
		static int _ReportError(Exception ex, TextWriter stderr)
		{
			_PrintUsage(stderr);
			stderr.WriteLine("Error: {0}", ex.Message);
			return -1;
		}
#endif
		static string _GetCodeBase()
		{
			try
			{
				return Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
			}
			catch
			{
				return Path.Combine(Environment.CurrentDirectory, "lexgen.exe");
			}
		}
		static string _GetName()
		{
			try
			{
				foreach (var attr in Assembly.GetExecutingAssembly().CustomAttributes)
				{
					if (typeof(AssemblyTitleAttribute) == attr.AttributeType)
					{
						return attr.ConstructorArguments[0].Value as string;
					}
				}
			}
			catch { }
			return Path.GetFileNameWithoutExtension(Filename);
		}
		static bool _IsStale(string inputfile, string outputfile)
		{
			var result = true;
			// File.Exists doesn't always work right
			try
			{
				if (File.GetLastWriteTimeUtc(outputfile) >= File.GetLastWriteTimeUtc(inputfile))
					result = false;
			}
			catch { }
			return result;
		}
		static string _MakeSafeName(string name)
		{
			StringBuilder sb;
			if (char.IsDigit(name[0]))
			{
				sb = new StringBuilder(name.Length + 1);
				sb.Append('_');
			}
			else
			{
				sb = new StringBuilder(name.Length);
			}
			for (var i = 0; i < name.Length; ++i)
			{
				var ch = name[i];
				if ('_' == ch || char.IsLetterOrDigit(ch))
					sb.Append(ch);
				else
					sb.Append('_');
			}
			return sb.ToString();
		}
		static void _PrintUsage(TextWriter w)
		{
			w.Write("Usage: " + Path.GetFileNameWithoutExtension(Filename) + " ");
#if FALIB_SPANS
			w.WriteLine("<inputfile> [/output <outputfile>] [/class <codeclass>] [/nospans]");
#else
			w.WriteLine("<inputfile> [/output <outputfile>] [/class <codeclass>]");
#endif
			w.WriteLine("   [/namespace <codenamespace>] [/language <codelanguage>] [/tables]");
			w.WriteLine("   [/textreader] [/ignorecase] [/noshared] [/runtime] [/ifstale]");
			w.WriteLine("   [/nfagraph <nfafile>] [/dfagraph <dfafile>] [/vertical] [/dpi <dpi>]");
			w.WriteLine();
			w.WriteLine(Name + " generates a lexer/scanner/tokenizer in the target .NET language");
			w.WriteLine();
			w.WriteLine("   <inputfile>      The input lexer specification");
			w.WriteLine("   <outputfile>     The output source file - defaults to STDOUT");
			w.WriteLine("   <codeclass>      The name of the main class to generate - default derived from <outputfile>");
#if FALIB_SPANS
			w.WriteLine("   <nospans>        Do not use the Span feature of .NET");
#endif
			w.WriteLine("   <codenamespace>  The namespace to generate the code under - defaults to none");
			w.WriteLine("   <codelanguage>   The .NET language to generate the code in - default derived from <outputfile>");
			w.WriteLine("   <tables>         Generate lexers based on DFA tables - defaults to compiled");
			w.WriteLine("   <textreader>     Generate lexers that stream off of TextReaders instead of strings");
			w.WriteLine("   <ignorecase>     Create a case insensitive lexer - defaults to case sensitive");
			w.WriteLine("   <noshared>       Do not generate the shared code as part of the output - defaults to generating the shared code");
			w.WriteLine("   <runtime>        Reference the Visual FA runtime - defaults to generating the shared code");
			w.WriteLine("   <ifstale>        Only generate if the input is newer than the output");
			w.WriteLine("   <staticprogress> Do not use dynamic console features for progress indicators");
			w.WriteLine("   <nfafile>        Write the NFA lexer graph to the specified image file.*");
			w.WriteLine("   <dfafile>        Write the DFA lexer graph to the specified image file.*");
			w.WriteLine("   <vertical>       Produce the graph(s) in vertical layout.*");
			w.WriteLine("   <dpi>            The DPI of any outputted graphs - defaults to 300.*");
			w.WriteLine();
			w.WriteLine("   * Requires GraphViz to be installed and in the PATH");
			w.WriteLine();
		}
		static int Main(string[] args)
		{
			return Run(args, Console.In, Console.Out, Console.Error);
		}
		sealed class Reporter : IProgress<int>
		{
			TextWriter _output;
			bool _dots;
			public Reporter(TextWriter output, bool dots)
			{
				_output = output;
				_dots = dots;

			}
			public void Report(int value)
			{
				if (_dots)
				{
					if (0 == (value % 10))
					{
						_output.Write(".");
					}
				}
				else
				{
					_WriteProgress(value, value != 0, _output);
				}
			}
		}
		public static void _WriteProgress(int progress, bool update, TextWriter output)
		{
			_ProgressBuffer.Clear();
			if (update)
				_ProgressBuffer.Append(_ProgressBackOne);
			_ProgressBuffer.Append(_ProgressTwirl[progress % _ProgressTwirl.Length]);
			output.Write(_ProgressBuffer.ToString());
		}
		static void _WriteProgressBar(int percent, bool update, TextWriter output)
		{
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
		static FA[] _BuildLexerFAs(IList<LexRule> rules, bool ignoreCase, string inputFile, TextWriter output)
		{
			var result = new FA[rules.Count];
			for (var i = 0; i < result.Length; ++i)
			{
				var rule = rules[i];
				FA fa;
				if (rule.Expression.StartsWith("\""))
				{
					var pc = LexContext.Create(rule.Expression);
					fa = FA.Literal(FA.ToUtf32(pc.ParseJsonString()), rule.Id, false);
				}
				else
					fa = FA.Parse(rule.Expression.Substring(1, rule.Expression.Length - 2), rule.Id, false);
				if (0 > rule.Id)
				{
					throw new InvalidOperationException(string.Format("A rule id was less than zero at line {0}", rule.ExpressionLine));
				}
				if (!ignoreCase)
				{
					var ic = (bool)rule.GetAttribute("ignoreCase", false);
					if (ic)
						fa = FA.CaseInsensitive(fa);
				}
				else
				{
					var ic = (bool)rule.GetAttribute("ignoreCase", true);
					if (ic)
						fa = FA.CaseInsensitive(fa);
				}

				result[i] = fa;
			}
			return result;
		}
		static string[] _BuildSymbolTable(IList<LexRule> rules)
		{
			int max = int.MinValue;
			for (int ic = rules.Count, i = 0; i < ic; ++i)
			{
				var rule = rules[i];
				if (rule.Id > max)
					max = rule.Id;
			}
			var result = new string[max + 1];
			for (int ic = rules.Count, i = 0; i < ic; ++i)
			{
				var rule = rules[i];
				result[rule.Id] = rule.Symbol;
			}
			return result;
		}
		static FA _ParseToFA(int id, LexRule rule, bool ignoreCase, string filename)
		{
			FA fa;
			if (rule.Expression.StartsWith("\""))
			{
				var pc = LexContext.Create(rule.Expression);
				fa = FA.Literal(FA.ToUtf32(pc.ParseJsonString()), id, false);
			}
			else
				fa = FA.Parse(rule.Expression.Substring(1, rule.Expression.Length - 2), id, false);
			if (!ignoreCase)
			{
				var ic = (bool)rule.GetAttribute("ignoreCase", false);
				if (ic)
					fa = FA.CaseInsensitive(fa);
			}
			else
			{
				var ic = (bool)rule.GetAttribute("ignoreCase", true);
				if (ic)
					fa = FA.CaseInsensitive(fa);
			}
			return fa;
		}
		static FA[] _BuildBlockEnds(IList<LexRule> rules, bool ignorecase, string filename)
		{
			int max = int.MinValue;
			for (int ic = rules.Count, i = 0; i < ic; ++i)
			{
				var rule = rules[i];
				if (rule.Id > max)
					max = rule.Id;
			}
			var result = new FA[max + 1];
			for (int ic = rules.Count, i = 0; i < ic; ++i)
			{
				var ci = ignorecase;
				var rule = rules[i];
				var ica = rule.GetAttribute("ignoreCase");
				if (null != ica && ica is bool)
				{
					ci = (bool)ica;
				}
				var v = rule.GetAttribute("blockEnd");
				var be = v as string;
				if (!string.IsNullOrEmpty(be))
				{
					var cfa = FA.Literal(FA.ToUtf32(be), rule.Id);
					if (ci)
						cfa = FA.CaseInsensitive(cfa);
					result[rule.Id] = cfa;
				}
				else
				{
					var lr = v as LexRule;
					if (null != lr)
					{
						var fa = _ParseToFA(rule.Id, lr, ci, filename);

						result[rule.Id] = fa;
					}
				}
			}
			return result;
		}
		sealed class LexRule
		{
			public int Id = int.MinValue;
			public string Symbol;
			public string Expression;
			public IList<KeyValuePair<string, object>> Attributes;
			public int Line;
			public int Column;
			public long Index;
			public long Position;
			public int ExpressionLine;
			public int ExpressionColumn;
			public long ExpressionIndex;
			public long ExpressionPosition;
			public object GetAttribute(string name, object @default = null)
			{
				var attrs = Attributes;
				if (null != attrs)
				{
					for (int ic = attrs.Count, i = 0; i < ic; ++i)
					{
						var attr = attrs[i];
						if (0 == string.Compare(attr.Key, name))
							return attr.Value;
					}
				}
				return @default;
			}
			internal static LexRule Parse(LexContext lc)
			{
				lc.Expecting();
				var result = new LexRule();
				result.Line = lc.Line;
				result.Column = lc.Column;
				result.Index = lc.Index;
				result.Position = lc.Position;
				var ll = lc.CaptureBuffer.Length;
				if (!lc.TryReadCIdentifier())
					throw new ExpectingException("Expecting identifier", lc.Line, lc.Column, lc.Index, lc.Position, lc.FileOrUrl, "identifier");
				result.Symbol = lc.GetCapture(ll);
				lc.TrySkipCCommentsAndWhiteSpace();
				lc.Expecting('=', '<');
				if ('<' == lc.Current)
				{
					lc.Advance();
					lc.Expecting();
					var attrs = new List<KeyValuePair<string, object>>();
					while (-1 != lc.Current && '>' != lc.Current)
					{
						lc.TrySkipCCommentsAndWhiteSpace();
						lc.ClearCapture();
						var l = lc.Line;
						var c = lc.Column;
						var i = lc.Index;
						var p = lc.Position;
						if (!lc.TryReadCIdentifier())
							throw new ExpectingException("Identifier expected", l, c, i, p, "identifier");
						var aname = lc.GetCapture();
						lc.TrySkipCCommentsAndWhiteSpace();
						lc.Expecting('=', '>', ',');
						if ('=' == lc.Current)
						{
							lc.Advance();
							lc.TrySkipCCommentsAndWhiteSpace();
							l = lc.Line;
							c = lc.Column;
							i = lc.Index;
							p = lc.Position;
							object value = null;
							if (lc.Current == '\'')
							{
								// this is a regular expression.
								// we store an anonymous LexRule for it in the attribute
								var newRule = new LexRule();
								newRule.ExpressionLine = lc.Line;
								newRule.ExpressionColumn = lc.Column;
								newRule.ExpressionPosition = lc.Position;
								lc.ClearCapture();
								lc.Capture();
								lc.Advance();
								lc.TryReadUntil('\'', '\\', false);
								lc.Expecting('\'');
								lc.Capture();
								newRule.Expression = lc.GetCapture();
								lc.Advance();
								value = newRule;
							}
							else
							{
								value = lc.ParseJsonValue();
							}
							attrs.Add(new KeyValuePair<string, object>(aname, value));
							if (0 == string.Compare("id", aname) && (value is double))
							{
								result.Id = (int)((double)value);
								if (0 > result.Id)
									throw new ExpectingException("Expecting a non-negative integer", l, c, i, p, "nonNegativeInteger");
							}
						}
						else
						{ // boolean true
							attrs.Add(new KeyValuePair<string, object>(aname, true));
						}
						lc.TrySkipCCommentsAndWhiteSpace();
						lc.Expecting(',', '>');
						if (',' == lc.Current)
							lc.Advance();
					}
					lc.Expecting('>');
					lc.Advance();
					result.Attributes = attrs;
					lc.TrySkipCCommentsAndWhiteSpace();
				}
				lc.Advance();
				lc.TrySkipCCommentsAndWhiteSpace();
				lc.Expecting('\'', '\"');
				result.ExpressionLine = lc.Line;
				result.ExpressionColumn = lc.Column;
				result.ExpressionIndex = lc.Index;
				result.ExpressionPosition = lc.Position;
				if ('\'' == lc.Current)
				{
					lc.ClearCapture();
					lc.Capture();
					lc.Advance();
					lc.TryReadUntil('\'', '\\', false);
					lc.Expecting('\'');
					lc.Capture();
					result.Expression = lc.GetCapture();
					lc.Advance();
				}
				else
				{
					lc.ClearCapture();
					lc.Capture();
					lc.Advance();
					lc.TryReadUntil('\"', '\\', false);
					lc.Expecting('\"');
					lc.Capture();
					result.Expression = lc.GetCapture();
				}
				return result;
			}
			public static void FillRuleIds(IList<LexRule> rules)
			{
				var ids = new HashSet<int>();
				for (int ic = rules.Count, i = 0; i < ic; ++i)
				{
					var rule = rules[i];
					if (int.MinValue != rule.Id && !ids.Add(rule.Id))
						throw new InvalidOperationException(string.Format("The input file has a rule with a duplicate id at line {0}, column {1}, position {2}", rule.Line, rule.Column, rule.Position));
				}
				var lastId = 0;
				for (int ic = rules.Count, i = 0; i < ic; ++i)
				{
					var rule = rules[i];
					if (int.MinValue == rule.Id)
					{
						rule.Id = lastId;
						ids.Add(lastId);
						while (ids.Contains(lastId))
							++lastId;
					}
					else
					{
						lastId = rule.Id;
						while (ids.Contains(lastId))
							++lastId;
					}
				}
			}

		}
	}
}