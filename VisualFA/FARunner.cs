using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace VisualFA
{
	#region FARunner
	/// <summary>
	/// Represents a runner that matches text
	/// </summary>
#if FALIB
	public
#endif
	abstract partial class FARunner : IEnumerable<FAMatch>
	{
		/// <summary>
		/// Constructs a new instance
		/// </summary>
		protected internal FARunner()
		{
			position = -1;
			line = 1;
			column = 1;
			tabWidth = 4;
		}
		/// <summary>
		/// The match enumerator
		/// </summary>
		public sealed class Enumerator : IEnumerator<FAMatch>
		{
			int _state;
			FAMatch _current;
			WeakReference<FARunner> _parent;
			/// <summary>
			/// Constructs a new instance
			/// </summary>
			/// <param name="parent">The parent runner</param>
			public Enumerator(FARunner parent)
			{
				_parent = new WeakReference<FARunner>(parent);
				_state = -2;
			}
			/// <summary>
			/// Retreives the current match
			/// </summary>
			public FAMatch Current {
				get {
					if (_state == -3)
					{
						throw new ObjectDisposedException(nameof(Enumerator));
					}
					if (_state < 0)
					{
						throw new InvalidOperationException("The enumerator is not positioned on an element");
					}
					return _current;
				}
			}
			
			object System.Collections.IEnumerator.Current { get { return Current; } }
			void IDisposable.Dispose() { _state = -3; }
			/// <summary>
			/// Move to the next match
			/// </summary>
			/// <returns>True if there were more matches, otherwise false</returns>
			/// <exception cref="ObjectDisposedException">The enumerator was disposed</exception>
			/// <exception cref="InvalidOperationException">The enumerator is not over a valid codepoint</exception>
			public bool MoveNext()
			{
				if (_state == -3)
				{
					throw new ObjectDisposedException(nameof(Enumerator));
				}
				if (_state == -1)
				{
					return false;
				}
				_state = 0;
				FARunner parent;
				if (!_parent.TryGetTarget(out parent))
				{
					throw new InvalidOperationException("The parent was destroyed");
				}
				_current = parent.NextMatch();
				if (_current.SymbolId == -2)
				{
					_state = -2;
					return false;
				}
				return true;
			}
			/// <summary>
			/// Resets the enumerator to the beginning
			/// </summary>
			/// <exception cref="ObjectDisposedException">The enumerator was disposed</exception>
			/// <exception cref="InvalidOperationException">The parent has been destroyed</exception>
			public void Reset()
			{
				if (_state == -3)
				{
					throw new ObjectDisposedException(nameof(Enumerator));
				}
				FARunner parent;
				if (!_parent.TryGetTarget(out parent))
				{
					throw new InvalidOperationException("The parent was destroyed");
				}
				parent.Reset();
				_state = -2;
			}
		}
		/// <summary>
		/// Indicates the width of a tab, in columns
		/// </summary>
		public int TabWidth {
			get {
				return tabWidth;
			}
			set {
				if (value < 1) { throw new ArgumentOutOfRangeException(); }
				tabWidth = value;
			}
		}
		/// <summary>
		/// The tab width
		/// </summary>
		protected int tabWidth;
		/// <summary>
		/// The current position
		/// </summary>
		protected int position;
		/// <summary>
		/// The line
		/// </summary>
		protected int line;
		/// <summary>
		/// The column
		/// </summary>
		protected int column;
		/// <summary>
		/// Throws an exception for an invalid Unicode stream
		/// </summary>
		/// <param name="pos">The position</param>
		/// <exception cref="IOException">Throws this when called</exception>
		protected static void ThrowUnicode(int pos)
		{
			throw new IOException("Unicode error in stream at position " + pos.ToString());
		}
		/// <summary>
		/// Retrieve the next match
		/// </summary>
		/// <returns>A <see cref="FAMatch"/> instance containing the match information</returns>
		public abstract FAMatch NextMatch();
		/// <summary>
		/// Resets the runner to the beginning of the stream
		/// </summary>
		public abstract void Reset();
		/// <summary>
		/// Retrieves the match enumerator
		/// </summary>
		/// <returns>An enumerator that retrieves matches</returns>
		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}
		IEnumerator<FAMatch> IEnumerable<FAMatch>.GetEnumerator() { return GetEnumerator(); }
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
	}
	#endregion // FARunner
	#region FAStringRunner
	/// <summary>
	/// Represents a <see cref="FARunner"/> over a string
	/// </summary>
#if FALIB
	public
