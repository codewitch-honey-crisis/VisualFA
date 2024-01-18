using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace VisualFA
{
#if FALIB
	public
#endif
	delegate bool RegexVisitAction(RegexExpression parent, RegexExpression expression);

	/// <summary>
	/// Represents the common functionality of all regular expression elements
	/// </summary>
#if FALIB
	public
#endif
	abstract partial class RegexExpression : ICloneable {
		/// <summary>
		/// Indicates the 0 based position on which the regular expression was found
		/// </summary>
		public long Position { get; set; } = 0L;
		/// <summary>
		/// A user defined, application specific value to associate with this expression
		/// </summary>
		public object Tag { get; set; } = null;
		/// <summary>
		/// Indicates whether or not this statement is a single element or not
		/// </summary>
		/// <remarks>If false, this statement will be wrapped in parentheses if necessary</remarks>
		public abstract bool IsSingleElement { get; }
		/// <summary>
		/// Sets the location information for the expression
		/// </summary>
		/// <param name="position">The 0 based position where the expression appears</param>
		public void SetLocation(long position)
		{
			Position = position;
		}
		bool _Visit(RegexExpression parent, RegexVisitAction action) {
			if (action(parent,this))
			{
				var unary = this as RegexUnaryExpression;
				if (unary != null && unary.Expression != null)
				{
					return unary.Expression._Visit(this,action);
				}
				var binary = this as RegexBinaryExpression;
				if (binary != null)
				{
					if (binary.Left != null)
					{
						if (!binary.Left._Visit(this,action))
						{
							return false;
						}
					}
					if (binary.Right != null)
					{
						return binary.Right._Visit(this,action);
					}
				}
				return true;
			}
			return false;
		}
		/// <summary>
		/// Visits each element in the AST
		/// </summary>
		/// <param name="action">The anonymous method to call for each element</param>
		public void Visit(RegexVisitAction action)
		{
			_Visit(null, action);
		}
		/// <summary>
		/// Creates a copy of the expression
		/// </summary>
		/// <returns>A copy of the expression</returns>
		protected abstract RegexExpression CloneImpl();
		object ICloneable.Clone() => CloneImpl();
		/// <summary>
		/// Creates a state machine representing this expression
		/// </summary>
		/// <param name="accept">The accept symbol to use for this expression</param>
		/// <param name="compact">True to generate a compact NFA, otherwise generated an expanded NFA</param>
		/// <returns>A new <see cref="FA"/> finite state machine representing this expression</returns>
		public abstract FA ToFA(int accept = 0,bool compact = true);
		/// <summary>
		/// Appends the textual representation to a <see cref="StringBuilder"/>
		/// </summary>
		/// <param name="sb">The string builder to use</param>
		/// <remarks>Used by ToString()</remarks>
		protected internal abstract void AppendTo(StringBuilder sb);
		/// <summary>
		/// Gets a textual representation of the expression
		/// </summary>
		/// <returns>A string representing the expression</returns>
		public override string ToString()
		{
			var result = new StringBuilder();
			AppendTo(result);
			return result.ToString();
		}
		/// <summary>
		/// Parses a regular expression from the specified string
		/// </summary>
		/// <param name="expression">The expression to parse</param>
		/// <returns>A new abstract syntax tree representing the expression</returns>
		public static RegexExpression Parse(string expression)
		{
			FA.StringCursor pc = new FA.StringCursor();
			pc.Input = expression;
			return _Parse(pc);
		}
		private static RegexExpression _Parse(FA.StringCursor pc)
		{
			
			RegexExpression result=null,next=null;
			int ich;
			pc.EnsureStarted();
			var position = pc.Position;
			while (true)
			{
				switch (pc.Codepoint)
				{
					case -1:
						return result;
					case '.':
						var nset = new RegexCharsetExpression(new RegexCharsetEntry[] { new RegexCharsetRangeEntry(0, 0x10ffff) }, false);
						nset.SetLocation(position);
						if (null == result)
							result = nset;
						else
						{
							result = new RegexConcatExpression(result, nset);
							result.SetLocation(position);
						}
						pc.Advance();
						result = _ParseModifier(result, pc);
						position = pc.Position;
						break;
					case '\\':
						
						pc.Advance();
						pc.Expecting();
						switch (pc.Codepoint)
						{
							case 'd':
								next = new RegexCharsetExpression(new RegexCharsetEntry[] { new RegexCharsetClassEntry("digit") });
								pc.Advance();
								break;
							case 'D':
								next = new RegexCharsetExpression(new RegexCharsetEntry[] { new RegexCharsetClassEntry("digit") },true);
								pc.Advance();
								break;
							case 'h':
								next = new RegexCharsetExpression(new RegexCharsetEntry[] { new RegexCharsetClassEntry("blank") });
								pc.Advance();
								break;
							case 'l':
								next = new RegexCharsetExpression(new RegexCharsetEntry[] { new RegexCharsetClassEntry("lower") });
								pc.Advance();
								break;
							case 's':
								next = new RegexCharsetExpression(new RegexCharsetEntry[] { new RegexCharsetClassEntry("space") });
								pc.Advance();
								break;
							case 'S':
								next = new RegexCharsetExpression(new RegexCharsetEntry[] { new RegexCharsetClassEntry("space") },true);
								pc.Advance();
								break;
							case 'u':
								next = new RegexCharsetExpression(new RegexCharsetEntry[] { new RegexCharsetClassEntry("upper") });
								pc.Advance();
								break;
							case 'w':
								next = new RegexCharsetExpression(new RegexCharsetEntry[] { new RegexCharsetClassEntry("word") });
								pc.Advance();
								break;
							case 'W':
								next = new RegexCharsetExpression(new RegexCharsetEntry[] { new RegexCharsetClassEntry("word") },true);
								pc.Advance();
								break;
							default:
								if (-1 != (ich = _ParseEscapePart(pc)))
								{
									if (result is RegexLiteralExpression)
									{
										var cptmp = new int[((RegexLiteralExpression)result).Codepoints.Length + 1];
										cptmp[cptmp.Length - 1] = ich;
										Array.Copy(((RegexLiteralExpression)result).Codepoints, cptmp, cptmp.Length - 1);
										((RegexLiteralExpression)result).Codepoints = cptmp;
										next = null;
									}
									else
									{
										next = new RegexLiteralExpression(new int[] { ich });
									}
								}
								else
								{
									pc.Expecting(); // throw an error
									return null; // doesn't execute
								}
								break;
						}
						if (next != null)
						{
							next.SetLocation(position);
							next = _ParseModifier(next, pc);
						}
						if (null != result)
						{
							result = new RegexConcatExpression(result, next);
							result.SetLocation(position);
						}
						else
							result = next;
						position = pc.Position;
						break;
					case ')':
						return result;
					case '(':
						pc.Advance();
						pc.Expecting();
						next = _Parse(pc);
						pc.Expecting(')');
						pc.Advance();
						next = _ParseModifier(next, pc);
						if (null == result)
							result = next;
						else
						{
							result = new RegexConcatExpression(result, next);
							result.SetLocation(position);
						}
						position = pc.Position;
						break;
					case '|':
						if (-1 != pc.Advance())
						{
							next = _Parse(pc);
							result = new RegexOrExpression(result, next);
							result.SetLocation(position);
						}
						else
						{
							result = new RegexOrExpression(result, null);
							result.SetLocation(position);
						}
						position = pc.Position;
						break;
					case '[':
						pc.CaptureBuffer.Clear();
						pc.Advance();
						pc.Expecting();
						bool not = false;
					
						
						if ('^' == pc.Codepoint)
						{
							not = true;
							pc.Advance();
							pc.Expecting();
						}
						var ranges = _ParseRanges(pc);
						
						pc.Expecting(']');
						pc.Advance();
						next = new RegexCharsetExpression(ranges, not);
						next.SetLocation(position);
						next = _ParseModifier(next, pc);
						
						if (null == result)
							result = next;
						else
						{
							result = new RegexConcatExpression(result, next);
							result.SetLocation(pc.Position);
						}
						position = pc.Position;
						break;
					default:
						ich = pc.Codepoint;
						if (result is RegexLiteralExpression)
						{
							var cptmp = new int[((RegexLiteralExpression)result).Codepoints.Length + 1];
							cptmp[cptmp.Length - 1] = ich;
							Array.Copy(((RegexLiteralExpression)result).Codepoints, cptmp, cptmp.Length - 1);
							((RegexLiteralExpression)result).Codepoints = cptmp;
							next = null;
						}
						else
						{
							next = new RegexLiteralExpression(new int[] { ich });
							next.SetLocation(position);
						}
						pc.Advance();
						if (next != null)
						{
							next = _ParseModifier(next, pc);
						}
						if (null == result)
							result = next;
						else
						{
							if (next != null)
							{
								result = new RegexConcatExpression(result, next);
							}
							result.SetLocation(position);
						}
						position = pc.Position;
						break;
				}
			}
		}
		static IList<RegexCharsetEntry> _ParseRanges(FA.StringCursor pc)
		{
			pc.EnsureStarted();
			var result = new List<RegexCharsetEntry>();
			RegexCharsetEntry next = null;
			bool readDash = false;
			while (-1 != pc.Codepoint && ']' != pc.Codepoint)
			{
				switch (pc.Codepoint)
				{
					case '[': // char partial class 
						if (null != next)
						{
							result.Add(next);
							if (readDash)
								result.Add(new RegexCharsetCharEntry('-'));
							result.Add(new RegexCharsetCharEntry('-'));
						}
						pc.Advance();
						pc.Expecting(':');
						pc.Advance();
						var l = pc.CaptureBuffer.Length;
						pc.TryReadUntil(':', false);
						var n = pc.GetCapture(l);
						pc.Advance();
						pc.Expecting(']');
						pc.Advance();
						result.Add(new RegexCharsetClassEntry(n));
						readDash = false;
						next = null;
						break;
					case '\\':
						pc.Advance();
						pc.Expecting();
						switch(pc.Codepoint)
						{
							case 'h':
								_ParseCharClassEscape(pc, "space", result, ref next, ref readDash);
								break;
							case 'd':
								_ParseCharClassEscape(pc, "digit", result, ref next, ref readDash);
								break;
							case 'D':
								_ParseCharClassEscape(pc, "^digit", result, ref next, ref readDash);
								break;
							case 'l':
								_ParseCharClassEscape(pc,"lower", result, ref next, ref readDash);
								break;
							case 's':
								_ParseCharClassEscape(pc, "space", result, ref next, ref readDash);
								break;
							case 'S':
								_ParseCharClassEscape(pc, "^space", result, ref next, ref readDash);
								break;
							case 'u':
								_ParseCharClassEscape(pc, "upper", result, ref next, ref readDash);
								break;
							case 'w':
								_ParseCharClassEscape(pc, "word", result, ref next, ref readDash);
								break;
							case 'W':
								_ParseCharClassEscape(pc, "^word", result, ref next, ref readDash);
								break;
							default:
								var ch = _ParseRangeEscapePart(pc);
								if (null == next)
									next = new RegexCharsetCharEntry(ch);
								else if (readDash)
								{
									result.Add(new RegexCharsetRangeEntry(((RegexCharsetCharEntry)next).Codepoint, ch));
									next = null;
									readDash = false;
								}
								else
								{
									result.Add(next);
									next = new RegexCharsetCharEntry(ch);
								}
								
								break;
						}
						
						break;
					case '-':
						pc.Advance();
						if (null == next)
						{
							next = new RegexCharsetCharEntry('-');
							readDash = false;
						}
						else
						{
							if (readDash)
								result.Add(next);
							readDash = true;
						}
						break;
					default:
						if (null == next)
						{
							next = new RegexCharsetCharEntry(pc.Codepoint);
						}
						else
						{
							if (readDash)
							{
								result.Add(new RegexCharsetRangeEntry(((RegexCharsetCharEntry)next).Codepoint, pc.Codepoint));
								next = null;
								readDash = false;
							}
							else
							{
								result.Add(next);
								next = new RegexCharsetCharEntry(pc.Codepoint);
							}
						}
						pc.Advance();
						break;
				}
			}
			if(null!=next)
			{
				result.Add(next);
				if(readDash)
				{
					next = new RegexCharsetCharEntry('-');
					result.Add(next);
				}
			}
			return result;
		}

		static void _ParseCharClassEscape(FA.StringCursor pc, string cls, List<RegexCharsetEntry> result, ref RegexCharsetEntry next, ref bool readDash)
		{
			if (null != next)
			{
				result.Add(next);
				if (readDash)
					result.Add(new RegexCharsetCharEntry('-'));
				result.Add(new RegexCharsetCharEntry('-'));
			}
			pc.Advance();
			result.Add(new RegexCharsetClassEntry(cls));
			next = null;
			readDash = false;
		}

		static RegexExpression _ParseModifier(RegexExpression expr,FA.StringCursor pc)
		{
			var position = pc.Position;
			switch (pc.Codepoint)
			{
				case '*':
					expr = new RegexRepeatExpression(expr);
					expr.SetLocation(position);
					pc.Advance();
					break;
				case '+':
					expr = new RegexRepeatExpression(expr, 1);
					expr.SetLocation(position);
					pc.Advance();
					break;
				case '?':
					expr = new RegexOptionalExpression(expr);
					expr.SetLocation(position);
					pc.Advance();
					break;
				case '{':
					pc.Advance();
					pc.TrySkipWhiteSpace();
					pc.Expecting('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ',','}');
					var min = -1;
					var max = -1;
					if(','!=pc.Codepoint && '}'!=pc.Codepoint)
					{
						var l = pc.CaptureBuffer.Length;
						pc.TryReadDigits();
						min = int.Parse(pc.GetCapture(l), CultureInfo.InvariantCulture.NumberFormat);
						pc.TrySkipWhiteSpace();
					}
					if (','==pc.Codepoint)
					{
						pc.Advance();
						pc.TrySkipWhiteSpace();
						pc.Expecting('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '}');
						if('}'!=pc.Codepoint)
						{
							var l = pc.CaptureBuffer.Length;
							pc.TryReadDigits();
							max = int.Parse(pc.GetCapture(l),CultureInfo.InvariantCulture.NumberFormat);
							pc.TrySkipWhiteSpace();
						}
					} else { max = min; }
					pc.Expecting('}');
					pc.Advance();
					expr = new RegexRepeatExpression(expr, min, max);
					expr.SetLocation(position);
					break;
			}
			return expr;
		}
		/// <summary>
		/// Appends a character escape to the specified <see cref="StringBuilder"/>
		/// </summary>
		/// <param name="ch">The character to escape</param>
		/// <param name="builder">The string builder to append to</param>
		internal static void AppendEscapedChar(string character,StringBuilder builder)
		{
			int codepoint = char.ConvertToUtf32(character,0);
			switch (codepoint)
			{
				case '.':
				case '/': // js expects this
				case '(':
				case ')':
				case '[':
				case ']':
				case '<': // flex expects this
				case '>':
				case '|':
				case ';': // flex expects this
				case '\'': // pck expects this
				case '\"':
				case '{':
				case '}':
				case '?':
				case '*':
				case '+':
				case '$':
				case '^':
				case '\\':
					builder.Append('\\');
					builder.Append(character);
					return;
				case '\t':
					builder.Append("\\t");
					return;
				case '\n':
					builder.Append("\\n");
					return;
				case '\r':
					builder.Append("\\r");
					return;
				case '\0':
					builder.Append("\\0");
					return;
				case '\f':
					builder.Append("\\f");
					return;
				case '\v':
					builder.Append("\\v");
					return;
				case '\b':
					builder.Append("\\b");
					return;
				
				default:
					if (!char.IsLetterOrDigit(character,0) && !char.IsSeparator(character,0) && !char.IsPunctuation(character,0) && !char.IsSymbol(character,0))
					{

						builder.Append("\\u");
						builder.Append(unchecked(codepoint.ToString("x4")));
					}
					else
						builder.Append(character);
					break;
			}

		}
		/// <summary>
		/// Escapes the specified character
		/// </summary>
		/// <param name="ch">The character to escape</param>
		/// <returns>A string representing the escaped character</returns>
		internal static string EscapeChar(string character)
		{
			var codepoint = char.ConvertToUtf32(character, 0);
			switch (codepoint)
			{
				case '.':
				case '/': // js expects this
				case '(':
				case ')':
				case '[':
				case ']':
				case '<': // flex expects this
				case '>':
				case '|':
				case ';': // flex expects this
				case '\'': // pck expects this
				case '\"':
				case '{':
				case '}':
				case '?':
				case '*':
				case '+':
				case '$':
				case '^':
				case '\\':
					return string.Concat("\\", character);
				case '\t':
					return "\\t";
				case '\n':
					return "\\n";
				case '\r':
					return "\\r";
				case '\0':
					return "\\0";
				case '\f':
					return "\\f";
				case '\v':
					return "\\v";
				case '\b':
					return "\\b";
				default:
					if (!char.IsLetterOrDigit(character, 0) && !char.IsSeparator(character, 0) && !char.IsPunctuation(character, 0) && !char.IsSymbol(character, 0))
					{

						return string.Concat("\\x",unchecked(codepoint).ToString("x4"));

					}
					else
						return string.Concat(character);
			}
		}
		/// <summary>
		/// Appends an escaped range character to the specified <see cref="StringBuilder"/>
		/// </summary>
		/// <param name="rangeCharacter">The range character to escape</param>
		/// <param name="builder">The string builder to append to</param>
		internal static void AppendEscapedRangeChar(string rangeCharacter,StringBuilder builder)
		{
			var codepoint = char.ConvertToUtf32(rangeCharacter, 0); 
			switch (codepoint)
			{
				case '.':
				case '/': // js expects this
				case '(':
				case ')':
				case '[':
				case ']':
				case '<': // flex expects this
				case '>':
				case '|':
				case ':': // expected by posix (sort of, Posix doesn't allow escapes in ranges, but standard extensions do)
				case ';': // flex expects this
				case '\'': // pck expects this
				case '\"':
				case '{':
				case '}':
				case '?':
				case '*':
				case '+':
				case '$':
				case '^':
				case '-':
				case '\\':
					builder.Append('\\');
					builder.Append(rangeCharacter);
					return;
				case '\t':
					builder.Append("\\t");
					return;
				case '\n':
					builder.Append("\\n");
					return;
				case '\r':
					builder.Append("\\r");
					return;
				case '\0':
					builder.Append("\\0");
					return;
				case '\f':
					builder.Append("\\f");
					return;
				case '\v':
					builder.Append("\\v");
					return;
				case '\b':
					builder.Append("\\b");
					return;
				default:
					if (!char.IsLetterOrDigit(rangeCharacter,0) && !char.IsSeparator(rangeCharacter,0) && !char.IsPunctuation(rangeCharacter,0) && !char.IsSymbol(rangeCharacter,0))
					{

						builder.Append("\\u");
						builder.Append(unchecked(codepoint).ToString("x4"));

					}
					else
						builder.Append(rangeCharacter);
					break;
			}
		}
		/// <summary>
		/// Escapes a range character
		/// </summary>
		/// <param name="ch">The character to escape</param>
		/// <returns>A string containing the escaped character</returns>
		internal static string EscapeRangeChar(string character)
		{
			var codepoint = char.ConvertToUtf32(character, 0);
			switch (codepoint)
			{
				case '.':
				case '/': // js expects this
				case '(':
				case ')':
				case '[':
				case ']':
				case '<': // flex expects this
				case '>':
				case '|':
				case ':': // expected by posix (sort of, Posix doesn't allow escapes in ranges, but standard extensions do)
				case ';': // flex expects this
				case '\'': // pck expects this
				case '\"':
				case '{':
				case '}':
				case '?':
				case '*':
				case '+':
				case '$':
				case '^':
				case '-':
				case '\\':
					return string.Concat("\\", character);
				case '\t':
					return "\\t";
				case '\n':
					return "\\n";
				case '\r':
					return "\\r";
				case '\0':
					return "\\0";
				case '\f':
					return "\\f";
				case '\v':
					return "\\v";
				case '\b':
					return "\\b";
				default:
					if (!char.IsLetterOrDigit(character,0) && !char.IsSeparator(character,0) && !char.IsPunctuation(character,0) && !char.IsSymbol(character,0))
					{

						return string.Concat("\\x", unchecked(codepoint).ToString("x4"));

					}
					else
						return string.Concat(character);
			}
		}
		static byte _FromHexChar(int hex)
		{
			if (':' > hex && '/' < hex)
				return (byte)(hex - '0');
			if ('G' > hex && '@' < hex)
				return (byte)(hex - '7'); // 'A'-10
			if ('g' > hex && '`' < hex)
				return (byte)(hex - 'W'); // 'a'-10
			throw new ArgumentException("The value was not hex.", "hex");
		}
		static bool _IsHexChar(int hex)
		{
			if (':' > hex && '/' < hex)
				return true;
			if ('G' > hex && '@' < hex)
				return true;
			if ('g' > hex && '`' < hex)
				return true;
			return false;
		}
		// return type is either char or ranges. this is kind of a union return type.
		static int _ParseEscapePart(FA.StringCursor pc)
		{
			if (-1 == pc.Codepoint) return -1;
			switch (pc.Codepoint)
			{
				case 'f':
					pc.Advance();
					return '\f';
				case 'v':
					pc.Advance();
					return '\v';
				case 't':
					pc.Advance();
					return '\t';
				case 'n':
					pc.Advance();
					return '\n';
				case 'r':
					pc.Advance();
					return '\r';
				case 'x':
					if (-1 == pc.Advance() || !_IsHexChar(pc.Codepoint))
						return 'x';
					byte b = _FromHexChar(pc.Codepoint);
					if (-1 == pc.Advance() || !_IsHexChar(pc.Codepoint))
						return unchecked(b);
					b <<= 4;
					b |= _FromHexChar(pc.Codepoint);
					if (-1 == pc.Advance() || !_IsHexChar(pc.Codepoint))
						return unchecked(b);
					b <<= 4;
					b |= _FromHexChar(pc.Codepoint);
					if (-1 == pc.Advance() || !_IsHexChar(pc.Codepoint))
						return unchecked(b);
					b <<= 4;
					b |= _FromHexChar(pc.Codepoint);
					return unchecked(b);
				case 'u':
					if (-1 == pc.Advance())
						return 'u';
					ushort u = _FromHexChar(pc.Codepoint);
					u <<= 4;
					if (-1 == pc.Advance())
						return unchecked(u);
					u |= _FromHexChar(pc.Codepoint);
					u <<= 4;
					if (-1 == pc.Advance())
						return unchecked(u);
					u |= _FromHexChar(pc.Codepoint);
					u <<= 4;
					if (-1 == pc.Advance())
						return unchecked(u);
					u |= _FromHexChar(pc.Codepoint);
					return unchecked(u);
				default:
					int i = pc.Codepoint;
					pc.Advance();
					return i;
			}
		}
		static int _ParseRangeEscapePart(FA.StringCursor pc)
		{
			if (-1== pc.Codepoint)
				return -1;
			switch (pc.Codepoint)
			{
				case 'f':
					pc.Advance();
					return '\f';
				case 'v':
					pc.Advance();
					return '\v';
				case 't':
					pc.Advance();
					return '\t';
				case 'n':
					pc.Advance();
					return '\n';
				case 'r':
					pc.Advance();
					return '\r';
				case 'x':
					if (-1 == pc.Advance() || !_IsHexChar(pc.Codepoint))
						return 'x';
					byte b = _FromHexChar(pc.Codepoint);
					if (-1 == pc.Advance() || !_IsHexChar(pc.Codepoint))
						return unchecked(b);
					b <<= 4;
					b |= _FromHexChar(pc.Codepoint);
					if (-1 == pc.Advance() || !_IsHexChar(pc.Codepoint))
						return unchecked(b);
					b <<= 4;
					b |= _FromHexChar(pc.Codepoint);
					if (-1 == pc.Advance() || !_IsHexChar(pc.Codepoint))
						return unchecked(b);
					b <<= 4;
					b |= _FromHexChar(pc.Codepoint);
					return unchecked(b);
				case 'u':
					if (-1 == pc.Advance())
						return 'u';
					ushort u = _FromHexChar(pc.Codepoint);
					u <<= 4;
					if (-1 == pc.Advance())
						return unchecked(u);
					u |= _FromHexChar(pc.Codepoint);
					u <<= 4;
					if (-1 == pc.Advance())
						return unchecked(u);
					u |= _FromHexChar(pc.Codepoint);
					u <<= 4;
					if (-1 == pc.Advance())
						return unchecked(u);
					u |= _FromHexChar(pc.Codepoint);
					return unchecked(u);
				default:
					int i = pc.Codepoint;
					pc.Advance();
					return i;
			}
		}
		static int _ReadRangeChar(IEnumerator<int> e)
		{
			int ch;
			if ('\\' != e.Current || !e.MoveNext())
			{
				return e.Current;
			}
			ch = e.Current;
			switch (ch)
			{
				case 't':
					ch = '\t';
					break;
				case 'n':
					ch = '\n';
					break;
				case 'r':
					ch = '\r';
					break;
				case '0':
					ch = '\0';
					break;
				case 'v':
					ch = '\v';
					break;
				case 'f':
					ch = '\f';
					break;
				case 'b':
					ch = '\b';
					break;
				case 'x':
					if (!e.MoveNext())
						throw new Exception("Expecting input for escape \\x");
					ch = e.Current;
					byte x = _FromHexChar(ch);
					if (!e.MoveNext())
					{
						ch = unchecked(x);
						return ch;
					}
					x *= 0x10;
					x += _FromHexChar(e.Current);
					ch = unchecked(x);
					break;
				case 'u':
					if (!e.MoveNext())
						throw new Exception("Expecting input for escape \\u");
					ch = e.Current;
					ushort u = _FromHexChar(ch);
					if (!e.MoveNext())
					{
						ch = unchecked(u);
						return ch;
					}
					u *= 0x10;
					u += _FromHexChar(e.Current);
					if (!e.MoveNext())
					{
						ch = unchecked(u);
						return ch;
					}
					u *= 0x10;
					u += _FromHexChar(e.Current);
					if (!e.MoveNext())
					{
						ch = unchecked(u);
						return ch;
					}
					u *= 0x10;
					u += _FromHexChar(e.Current);
					ch = unchecked(u);
					break;
				default: // return itself
					break;
			}
			return ch;
		}
		static IEnumerable<KeyValuePair<int,int>> _ParseRanges(IEnumerable<int> charRanges)
		{
			using (var e = charRanges.GetEnumerator())
			{
				var skipRead = false;

				while (skipRead || e.MoveNext())
				{
					skipRead = false;
					int first = _ReadRangeChar(e);
					if (e.MoveNext())
					{
						if ('-' == e.Current)
						{
							if (e.MoveNext())
								yield return new KeyValuePair<int, int>(first, _ReadRangeChar(e));
							else
								yield return new KeyValuePair<int, int>('-', '-');
						}
						else
						{
							yield return new KeyValuePair<int, int>(first, first);
							skipRead = true;
							continue;

						}
					}
					else
					{
						yield return new KeyValuePair<int, int>(first, first);
						yield break;
					}
				}
			}
			yield break;
		}
	}
	/// <summary>
	/// Represents a binary expression
	/// </summary>
#if FALIB
	public
#endif
	abstract partial class RegexBinaryExpression : RegexExpression
	{
		/// <summary>
		/// Indicates the left hand expression
		/// </summary>
		public RegexExpression Left { get; set; }
		/// <summary>
		/// Indicates the right hand expression
		/// </summary>
		public RegexExpression Right { get; set; }
	}
	/// <summary>
	/// Represents an expression with a single target expression
	/// </summary>
#if FALIB
	public
#endif

	abstract partial class RegexUnaryExpression : RegexExpression
	{
		/// <summary>
		/// Indicates the target expression
		/// </summary>
		public RegexExpression Expression { get; set; }

	}
	/// <summary>
	/// Represents a single character literal
	/// </summary>
#if FALIB
	public
#endif
	partial class RegexLiteralExpression : RegexExpression, IEquatable<RegexLiteralExpression>
	{
		/// <summary>
		/// Indicates whether or not this statement is a single element or not
		/// </summary>
		/// <remarks>If false, this statement will be wrapped in parentheses if necessary</remarks>
		public override bool IsSingleElement => Codepoints == null || Codepoints.Length < 2;
		/// <summary>
		/// Indicates the codepoints in this expression
		/// </summary>
		public int[] Codepoints { get; set; } = null;
		/// <summary>
		/// Indicates the string literal of this expression
		/// </summary>
		public string Value {
			get {
				if(Codepoints==null)
				{
					return null;
				}
				if(Codepoints.Length==0)
				{
					return "";
				}
				var sb = new StringBuilder();
				for(int i = 0;i<Codepoints.Length;i++)
				{
					sb.Append(char.ConvertFromUtf32(Codepoints[i]));
				}
				return sb.ToString();
			}
			set {
				if (value == null) throw new NullReferenceException();
				if (value.Length == 0 || value.Length > 2) throw new InvalidOperationException();
				if (value == null)
				{
					Codepoints = null;
				} else if (value.Length == 0)
				{
					Codepoints = new int[0];
				} else {
					var list = new List<int>(FA.ToUtf32(value));
					Codepoints = list.ToArray();
				}
			}
		}
		/// <summary>
		/// Creates a literal expression with the specified codepoints
		/// </summary>
		/// <param name="codepoints">The characters to represent</param>
		public RegexLiteralExpression(int[] codepoints) { Codepoints = codepoints; }

		/// <summary>
		/// Creates a literal expression with the specified string
		/// </summary>
		/// <param name="string">The string to represent</param>
		public RegexLiteralExpression(string @string) { Value = @string; }
		/// <summary>
		/// Creates a default instance of the expression
		/// </summary>
		public RegexLiteralExpression() { }
		/// <summary>
		/// Creates a state machine representing this expression
		/// </summary>
		/// <param name="accept">The accept symbol to use for this expression</param>
		/// <returns>A new <see cref="FA"/> finite state machine representing this expression</returns>
		public override FA ToFA(int accept = 0, bool compact = true)
			=> FA.Literal(Codepoints, accept, compact);
		/// <summary>
		/// Appends the textual representation to a <see cref="StringBuilder"/>
		/// </summary>
		/// <param name="sb">The string builder to use</param>
		/// <remarks>Used by ToString()</remarks>
		protected internal override void AppendTo(StringBuilder sb)
		{
			foreach(var cp in Codepoints)
			{
				AppendEscapedChar(char.ConvertFromUtf32(cp), sb);
			}
		}
			
			
		/// <summary>
		/// Creates a new copy of this expression
		/// </summary>
		/// <returns>A new copy of this expression</returns>
		protected override RegexExpression CloneImpl()
			=> Clone();
		/// <summary>
		/// Creates a new copy of this expression
		/// </summary>
		/// <returns>A new copy of this expression</returns>
		public RegexLiteralExpression Clone()
		{
			return new RegexLiteralExpression(Value);
		}

		#region Value semantics
		/// <summary>
		/// Indicates whether this expression is the same as the right hand expression
		/// </summary>
		/// <param name="rhs">The expression to compare</param>
		/// <returns>True if the expressions are the same, otherwise false</returns>
		public bool Equals(RegexLiteralExpression rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			if (ReferenceEquals(Codepoints, rhs.Codepoints)) return true;
			if (ReferenceEquals(Codepoints, null) || ReferenceEquals(rhs.Codepoints, null)) return false;
            if (Codepoints.Length!=rhs.Codepoints.Length)
			{
				return false;
			}
            for(int i = 0; i < Codepoints.Length;i++)
			{
				if (Codepoints[i] != rhs.Codepoints[i])
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// Indicates whether this expression is the same as the right hand expression
		/// </summary>
		/// <param name="rhs">The expression to compare</param>
		/// <returns>True if the expressions are the same, otherwise false</returns>
		public override bool Equals(object rhs)
			=> Equals(rhs as RegexLiteralExpression);
		/// <summary>
		/// Computes a hash code for this expression
		/// </summary>
		/// <returns>A hash code for this expression</returns>
		public override int GetHashCode()
			=> Value.GetHashCode();
		/// <summary>
		/// Indicates whether or not two expression are the same
		/// </summary>
		/// <param name="lhs">The left hand expression to compare</param>
		/// <param name="rhs">The right hand expression to compare</param>
		/// <returns>True if the expressions are the same, otherwise false</returns>
		public static bool operator ==(RegexLiteralExpression lhs, RegexLiteralExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		/// <summary>
		/// Indicates whether or not two expression are different
		/// </summary>
		/// <param name="lhs">The left hand expression to compare</param>
		/// <param name="rhs">The right hand expression to compare</param>
		/// <returns>True if the expressions are different, otherwise false</returns>
		public static bool operator !=(RegexLiteralExpression lhs, RegexLiteralExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion

	}
	/// <summary>
	/// Represents the base partial class for regex charset entries
	/// </summary>
#if FALIB
	public
#endif
	abstract partial class RegexCharsetEntry : ICloneable
	{
		/// <summary>
		/// Initializes the charset entry
		/// </summary>
		internal RegexCharsetEntry() { } // nobody can make new derivations
		/// <summary>
		/// Implements the clone method
		/// </summary>
		/// <returns>A copy of the charset entry</returns>
		protected abstract RegexCharsetEntry CloneImpl();
		/// <summary>
		/// Creates a copy of the charset entry
		/// </summary>
		/// <returns>A new copy of the charset entry</returns>
		object ICloneable.Clone() => CloneImpl();
	}
	/// <summary>
	/// Represents a character partial class charset entry
	/// </summary>
#if FALIB
	public
#endif
	partial class RegexCharsetClassEntry : RegexCharsetEntry
	{
		/// <summary>
		/// Initializes a partial class entry with the specified character partial class
		/// </summary>
		/// <param name="name">The name of the character partial class</param>
		public RegexCharsetClassEntry(string name)
		{
			Name = name;
		}
		/// <summary>
		/// Initializes a default instance of the charset entry
		/// </summary>
		public RegexCharsetClassEntry() { }
		/// <summary>
		/// Indicates the name of the character partial class
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Gets a string representation of this instance
		/// </summary>
		/// <returns>The string representation of this character partial class</returns>
		public override string ToString()
		{
			return string.Concat("[:", Name, ":]");
		}
		/// <summary>
		/// Clones the object
		/// </summary>
		/// <returns>A new copy of the charset entry</returns>
		protected override RegexCharsetEntry CloneImpl()
			=> Clone();
		/// <summary>
		/// Clones the object
		/// </summary>
		/// <returns>A new copy of the charset entry</returns>
		public RegexCharsetClassEntry Clone()
		{
			return new RegexCharsetClassEntry(Name);
		}

		#region Value semantics
		/// <summary>
		/// Indicates whether this charset entry is the same as the right hand charset entry
		/// </summary>
		/// <param name="rhs">The charset entry to compare</param>
		/// <returns>True if the charset entries are the same, otherwise false</returns>
		public bool Equals(RegexCharsetClassEntry rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return Name == rhs.Name;
		}
		/// <summary>
		/// Indicates whether this charset entry is the same as the right hand charset entry
		/// </summary>
		/// <param name="rhs">The charset entry to compare</param>
		/// <returns>True if the charset entries are the same, otherwise false</returns>
		public override bool Equals(object rhs)
			=> Equals(rhs as RegexCharsetClassEntry);
		/// <summary>
		/// Computes a hash code for this charset entry
		/// </summary>
		/// <returns>A hash code for this charset entry</returns>
		public override int GetHashCode()
		{
			if (string.IsNullOrEmpty(Name)) return Name.GetHashCode();
			return 0;
		}
		/// <summary>
		/// Indicates whether or not two charset entries are the same
		/// </summary>
		/// <param name="lhs">The left hand charset entry to compare</param>
		/// <param name="rhs">The right hand charset entry to compare</param>
		/// <returns>True if the charset entries are the same, otherwise false</returns>
		public static bool operator ==(RegexCharsetClassEntry lhs, RegexCharsetClassEntry rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		/// <summary>
		/// Indicates whether or not two charset entries are different
		/// </summary>
		/// <param name="lhs">The left hand charset entry to compare</param>
		/// <param name="rhs">The right hand charset entry to compare</param>
		/// <returns>True if the charset entries are different, otherwise false</returns>
		public static bool operator !=(RegexCharsetClassEntry lhs, RegexCharsetClassEntry rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion
	}
	/// <summary>
	/// Represents a single character charset entry
	/// </summary>
#if FALIB
	public
#endif
	partial class RegexCharsetCharEntry : RegexCharsetEntry, IEquatable<RegexCharsetCharEntry>
	{
		/// <summary>
		/// Initializes the entry with a character
		/// </summary>
		/// <param name="codepoint">The character to use</param>
		public RegexCharsetCharEntry(int codepoint)
		{
			Codepoint = codepoint;
		}
		/// <summary>
		/// Initializes the entry with a character
		/// </summary>
		/// <param name="character">The character to use</param>
		public RegexCharsetCharEntry(string character)
		{
			Value = character;
		}
		/// <summary>
		/// Initializes a default instance of the charset entry
		/// </summary>
		public RegexCharsetCharEntry() { }
		/// <summary>
		/// Indicates the character the charset entry represents
		/// </summary>
		public int Codepoint { get; set; }
		/// <summary>
		/// Indicates the character literal of this expression
		/// </summary>
		public string Value {
			get {
				return char.ConvertFromUtf32(Codepoint);
			}
			set {
				if (value == null) throw new NullReferenceException();
				if (value.Length == 0 || value.Length > 2) throw new InvalidOperationException();
				Codepoint = char.ConvertToUtf32(value, 0);
			}
		}
		/// <summary>
		/// Gets a string representation of the charset entry
		/// </summary>
		/// <returns>The string representation of this charset entry</returns>
		public override string ToString()
		{
			return RegexExpression.EscapeRangeChar(Value);
		}
		/// <summary>
		/// Clones the object
		/// </summary>
		/// <returns>A new copy of the charset entry</returns>
		protected override RegexCharsetEntry CloneImpl()
			=> Clone();
		/// <summary>
		/// Clones the object
		/// </summary>
		/// <returns>A new copy of the charset entry</returns>
		public RegexCharsetCharEntry Clone()
		{
			return new RegexCharsetCharEntry(Codepoint);
		}

		#region Value semantics
		/// <summary>
		/// Indicates whether this charset entry is the same as the right hand charset entry
		/// </summary>
		/// <param name="rhs">The charset entry to compare</param>
		/// <returns>True if the charset entries are the same, otherwise false</returns>
		public bool Equals(RegexCharsetCharEntry rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return Codepoint == rhs.Codepoint;
		}
		/// <summary>
		/// Indicates whether this charset entry is the same as the right hand charset entry
		/// </summary>
		/// <param name="rhs">The charset entry to compare</param>
		/// <returns>True if the charset entries are the same, otherwise false</returns>
		public override bool Equals(object rhs)
			=> Equals(rhs as RegexCharsetCharEntry);
		/// <summary>
		/// Computes a hash code for this charset entry
		/// </summary>
		/// <returns>A hash code for this charset entry</returns>
		public override int GetHashCode()
			=> Codepoint.GetHashCode();
		/// <summary>
		/// Indicates whether or not two charset entries are the same
		/// </summary>
		/// <param name="lhs">The left hand charset entry to compare</param>
		/// <param name="rhs">The right hand charset entry to compare</param>
		/// <returns>True if the charset entries are the same, otherwise false</returns>
		public static bool operator ==(RegexCharsetCharEntry lhs, RegexCharsetCharEntry rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		/// <summary>
		/// Indicates whether or not two charset entries are different
		/// </summary>
		/// <param name="lhs">The left hand charset entry to compare</param>
		/// <param name="rhs">The right hand charset entry to compare</param>
		/// <returns>True if the charset entries are different, otherwise false</returns>
		public static bool operator !=(RegexCharsetCharEntry lhs, RegexCharsetCharEntry rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion
	}
	/// <summary>
	/// Represents a character set range entry
	/// </summary>
#if FALIB
	public
#endif
	partial class RegexCharsetRangeEntry : RegexCharsetEntry
	{
		/// <summary>
		/// Creates a new range entry with the specified first and last characters
		/// </summary>
		/// <param name="firstCodepoint">The first character in the range</param>
		/// <param name="lastCodepoint">The last character in the range</param>
		public RegexCharsetRangeEntry(int firstCodepoint, int lastCodepoint)
		{
			FirstCodepoint = firstCodepoint;
			LastCodepoint = lastCodepoint;
		}
		/// <summary>
		/// Creates a new range entry with the specified first and last characters
		/// </summary>
		/// <param name="first">The first character in the range</param>
		/// <param name="last">The last character in the range</param>
		public RegexCharsetRangeEntry(string first, string last)
		{
			First = first;
			Last = last;
		}
		/// <summary>
		/// Creates a default instance of the range entry
		/// </summary>
		public RegexCharsetRangeEntry()
		{
		}
		/// <summary>
		/// Indicates the first character in the range
		/// </summary>
		public int FirstCodepoint { get; set; }
		/// <summary>
		/// Indicates the first character literal of this expression
		/// </summary>
		public string First {
			get {
				return char.ConvertFromUtf32(FirstCodepoint);
			}
			set {
				if (value == null) throw new NullReferenceException();
				if (value.Length == 0 || value.Length > 2) throw new InvalidOperationException();
				FirstCodepoint = char.ConvertToUtf32(value, 0);
			}
		}
		/// <summary>
		/// Indicates the last character in the range
		/// </summary>
		public int LastCodepoint { get; set; }
		/// <summary>
		/// Indicates the last character literal of this expression
		/// </summary>
		public string Last {
			get {
				return char.ConvertFromUtf32(LastCodepoint);
			}
			set {
				if (value == null) throw new NullReferenceException();
				if (value.Length == 0 || value.Length > 2) throw new InvalidOperationException();
				LastCodepoint = char.ConvertToUtf32(value, 0);
			}
		}
		/// <summary>
		/// Clones the object
		/// </summary>
		/// <returns>A new copy of the charset entry</returns>
		protected override RegexCharsetEntry CloneImpl()
			=> Clone();
		/// <summary>
		/// Clones the object
		/// </summary>
		/// <returns>A new copy of the charset entry</returns>
		public RegexCharsetRangeEntry Clone()
		{
			return new RegexCharsetRangeEntry(FirstCodepoint, LastCodepoint);
		}
		/// <summary>
		/// Gets a string representation of the charset entry
		/// </summary>
		/// <returns>The string representation of this charset entry</returns>
		public override string ToString()
		{
			return string.Concat(RegexExpression.EscapeRangeChar(First), "-", RegexExpression.EscapeRangeChar(Last));
		}
		#region Value semantics
		/// <summary>
		/// Indicates whether this charset entry is the same as the right hand charset entry
		/// </summary>
		/// <param name="rhs">The charset entry to compare</param>
		/// <returns>True if the charset entries are the same, otherwise false</returns>
		public bool Equals(RegexCharsetRangeEntry rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return FirstCodepoint == rhs.FirstCodepoint && LastCodepoint == rhs.LastCodepoint;
		}
		/// <summary>
		/// Indicates whether this charset entry is the same as the right hand charset entry
		/// </summary>
		/// <param name="rhs">The charset entry to compare</param>
		/// <returns>True if the charset entries are the same, otherwise false</returns>
		public override bool Equals(object rhs)
			=> Equals(rhs as RegexCharsetRangeEntry);
		/// <summary>
		/// Computes a hash code for this charset entry
		/// </summary>
		/// <returns>A hash code for this charset entry</returns>
		public override int GetHashCode()
			=> FirstCodepoint.GetHashCode() ^ LastCodepoint.GetHashCode();
		/// <summary>
		/// Indicates whether or not two charset entries are the same
		/// </summary>
		/// <param name="lhs">The left hand charset entry to compare</param>
		/// <param name="rhs">The right hand charset entry to compare</param>
		/// <returns>True if the charset entries are the same, otherwise false</returns>
		public static bool operator ==(RegexCharsetRangeEntry lhs, RegexCharsetRangeEntry rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		/// <summary>
		/// Indicates whether or not two charset entries are different
		/// </summary>
		/// <param name="lhs">The left hand charset entry to compare</param>
		/// <param name="rhs">The right hand charset entry to compare</param>
		/// <returns>True if the charset entries are different, otherwise false</returns>
		public static bool operator !=(RegexCharsetRangeEntry lhs, RegexCharsetRangeEntry rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion
	}
	/// <summary>
	/// Indicates a charset expression
	/// </summary>
	/// <remarks>Represented by [] in regular expression syntax</remarks>
#if FALIB
	public
#endif
	partial class RegexCharsetExpression : RegexExpression, IEquatable<RegexCharsetExpression>
	{
		/// <summary>
		/// Indicates the <see cref="RegexCharsetEntry"/> entries in the character set
		/// </summary>
		public IList<RegexCharsetEntry> Entries { get; } = new List<RegexCharsetEntry>();
		/// <summary>
		/// Creates a new charset expression with the specified entries and optionally negated
		/// </summary>
		/// <param name="entries">The entries to initialize the charset with</param>
		/// <param name="hasNegatedRanges">True if the range is a "not range" like [^], otherwise false</param>
		public RegexCharsetExpression(IEnumerable<RegexCharsetEntry> entries, bool hasNegatedRanges = false)
		{
			foreach (var entry in entries)
				Entries.Add(entry);
			HasNegatedRanges = hasNegatedRanges;
		}
		/// <summary>
		/// Creates a default instance of the expression
		/// </summary>
		public RegexCharsetExpression() { }
		/// <summary>
		/// Creates a state machine representing this expression
		/// </summary>
		/// <param name="accept">The accept symbol to use for this expression</param>
		/// <returns>A new <see cref="FA"/> finite state machine representing this expression</returns>
		public override FA ToFA(int accept = 0, bool compact = true)
		{
			var ranges = new List<FARange>();
			for (int ic = Entries.Count, i = 0; i < ic; ++i)
			{
				var entry = Entries[i];
				var crc = entry as RegexCharsetCharEntry;
				if (null != crc)
					ranges.Add(new FARange(crc.Codepoint, crc.Codepoint));
				var crr = entry as RegexCharsetRangeEntry;
				if (null != crr)
					ranges.Add(new FARange(crr.FirstCodepoint, crr.LastCodepoint));
				var crcl = entry as RegexCharsetClassEntry;
				if (null != crcl)
				{
					var known = FA.CharacterClasses.Known[crcl.Name];
					for (int j = 0; j < known.Length; j += 2)
					{
						ranges.Add(new FARange(known[j], known[j + 1]));
					}
				}
			}
			if (HasNegatedRanges)
				return FA.Set(FARange.ToNotRanges(ranges), accept,compact);
			return FA.Set(ranges, accept,compact);
		}
		/// <summary>
		/// Indicates whether the range is a "not range"
		/// </summary>
		/// <remarks>This is represented by the [^] regular expression syntax</remarks>
		public bool HasNegatedRanges { get; set; } = false;
		/// <summary>
		/// Indicates whether or not this statement is a single element or not
		/// </summary>
		/// <remarks>If false, this statement will be wrapped in parentheses if necessary</remarks>
		public override bool IsSingleElement => true;
		/// <summary>
		/// Appends the textual representation to a <see cref="StringBuilder"/>
		/// </summary>
		/// <param name="sb">The string builder to use</param>
		/// <remarks>Used by ToString()</remarks>
		protected internal override void AppendTo(StringBuilder sb)
		{
			// special case for "."
			if (1 == Entries.Count)
			{
				var dotE = Entries[0] as RegexCharsetRangeEntry;
				if (!HasNegatedRanges && null != dotE && dotE.FirstCodepoint == char.MinValue && dotE.LastCodepoint == char.MaxValue)
				{
					sb.Append(".");
					return;
				}
				var cls = Entries[0] as RegexCharsetClassEntry;
				if (null != cls)
				{
					switch (cls.Name)
					{
						case "blank":
							if (!HasNegatedRanges)
								sb.Append(@"\h");
							return;
						case "digit":
							if (!HasNegatedRanges)
								sb.Append(@"\d");
							else
								sb.Append(@"\D");
							return;
						case "lower":
							if (!HasNegatedRanges)
								sb.Append(@"\l");
							return;
						case "space":
							if (!HasNegatedRanges)
								sb.Append(@"\s");
							else
								sb.Append(@"\S");
							return;
						case "upper":
							if (!HasNegatedRanges)
								sb.Append(@"\u");
							return;
						case "word":
							if (!HasNegatedRanges)
								sb.Append(@"\w");
							else
								sb.Append(@"\W");
							return;

					}
				}
			}

			sb.Append('[');
			if (HasNegatedRanges)
				sb.Append('^');
			for (int ic = Entries.Count, i = 0; i < ic; ++i)
				sb.Append(Entries[i]);
			sb.Append(']');
		}
		/// <summary>
		/// Creates a new copy of this expression
		/// </summary>
		/// <returns>A new copy of this expression</returns>
		protected override RegexExpression CloneImpl()
			=> Clone();
		/// <summary>
		/// Creates a new copy of this expression
		/// </summary>
		/// <returns>A new copy of this expression</returns>
		public RegexCharsetExpression Clone()
		{
			return new RegexCharsetExpression(Entries, HasNegatedRanges);
		}
		#region Value semantics
		/// <summary>
		/// Indicates whether this expression is the same as the right hand expression
		/// </summary>
		/// <param name="rhs">The expression to compare</param>
		/// <returns>True if the expressions are the same, otherwise false</returns>
		public bool Equals(RegexCharsetExpression rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			if (HasNegatedRanges == rhs.HasNegatedRanges && rhs.Entries.Count == Entries.Count)
			{
				for (int ic = Entries.Count, i = 0; i < ic; ++i)
					if (Entries[i] != rhs.Entries[i])
						return false;
				return true;
			}
			return false;
		}
		/// <summary>
		/// Indicates whether this expression is the same as the right hand expression
		/// </summary>
		/// <param name="rhs">The expression to compare</param>
		/// <returns>True if the expressions are the same, otherwise false</returns>
		public override bool Equals(object rhs)
			=> Equals(rhs as RegexCharsetExpression);
		/// <summary>
		/// Computes a hash code for this expression
		/// </summary>
		/// <returns>A hash code for this expression</returns>
		public override int GetHashCode()
		{
			var result = HasNegatedRanges.GetHashCode();
			for (int ic = Entries.Count, i = 0; i < ic; ++i)
				result ^= Entries[i].GetHashCode();
			return result;
		}
		/// <summary>
		/// Indicates whether or not two expression are the same
		/// </summary>
		/// <param name="lhs">The left hand expression to compare</param>
		/// <param name="rhs">The right hand expression to compare</param>
		/// <returns>True if the expressions are the same, otherwise false</returns>
		public static bool operator ==(RegexCharsetExpression lhs, RegexCharsetExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		/// <summary>
		/// Indicates whether or not two expression are different
		/// </summary>
		/// <param name="lhs">The left hand expression to compare</param>
		/// <param name="rhs">The right hand expression to compare</param>
		/// <returns>True if the expressions are different, otherwise false</returns>
		public static bool operator !=(RegexCharsetExpression lhs, RegexCharsetExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion
	}
	/// <summary>
	/// Represents a concatenation between two expression. This has no operator as it is implicit.
	/// </summary>
#if FALIB
	public
#endif
	partial class RegexConcatExpression : RegexBinaryExpression, IEquatable<RegexConcatExpression>
	{
		/// <summary>
		/// Indicates whether or not this statement is a single element or not
		/// </summary>
		/// <remarks>If false, this statement will be wrapped in parentheses if necessary</remarks>
		public override bool IsSingleElement {
			get {
				if (null == Left)
					return null == Right ? false : Right.IsSingleElement;
				else if (null == Right)
					return Left.IsSingleElement;
				return false;
			}
		}
		/// <summary>
		/// Creates a new expression with the specified left and right hand sides
		/// </summary>
		/// <param name="left">The left expression</param>
		/// <param name="right">The right expressions</param>
		public RegexConcatExpression(RegexExpression left, params RegexExpression[] right)
		{
			Left = left;
			for (int i = 0; i < right.Length; i++)
			{
				var r = right[i];
				if (null == Right)
					Right = r;
				if (i != right.Length - 1)
				{
					var c = new RegexConcatExpression();
					c.Left = Left;
					c.Right = Right;
					Right = null;
					Left = c;
				}

			}
		}
		/// <summary>
		/// Creates a default instance of the expression
		/// </summary>
		public RegexConcatExpression() { }
		/// <summary>
		/// Creates a state machine representing this expression
		/// </summary>
		/// <param name="accept">The accept symbol to use for this expression</param>
		/// <returns>A new <see cref="FA"/> finite state machine representing this expression</returns>
		public override FA ToFA(int accept = 0, bool compact = true)
		{
			if (null == Left)
				return (null != Right) ? Right.ToFA(accept) : null;
			else if (null == Right)
				return Left.ToFA(accept);
			return FA.Concat(new FA[] { Left.ToFA(accept), Right.ToFA(accept) }, accept,compact);
		}
		/// <summary>
		/// Creates a new copy of this expression
		/// </summary>
		/// <returns>A new copy of this expression</returns>
		protected override RegexExpression CloneImpl()
			=> Clone();
		/// <summary>
		/// Creates a new copy of this expression
		/// </summary>
		/// <returns>A new copy of this expression</returns>
		public RegexConcatExpression Clone()
		{
			return new RegexConcatExpression(Left, Right);
		}
		#region Value semantics
		/// <summary>
		/// Indicates whether this expression is the same as the right hand expression
		/// </summary>
		/// <param name="rhs">The expression to compare</param>
		/// <returns>True if the expressions are the same, otherwise false</returns>
		public bool Equals(RegexConcatExpression rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return Left == rhs.Left && Right == rhs.Right;
		}
		/// <summary>
		/// Indicates whether this expression is the same as the right hand expression
		/// </summary>
		/// <param name="rhs">The expression to compare</param>
		/// <returns>True if the expressions are the same, otherwise false</returns>
		public override bool Equals(object rhs)
			=> Equals(rhs as RegexConcatExpression);
		/// <summary>
		/// Computes a hash code for this expression
		/// </summary>
		/// <returns>A hash code for this expression</returns>
		public override int GetHashCode()
		{
			var result = 0;
			if (null != Left)
				result ^= Left.GetHashCode();
			if (null != Right)
				result ^= Right.GetHashCode();
			return result;
		}
		/// <summary>
		/// Indicates whether or not two expression are the same
		/// </summary>
		/// <param name="lhs">The left hand expression to compare</param>
		/// <param name="rhs">The right hand expression to compare</param>
		/// <returns>True if the expressions are the same, otherwise false</returns>
		public static bool operator ==(RegexConcatExpression lhs, RegexConcatExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		/// <summary>
		/// Indicates whether or not two expression are different
		/// </summary>
		/// <param name="lhs">The left hand expression to compare</param>
		/// <param name="rhs">The right hand expression to compare</param>
		/// <returns>True if the expressions are different, otherwise false</returns>
		public static bool operator !=(RegexConcatExpression lhs, RegexConcatExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion
		/// <summary>
		/// Appends the textual representation to a <see cref="StringBuilder"/>
		/// </summary>
		/// <param name="sb">The string builder to use</param>
		/// <remarks>Used by ToString()</remarks>
		protected internal override void AppendTo(StringBuilder sb)
		{
			if (null != Left)
			{
				var oe = Left as RegexOrExpression;
				if (null != oe)
					sb.Append('(');
				Left.AppendTo(sb);
				if (null != oe)
					sb.Append(')');
			}
			if (null != Right)
			{
				var oe = Right as RegexOrExpression;
				if (null != oe)
					sb.Append('(');
				Right.AppendTo(sb);
				if (null != oe)
					sb.Append(')');
			}
		}
	}
	/// <summary>
	/// Represents an "or" regular expression as indicated by |
	/// </summary>
#if FALIB
	public
#endif
	partial class RegexOrExpression : RegexBinaryExpression, IEquatable<RegexOrExpression>
	{
		/// <summary>
		/// Indicates whether or not this statement is a single element or not
		/// </summary>
		/// <remarks>If false, this statement will be wrapped in parentheses if necessary</remarks>
		public override bool IsSingleElement => false;
		/// <summary>
		/// Creates a new expression with the specified left and right hand sides
		/// </summary>
		/// <param name="left">The left expression</param>
		/// <param name="right">The right expressions</param>
		public RegexOrExpression(RegexExpression left, params RegexExpression[] right)
		{
			Left = left;
			for (int i = 0; i < right.Length; i++)
			{
				var r = right[i];
				if (null == Right)
					Right = r;
				if (i != right.Length - 1)
				{
					var c = new RegexOrExpression();
					c.Left = Left;
					c.Right = Right;
					Right = null;
					Left = c;
				}

			}
		}
		/// <summary>
		/// Creates a default instance of the expression
		/// </summary>
		public RegexOrExpression() { }
		/// <summary>
		/// Creates a state machine representing this expression
		/// </summary>
		/// <param name="accept">The accept symbol to use for this expression</param>
		/// <returns>A new <see cref="FA"/> finite state machine representing this expression</returns>
		public override FA ToFA(int accept = 0, bool compact = true)
		{
			var left = (null != Left) ? Left.ToFA(accept,compact) : null;
			var right = (null != Right) ? Right.ToFA(accept,compact) : null;
			return FA.Or(new FA[] { left, right }, accept,compact);
		}
		/// <summary>
		/// Appends the textual representation to a <see cref="StringBuilder"/>
		/// </summary>
		/// <param name="sb">The string builder to use</param>
		/// <remarks>Used by ToString()</remarks>
		protected internal override void AppendTo(StringBuilder sb)
		{
			if (null != Left)
				Left.AppendTo(sb);
			sb.Append('|');
			if (null != Right)
				Right.AppendTo(sb);
		}
		/// <summary>
		/// Creates a new copy of this expression
		/// </summary>
		/// <returns>A new copy of this expression</returns>
		protected override RegexExpression CloneImpl()
			=> Clone();
		/// <summary>
		/// Creates a new copy of this expression
		/// </summary>
		/// <returns>A new copy of this expression</returns>
		public RegexOrExpression Clone()
		{
			return new RegexOrExpression(Left, Right);
		}
		#region Value semantics
		/// <summary>
		/// Indicates whether this expression is the same as the right hand expression
		/// </summary>
		/// <param name="rhs">The expression to compare</param>
		/// <returns>True if the expressions are the same, otherwise false</returns>
		public bool Equals(RegexOrExpression rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return (Left == rhs.Left && Right == rhs.Right) ||
				(Left == rhs.Right && Right == rhs.Left);
		}
		/// <summary>
		/// Indicates whether this expression is the same as the right hand expression
		/// </summary>
		/// <param name="rhs">The expression to compare</param>
		/// <returns>True if the expressions are the same, otherwise false</returns>
		public override bool Equals(object rhs)
			=> Equals(rhs as RegexOrExpression);
		/// <summary>
		/// Computes a hash code for this expression
		/// </summary>
		/// <returns>A hash code for this expression</returns>
		public override int GetHashCode()
		{
			var result = 0;
			if (null != Left)
				result ^= Left.GetHashCode();
			if (null != Right)
				result ^= Right.GetHashCode();
			return result;
		}
		/// <summary>
		/// Indicates whether or not two expression are the same
		/// </summary>
		/// <param name="lhs">The left hand expression to compare</param>
		/// <param name="rhs">The right hand expression to compare</param>
		/// <returns>True if the expressions are the same, otherwise false</returns>
		public static bool operator ==(RegexOrExpression lhs, RegexOrExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		/// <summary>
		/// Indicates whether or not two expression are different
		/// </summary>
		/// <param name="lhs">The left hand expression to compare</param>
		/// <param name="rhs">The right hand expression to compare</param>
		/// <returns>True if the expressions are different, otherwise false</returns>
		public static bool operator !=(RegexOrExpression lhs, RegexOrExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion

	}
	/// <summary>
	/// Represents an optional expression, as indicated by ?
	/// </summary>
#if FALIB
	public
#endif
	partial class RegexOptionalExpression : RegexUnaryExpression, IEquatable<RegexOptionalExpression>
	{
		/// <summary>
		/// Indicates whether or not this statement is a single element or not
		/// </summary>
		/// <remarks>If false, this statement will be wrapped in parentheses if necessary</remarks>
		public override bool IsSingleElement => true;
		/// <summary>
		/// Creates an optional expression using the specified target expression
		/// </summary>
		/// <param name="expression">The target expression to make optional</param>
		public RegexOptionalExpression(RegexExpression expression) { Expression = expression; }
		/// <summary>
		/// Creates a default instance of the expression
		/// </summary>
		public RegexOptionalExpression() { }
		/// <summary>
		/// Creates a state machine representing this expression
		/// </summary>
		/// <param name="accept">The accept symbol to use for this expression</param>
		/// <returns>A new <see cref="FA"/> finite state machine representing this expression</returns>
		public override FA ToFA(int accept = 0, bool compact = true)
			=> null != Expression ? FA.Optional(Expression.ToFA(accept,compact), accept,compact) : null;
		/// <summary>
		/// Appends the textual representation to a <see cref="StringBuilder"/>
		/// </summary>
		/// <param name="sb">The string builder to use</param>
		/// <remarks>Used by ToString()</remarks>
		protected internal override void AppendTo(StringBuilder sb)
		{
			if (null == Expression)
				sb.Append("()?");
			else
			{
				var ise = Expression.IsSingleElement;
				if (!ise)
					sb.Append('(');
				Expression.AppendTo(sb);
				if (!ise)
					sb.Append(")?");
				else
					sb.Append('?');
			}
		}
		/// <summary>
		/// Creates a new copy of this expression
		/// </summary>
		/// <returns>A new copy of this expression</returns>
		protected override RegexExpression CloneImpl()
			=> Clone();
		/// <summary>
		/// Creates a new copy of this expression
		/// </summary>
		/// <returns>A new copy of this expression</returns>
		public RegexOptionalExpression Clone()
		{
			return new RegexOptionalExpression(Expression);
		}
		#region Value semantics
		/// <summary>
		/// Indicates whether this expression is the same as the right hand expression
		/// </summary>
		/// <param name="rhs">The expression to compare</param>
		/// <returns>True if the expressions are the same, otherwise false</returns>
		public bool Equals(RegexOptionalExpression rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return Equals(Expression, rhs.Expression);
		}
		/// <summary>
		/// Indicates whether this expression is the same as the right hand expression
		/// </summary>
		/// <param name="rhs">The expression to compare</param>
		/// <returns>True if the expressions are the same, otherwise false</returns>
		public override bool Equals(object rhs)
			=> Equals(rhs as RegexOptionalExpression);
		/// <summary>
		/// Computes a hash code for this expression
		/// </summary>
		/// <returns>A hash code for this expression</returns>
		public override int GetHashCode()
		{
			if (null != Expression)
				return Expression.GetHashCode();
			return 0;
		}
		/// <summary>
		/// Indicates whether or not two expression are the same
		/// </summary>
		/// <param name="lhs">The left hand expression to compare</param>
		/// <param name="rhs">The right hand expression to compare</param>
		/// <returns>True if the expressions are the same, otherwise false</returns>
		public static bool operator ==(RegexOptionalExpression lhs, RegexOptionalExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		/// <summary>
		/// Indicates whether or not two expression are different
		/// </summary>
		/// <param name="lhs">The left hand expression to compare</param>
		/// <param name="rhs">The right hand expression to compare</param>
		/// <returns>True if the expressions are different, otherwise false</returns>
		public static bool operator !=(RegexOptionalExpression lhs, RegexOptionalExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion
	}
	/// <summary>
	/// Represents a repeat regular expression as indicated by *, +, or {min,max}
	/// </summary>
#if FALIB
	public
#endif
	partial class RegexRepeatExpression : RegexUnaryExpression, IEquatable<RegexRepeatExpression>
	{
		/// <summary>
		/// Indicates whether or not this statement is a single element or not
		/// </summary>
		/// <remarks>If false, this statement will be wrapped in parentheses if necessary</remarks>
		public override bool IsSingleElement => true;
		/// <summary>
		/// Creates a repeat expression with the specifed target expression, and minimum and maximum occurances
		/// </summary>
		/// <param name="expression">The target expression</param>
		/// <param name="minOccurs">The minimum number of times the target expression can occur or -1</param>
		/// <param name="maxOccurs">The maximum number of times the target expression can occur or -1</param>
		public RegexRepeatExpression(RegexExpression expression, int minOccurs = -1, int maxOccurs = -1)
		{
			Expression = expression;
			MinOccurs = minOccurs;
			MaxOccurs = maxOccurs;
		}
		/// <summary>
		/// Creates a default instance of the expression
		/// </summary>
		public RegexRepeatExpression() { }
		/// <summary>
		/// Indicates the minimum number of times the target expression can occur, or 0 or -1 for no minimum
		/// </summary>
		public int MinOccurs { get; set; } = -1;
		/// <summary>
		/// Indicates the maximum number of times the target expression can occur, or 0 or -1 for no maximum
		/// </summary>
		public int MaxOccurs { get; set; } = -1; // kleene by default
		/// <summary>
		/// Creates a state machine representing this expression
		/// </summary>
		/// <param name="accept">The accept symbol to use for this expression</param>
		/// <returns>A new <see cref="FA"/> finite state machine representing this expression</returns>		
		public override FA ToFA(int accept = 0, bool compact = true)
			=> null != Expression ? FA.Repeat(Expression.ToFA(accept,compact), MinOccurs, MaxOccurs, accept,compact) : null;
		/// <summary>
		/// Appends the textual representation to a <see cref="StringBuilder"/>
		/// </summary>
		/// <param name="sb">The string builder to use</param>
		/// <remarks>Used by ToString()</remarks>
		protected internal override void AppendTo(StringBuilder sb)
		{
			var ise = null != Expression && Expression.IsSingleElement;
			if (!ise)
				sb.Append('(');
			if (null != Expression)
				Expression.AppendTo(sb);
			if (!ise)
				sb.Append(')');

			switch (MinOccurs)
			{
				case -1:
				case 0:
					switch (MaxOccurs)
					{
						case -1:
						case 0:
							sb.Append('*');
							break;
						default:
							sb.Append('{');
							if (-1 != MinOccurs)
								sb.Append(MinOccurs);
							sb.Append(',');
							sb.Append(MaxOccurs);
							sb.Append('}');
							break;
					}
					break;
				case 1:
					switch (MaxOccurs)
					{
						case -1:
						case 0:
							sb.Append('+');
							break;
						default:
							sb.Append("{1,");
							sb.Append(MaxOccurs);
							sb.Append('}');
							break;
					}
					break;
				default:
					sb.Append('{');
					if (-1 != MinOccurs)
						sb.Append(MinOccurs);
					sb.Append(',');
					if (-1 != MaxOccurs)
						sb.Append(MaxOccurs);
					sb.Append('}');
					break;
			}
		}
		/// <summary>
		/// Creates a new copy of this expression
		/// </summary>
		/// <returns>A new copy of this expression</returns>
		protected override RegexExpression CloneImpl()
			=> Clone();
		/// <summary>
		/// Creates a new copy of this expression
		/// </summary>
		/// <returns>A new copy of this expression</returns>
		public RegexRepeatExpression Clone()
		{
			return new RegexRepeatExpression(Expression, MinOccurs, MaxOccurs);
		}
		#region Value semantics
		/// <summary>
		/// Indicates whether this expression is the same as the right hand expression
		/// </summary>
		/// <param name="rhs">The expression to compare</param>
		/// <returns>True if the expressions are the same, otherwise false</returns>
		public bool Equals(RegexRepeatExpression rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			if (Equals(Expression, rhs.Expression))
			{
				var lmio = Math.Max(0, MinOccurs);
				var lmao = Math.Max(0, MaxOccurs);
				var rmio = Math.Max(0, rhs.MinOccurs);
				var rmao = Math.Max(0, rhs.MaxOccurs);
				return lmio == rmio && lmao == rmao;
			}
			return false;
		}
		/// <summary>
		/// Indicates whether this expression is the same as the right hand expression
		/// </summary>
		/// <param name="rhs">The expression to compare</param>
		/// <returns>True if the expressions are the same, otherwise false</returns>
		public override bool Equals(object rhs)
			=> Equals(rhs as RegexRepeatExpression);
		/// <summary>
		/// Computes a hash code for this expression
		/// </summary>
		/// <returns>A hash code for this expression</returns>
		public override int GetHashCode()
		{
			var result = MinOccurs ^ MaxOccurs;
			if (null != Expression)
				return result ^ Expression.GetHashCode();
			return result;
		}
		/// <summary>
		/// Indicates whether or not two expression are the same
		/// </summary>
		/// <param name="lhs">The left hand expression to compare</param>
		/// <param name="rhs">The right hand expression to compare</param>
		/// <returns>True if the expressions are the same, otherwise false</returns>
		public static bool operator ==(RegexRepeatExpression lhs, RegexRepeatExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		/// <summary>
		/// Indicates whether or not two expression are different
		/// </summary>
		/// <param name="lhs">The left hand expression to compare</param>
		/// <param name="rhs">The right hand expression to compare</param>
		/// <returns>True if the expressions are different, otherwise false</returns>
		public static bool operator !=(RegexRepeatExpression lhs, RegexRepeatExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion
	}
}
