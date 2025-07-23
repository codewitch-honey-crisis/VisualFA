using System;
using System.Diagnostics.Metrics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using VisualFA;

namespace Scratch2
{
	partial class FAStringStateRunner2 : FAStringRunner
	{
		readonly FA _fa;
		readonly FA[] _blockEnds;
		readonly List<FA> _states;
		readonly List<FA> _nexts;
		readonly List<FA> _initial;
		public FAStringStateRunner2(FA fa, FA[] blockEnds = null)
		{
			if (null == fa)
			{
				throw new ArgumentNullException(nameof(fa));
			}
			_fa = fa;
			_blockEnds = blockEnds;
			_states = new List<FA>();
			_nexts = new List<FA>();
			_initial = new List<FA>();
		}
		public override FAMatch NextMatch()
		{
			return _NextImpl(input_string);
		}
		private FAMatch _NextImpl(
			string s
			)
		{
			FA dfaState = null, dfaNext = null, dfaInitial = null;
			if (position == -1)
			{
				// first read
				++position;
			}
			int len = 0;
			long cursor_pos = position;
			int line = this.line;
			int column = this.column;
			int ch = -1;
			Advance(s, ref ch, ref len, true);
			if (_fa.IsDeterministic)
			{
				dfaState = _fa;
				dfaInitial = _fa;
			}
			else
			{
				_initial.Clear();
				if (_fa.IsCompact)
				{
					_initial.Add(_fa);
				}
				else
				{
					FA.FillEpsilonClosure(_fa, _initial);
				}
				_states.Clear();
				_states.AddRange(_initial);
			}
			while (true)
			{
				if (dfaState != null)
				{
					dfaNext = dfaState.Move(ch);
				}
				else
				{
					dfaNext = null;
					_nexts.Clear();
					FA.FillMove(_states, ch, _nexts);
				}
				if (dfaNext != null)
				{
					Advance(s, ref ch, ref len, false);
					if (dfaNext.IsDeterministic)
					{
						dfaState = dfaNext;

					}
					else
					{
						_states.Clear();
						if (dfaNext.IsCompact)
						{
							_states.Add(dfaNext);
						}
						else
						{
							FA.FillEpsilonClosure(dfaNext, _states);
						}
						dfaState = null;
					}
					dfaNext = null;
				}
				else if (_nexts.Count > 0)
				{
					Advance(s, ref ch, ref len, false);
					if (_nexts.Count == 1)
					{
						var ffa = _nexts[0];
						// switch to deterministic mode if needed
						if (ffa.IsDeterministic)
						{
							dfaState = ffa;
						}
						else
						{
							dfaNext = null;
							_states.Clear();
							if (!ffa.IsCompact)
							{
								FA.FillEpsilonClosure(ffa, _states);
							}
							else
							{
								_states.Add(ffa);
							}
							dfaState = null;
						}
					}
					else
					{
						_states.Clear();
						FA.FillEpsilonClosure(_nexts, _states);
					}
					_nexts.Clear();
				}
				else
				{
					int acc;
					if (dfaState != null)
					{
						acc = dfaState.AcceptSymbol;
					}
					else
					{
						acc = FA.GetFirstAcceptSymbol(_states);
					}
					if (acc > -1)
					{
						if (_blockEnds != null && _blockEnds.Length > acc && _blockEnds[acc] != null)
						{
							var be = _blockEnds[acc];
							if (be.IsDeterministic)
							{
								dfaState = be;
								dfaInitial = be;
								_states.Clear();
							}
							else
							{
								dfaState = null;
								dfaInitial = null;
								_initial.Clear();
								if (be.IsCompact)
								{
									_initial.Add(be);
								}
								else
								{
									FA.FillEpsilonClosure(be, _initial);
								}
								_states.Clear();
								_states.AddRange(_initial);
							}
							while (true)
							{
								if (dfaState != null)
								{
									dfaNext = dfaState.Move(ch);
								}
								else
								{
									dfaNext = null;
									_nexts.Clear();
									FA.FillMove(_states, ch, _nexts);
								}
								if (dfaNext != null)
								{
									Advance(s, ref ch, ref len, false);
									if (dfaNext.IsDeterministic)
									{
										dfaState = dfaNext;
									}
									else
									{
										_states.Clear();
										if (dfaNext.IsCompact)
										{
											_states.Add(dfaNext);
										}
										else
										{
											FA.FillEpsilonClosure(dfaNext, _states);
										}
										dfaState = null;
									}
									dfaNext = null;

								}
								else if (_nexts.Count > 0)
								{
									Advance(s, ref ch, ref len, false);
									if (_nexts.Count == 1)
									{
										var ffa = _nexts[0];
										// switch to deterministic mode if needed
										if (ffa.IsDeterministic)
										{
											dfaState = ffa;
											_states.Clear();
										}
										else
										{
											dfaNext = null;
											_states.Clear();
											if (!ffa.IsCompact)
											{
												FA.FillEpsilonClosure(ffa, _states);
											}
											else
											{
												_states.Add(ffa);
											}
											dfaState = null;
										}
									}
									else
									{
										_states.Clear();
										FA.FillEpsilonClosure(_nexts, _states);
									}
									_nexts.Clear();
								}
								else
								{
									if (dfaState != null)
									{
										if (dfaState.IsAccepting)
										{
											return FAMatch.Create(acc,
							s.Substring(unchecked((int)cursor_pos), len)
												, cursor_pos, line, column);
										}
									}
									else
									{
										if (-1 < FA.GetFirstAcceptSymbol(_states))
										{
											return FAMatch.Create(acc,
							s.Substring(unchecked((int)cursor_pos), len)
												, cursor_pos, line, column);
										}
									}
									Advance(s, ref ch, ref len, false);
									if (dfaInitial != null)
									{
										_states.Clear();
										dfaState = dfaInitial;
									}
									else
									{
										dfaState = null;
										_states.Clear();
										_states.AddRange(_initial);
									}
									if (ch == -1)
									{
										return FAMatch.Create(-1,
							s.Substring(unchecked((int)cursor_pos), len)
											, cursor_pos, line, column);
									}
								}
							}
						}
						else
						{
							return FAMatch.Create(acc,
							s.Substring(unchecked((int)cursor_pos), len)
								, cursor_pos, line, column);
						}
					}
					else
					{
						if (dfaInitial != null)
						{
							while (ch != -1 && dfaInitial.Move(ch) == null)
							{
								Advance(s, ref ch, ref len, false);
							}
						}
						else
						{
							_states.Clear();
							while (ch != -1 && FA.FillMove(_initial, ch, _states).Count == 0)
							{
								Advance(s, ref ch, ref len, false);
							}
						}
						if (len == 0)
						{
							return FAMatch.Create(-2, null, 0, 0, 0);
						}
						return FAMatch.Create(-1,
							s.Substring(unchecked((int)cursor_pos), len)
							, cursor_pos, line, column);
					}
				}
			}
		}
	}
	internal class Program
	{
		static void PrintArray(int[] arr)
		{
			Console.Write("[");
			Console.Write(arr[0]);
			for (int i = 1; i < arr.Length; i++)
			{
				Console.Write($", {arr[i]}");
			}
            Console.WriteLine("]");
        }
		static void Main()
		{
			string exp1 = @"([^\*]|\*+[^\/])*\*\/";
			string exp2 = @"/\*([^\*\-]|\*+[^\/]|\-+[^\/])*[\*\-]/";
			string exp3 = @"/\*";
			string exp4 = @"\*/";
			var nfa = FA.Parse(exp3, 0, false);
			nfa = FA.ConcatLazy(nfa, FA.Parse(exp4, 0, false));
            var opts = new FADotGraphOptions();
			opts.HideAcceptSymbolIds = true;
			opts.Vertical = true;
			var dfa = nfa.ToDfa();
			var mdfa = dfa.ToMinimizedDfa();
			nfa.RenderToFile(@"..\..\..\nfa.jpg", opts);
            dfa.RenderToFile(@"..\..\..\dfa.jpg", opts);
            mdfa.RenderToFile(@"..\..\..\mdfa.jpg", opts);
			//mdfa.RenderToFile(@"..\..\..\mdfa.dot", opts);

			//var next = FA.Set(new FARange[] { new FARange('A', 'Z'), new FARange('a', 'z'), new FARange('_', '_'), new FARange('0', '9') }, 0, false);
			//var ident = FA.Concat(new FA[] { first, FA.Repeat(next, 0, 0, 0, false) }, 0, false);
			//PrintArray(ident.ToArray());
			//var dfa = ident.ToDfa();
			//PrintArray(dfa.ToArray());
			//var mdfa = ident.ToMinimizedDfa();
			//PrintArray(mdfa.ToArray());
			//var test = FA.Literal("h", 0, false);
			//         test = FA.Repeat(test, 0, 0, 0, false);
			//int sum = 0;
			//foreach(var fa in test.FillClosure())
			//{
			//	sum += fa.Transitions.Count;
			//}
			//Console.WriteLine($"Transitions: {sum}");
			//var cl = test.FillClosure();
			//PrintArray(test.ToArray());
			//PrintArray(test.ToDfa().ToArray());

			//FA.FromArray( fsm ).RenderToFile(@"..\..\..\test.jpg");
			Console.WriteLine(nfa.ToString("e"));
			Console.WriteLine();
			Console.WriteLine(dfa.ToString("e"));
        }
        static void Main3() {
			var fa = FA.Parse("(\\/api\\/spiffs\\/(.*))|(\\/api\\/sdcard\\/(.*))");
			fa.RenderToFile(@"..\..\..\debug.jpg", new FADotGraphOptions() { Vertical = true });

		}
		static void _PrintStates(IEnumerable<FA> states)
		{
			Console.Write("{ ");
			var delim = "";
			foreach (var fa in states)
			{
				// we use the Id from SetIds() here
				Console.Write(delim + "q" + fa.Id.ToString());
				delim = ", ";
			}
			Console.WriteLine(" }");
		}
		
	}
}