#endif
	abstract partial class FAStringRunner : FARunner
	{
		/// <summary>
		/// Indicates that string spans are being used
		/// </summary>
		public static readonly bool UsingSpans =
#if FALIB_SPANS
			true;
#else
			false;
#endif
		/// <summary>
		/// The input string
		/// </summary>
		protected string input_string;
		/// <summary>
		/// Sets the input
		/// </summary>
		/// <param name="string">The input string</param>
		public void Set(string @string)
		{
			this.input_string = @string;
			position = -1;
			line = 1;
			column = 1;
		}
		/// <summary>
		/// Resets the cursor to the beginning
		/// </summary>
		public override void Reset()
		{
			position = -1;
			line = 1;
			column = 1;
		}
		/// <summary>
		/// Advances the cursor by one codepoint
		/// </summary>
		/// <param name="s">THe string</param>
		/// <param name="ch">THe codepoint</param>
		/// <param name="len">THe capture length</param>
		/// <param name="first">True if this is the first advance, otherwise false</param>
#if !FALIB_SMALLER
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		protected void Advance(
#if FALIB_SPANS
			ReadOnlySpan<char> s
#else    
			string s
#endif
			, ref int ch, ref int len, bool first)
		{
			if (!first)
			{
				switch (ch)
				{
					case '\n':
						++line;
						column = 1;
						break;
					case '\r':
						column = 1;
						break;
					case '\t':
						column = ((column - 1) / tabWidth) * (tabWidth + 1);
						break;
					default:
						if (ch > 31)
						{
							++column;
						}
						break;
				}
				++len;
				if (ch > 65535)
				{
					++position;
					++len;
				}
				++position;
			}
			if (position < s.Length)
			{
				char ch1 = s[position];
				if (char.IsHighSurrogate(ch1))
				{
					++position;
					if (position >= s.Length)
					{
						ThrowUnicode(position);
					}
					char ch2 = s[position];
					ch = char.ConvertToUtf32(ch1, ch2);
				}
				else
				{
					ch = ch1;
				}
			}
			else
			{
				ch = -1;
			}
		}
	}
#endregion // FAStringRunner
	#region FATextReaderRunner
	/// <summary>
	/// Rerpresents a <see cref="FARunner"/> over a <see cref="TextReader"/>
	/// </summary>
#if FALIB
	public
#endif
	abstract partial class FATextReaderRunner : FARunner
	{
		/// <summary>
		/// The reader
		/// </summary>
		protected TextReader input_reader;
		/// <summary>
		/// The capture buffer
		/// </summary>
		protected StringBuilder capture = new StringBuilder();
		/// <summary>
		/// The current codepoint
		/// </summary>
		protected int current;
		/// <summary>
		/// Sets the source <see cref="TextReader"/> for the runner
		/// </summary>
		/// <param name="reader">The reader instance</param>
		public void Set(TextReader reader)
		{
			this.input_reader = reader;
			current = -2;
			position = -1;
			line = 1;
			column = 1;
		}
		/// <summary>
		/// Resets the runner (not supported for this type of runner)
		/// </summary>
		/// <exception cref="NotSupportedException">This operation is not supported</exception>
		public override void Reset()
		{
			throw new NotSupportedException();
		}
		/// <summary>
		/// Advances to the next character position
		/// </summary>
		protected void Advance()
		{
			switch (this.current)
			{
				case '\n':
					++line;
					column = 1;
					break;
				case '\r':
					column = 1;
					break;
				case '\t':
					column = ((column - 1) / tabWidth) * (tabWidth + 1);
					break;
				default:
					if (this.current > 31)
					{
						++column;
					}
					break;
			}
			if (current > -1)
			{
				capture.Append(char.ConvertFromUtf32(current));
			}
			current = input_reader.Read();
			if (current == -1)
			{
				return;
			}
			++position;
			char ch1 = unchecked((char)current);
			if (char.IsHighSurrogate(ch1))
			{
				current = input_reader.Read();
				if (current == -1)
				{
					ThrowUnicode(position);
				}
				char ch2 = unchecked((char)current);
				current = char.ConvertToUtf32(ch1, ch2);
				++position;
			}
		}
	}
	#endregion // FATextReaderRunner
	#region FAStringDfaTableRunner
	/// <summary>
	/// Represents a <see cref="FARunner"/> over a DFA state table and a string
	/// </summary>
#if FALIB
	public
