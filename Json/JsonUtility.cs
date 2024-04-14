using System.Text;

namespace Json
{

	internal static class JsonUtility
	{
		static int _FromHexChar(char c)
		{
			if (c >= '0' && c <= '9')
			{
				return c - '0';
			}
			if (c >= 'A' && c <= 'F')
			{
				return c - 'A' + 0x10;
			}
			if (c >= 'a' && c <= 'f')
			{
				return c - 'a' + 0x10;
			}
			return -1;
		}
		public static string DeescapeString(string s)
		{
			if (string.IsNullOrEmpty(s)) return s;

			var result = new StringBuilder(s.Length);
			for (int i = 0; i < s.Length; ++i)
			{
				var c = s[i];
				if (c == '\\')
				{
					++i;
					if (i >= s.Length)
					{
						result.Append('\\');
						break;
					}
					c = s[i];
					switch (c)
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
							if (i + 4 >= s.Length)
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
				}
				else
				{
					result.Append(c);
				}
			}
			return result.ToString();
		}
		public static string EscapeString(string str)
		{
			var result = new StringBuilder(str.Length * 2);
			for (int i = 0; i < str.Length; i++)
			{
				var c = str[i];
				switch (c)
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
						if (((int)c) < 128)
						{
							result.Append(c);
						}
						else
						{
							result.Append("\\u");
							result.Append(((int)c).ToString("x4"));
						}
						break;
				}
			}
			return result.ToString();
		}
		public static object? Wrap(object? value)
		{
			if (value == null) return null;
			if (value is IJsonElement) return value;
			if (value is IList<object?> list)
			{
				value = new JsonArray(list);
			}
			else if (value is IDictionary<string, object?> dictionary)
			{
				value = new JsonObject(dictionary);
			}
			return value;
		}
	}	
}
