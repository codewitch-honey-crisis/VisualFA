using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace VisualFA
{
	#region FAParseException
	/// <summary>
	/// Indicates an exception occurred parsing a regular expression
	/// </summary>
	
#if FALIB
	public
#endif
	sealed class FAParseException : FAException
	{
		/// <summary>
		/// Indicates the strings that were expected
		/// </summary>
		public string[] Expecting { get; } = null;
		/// <summary>
		/// Indicates the position where the error occurred
		/// </summary>
		public int Position { get; }
		/// <summary>
		/// Constructs a new instance
		/// </summary>
		/// <param name="message">The error</param>
		/// <param name="position">The position</param>
		public FAParseException(string message, int position) : base(message)
		{
			Position = position;
		}
		/// <summary>
		/// Constructs a new instance
		/// </summary>
		/// <param name="message">The error</param>
		/// <param name="position">The position</param>
		/// <param name="innerException">The inner exception</param>
		public FAParseException(string message, int position, Exception innerException) : base(message, innerException)
		{
			Position = position;
		}
		/// <summary>
		/// Constructs a new instance
		/// </summary>
		/// <param name="message">The error</param>
		/// <param name="position">The position</param>
		/// <param name="expecting">The strings that were expected</param>
		public FAParseException(string message, int position, string[] expecting) : base(message)
		{
			Position = position;
			Expecting = expecting;
		}
	}
	#endregion FAException
	partial class FA
	{
		#region Parse
		static FA _ParseModifier(FA expr, StringCursor pc, int accept, bool compact)
		{
			var position = pc.Position;
			switch (pc.Codepoint)
			{
				case '*':
					expr = Repeat(expr, 0, 0, accept, compact);
					pc.Advance();
					break;
				case '+':
					expr = Repeat(expr, 1, 0, accept, compact);
					pc.Advance();
					break;
				case '?':
					expr = Optional(expr, accept, compact);
					pc.Advance();
					break;
				case '{':
					pc.Advance();
					pc.TrySkipWhiteSpace();
					pc.Expecting('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ',', '}');
					var min = -1;
					var max = -1;
					if (',' != pc.Codepoint && '}' != pc.Codepoint)
					{
						var l = pc.CaptureBuffer.Length;
						if (!pc.TryReadDigits())
						{
							pc.Expecting('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
						}
						min = int.Parse(pc.GetCapture(l), CultureInfo.InvariantCulture.NumberFormat);
						pc.TrySkipWhiteSpace();
					}
					if (',' == pc.Codepoint)
					{
						pc.Advance();
						pc.TrySkipWhiteSpace();
						pc.Expecting('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '}');
						if ('}' != pc.Codepoint)
						{
							var l = pc.CaptureBuffer.Length;
							pc.TryReadDigits();
							max = int.Parse(pc.GetCapture(l), CultureInfo.InvariantCulture.NumberFormat);
							pc.TrySkipWhiteSpace();
						}
					}
					else { max = min; }
					pc.Expecting('}');
					pc.Advance();
					expr = Repeat(expr, min, max, accept, compact);
					break;
			}
			return expr;
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
		static int _ParseEscapePart(StringCursor pc)
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
		static int _ParseRangeEscapePart(StringCursor pc)
		{
			if (-1 == pc.Codepoint)
				return -1;
			switch (pc.Codepoint)
			{
				case '0':
					pc.Advance();
					return '\0';
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

		static KeyValuePair<bool, FARange[]> _ParseSet(StringCursor pc)
		{
			var result = new List<FARange>();
			pc.EnsureStarted();
			pc.Expecting('[');
			pc.Advance();
			pc.Expecting();
			var firstRead = true;
			int firstChar = '\0';
			var readFirstChar = false;
			var wantRange = false;

			var isNot = false;
			if ('^' == pc.Codepoint)
			{
				isNot = true;
				pc.Advance();
				pc.Expecting();
			}
			while (-1 != pc.Codepoint && (firstRead || ']' != pc.Codepoint))
			{
				if (!wantRange)
				{
					// can be a single char,
					// a range
					// or a named character class
					if ('[' == pc.Codepoint) // named char class
					{
						int epos = pc.Position;
						pc.Advance();
						pc.Expecting();
						if (':' != pc.Codepoint)
						{
							firstChar = '[';
							readFirstChar = true;
						}
						else
						{
							firstRead = false;
							pc.Advance();
							pc.Expecting();
							var ll = pc.CaptureBuffer.Length;
							if (!pc.TryReadUntil(':', false))
								throw new FAParseException("Expecting character class", pc.Position);
							pc.Expecting(':');
							pc.Advance();
							pc.Expecting(']');
							pc.Advance();
							var cls = pc.GetCapture(ll);
							int[] ranges;
							if (!CharacterClasses.Known.TryGetValue(cls, out ranges))
								throw new FAParseException("Unknown character class \"" + cls + "\" specified", epos);
							if (ranges != null)
							{
								result.AddRange(FARange.ToUnpacked(ranges));
							}
							readFirstChar = false;
							wantRange = false;
							firstRead = false;
							continue;
						}
					}
					if (!readFirstChar)
					{
						if ('\\' == pc.Codepoint)
						{
							pc.Advance();
							firstChar = _ParseRangeEscapePart(pc);
						}
						else
						{
							firstChar = pc.Codepoint;
							pc.Advance();
							pc.Expecting();
						}
						readFirstChar = true;

					}
					else
					{
						if ('-' == pc.Codepoint)
						{
							pc.Advance();
							pc.Expecting();
							wantRange = true;
						}
						else
						{
							result.Add(new FARange(firstChar, firstChar));
							readFirstChar = false;
						}
					}
					firstRead = false;
				}
				else
				{
					if ('\\' != pc.Codepoint)
					{
						var ch = pc.Codepoint;
						pc.Advance();
						pc.Expecting();
						result.Add(new FARange(firstChar, ch));
					}
					else
					{
						var min = firstChar;
						pc.Advance();
						result.Add(new FARange(min, _ParseRangeEscapePart(pc)));
					}
					wantRange = false;
					readFirstChar = false;
				}

			}
			if (readFirstChar)
			{
				result.Add(new FARange(firstChar, firstChar));
				if (wantRange)
				{
					result.Add(new FARange('-', '-'));
				}
			}
			pc.Expecting(']');
			pc.Advance();
			return new KeyValuePair<bool, FARange[]>(isNot, result.ToArray());
		}
		static FA _Parse(StringCursor pc, int accept, bool compact)
		{

			FA result = null;
			FA next = null;
			int ich;
			pc.EnsureStarted();
			while (true)
			{
				switch (pc.Codepoint)
				{
					case -1:
						if (result == null)
						{
							// empty string
							result = new FA(accept);
						}
						return result;
					case '.':
						var dot = FA.Set(new FARange[] { new FARange(0, 0x10ffff) }, accept, compact);
						if (null == result)
							result = dot;
						else
						{
							result = FA.Concat(new FA[] { result, dot }, accept, compact);
						}
						pc.Advance();
						result = _ParseModifier(result, pc, accept, compact);
						break;
					case '\\':

						pc.Advance();
						pc.Expecting();
						var isNot = false;
						switch (pc.Codepoint)
						{
							case 'P':
								isNot = true;
								goto case 'p';
							case 'p':
								pc.Advance();
								pc.Expecting('{');
								var uc = new StringBuilder();
								while (-1 != pc.Advance() && '}' != pc.Codepoint)
									uc.Append(char.ConvertFromUtf32(pc.Codepoint));
								pc.Expecting('}');
								pc.Advance();
								int uci = 0;
								switch (uc.ToString())
								{
									case "Pe":
										uci = 21;
										break;
									case "Pc":
										uci = 19;
										break;
									case "Cc":
										uci = 14;
										break;
									case "Sc":
										uci = 26;
										break;
									case "Pd":
										uci = 19;
										break;
									case "Nd":
										uci = 8;
										break;
									case "Me":
										uci = 7;
										break;
									case "Pf":
										uci = 23;
										break;
									case "Cf":
										uci = 15;
										break;
									case "Pi":
										uci = 22;
										break;
									case "Nl":
										uci = 9;
										break;
									case "Zl":
										uci = 12;
										break;
									case "Ll":
										uci = 1;
										break;
									case "Sm":
										uci = 25;
										break;
									case "Lm":
										uci = 3;
										break;
									case "Sk":
										uci = 27;
										break;
									case "Mn":
										uci = 5;
										break;
									case "Ps":
										uci = 20;
										break;
									case "Lo":
										uci = 4;
										break;
									case "Cn":
										uci = 29;
										break;
									case "No":
										uci = 10;
										break;
									case "Po":
										uci = 24;
										break;
									case "So":
										uci = 28;
										break;
									case "Zp":
										uci = 13;
										break;
									case "Co":
										uci = 17;
										break;
									case "Zs":
										uci = 11;
										break;
									case "Mc":
										uci = 6;
										break;
									case "Cs":
										uci = 16;
										break;
									case "Lt":
										uci = 2;
										break;
									case "Lu":
										uci = 0;
										break;
								}
								if (isNot)
								{
									next = FA.Set(FARange.ToUnpacked(CharacterClasses.UnicodeCategories[uci]), accept, compact);
								}
								else
									next = FA.Set(FARange.ToUnpacked(CharacterClasses.NotUnicodeCategories[uci]), accept, compact);
								break;
							case 'd':
								next = FA.Set(FARange.ToUnpacked(CharacterClasses.digit), accept, compact);
								pc.Advance();
								break;
							case 'D':
								next = FA.Set(FARange.ToNotRanges(FARange.ToUnpacked(CharacterClasses.digit)), accept, compact);
								pc.Advance();
								break;

							case 's':
								next = FA.Set(FARange.ToUnpacked(CharacterClasses.space), accept, compact);
								pc.Advance();
								break;
							case 'S':
								next = FA.Set(FARange.ToNotRanges(FARange.ToUnpacked(CharacterClasses.space)), accept, compact);
								pc.Advance();
								break;
							case 'w':
								next = FA.Set(FARange.ToUnpacked(CharacterClasses.word), accept, compact);
								pc.Advance();
								break;
							case 'W':
								next = FA.Set(FARange.ToNotRanges(FARange.ToUnpacked(CharacterClasses.word)), accept, compact);
								pc.Advance();
								break;
							default:
								if (-1 != (ich = _ParseEscapePart(pc)))
								{
									next = FA.Literal(new int[] { ich }, accept, compact);

								}
								else
								{
									pc.Expecting(); // throw an error
									System.Diagnostics.Debug.Assert(false);
								}
								break;
						}
						next = _ParseModifier(next, pc, accept, compact);
						if (null != result)
						{
							result = FA.Concat(new FA[] { result, next }, accept, compact);
						}
						else
							result = next;
						break;
					case ')':
						//result = result.ToMinimized();
						System.Diagnostics.Debug.Assert(result != null);
						return result;
					case '(':
						pc.Advance();
						if (pc.Codepoint == '?')
						{
							pc.Advance();
							pc.Expecting(':');
							pc.Advance();
						}
						pc.Expecting();
						next = _Parse(pc, accept, compact);
						pc.Expecting(')');
						pc.Advance();
						next = _ParseModifier(next, pc, accept, compact);
						if (null == result)
							result = next;
						else
						{
							result = FA.Concat(new FA[] { result, next }, accept, compact);
						}
						break;
					case '|':
						if (-1 != pc.Advance())
						{
							System.Diagnostics.Debug.Assert(result != null);
							next = _Parse(pc, accept, compact);
							result = FA.Or(new FA[] { result, next }, accept, compact);
						}
						else
						{
							System.Diagnostics.Debug.Assert(result != null);
							result = FA.Optional(result, accept, compact);
						}
						break;
					case '[':
						var seti = _ParseSet(pc);
						//IEnumerable<FARange> set;
						List<FARange> set = new List<FARange>(seti.Value);
						set.Sort((x, y) => { var c = x.Min.CompareTo(y.Min); if (0 != c) return c; return x.Max.CompareTo(y.Max); });
						_NormalizeSortedRangeList(set);
						if (seti.Key)
							set = new List<FARange>(FARange.ToNotRanges(set));

						next = FA.Set(set, accept);
						next = _ParseModifier(next, pc, accept, compact);

						if (null == result)
							result = next;
						else
						{
							result = FA.Concat(new FA[] { result, next }, accept, compact);

						}
						break;
					default:
						ich = pc.Codepoint;
						next = FA.Literal(new int[] { ich }, accept, compact);
						pc.Advance();
						next = _ParseModifier(next, pc, accept, compact);
						if (null == result)
							result = next;
						else
						{
							result = FA.Concat(new FA[] { result, next }, accept, compact);
						}
						break;
				}
			}
		}
		/// <summary>
		/// Parses a regular expression into a state machine
		/// </summary>
		/// <param name="text">The text to parse</param>
		/// <param name="accept">The accept id</param>
		/// <param name="compact">True to create a compact NFA, false to create an expanded NFA</param>
		/// <returns></returns>
		public static FA Parse(string text, int accept = 0, bool compact = true)
		{
			StringCursor pc = new StringCursor();
			pc.Input = text;
			return _Parse(pc, accept, compact);
		}
		#endregion // Parse

		#region StringCursor
		internal sealed class StringCursor
		{
			const int BeforeInput = -2;
			const int EndOfInput = -1;
			public string Input = null;
			public int Position = -1;
			public int Codepoint = -2;
			public StringBuilder CaptureBuffer { get; } = new StringBuilder();
			public void Capture()
			{
				if (Codepoint < 0)
				{
					return;
				}
				CaptureBuffer.Append(char.ConvertFromUtf32(Codepoint));
			}
			public string GetCapture(int index = 0, int length = -1)
			{
				if (length == -1)
				{
					if (index == 0)
					{
						return CaptureBuffer.ToString();
					}
					return CaptureBuffer.ToString(index, CaptureBuffer.Length - index);
				}
				return CaptureBuffer.ToString(index, length);
			}
			public void EnsureStarted()
			{
				if (Codepoint == -2)
				{
					Advance();
				}
			}
			public int Advance()
			{
				if (Input == null)
				{
					Codepoint = -1;
					return -1;
				}
				if (++Position >= Input.Length)
				{
					Codepoint = -1;
					return -1;
				}
				Codepoint = Input[Position];
				if (Codepoint <= 0xFFFF && char.IsHighSurrogate((char)Codepoint))
				{
					if (++Position >= Input.Length)
					{
						throw new IOException("Unexpected end of input in Unicode stream");
					}
					var tmp = Input[Position];
					if (tmp > 0xFFFF || !char.IsLowSurrogate((char)tmp))
					{
						throw new IOException("Unterminated surrogate Unicode stream");
					}
					Codepoint = char.ConvertToUtf32((char)Codepoint, (char)tmp);
				}
				return Codepoint;
			}
			public void Expecting(params int[] codepoints)
			{
				if (BeforeInput == Codepoint)
					throw new FAParseException("The cursor is before the beginning of the input", Position);
				switch (codepoints.Length)
				{
					case 0:
						if (EndOfInput == Codepoint)
							throw new FAParseException("Unexpected end of input", Position);
						break;
					case 1:
						if (codepoints[0] != Codepoint)
							throw new FAParseException(_GetErrorMessage(codepoints), Position, _GetErrorExpecting(codepoints));
						break;
					default:
						if (0 > Array.IndexOf(codepoints, Codepoint))
							throw new FAParseException(_GetErrorMessage(codepoints), Position, _GetErrorExpecting(codepoints));
						break;
				}
			}
			string[] _GetErrorExpecting(int[] codepoints)
			{
				var result = new string[codepoints.Length];
				for (var i = 0; i < codepoints.Length; i++)
				{
					if (-1 != codepoints[i])
						result[i] = char.ConvertFromUtf32(codepoints[i]);
					else
						result[i] = "end of input";
				}
				return result;
			}
			string _GetErrorMessage(int[] expecting)
			{
				StringBuilder sb = null;
				switch (expecting.Length)
				{
					case 0:
						break;
					case 1:
						sb = new StringBuilder();
						if (-1 == expecting[0])
							sb.Append("end of input");
						else
						{
							sb.Append("\"");
							sb.Append(char.ConvertFromUtf32(expecting[0]));
							sb.Append("\"");
						}
						break;
					case 2:
						sb = new StringBuilder();
						if (-1 == expecting[0])
							sb.Append("end of input");
						else
						{
							sb.Append("\"");
							sb.Append(char.ConvertFromUtf32(expecting[0]));
							sb.Append("\"");
						}
						sb.Append(" or ");
						if (-1 == expecting[1])
							sb.Append("end of input");
						else
						{
							sb.Append("\"");
							sb.Append(char.ConvertFromUtf32(expecting[1]));
							sb.Append("\"");
						}
						break;
					default: // length > 2
						sb = new StringBuilder();
						if (-1 == expecting[0])
							sb.Append("end of input");
						else
						{
							sb.Append("\"");
							sb.Append(char.ConvertFromUtf32(expecting[0]));
							sb.Append("\"");
						}
						int l = expecting.Length - 1;
						int i = 1;
						for (; i < l; ++i)
						{
							sb.Append(", ");
							if (-1 == expecting[i])
								sb.Append("end of input");
							else
							{
								sb.Append("\"");
								sb.Append(char.ConvertFromUtf32(expecting[1]));
								sb.Append("\"");
							}
						}
						sb.Append(", or ");
						if (-1 == expecting[i])
							sb.Append("end of input");
						else
						{
							sb.Append("\"");
							sb.Append(char.ConvertFromUtf32(expecting[i]));
							sb.Append("\"");
						}
						break;
				}
				if (-1 == Codepoint)
				{
					if (0 == expecting.Length)
						return "Unexpected end of input";
					System.Diagnostics.Debug.Assert(sb != null); // shut up code analysis
					return string.Concat("Unexpected end of input. Expecting ", sb.ToString());
				}
				if (0 == expecting.Length)
					return string.Concat("Unexpected character \"", char.ConvertFromUtf32(Codepoint), "\" in input");
				System.Diagnostics.Debug.Assert(sb != null); // shut up code analysis
				return string.Concat("Unexpected character \"", char.ConvertFromUtf32(Codepoint), "\" in input. Expecting ", sb.ToString());
			}
			public bool TrySkipWhiteSpace()
			{
				EnsureStarted();
				if (Input == null || Position >= Input.Length) return false;
				if (!char.IsWhiteSpace(Input, Position))
					return false;
				Advance();
				if (Position < Input.Length && char.IsLowSurrogate(Input, Position)) ++Position;
				while (Position < Input.Length && char.IsWhiteSpace(Input, Position))
				{
					Advance();
				}
				return true;
			}
			public bool TryReadDigits()
			{
				EnsureStarted();
				if (Input == null || Position >= Input.Length) return false;
				if (!char.IsDigit(Input, Position))
					return false;
				Capture();
				Advance();
				while (Position < Input.Length && char.IsDigit(Input, Position))
				{
					Capture();
					Advance();
				}
				return true;
			}
			public bool TryReadUntil(int character, bool readCharacter = true)
			{
				EnsureStarted();
				if (0 > character) character = -1;
				Capture();
				
				if (Codepoint == character)
				{
					return true;
				}
				while (-1 != Advance() && Codepoint != character)
					Capture();
				//
				if (Codepoint == character)
				{
					if (readCharacter)
					{
						Capture();
						Advance();
					}
					return true;
				}
				return false;
			}
		}
		#endregion // StringCursor

	}
}