#endif
	partial class FAStringDfaTableRunner : FAStringRunner
	{
		private readonly int[] _dfa;
		private readonly int[][] _blockEnds;
		/// <summary>
		/// Constructs a new instance
		/// </summary>
		/// <param name="dfa">The DFA table</param>
		/// <param name="blockEnds">The array of block end DFA tables</param>
		public FAStringDfaTableRunner(int[] dfa, int[][] blockEnds = null)
		{
			_dfa = dfa;
			_blockEnds = blockEnds;
		}
		/// <summary>
		/// Retrieves the next match
		/// </summary>
		/// <returns>A <see cref="FAMatch"/> instance containing the match information</returns>
		public override FAMatch NextMatch()
		{
			return _NextImpl(input_string);
		}
		private FAMatch _NextImpl(
#if FALIB_SPANS
			ReadOnlySpan<char> s
#else
			string s
#endif
			)
		{
			int tlen;
			int tto;
			int prlen;
			int pmin;
			int pmax;
			int i;
			int j;
			int state = 0;
			int acc;
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
			start_dfa:
			acc = _dfa[state];
			++state;
			tlen = _dfa[state];
			++state;
			for (i = 0; i < tlen; ++i)
			{
				tto = _dfa[state];
				++state;
				prlen = _dfa[state];
				++state;
				for (j = 0; j < prlen; ++j)
				{
					pmin = _dfa[state];
					++state;
					pmax = _dfa[state];
					++state;
					if (ch < pmin)
					{
						state += ((prlen - (j + 1)) * 2);
						j = prlen;
					}
					else if (ch <= pmax)
					{
						Advance(s, ref ch, ref len, false);
						state = tto;
						goto start_dfa;
					}
				}
			}
			if (acc != -1)
			{
				int sym = acc;
				int[] be = (_blockEnds != null && _blockEnds.Length > acc) ? _blockEnds[acc] : null;
				if (be != null)
				{
					state = 0;
					start_be_dfa:
					acc = be[state];
					++state;
					tlen = be[state];
					++state;
					for (i = 0; i < tlen; ++i)
					{
						tto = be[state];
						++state;
						prlen = be[state];
						++state;
						for (j = 0; j < prlen; ++j)
						{
							pmin = be[state];
							++state;
							pmax = be[state];
							++state;
							if (ch < pmin)
							{
								state += ((prlen - (j + 1)) * 2);
								j = prlen;
							}
							else if (ch <= pmax)
							{
								Advance(s, ref ch, ref len, false);
								state = tto;
								goto start_be_dfa;
							}
						}
					}
					if (acc != -1)
					{
						return FAMatch.Create(sym,
#if FALIB_SPANS
							s.Slice(unchecked((int)cursor_pos), len).ToString()
#else
							s.Substring(unchecked((int)cursor_pos), len)
#endif
							, cursor_pos, line, column);
					}
					if (ch == -1)
					{
						return FAMatch.Create(-1,
#if FALIB_SPANS
							s.Slice(unchecked((int)cursor_pos), len).ToString()
#else
							s.Substring(unchecked((int)cursor_pos), len)
#endif
							, cursor_pos, line, column);
					}
					Advance(s, ref ch, ref len, false);
					state = 0;
					goto start_be_dfa;
				}
				return FAMatch.Create(acc,
#if FALIB_SPANS
							s.Slice(unchecked((int)cursor_pos), len).ToString()
#else
							s.Substring(unchecked((int)cursor_pos), len)
#endif
					, cursor_pos, line, column);
			}
			// error. keep trying until we find a potential transition.
			while (ch != -1)
			{
				var moved = false;
				state = 1;
				tlen = _dfa[state];
				++state;
				for (i = 0; i < tlen; ++i)
				{
					++state;
					prlen = _dfa[state];
					++state;
					for (j = 0; j < prlen; ++j)
					{
						pmin = _dfa[state];
						++state;
						pmax = _dfa[state];
						++state;
						if (ch < pmin)
						{
							state += ((prlen - (j + 1)) * 2);
							j = prlen;
						}
						else if (ch <= pmax)
						{
							moved = true;
						}
					}
				}
				if (moved)
				{
					break;
				}
				Advance(s, ref ch, ref len, false);
			}
			if (len == 0)
			{
				return FAMatch.Create(-2, null, 0, 0, 0);
			}
			return FAMatch.Create(-1,
#if FALIB_SPANS
							s.Slice(unchecked((int)cursor_pos), len).ToString()
#else
							s.Substring(unchecked((int)cursor_pos), len)
#endif
				, cursor_pos, line, column);
		}
	}
#endregion // FAStringDfaTableRunner
	#region FATextReaderDfaTableRunner
	/// <summary>
	/// Represents a <see cref="FARunner"/> over a DFA state table and a <see cref="TextReader"/>
	/// </summary>
#if FALIB
	public
