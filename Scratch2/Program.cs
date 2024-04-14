using System.Text;

using VisualFA;
namespace Scratch2
{
    internal class Program
   {
        static int _FromHexChar(char c)
        {
            if(c >= '0' && c <='9')
            {
                return c - '0';
            }
            if(c >= 'A' && c <='F')
            {
                return c - 'A' + 0x10;
            }
			if (c >= 'a' && c <= 'f')
			{
				return c - 'a' + 0x10;
			}
            return -1;
		}
        static string _DeescapeString(string s)
        {
            if(string.IsNullOrEmpty(s)) return s;
			
            var result = new StringBuilder(s.Length);
            for(int i = 0;i<s.Length;++i)
            {
                var c = s[i];
                if(c=='\\')
                {
                    ++i;
                    if(i>=s.Length)
                    {
                        result.Append('\\');
                        break;
                    }
                    c = s[i];
                    switch(c)
                    {
                        case '/':
							result.Append('/');
							break;
						case '\\':
                            result.Append('\\');
                            break;
                        case '\"':
                            result.Append("\"");
                            break;
                        case 't':
                            result.Append('\t');
                            break;
                        case 'r':
                            result.Append('\r');
                            break;
                        case 'n':
                            result.Append('\n');
                            break;
                        case 'f':
                            result.Append('\f');
                            break;
                        case 'b':
                            result.Append('\b');
                            break;
                        case 'u':
							++i;
							if (i >= s.Length)
							{
								result.Append("\\u");
								break;
							}
							if(i+4>=s.Length)
                            {
                                result.Append(s.Substring(i));
                                break;
                            }
                            
                            var cp = _FromHexChar(s[i++]);
							cp <<= 8;
							cp |= _FromHexChar(s[i++]);
							cp <<= 8;
							cp |= _FromHexChar(s[i++]);
							cp <<= 8;
							cp |= _FromHexChar(s[i]);
							result.Append((char)cp);
                            break;
						default:
                            break;

					}
                } else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }
        static void _SkipWS(IEnumerator<FAMatch> cursor)
        {
            while (cursor.Current.SymbolId == 10 && cursor.MoveNext()) ;
        }
        static List<object> _ParseArray(IEnumerator<FAMatch> cursor)
        {
            var result = new List<object>();
			_SkipWS(cursor);
			if (cursor.Current.SymbolId != 2) throw new Exception("Expected an array");
            if (!cursor.MoveNext()) throw new Exception("Unterminated array");
            while(cursor.Current.SymbolId != 3)
            {
				result.Add(_ParseValue(cursor));
				_SkipWS(cursor);
				if (cursor.Current.SymbolId == 5)
				{
					cursor.MoveNext();
				}
			}
			return result;
        }
        static object _ParseValue(IEnumerator<FAMatch> cursor)
        {
            object? result = null;
            _SkipWS(cursor);
            switch(cursor.Current.SymbolId)
            {
                case 0: // object
					result = _ParseObject(cursor);
					break;
				case 2: // array
                    result = _ParseArray(cursor);
                    break;
                case 6: // number
                    result = double.Parse(cursor.Current.Value,System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                    break;
                case 7: // boolean
                    result = cursor.Current.Value[0] == 't';
                    
                    break;
                case 8: // null
                    break;
                case 9: // string
    				result = _DeescapeString(cursor.Current.Value.Substring(1, cursor.Current.Value.Length - 2));
					break;
                default:
                    throw new Exception("Expecting a value");
            }
            cursor.MoveNext();
            return result!;
        }
        static Dictionary<string,object> _ParseObject(IEnumerator<FAMatch> cursor)
        {
            var result = new Dictionary<string,object>();
            _SkipWS(cursor);
            if(cursor.Current.SymbolId !=0) throw new Exception("Expecting a JSON object");
			if (!cursor.MoveNext()) throw new Exception("Unterminated JSON object");
            while(cursor.Current.SymbolId != 1)
            {
                _SkipWS(cursor);
                if(cursor.Current.SymbolId != 9) throw new Exception("Expecting a field name");
                var name = _DeescapeString(cursor.Current.Value.Substring(1,cursor.Current.Value.Length-2));
				_SkipWS(cursor);
				if (!cursor.MoveNext()) throw new Exception("Unterminated JSON field");
				if (cursor.Current.SymbolId != 4) throw new Exception("Expecting a field separator");
				_SkipWS(cursor);
				if (!cursor.MoveNext()) throw new Exception("JSON field missing value");
                object value = _ParseValue(cursor);
                result.Add(name, value);
                _SkipWS(cursor);
                if(cursor.Current.SymbolId == 5)
                {
                    cursor.MoveNext();
                }
			}
			return result;
        }
        static string _EscapeString(string str)
        {
            var result = new StringBuilder(str.Length * 2);
            for(int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                switch(c)
                {
                    case '\\':
                    case '\"':
                        result.Append("\\\"");
                        break;
					case '\t':
						result.Append("\\t");
						break;
					case '\r':
						result.Append("\\r");
						break;
					case '\n':
						result.Append("\\n");
						break;
					case '\b':
						result.Append("\\b");
						break;
					case '\f':
						result.Append("\\f");
						break;
                    default:
                        if(((int)c)<128)
                        {
                            result.Append(c);
                        } else
                        {
                            result.Append("\\u");
                            result.Append(((int)c).ToString("x4"));
                        }
                        break;
				}
            }
            return result.ToString();
        }
        static void _WriteValue(object value,TextWriter writer, int depth = 0)
        {
            if(value == null)
            {
                writer.Write("null");
            } else if(value is string)
            {
                writer.Write("\"");
                writer.Write(_EscapeString((string)value));
				writer.Write("\"");
			} else if(value is double)
            {
                writer.Write(((double)value).ToString("r"));
            } else if(value is bool)
            {
                writer.Write(((bool)value) ? "true" : "false");
            } else if(value is Dictionary<string,object>)
            {
                _WriteObject((Dictionary<string,object>)value, writer, depth);
            } else if(value is List<object>)
            {
                _WriteArray((List<object>)value, writer, depth);
            }
            else
            {
                throw new NotSupportedException("The value type cannot be written");
            }
        }
		static void _WriteArray(List<object> result, TextWriter writer, int depth = 0)
		{
			var tabs = new string(' ', 4 * depth);
			writer.WriteLine("[");
			var innerTabs = new string(' ', 4 * (depth + 1));
			var c = result.Count;
			foreach (var value in result)
			{
				writer.Write(innerTabs);
				_WriteValue(value, writer, depth + 1);
				--c;
				if (c > 0)
				{
					writer.WriteLine(",");
				}
				else
				{
					writer.WriteLine();
				}
			}
			writer.Write(tabs);
			writer.Write("]");
		}
		static void _WriteObject(Dictionary<string,object> result, TextWriter writer, int depth = 0)
        {
            var tabs = new string(' ', 4 * depth);
            writer.WriteLine("{");
            var innerTabs = new string(' ', 4 * (depth+1));
            var c = result.Count;
            foreach(var field in result)
            {
                writer.Write(innerTabs);
                _WriteValue(field.Key,writer,depth);
                writer.Write(": ");
				_WriteValue(field.Value, writer,depth+1);
                --c;
                if(c>0)
                {
                    writer.WriteLine(",");
                } else
                {
                    writer.WriteLine();
                }
			}
            writer.Write(tabs);
            writer.Write("}");
		}
        static void Main(string[] args)
        {
            int id = 0;
            var @object = FA.Parse(@"\{",id++);
            var object_end = FA.Parse(@"\}", id++);
			var array = FA.Parse(@"\[", id++);
			var array_end = FA.Parse(@"\]", id++);
            var field = FA.Parse(@":", id++);
			var comma = FA.Parse(@",", id++);
			var number = FA.Parse(@"-?(?:0|[1-9][0-9]*)(?:\.[0-9]+)?(?:[eE][+-]?[0-9]+)?", id++);
            var boolean = FA.Parse("true|false", id++);
            var @null = FA.Parse("null", id++);
            var @string = FA.Parse(@"""([^\n""\\]|\\([btrnf""\\/]|(u[0-9A-Fa-f]{4})))*""",id++);
            var white_space = FA.Parse(@"[ \t\r\n]+",id++);
            var symbols = new string[] { "object", "object end", "array", "array end", "field", "comma", "number", "boolean", "null", "string" , "white space"};
            var dgo = new FADotGraphOptions();
            dgo.AcceptSymbolNames = symbols;
            var lexer = FA.ToLexer(new FA[] { @object, object_end, array, array_end, field, comma, number, boolean, @null, @string, white_space });
			lexer.RenderToFile(@"..\..\..\lexer.jpg", dgo);
            using(var reader = new StreamReader(@"..\..\..\data.json"))
            {
                var runner = lexer.Run(reader);
                var cursor = runner.GetEnumerator();
                if(cursor.MoveNext())
                {
					var obj = _ParseValue(cursor);
                    _WriteValue(obj, Console.Out, 0);
                    return;
				}
            }
		}
    }
}
