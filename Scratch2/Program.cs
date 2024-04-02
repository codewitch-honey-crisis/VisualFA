using VisualFA;
namespace Scratch2
{
    internal class Program
   {
        static string _DeescapeString(string s)
        {
            if(string.IsNullOrEmpty(s)) return s;
            s= s.Substring(1,s.Length- 2);
            s = s.Replace(@"\b", "\b").Replace(@"\r", "\r").Replace(@"\n", "\n").Replace(@"\t", "\t").Replace(@"\f", "\f");
			return s;
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
				result.Add(_ParseAny(cursor));
				_SkipWS(cursor);
				if (cursor.Current.SymbolId == 5)
				{
					cursor.MoveNext();
				}
			}
			return result;
        }
        static object _ParseAny(IEnumerator<FAMatch> cursor)
        {
            object result = null;
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
                    result = double.Parse(cursor.Current.Value);
                    break;
                case 7: // boolean
                    if(0==string.CompareOrdinal(cursor.Current.Value,"true"))
                    {
                        result = true;
                    } else if (0 == string.CompareOrdinal(cursor.Current.Value, "false"))
                    {
                        result = false;
                    }
                    else
                    {
                        throw new Exception("Expecting true or false");
                    }
                    break;
                case 8: // null
                    break;
                case 9: // string
    				result = _DeescapeString(cursor.Current.Value);
					break;
                default:
                    throw new Exception("Expecting a value");
            }
            cursor.MoveNext();
            return result;
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
                var name = _DeescapeString(cursor.Current.Value);
				_SkipWS(cursor);
				if (!cursor.MoveNext()) throw new Exception("Unterminated JSON field");
				if (cursor.Current.SymbolId != 4) throw new Exception("Expecting a field separator");
				_SkipWS(cursor);
				if (!cursor.MoveNext()) throw new Exception("JSON field missing value");
                object value = _ParseAny(cursor);
                result.Add(name, value);
                _SkipWS(cursor);
                if(cursor.Current.SymbolId == 5)
                {
                    cursor.MoveNext();
                }
			}
			return result;
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
					var obj = _ParseObject(cursor);
                    return;
				}
            }
		}
    }
}
