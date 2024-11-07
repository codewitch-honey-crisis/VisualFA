using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace VisualFA
{
	/// <summary>
	/// Indicates an action to take when a node is visited
	/// </summary>
	/// <param name="parent">The parent node</param>
	/// <param name="expression">The current expression</param>
	/// <param name="childIndex">The index of the expression within the parent</param>
	/// <param name="level">The nexting level</param>
	/// <returns></returns>
#if FALIB
	public
#endif
	delegate bool RegexVisitAction(RegexExpression parent, RegexExpression expression, int childIndex, int level);
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
		/// Indicates whether or not this statement is a empty element or not
		/// </summary>
		public abstract bool IsEmptyElement { get; }
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
		private bool _Visit(RegexExpression parent, RegexVisitAction action, int childIndex, int level) {
			if (action(parent, this, childIndex, level))
			{
				var unary = this as RegexUnaryExpression;
				if (unary != null && unary.Expression != null)
				{
					return unary.Expression._Visit(this, action, 0, level + 1);
				}
				var multi = this as RegexMultiExpression;
				if (multi != null)
				{
					for (int i = 0; i < multi.Expressions.Count; ++i)
					{
						var e = multi.Expressions[i];
						if (e != null)
						{
							e._Visit(this, action, i, level + 1);
						}
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
			_Visit(null, action, 0, 0);
		}
		/// <summary>
		/// Should skip the reduction of this expression
		/// </summary>
		internal bool SkipReduce { get; set; }
		/// <summary>
		/// Attempts to reduce the expression to a simpler form
		/// </summary>
		/// <param name="reduced">The reduced expression</param>
		/// <returns>True if a reduction occurred, otherwise false</returns>
		public abstract bool TryReduce(out RegexExpression reduced);
		/// <summary>
		/// Reduces an expression to a simpler form, if possible
		/// </summary>
		/// <param name="maxPasses">The maximum number of reduction passes to perform or -1 for no limit</param>
		/// <returns>The new reduced expression</returns>
		public RegexExpression Reduce(int maxPasses = -1)
		{
			RegexExpression result = this;
			while (result!=null && (maxPasses==-1 || (maxPasses--)>0) && !result.SkipReduce && result.TryReduce(out result)) ;
			return result;
		}
		
		/// <summary>
		/// Creates a copy of the expression
		/// </summary>
		/// <returns>A copy of the expression</returns>
		protected abstract RegexExpression CloneImpl();
		object ICloneable.Clone() => CloneImpl();
		/// <summary>
		/// Clones the expression
		/// </summary>
		/// <returns>A deep copy of the expression</returns>
		public RegexExpression Clone()
		{
			return CloneImpl();
		}
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
							result = new RegexConcatExpression(new RegexExpression[] { result, nset });
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
							result = new RegexConcatExpression(new RegexExpression[] { result, next });
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
							result = new RegexConcatExpression(new RegexExpression[] { result, next });
							result.SetLocation(position);
						}
						position = pc.Position;
						break;
					case '|':
						if (-1 != pc.Advance())
						{
							next = _Parse(pc);
							result = new RegexOrExpression(new RegexExpression[] { result, next });
							result.SetLocation(position);
						}
						else
						{
							result = new RegexOrExpression(new RegexExpression[] { result, null });
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
							result = new RegexConcatExpression(new RegexExpression[] { result, next });
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
								result = new RegexConcatExpression(new RegexExpression[] { result, next });
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
					expr = new RegexRepeatExpression(expr,0,1);
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
		/// <param name="character">The character to escape</param>
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
		/// <param name="character">The character to escape</param>
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
		/// <param name="character">The character to escape</param>
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
		private struct _ExpEdge
		{
			public RegexExpression Exp;
			public FA From;
			public FA To;
		}
		static void _ToExpressionFillEdgesIn(IList<_ExpEdge> edges, FA node, IList<_ExpEdge> result)
		{
			for (int i = 0; i < edges.Count; ++i)
			{
				if (edges[i].To == node)
				{
					result.Add(edges[i]);
				}
			}
		}
		static void _ToExpressionFillEdgesOut(IList<_ExpEdge> edges, FA node, IList<_ExpEdge> result)
		{
			for (int i = 0; i < edges.Count; ++i)
			{
				var edge = edges[i];
				if (edge.From == node)
				{
					result.Add(edge);
				}
			}
		}
		static RegexExpression _ToExpressionOrJoin(IList<RegexExpression> exps)
		{
			if (exps.Count == 0) return null;
			if (exps.Count == 1) return exps[0];
			return new RegexOrExpression(exps);
		}
		/// <summary>
		/// Parses an expression from a state machine
		/// </summary>
		/// <param name="fa">The state machine to parse</param>
		/// <returns>An expression that is equivalent to the state machine</returns>
		public static RegexExpression FromFA(FA fa)
		{
			if(fa==null)
			{
				return null;
			}
			List<RegexExpression> tocat = new List<RegexExpression>();
			List<FA> closure = new List<FA>();
			List<_ExpEdge> fsmEdges = new List<_ExpEdge>();
			FA first, final = null;

			first = fa;
			var acc = first.FillFind(FA.AcceptingFilter);
			if (acc.Count == 1)
			{
				final = acc[0];
			}
			else if (acc.Count > 1)
			{
				fa = fa.Clone();
				first = fa;
				acc = fa.FillFind(FA.AcceptingFilter);
				final = new FA(acc[0].AcceptSymbol);
				for (int i = 0; i < acc.Count; ++i)
				{
					var a = acc[i];
					a.AddEpsilon(final, false);
					a.AcceptSymbol = -1;
				}
			}
			closure.Clear();
			first.FillClosure(closure);
			// build the machine from the FA
			var trnsgrp = new Dictionary<FA, IList<FARange>>(closure.Count);
			for (int q = 0; q < closure.Count; ++q)
			{
				var cfa = closure[q];
				trnsgrp.Clear();
				cfa.FillInputTransitionRangesGroupedByState(true, trnsgrp);
				foreach (var trns in trnsgrp)
				{
					RegexExpression rx;
					if (trns.Value.Count == 1 && trns.Value[0].Min == trns.Value[0].Max)
					{
						var range = trns.Value[0];
						if (range.Min == -1 && range.Max == -1)
						{
							var eedge = new _ExpEdge();
							eedge.Exp = null;
							eedge.From = cfa;
							eedge.To = trns.Key;
							fsmEdges.Add(eedge);
							continue;
						}
						var rxl = new RegexLiteralExpression(new int[] { range.Min });
						rx = rxl;
					}
					else
					{
						var rxcs = new RegexCharsetExpression();
						rx = rxcs;
						for (int rr = 0; rr < trns.Value.Count; ++rr)
						{
							var range = trns.Value[rr];
							if (range.Min != range.Max)
							{
								rxcs.Entries.Add(new RegexCharsetRangeEntry(range.Min, range.Max));
							} else
							{
								if (range.Min == -1 && range.Max == -1)
								{
									var eedge = new _ExpEdge();
									eedge.Exp = null;
									eedge.From = cfa;
									eedge.To = trns.Key;
									fsmEdges.Add(eedge);
									continue;
								} else
								{
									if(range.Min==range.Max)
									{
										rxcs.Entries.Add(new RegexCharsetCharEntry(range.Min));
									} else
									{
										rxcs.Entries.Add(new RegexCharsetRangeEntry(range.Min, range.Max));
									}
									
								}
							}
						}
					}
					var edge = new _ExpEdge();
					edge.Exp = rx;
					edge.From = cfa;
					edge.To = trns.Key;
					fsmEdges.Add(edge);
				}
			}
			var tmp = new FA();
			tmp.AddEpsilon(first, false);
			var q0 = first;
			first = tmp;
			tmp = new FA(final.AcceptSymbol);
			var qLast = final;
			final.AcceptSymbol = -1;
			final.AddEpsilon(tmp, false);
			final = tmp;
			// add first and final
			var newEdge = new _ExpEdge();
			newEdge.Exp = null;
			newEdge.From = first;
			newEdge.To = q0;
			fsmEdges.Add(newEdge);
			newEdge = new _ExpEdge();
			newEdge.Exp = null;
			newEdge.From = qLast;
			newEdge.To = final;
			fsmEdges.Add(newEdge);
			closure.Insert(0, first);
			closure.Add(final);
			var inEdges = new List<_ExpEdge>();
			var outEdges = new List<_ExpEdge>();
			while (closure.Count > 2)
			{
				for (int q = 1; q < closure.Count - 1; ++q)
				{
					var node = closure[q];
					var loops = new List<RegexExpression>();
					inEdges.Clear();
					_ToExpressionFillEdgesIn(fsmEdges, node, inEdges);
					for (int i = 0; i < inEdges.Count; ++i)
					{
						var edge = inEdges[i];
						if (edge.From == edge.To)
						{
							loops.Add(new RegexRepeatExpression(edge.Exp,0,0));
						}
					}
					RegexExpression middleExp = _ToExpressionOrJoin(loops);
					for (int i = 0; i < inEdges.Count; ++i)
					{
						var inEdge = inEdges[i];
						if (inEdge.From == inEdge.To)
						{
							continue;
						}
						outEdges.Clear();
						_ToExpressionFillEdgesOut(fsmEdges, node, outEdges);
						for (int j = 0; j < outEdges.Count; ++j)
						{
							var outEdge = outEdges[j];
							if (outEdge.From == outEdge.To)
							{
								continue;
							}
							var expEdge = new _ExpEdge();
							expEdge.From = inEdge.From;
							expEdge.To = outEdge.To;
							tocat.Clear();
							var cat = inEdge.Exp as RegexConcatExpression;
							if (cat != null)
							{
								tocat.AddRange(cat.Expressions);
							} else
							{
								if (inEdge.Exp != null)
									tocat.Add(inEdge.Exp);
							}
							cat = middleExp as RegexConcatExpression;
							if(cat!=null)
							{
								tocat.AddRange(cat.Expressions);
							} else
							{
								if(middleExp!=null)
									tocat.Add(middleExp);
							}
							cat = outEdge.Exp as RegexConcatExpression;
							if (cat != null)
							{
								tocat.AddRange(cat.Expressions);
							}
							else
							{
								if (outEdge.Exp!= null)
									tocat.Add(outEdge.Exp);
							}
							for(int k = 1;k<tocat.Count;++k)
							{
								var lit1 = tocat[k - 1] as RegexLiteralExpression;
								if(lit1 != null)
								{
									var lit2 = tocat[k] as RegexLiteralExpression;
									if(lit2 != null)
									{
										lit1.Value += lit2.Value;
										tocat.RemoveAt(k);
										--k;
									}
								}
							}
							if (tocat.Count > 1)
							{
								expEdge.Exp = new RegexConcatExpression(tocat);
							} else if(tocat.Count > 0)
							{
								expEdge.Exp = tocat[0];
							} else
							{
								expEdge.Exp = null;
							}
							fsmEdges.Add(expEdge);
						}
					}
					// reuse inedges since we're not using it
					inEdges.Clear();
					_ToExpressionFillEdgesOrphanState(fsmEdges, node, inEdges);
					fsmEdges.Clear();
					fsmEdges.AddRange(inEdges);
					closure.Remove(node);
				}
			}
			var result = new List<RegexExpression>(fsmEdges.Count);
			for (int i = 0; i < fsmEdges.Count; ++i)
			{
				var edge = fsmEdges[i];
				result.Add(edge.Exp);
			}

			return _ToExpressionOrJoin(result);

		}
		static void _ToExpressionFillEdgesOrphanState(IList<_ExpEdge> edges, FA node, IList<_ExpEdge> result)
		{
			for (int i = 0; i < edges.Count; ++i)
			{
				var edge = edges[i];
				if (edge.From == node || edge.To == node)
				{
					continue;
				}
				result.Add(edge);
			}
		}
	}
	/// <summary>
	/// Represents a binary expression
	/// </summary>
#if FALIB
	public
#endif
	abstract partial class RegexMultiExpression : RegexExpression
	{
		/// <summary>
		/// Indicates the expressions
		/// </summary>
		public List<RegexExpression> Expressions { get; } = new List<RegexExpression>();
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
		public RegexExpression Expression { get; set; } = null;

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
		public override bool IsSingleElement => Codepoints != null && Codepoints.Length < 2;
		/// <summary>
		/// Indicates whether or not this statement is a empty element or not
		/// </summary>
		public override bool IsEmptyElement => Codepoints == null || Codepoints.Length == 0;
		/// <summary>
		/// Indicates the codepoints in this expression
		/// </summary>
		public int[] Codepoints { get; set; } = null;
		/// <summary>
		/// Indicates the string literal of this expression
		/// </summary>
		public string Value {
			get {
				if (Codepoints == null)
				{
					return null;
				}
				if (Codepoints.Length == 0)
				{
					return "";
				}
				var sb = new StringBuilder();
				for (int i = 0; i < Codepoints.Length; i++)
				{
					sb.Append(char.ConvertFromUtf32(Codepoints[i]));
				}
				return sb.ToString();
			}
			set {
				if (value == null) throw new NullReferenceException();
				if (value.Length == 0) throw new InvalidOperationException();
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
		#region Main_Lorentz algo
		static List<int> _MainLorentzZFunction(string s)
		{
			int n = s.Length;
			List<int> zvector = new List<int>(n);
			for (int i = 0; i < n; ++i)
			{
				zvector.Add(0);
			}
			for (int j = 1, l = 0, r = 0; j < n; j++)
			{
				if (j <= r)
					zvector[j] = Math.Min(r - j + 1, zvector[j - l]);
				while (j + zvector[j] < n && s[zvector[j]] == s[j + zvector[j]])
					zvector[j]++;
				if (j + zvector[j] - 1 > r)
				{
					l = j;
					r = j + zvector[j] - 1;
				}
			}
			return zvector;
		}
		struct _MainLorentzRep
		{
			public int Start;
			public int End;
			public int Len;
			public _MainLorentzRep(int start, int end, int len)
			{
				Start = start;
				End = end;
				Len = len;
			}
		}
		static void _MainLorentzGetRepetitions(IList<_MainLorentzRep> repetitions, string s, int shift, bool left, int cntr, int l, int k1, int k2)
		{
			for (int l1 = Math.Max(1, l - k2); l1 <= Math.Min(l, k1); ++l1)
			{
				if (left && l1 == l)
					break;
				int l2 = l - l1;
				int pos = shift + (left ? cntr - l1 : cntr - l - l1 + 1);
				repetitions.Add(new _MainLorentzRep(pos, pos + 2 * l - 1, l));
			}
		}
		static string _MainLorentzReverse(string input)
		{
			// allocate a buffer to hold the output
			char[] output = new char[input.Length];
			for (int outputIndex = 0, inputIndex = input.Length - 1; outputIndex < input.Length; outputIndex++, inputIndex--)
			{
				// check for surrogate pair
				if (input[inputIndex] >= 0xDC00 && input[inputIndex] <= 0xDFFF &&
					inputIndex > 0 && input[inputIndex - 1] >= 0xD800 && input[inputIndex - 1] <= 0xDBFF)
				{
					// preserve the order of the surrogate pair code units
					output[outputIndex + 1] = input[inputIndex];
					output[outputIndex] = input[inputIndex - 1];
					outputIndex++;
					inputIndex--;
				}
				else
				{
					output[outputIndex] = input[inputIndex];
				}
			}

			return new string(output);
		}
		static void _MainLorentzRepetitions(IList<_MainLorentzRep> repetitions, string s, int shift = 0)
		{ 
			int n = s.Length;
			if (n == 1)
				return;
			/* Calculating size of each half */
			int nu = n / 2;
			int nv = n - nu;

			/* Defining half strings and their reverse*/
			string u = s.Substring(0, nu);
			string v = s.Substring(nu);
			string ru = _MainLorentzReverse(u);
			string rv = _MainLorentzReverse(v);

			/* Implementing recursion for both halves */
			_MainLorentzRepetitions(repetitions, u, shift);
			_MainLorentzRepetitions(repetitions, v, shift + nu);

			/* Calling zFunction */
			List<int> z1 = _MainLorentzZFunction(ru);
			List<int> z2 = _MainLorentzZFunction(v + '#' + u);
			List<int> z3 = _MainLorentzZFunction(ru + '#' + rv);
			List<int> z4 = _MainLorentzZFunction(v);

			/* Calculating the number of repetitions through fixed center and
				length l, k1, and k2*/
			for (int cntr = 0; cntr < n; cntr++)
			{
				int l, k1, k2;
				if (cntr < nu)
				{
					l = nu - cntr;
					k1 = _MainLorentzGetZ(z1, nu - cntr);
					k2 = _MainLorentzGetZ(z2, nv + 1 + cntr);
				}
				else
				{
					l = cntr - nu + 1;
					k1 = _MainLorentzGetZ(z3, nu + 1 + nv - 1 - (cntr - nu));
					k2 = _MainLorentzGetZ(z4, (cntr - nu) + 1);
				}
				if (k1 + k2 >= l)
					_MainLorentzGetRepetitions(repetitions, s, shift, cntr < nu, cntr, l, k1, k2);
			}
		}

		static bool _MainLorentz(string input, ref RegexExpression result)
		{
			if (input == null)
			{
				result = null;
				return false;
			}
			if (input.Length < 2)
			{
				if (result != null)
				{
					result.SkipReduce = true;
				}
				return false;
			}
			var reps = new List<_MainLorentzRep>();
			_MainLorentzRepetitions(reps, input);
			reps.Sort((lhs, rhs) => {
				int ldist = lhs.End - lhs.Start;
				int rdist = rhs.End - rhs.Start;
				if (rdist == ldist)
				{
					return lhs.Start - rhs.Start;
				}
				if (rdist == ldist)
				{
					return rhs.Len - lhs.Len;
				}

				return rdist - ldist;
			});
			if (reps.Count > 0)
			{
				var rep = reps[0];
				if (rep.End - rep.Start + 1 < 3)
				{
					if (result != null)
					{
						result.SkipReduce = true;
					}
					return false;
				}
				string ss = input.Substring(rep.Start, rep.End - rep.Start + 1);
				//var part = _MainLorentzGetRepeatedPart(ss);
				var part = ss.Substring(0, rep.Len);
				// Main-Lorentz won't catch foofoofoof because it subdivides
				// here we catch the extra foo
				while (ss.Length + rep.Start < input.Length - part.Length)
				{
					var css = input.Substring(rep.Start + ss.Length, part.Length);
					if(css==part)
					{
						ss += css;
					}
				}
				var repCount = ss.Length / rep.Len;

				//System.Diagnostics.Debug.Assert(repCount > 1);
				var exps = new List<RegexExpression>(3);
				RegexLiteralExpression lit;
				if (rep.Start > 0)
				{
					lit = new RegexLiteralExpression(input.Substring(0, rep.Start));
					lit.SkipReduce = true;
					exps.Add(lit);
				}
				lit = new RegexLiteralExpression(part);
				lit.SkipReduce = true;
				exps.Add(new RegexRepeatExpression(lit, repCount, repCount));
				if ((ss.Length + rep.Start) < input.Length)
				{
					lit = new RegexLiteralExpression(input.Substring(ss.Length + rep.Start));
					lit.SkipReduce = true;
					exps.Add(lit);
				}
				if (exps.Count > 1)
				{
					result = new RegexConcatExpression(exps);
					return true;
				}
				else
				{
					result = exps[0];
					return true;
				}
			}
			return false;
		}
		private static int _MainLorentzGetZ(IList<int> z, int i)
		{
			if (0 < i && i < z.Count)
			{
				return z[i];
			}
			return 0;
		}
		#endregion
		/// <summary>
		/// Attempts to reduce the expression
		/// </summary>
		/// <param name="reduced">The reduced expression</param>
		/// <returns>True if reduced, otherwise false</returns>
		public override bool TryReduce(out RegexExpression reduced)
		{
			reduced = this;
			if (SkipReduce || Codepoints==null || Codepoints.Length<3)
			{
				return false;
			}
			return _MainLorentz(Value, ref reduced);
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
		/// <param name="compact">True to create a compact NFA, false to create an expanded NFA</param>
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
		public new RegexLiteralExpression Clone()
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
		/// Indicates whether or not this statement is a empty element or not
		/// </summary>
		public override bool IsEmptyElement => Entries.Count == 0;
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
		/// <param name="compact">True to create a compact NFA, false to create an expanded NFA</param>
		/// <returns>A new <see cref="FA"/> finite state machine representing this expression</returns>
		public override FA ToFA(int accept = 0, bool compact = true)
		{
			var ranges = GetRanges();
			return FA.Set(ranges, accept,compact);
		}
		/// <summary>
		/// Retrieve the codepoint ranges for the character set
		/// </summary>
		/// <returns></returns>
		public IList<FARange> GetRanges()
		{
			var result = new List<FARange>();
			for (int ic = Entries.Count, i = 0; i < ic; ++i)
			{
				var entry = Entries[i];
				var crc = entry as RegexCharsetCharEntry;
				if (null != crc)
					result.Add(new FARange(crc.Codepoint, crc.Codepoint));
				var crr = entry as RegexCharsetRangeEntry;
				if (null != crr)
					result.Add(new FARange(crr.FirstCodepoint, crr.LastCodepoint));
				var crcl = entry as RegexCharsetClassEntry;
				if (null != crcl)
				{
					var known = FA.CharacterClasses.Known[crcl.Name];
					for (int j = 0; j < known.Length; j += 2)
					{
						result.Add(new FARange(known[j], known[j + 1]));
					}
				}
			}
			if(HasNegatedRanges)
			{
				return new List<FARange>(FARange.ToNotRanges(result));
			}
			return result;
		}
		/// <summary>
		/// Attempts to reduce the expression
		/// </summary>
		/// <param name="reduced">The reduced expression</param>
		/// <returns>True if reduced, otherwise false</returns>
		public override bool TryReduce(out RegexExpression reduced)
		{
			if(SkipReduce)
			{
				reduced = this;
				return false;
			}
			if(Entries.Count == 0)
			{
				reduced = null;
				return true;
			}
			var c = Entries.Count;
			var rngs = GetRanges();
			if (rngs.Count == 1)
			{
				if (rngs[0].Min == rngs[0].Max)
				{
					reduced = new RegexLiteralExpression(new int[] { rngs[0].Min });
					return true;
				}
			}
			if (c <= rngs.Count)
			{
				reduced = this;
				return false;
			}
			
			var sx = new RegexCharsetExpression();
			for (var i = 0; i < rngs.Count; ++i)
			{
				var rng = rngs[i];
				var r = new RegexCharsetRangeEntry();
				r.FirstCodepoint = rng.Min;
				r.LastCodepoint = rng.Max;
				sx.Entries.Add(r);
				
			}
			reduced = sx;
			return true;
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
		public new RegexCharsetExpression Clone()
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
				{
					if (!Entries[i].Equals(rhs.Entries[i]))
					{
						return false;
					}
				}
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
	partial class RegexConcatExpression : RegexMultiExpression, IEquatable<RegexConcatExpression>
	{
		/// <summary>
		/// Indicates whether or not this statement is a single element or not
		/// </summary>
		/// <remarks>If false, this statement will be wrapped in parentheses if necessary</remarks>
		public override bool IsSingleElement {
			get {
				return Expressions.Count == 1 && Expressions[0] != null && Expressions[0].IsSingleElement;
			}
		}
		/// <summary>
		/// Indicates whether or not this statement is a empty element or not
		/// </summary>
		public override bool IsEmptyElement => Expressions.Count == 0 || (Expressions.Count == 1 && Expressions[0].IsEmptyElement);
		/// <summary>
		/// Creates a new expression with the specified left and right hand sides
		/// </summary>
		/// <param name="expressions">The right expressions</param>
		public RegexConcatExpression(IList<RegexExpression> expressions) 
		{
			for (int i = 0; i < expressions.Count; ++i)
			{
				var e = expressions[i];
				if (e != null && !e.IsEmptyElement)
				{
					Expressions.Add(e);
				}
			}
		}
		/// <summary>
		/// Creates a default instance of the expression
		/// </summary>
		public RegexConcatExpression() { }
		private bool _AddReduced(RegexExpression e)
		{
			if (e == null) return true;
			var r = false;
			var oe = e;
			while (e != null && e.TryReduce(out oe)) { r = true; e = oe;  }
			if (e != null)
			{
				var c = e as RegexConcatExpression;
				if (null != c)
				{
					for (var i = 0; i < c.Expressions.Count; ++i)
					{
						var ce = c.Expressions[i];
						if (ce != null)
						{
							while (ce != null && ce.TryReduce(out oe)) { r = true; ce = oe; }
							if (ce != null)
							{
								Expressions.Add(ce);
							}
						}
					}
					return true;
				}
				Expressions.Add(e);
			} else
			{
				if(!Expressions.Contains(null))
				{
					Expressions.Add(null);
				}
			}
			return r;
		}
		/// <summary>
		/// Attempts to reduce the expression
		/// </summary>
		/// <param name="reduced">The reduced expression</param>
		/// <returns>True if reduced, otherwise false</returns>
		public override bool TryReduce(out RegexExpression reduced)
		{
			if(SkipReduce)
			{
				reduced = this;
				return false;
			}
			var result = false;
			var cat = new RegexConcatExpression();

			for (var i = 0; i < Expressions.Count; ++i)
			{
				var e = Expressions[i];
				if (e == null )
				{
					result = true;
					continue;
				}
				if (cat._AddReduced(e))
				{
					result = true;
				}
			}
			
			switch (cat.Expressions.Count)
			{
				case 0:
					reduced = null;
					return true;
				case 1:
					if (cat.Expressions[0] != null)
					{
						reduced = cat.Expressions[0];
					} else
					{
						reduced = null;
					}
					return true;
				default:
					// fixup things like zz* so it's z+
					for (var i = 1; i < cat.Expressions.Count; ++i)
					{
						var e = cat.Expressions[i];
						
						var rep = e as RegexRepeatExpression;
						
						if (rep != null)
						{
							var ee = rep.Expression;
							var cc = ee as RegexConcatExpression;
							if (cc != null && i>=cc.Expressions.Count)
							{
								var k = 0;
								for (var j = i - cc.Expressions.Count; j < i; ++j)
								{
									if (!cc.Expressions[k].Equals(cat.Expressions[j]))
									{
										reduced = result ? cat : this;
										return result;
									}
									++k;
								}
								cat.Expressions[i] = new RegexRepeatExpression(cc, rep.MinOccurs + 1, rep.MaxOccurs > 0 ? rep.MaxOccurs + 1 : 0);
								cat.Expressions.RemoveRange(i - cc.Expressions.Count, cc.Expressions.Count);
								result = true;
							}
							else
							{
								if (cat.Expressions[i - 1].Equals(ee))
								{
									cat.Expressions[i] = new RegexRepeatExpression(ee, rep.MinOccurs + 1, rep.MaxOccurs > 0 ? rep.MaxOccurs + 1 : 0);
									cat.Expressions.RemoveAt(i - 1);
									result = true;
								}
							}
						}
						var lit = e as RegexLiteralExpression;
						if(lit!=null)
						{
							var litp = cat.Expressions[i - 1] as RegexLiteralExpression;
							if(litp!=null)
							{
								cat.Expressions[i] = new RegexLiteralExpression(litp.Value + lit.Value);
								cat.Expressions.RemoveAt(i - 1);
								result = true;
							}
						}
					}
					
					reduced = result ? cat : this;
					return result;
			}
		}
		/// <summary>
		/// Creates a state machine representing this expression
		/// </summary>
		/// <param name="accept">The accept symbol to use for this expression</param>
		/// <param name="compact">True to create a compact NFA, false to create an expanded NFA</param>
		/// <returns>A new <see cref="FA"/> finite state machine representing this expression</returns>
		public override FA ToFA(int accept = 0, bool compact = true)
		{
			FA current = null;
			for(int i = 0;i<Expressions.Count;++i)
			{
				var e = Expressions[i];
				var fa = e.ToFA(accept, compact);
				if(current==null)
				{
					current = fa;
				} else
				{
					var newFA = new FA(accept);
					var acc = fa.FillFind(FA.AcceptingFilter);
					for(int j = 0;j<acc.Count;++j)
					{
						var afa = acc[j];
						afa.AcceptSymbol = -1;
						afa.AddEpsilon(newFA, compact);
					}
					current = newFA;
				}
			}
			if(Expressions.Count==0)
			{
				return new FA(accept);
			}
			return current;
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
		public new RegexConcatExpression Clone()
		{
			var result = new RegexConcatExpression();
			for(int i = 0;i<Expressions.Count;++i)
			{
				var e = Expressions[i];
				result.Expressions.Add(e.Clone());
			}
			return result;
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
			int i = 0, j = 0;
			while(i<Expressions.Count && j<rhs.Expressions.Count) {
				var l = Expressions[i];
				var r = rhs.Expressions[j];
				if(l==null)
				{
					++i;
					continue;
				}
				if(r==null)
				{
					++j;
					continue;
				}
				if(!l.Equals(r))
				{
					return false;
				}
				++i;
				++j;
			}
			return (i == j);
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
			for (int i = 0; i < Expressions.Count; ++i)
			{
				var e = Expressions[i];
				if (e != null)
				{
					result ^= e.GetHashCode();
				}
			}
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
			for(int i = 0; i<Expressions.Count;++i)
			{
				var e = Expressions[i];
				if(e!=null)
				{
					var oe = e as RegexOrExpression;
					if (null != oe)
						sb.Append('(');
					e.AppendTo(sb);
					if (null != oe)
						sb.Append(')');
				}
			}
		}
	}
	/// <summary>
	/// Represents an "or" regular expression as indicated by |
	/// </summary>
#if FALIB
	public
#endif
	partial class RegexOrExpression : RegexMultiExpression, IEquatable<RegexOrExpression>
	{
		/// <summary>
		/// Indicates whether or not this statement is a single element or not
		/// </summary>
		/// <remarks>If false, this statement will be wrapped in parentheses if necessary</remarks>
		public override bool IsSingleElement => Expressions.Count==1 && Expressions[0]!=null && Expressions[0].IsSingleElement;
		/// <summary>
		/// Indicates whether or not this statement is a empty element or not
		/// </summary>
		public override bool IsEmptyElement => Expressions.Count==0 || (Expressions.Count==1 && Expressions[0].IsEmptyElement);
		/// <summary>
		/// Creates a new instance from a list of expressions
		/// </summary>
		/// <param name="expressions">The expressions</param>
		/// <exception cref="ArgumentNullException"><paramref name="expressions"/> was null</exception>
		/// <exception cref="ArgumentException"><paramref name="expressions"/> was empty</exception>
		public RegexOrExpression(IList<RegexExpression> expressions)
		{
			if (expressions == null) throw new ArgumentNullException(nameof(expressions));
			Expressions.AddRange(expressions);
		}
		private bool _AddReduced(RegexExpression e, ref bool hasnull)
		{
			if (e == null) return hasnull;
			var r = false;
			while (e != null && e.TryReduce(out e)) { r = true; }
			if (e == null) return true;
			var o = e as RegexOrExpression;
			if (null != o)
			{
				for (var i = 0; i < o.Expressions.Count; ++i)
				{
					var oe = o.Expressions[i];
					if (oe != null)
					{
						_AddReduced(oe, ref hasnull);
					}
					else
						hasnull = true;
				}
				return true;
			}
			Expressions.Add(e);
			return r;
		}
		static RegexExpression _CatIfNeeded(IList<RegexExpression> exprs)
		{
			if(exprs==null) return null;
			if(exprs.Count==0) return null;
			if (exprs.Count == 1) return exprs[0];
			return new RegexConcatExpression(exprs);
		}
		static int _IndexOfSubrange(List<RegexExpression> exprs, IList<RegexExpression> sub)
		{
			if (sub == null || exprs==null) return -1;
			if (sub.Count > exprs.Count) return -1;
			
			for(int i = 0;i<exprs.Count-sub.Count+1;++i)
			{
				var range = exprs.GetRange(i, sub.Count);
				var found = true;
				for(var j = 0;j<range.Count;++j) {
					if (!range[j].Equals(sub[j]))
					{
						found = false;
						break;
					}
				}
				if (found)
				{
					return i;
				}
			}
			return -1;
		}
		static bool _HuntDups(ref RegexExpression lhs, ref RegexExpression rhs)
		{
			if (lhs == null || rhs == null) return false;
			if (object.ReferenceEquals(lhs, rhs)) return false;
			var lexps = new List<RegexExpression>();
			var rexps = new List<RegexExpression>();
			var cat = rhs as RegexConcatExpression;
			if(cat == null)
			{
				return false;
			}
			rexps.AddRange(cat.Expressions);
			cat = lhs as RegexConcatExpression;
			if(cat!=null)
			{
				lexps.AddRange(cat.Expressions);
			} else
			{
				lexps.Add(lhs);
			}
			if(lexps.Count > rexps.Count)
			{
				return false;
			}
			int rfi = -1;
			int rfl = -1;
			for(var j = lexps.Count;j>0;--j)
			{
				rfi = _IndexOfSubrange(rexps, lexps.GetRange(0, j));
				if (rfi > -1)
				{
					rfl = j;
					break;
				}
			}
			if (rfi!=-1)
			{
				if (rfi == 0)
				{
					// matched at the beginning foo|foo(bar)+
					// will be foo((bar)+)? and foo(bar)* after reduction
					lexps.Add(new RegexRepeatExpression(_CatIfNeeded(rexps.GetRange(lexps.Count, rexps.Count - lexps.Count)), 0, 1));
					lhs = _CatIfNeeded(lexps);
					rhs = null;
					return true;
				} else if (rfi+lexps.Count<rexps.Count)
				{
					// matched in the middle foo|(buzz)+foo(bar)*
					// will be ((buzz)+)?foo((bar)*)? or (buzz)*foo(bar)*
					// after reduction
					int lc = lexps.Count;
					lexps.Clear();
					lexps.Add(new RegexRepeatExpression(_CatIfNeeded(rexps.GetRange(0, rfi)), 0, 1));
					lexps.Add(_CatIfNeeded(rexps.GetRange(rfi,lc)));
					lexps.Add(new RegexRepeatExpression(_CatIfNeeded(rexps.GetRange(rfi+lc, rexps.Count-(lc +1))), 0, 1));
					lhs = _CatIfNeeded(lexps);
					rhs = null;
					return true;
				} else 
				{
					// matched at end foo|(bar)+foo
					// will be ((bar)+)?foo or
					// (bar)*foo after reduction
					var rng = rexps.GetRange(0, rfi);
					lexps.Insert(0,new RegexRepeatExpression(_CatIfNeeded(rng), 0, 1));
					lhs = _CatIfNeeded(lexps);
					rhs = null;
					return true;

				}
			}
			return false;
		}
		/// <summary>
		/// Creates a default instance of the expression
		/// </summary>
		public RegexOrExpression() { }
		/// <summary>
		/// Attempts to reduce the expression
		/// </summary>
		/// <param name="reduced">The reduced expression</param>
		/// <returns>True if reduced, otherwise false</returns>
		public override bool TryReduce(out RegexExpression reduced)
		{
			if (SkipReduce)
			{
				reduced = this;
				return false;
			}
			var result = false;
			var or = new RegexOrExpression();
			var hasnull = false;
			for (var i = 0; i < Expressions.Count; ++i)
			{
				var e = Expressions[i];
				if (e == null || e.IsEmptyElement)
				{
					if (hasnull)
					{
						result = true;
					}
					hasnull = true;
				}
				else
				{
					if (or._AddReduced(e, ref hasnull))
					{ 
						result = true;
					}
				}
			}

			if (!result)
			{
				for(int i = 0;i<or.Expressions.Count;++i)
				{
					var lhs = or.Expressions[i];
					for(int j = 0;j<i;++j)
					{
						var rhs = or.Expressions[j];

						if(_HuntDups(ref lhs,ref rhs))
						{
							if (rhs == null)
							{
								or.Expressions[i] = lhs;
								or.Expressions.RemoveAt(j);
								--j;
								--i;
							}
							else
							{
								or.Expressions[i] = lhs;
								or.Expressions[j] = rhs;
							}
							result = true;
						} else if(_HuntDups(ref rhs, ref lhs))
						{
							if (lhs == null)
							{
								or.Expressions[j] = rhs;
								or.Expressions.RemoveAt(i);
								--j;
								--i;
							}
							else
							{
								or.Expressions[j] = rhs;
								or.Expressions[i] = lhs;
							}
							result = true;
						}
					}
				}
				if(result)
				{
					for(int i = 0;i<or.Expressions.Count;++i)
					{
						if (or.Expressions[i] == null)
						{
							or.Expressions.RemoveAt(i);
							--i;
						}
					}
				}
				if (hasnull)
				{
					or.Expressions.Add(null);
				}
				if(result)
				{
					if(or.Expressions.Count == 0)
					{
						reduced = null;
						return true;
					} else if(or.Expressions.Count == 1)
					{
						reduced = or.Expressions[0];
						return true;
					} else
					{
						reduced = or;
						return true;
					}
				}
			}
			switch (or.Expressions.Count)
			{
				case 0:
					reduced = null;
					return true;
				case 1:
					if (!hasnull)
					{
						reduced = or.Expressions[0];
						return true;
					}
					reduced = new RegexRepeatExpression(or.Expressions[0], 0, 1);
					while (reduced != null && reduced.TryReduce(out reduced)) ;
					return true;
				default:
					RegexCharsetExpression s = null;
					RegexCharsetEntry c = null;
					for (var i = 0; i < or.Expressions.Count; ++i)
					{
						var e = or.Expressions[i];
						var lit = e as RegexLiteralExpression;
						var st = e as RegexCharsetExpression;
						if (lit != null && lit.Codepoints.Length == 1)
						{
							var r = new RegexCharsetCharEntry();
							r.Codepoint = lit.Codepoints[0];
							if (c == null)
							{
								c = r;
								if (s == null)
								{
									s = new RegexCharsetExpression();
								}
								s.Entries.Add(c);
								result = true;
							}
							else
							{
								result = true;
								s.Entries.Add(r);
								c = r;
							}
							result = true;
							or.Expressions.RemoveAt(i);
							--i;
						}
						else if (st != null)
						{
							if (st.HasNegatedRanges)
							{
								foreach (var range in st.GetRanges())
								{
									var r = new RegexCharsetRangeEntry();
									r.FirstCodepoint = range.Min;
									r.LastCodepoint = range.Max;
									if (c == null)
									{
										c = r;
										if (s == null)
										{
											s = new RegexCharsetExpression();
										}
										result = true;
										s.Entries.Add(c);
									}
									else
									{
										result = true;
										s.Entries.Add(r);
										c = r;
									}
								}
								result = true;
								or.Expressions.RemoveAt(i);
								--i;
							}
						}
					}
					if (s != null && !s.IsEmptyElement && !s.SkipReduce)
					{
						RegexExpression se = s;
						while (se != null && se.TryReduce(out se)) ;
						or.Expressions.Add(se);
					}
					if (hasnull)
					{
						or.Expressions.Add(null);
					}
					reduced = result?or:this;
					return result;
			}
		}
		/// <summary>
		/// Creates a state machine representing this expression
		/// </summary>
		/// <param name="accept">The accept symbol to use for this expression</param>
		/// <param name="compact">True to create a compact NFA, false to create an expanded NFA</param>
		/// <returns>A new <see cref="FA"/> finite state machine representing this expression</returns>
		public override FA ToFA(int accept = 0, bool compact = true)
		{
			var hasNull = false;
			var result = new FA();
			for (int i = 0; i < Expressions.Count; ++i)
			{
				var e = Expressions[i];
				if (e == null) { hasNull = true; continue; }
				result.AddEpsilon(e.ToFA(accept, compact),compact);
			}
			if(hasNull)
			{
				result.AcceptSymbol = accept;
			}
			return result;
		}
		/// <summary>
		/// Appends the textual representation to a <see cref="StringBuilder"/>
		/// </summary>
		/// <param name="sb">The string builder to use</param>
		/// <remarks>Used by ToString()</remarks>
		protected internal override void AppendTo(StringBuilder sb)
		{
			bool hasNull = false;
			for(int i = 0;i < Expressions.Count; ++i)
			{
				var e = Expressions[i];
				if (e == null) { hasNull = true; continue; }
				if(i>0)
				{
					sb.Append("|");
				}
				e.AppendTo(sb);
			}
			if(hasNull)
			{
				sb.Append("|");
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
		public new RegexOrExpression Clone()
		{
			var result = new RegexOrExpression();
			bool hasNull = false;
			for (int i = 0;i<Expressions.Count;++i)
			{
				var e = Expressions[i];
				if (e != null) {
					result.Expressions.Add(e.Clone());
				} else hasNull = true;
			}
			if(hasNull)
			{
				result.Expressions.Add(null);
			}
			return result;
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
			var hl = new HashSet<RegexExpression>(Expressions);
			var hr = new HashSet<RegexExpression>(rhs.Expressions);
			return hl.SetEquals(hr);
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
			for(int i = 0;i<Expressions.Count;++i)
			{
				var e= Expressions[i];
				if(e!=null)
				{
					result ^= e.GetHashCode();
				}
			}
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
		/// Indicates whether or not this statement is a empty element or not
		/// </summary>
		public override bool IsEmptyElement => Expression==null || Expression.IsEmptyElement;
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
		/// <param name="compact">True to create a compact NFA, otherwise create an expanded NFA</param>
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
						case 1:
							sb.Append('?');
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
					if (MaxOccurs != MinOccurs)
					{
						if (-1 != MinOccurs)
							sb.Append(MinOccurs);
						sb.Append(',');
						if (-1 != MaxOccurs)
							sb.Append(MaxOccurs);
					} else
					{
						sb.Append(MinOccurs);
					}
					sb.Append('}');
					break;
			}
		}
		/// <summary>
		/// Attempt to resude the expression
		/// </summary>
		/// <param name="reduced">The reduced expression</param>
		/// <returns>True if a reduction happened, otherwise false</returns>
		public override bool TryReduce(out RegexExpression reduced)
		{
			if (SkipReduce)
			{
				reduced = this;
				return false;
			}
			if (Expression == null)
			{
				reduced = null;
				return true;
			}
			if(MinOccurs == 1 && MaxOccurs == 1)
			{
				reduced = Expression;
				return true;
			}
			RegexExpression rexp=Expression;
			reduced = this;
			var lit = Expression as RegexLiteralExpression;
			if (lit == null)
			{
				if (Expression.TryReduce(out rexp))
				{
					Expression = rexp;
					return true;
				}
			} 
			var rep = rexp as RegexRepeatExpression;
			if (rep != null)
			{
				// TODO: More combinations here
				switch(MinOccurs)
				{
					case -1:
					case 0:
						switch(MaxOccurs)
						{
							case 1:
								if(rep.MaxOccurs ==0 && rep.MinOccurs==1)
								{
									rep.MinOccurs = 0;
									reduced = rep;
									return true;
								}
								break;
						}
						break;
				}
			}
			return false;
			
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
		public new RegexRepeatExpression Clone()
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