#endif
	partial class FATextReaderDfaTableRunner : FATextReaderRunner
	{
		private readonly int[] _dfa;
		private readonly int[][] _blockEnds;
		/// <summary>
		/// Constructs a new instance
		/// </summary>
		/// <param name="dfa">The dfa array</param>
		/// <param name="blockEnds">The block end arrays</param>
		public FATextReaderDfaTableRunner(int[] dfa, int[][] blockEnds = null)
		{
			_dfa = dfa;
			_blockEnds = blockEnds;
		}
		/// <summary>
		/// Retrieve the next match
		/// </summary>
		/// <returns>A <see cref="FAMatch"/> instance with the match infornmation</returns>
		public override FAMatch NextMatch()
		{
			int tlen;
			int tto;
			int prlen;
			int pmin;
			int pmax;
			int i;
			int j;
			int state = 0;
			int acc;
			capture.Clear();
			if (current == -2)
			{
				Advance();
			}
			long cursor_pos = position;
			int line = this.line;
			int column = this.column;
			start_dfa:
			acc = _dfa[state];
			++state;
			tlen = _dfa[state];
			++state;
			for (i = 0; i < tlen; ++i)
			{
				tto = _dfa[state];
				++state;
				prlen = _dfa[state];
				++state;
				for (j = 0; j < prlen; ++j)
				{
					pmin = _dfa[state];
					++state;
					pmax = _dfa[state];
					++state;
					if (current < pmin)
					{
						state += ((prlen - (j + 1)) * 2);
						j = prlen;
					}
					else if (current <= pmax)
					{
						Advance();
						state = tto;
						goto start_dfa;
					}
				}
			}
			if (acc != -1)
			{
				int sym = acc;
				int[] be = (_blockEnds != null && _blockEnds.Length > acc) ? _blockEnds[acc] : null;
				if (be != null)
				{
					state = 0;
					start_be_dfa:
					acc = be[state];
					++state;
					tlen = be[state];
					++state;
					for (i = 0; i < tlen; ++i)
					{
						tto = be[state];
						++state;
						prlen = be[state];
						++state;
						for (j = 0; j < prlen; ++j)
						{
							pmin = be[state];
							++state;
							pmax = be[state];
							++state;
							if (current < pmin)
							{
								state += ((prlen - (j + 1)) * 2);
								j = prlen;
							}
							else if (current <= pmax)
							{
								Advance();
								state = tto;
								goto start_be_dfa;
							}
						}
					}
					if (acc != -1)
					{
						return FAMatch.Create(sym, capture.ToString(), cursor_pos, line, column);
					}
					if (current == -1)
					{
						return FAMatch.Create(-1, capture.ToString(), cursor_pos, line, column);
					}
					Advance();
					state = 0;
					goto start_be_dfa;
				}
				return FAMatch.Create(acc, capture.ToString(), cursor_pos, line, column);
			}
			// error. keep trying until we find a potential transition.
			while (current != -1)
			{
				var moved = false;
				state = 1;
				tlen = _dfa[state];
				++state;
				for (i = 0; i < tlen; ++i)
				{
					++state;
					prlen = _dfa[state];
					++state;
					for (j = 0; j < prlen; ++j)
					{
						pmin = _dfa[state];
						++state;
						pmax = _dfa[state];
						++state;
						if (current < pmin)
						{
							state += ((prlen - (j + 1)) * 2);
							j = prlen;
						}
						else if (current <= pmax)
						{
							moved = true;
						}
					}
				}
				if (moved)
				{
					break;
				}
				Advance();
			}
			if (capture.Length == 0)
			{
				return FAMatch.Create(-2, null, 0, 0, 0);
			}
			return FAMatch.Create(-1, capture.ToString(), cursor_pos, line, column);
		}
	}
	#endregion // FATextReaderDfaTableRunner
	#region FAStringStateRunner
	/// <summary>
	/// Represents a <see cref="FARunner"/> over a state machine and a string
	/// </summary>
#if FALIB
	public
#endif
	partial class FAStringStateRunner : FAStringRunner
	{
		readonly FA _fa;
		readonly FA[] _blockEnds;
		readonly List<FA> _states;
		readonly List<FA> _nexts;
		readonly List<FA> _initial;
		/// <summary>
		/// Constructs a new instance
		/// </summary>
		/// <param name="fa">The FSM</param>
		/// <param name="blockEnds">The block end array</param>
		/// <exception cref="ArgumentNullException"></exception>
		public FAStringStateRunner(FA fa, FA[] blockEnds = null)
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
		/// <summary>
		/// Rerieves the next match
		/// </summary>
		/// <returns>A <see cref="FAMatch"/> instance containing the match information</returns>
		public override FAMatch NextMatch()
		{
			return _NextImpl(input_string);
		}
		private FAMatch _NextImpl(
#if FALIB_SPANS
			ReadOnlySpan<char> s
#else
			string s
#endif
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
#if FALIB_SPANS
							s.Slice(unchecked((int)cursor_pos), len).ToString()
#else
							s.Substring(unchecked((int)cursor_pos), len)
#endif
												, cursor_pos, line, column);
										}
									}
									else
									{
										if (-1 < FA.GetFirstAcceptSymbol(_states))
										{
											return FAMatch.Create(acc,
#if FALIB_SPANS
							s.Slice(unchecked((int)cursor_pos), len).ToString()
#else
							s.Substring(unchecked((int)cursor_pos), len)
#endif
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
#if FALIB_SPANS
							s.Slice(unchecked((int)cursor_pos), len).ToString()
#else
							s.Substring(unchecked((int)cursor_pos), len)
#endif
											, cursor_pos, line, column);
									}
								}
							}
						}
						else
						{
							return FAMatch.Create(acc,
#if FALIB_SPANS
							s.Slice(unchecked((int)cursor_pos), len).ToString()
#else
							s.Substring(unchecked((int)cursor_pos), len)
#endif
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
#if FALIB_SPANS
							s.Slice(unchecked((int)cursor_pos), len).ToString()
#else
							s.Substring(unchecked((int)cursor_pos), len)
#endif

							, cursor_pos, line, column);
					}
				}
			}
		}
	}
#endregion // FAStringStateRunner
	#region FATextReaderDfaTableRunner
	/// <summary>
	/// Represents a <see cref="FARunner"/> over a <see cref="TextReader"/> and a <see cref="FA"/> state machine
	/// </summary>
#if FALIB
	public
#endif
	partial class FATextReaderStateRunner : FATextReaderRunner
	{
		readonly FA _fa;
		readonly FA[] _blockEnds;
		readonly List<FA> _states;
		readonly List<FA> _nexts;
		readonly List<FA> _initial;
		/// <summary>
		/// Constructs a new instance
		/// </summary>
		/// <param name="fa">The state machine</param>
		/// <param name="blockEnds">An array of state machines representing the block ends</param>
		/// <exception cref="ArgumentNullException"><paramref name="fa"/> was null</exception>
		public FATextReaderStateRunner(FA fa, FA[] blockEnds = null)
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
		/// <summary>
		/// Retrieved the next match in the text
		/// </summary>
		/// <returns></returns>
		public override FAMatch NextMatch()
		{
			capture.Clear();
			FA dfaState = null, dfaNext = null, dfaInitial = null;
			if (current == -2)
			{
				Advance();
			}
			long cursor_pos = position;
			int line = this.line;
			int column = this.column;
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
					dfaNext = dfaState.Move(current);
				}
				else
				{
					dfaNext = null;
					_nexts.Clear();
					FA.FillMove(_states, current, _nexts);
				}
				if (dfaNext != null)
				{
					Advance();
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
					}
					dfaNext = null;
				}
				else if (_nexts.Count > 0)
				{
					Advance();
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
							}
							else
							{
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
									dfaNext = dfaState.Move(current);
								}
								else
								{
									dfaNext = null;
									_nexts.Clear();
									FA.FillMove(_states, current, _nexts);
								}
								if (dfaNext != null)
								{
									Advance();
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
									Advance();
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
									if (dfaState != null)
									{
										if (dfaState.IsAccepting)
										{
											return FAMatch.Create(acc, capture.ToString(), cursor_pos, line, column);
										}
									}
									else
									{
										if (-1 < FA.GetFirstAcceptSymbol(_states))
										{
											return FAMatch.Create(acc, capture.ToString(), cursor_pos, line, column);
										}
									}
									Advance();
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
									if (current == -1)
									{
										return FAMatch.Create(-1, capture.ToString(), cursor_pos, line, column);
									}
								}
							}
						}
						else
						{
							return FAMatch.Create(acc, capture.ToString(), cursor_pos, line, column);
						}
					}
					else
					{
						if (dfaInitial != null)
						{
							while (current != -1 && dfaInitial.Move(current) == null)
							{
								Advance();
							}
						}
						else
						{
							_states.Clear();
							while (current != -1 && FA.FillMove(_initial, current, _states).Count == 0)
							{
								Advance();
							}
						}
						if (capture.Length == 0)
						{
							return FAMatch.Create(-2, null, 0, 0, 0);
						}
						return FAMatch.Create(-1, capture.ToString(), cursor_pos, line, column);
					}
				}
			}

		}
	}
	#endregion // FATextReaderDfaTableRunner


}
