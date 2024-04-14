using System.Text;
using System;
using System.Dynamic;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using System.Globalization;

namespace Json
{
	[Serializable]
	public sealed class JsonException : Exception
	{
		public int Line { get; set; }
		public int Column { get; set; }
		public long Position { get; set; }
		public JsonException(string message, long position, int line, int column, Exception innerException) : base(message, innerException)
		{
			Position = position;
			Line = line;
			Column = column;
		}
		public JsonException(string message, long position, int line, int column) : base(message)
		{
			Position = position;
			Line = line;
			Column = column;
		}
	}
	public interface IJsonElement
	{
		public void WriteTo(TextWriter writer, bool minimized=false);
		public string ToString(string? format);
		public string ToString();
	}
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
		static string _DeescapeString(string s)
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
		static void _SkipWS(IEnumerator<FAMatch> cursor)
		{
			while (cursor.Current.SymbolId == JsonStringRunner.white_space && cursor.MoveNext()) ;
		}
		static JsonArray _ParseArray(IEnumerator<FAMatch> cursor)
		{
			var position = cursor.Current.Position;
			var line = cursor.Current.Line;
			var column = cursor.Current.Column;
			var result = new JsonArray();
			_SkipWS(cursor);
			if (cursor.Current.SymbolId != JsonStringRunner.array) throw new Exception("Expected an array");
			if (!cursor.MoveNext()) throw new JsonException("Unterminated array",position,line,column);
			while (cursor.Current.SymbolId != JsonStringRunner.array_end)
			{
				result.Add(_ParseValue(cursor));
				_SkipWS(cursor);
				if (cursor.Current.SymbolId == JsonStringRunner.comma)
				{
					cursor.MoveNext();
					_SkipWS(cursor);
				}
			}
			return result;
		}
		static object _ParseValue(IEnumerator<FAMatch> cursor)
		{
			var position = cursor.Current.Position;
			var line = cursor.Current.Line;
			var column = cursor.Current.Column;

			object? result = null;
			_SkipWS(cursor);
			switch (cursor.Current.SymbolId)
			{
				case JsonStringRunner.@object:
					result = _ParseObject(cursor);
					break;
				case JsonStringRunner.array:
					result = _ParseArray(cursor);
					break;
				case JsonStringRunner.number:
					result = double.Parse(cursor.Current.Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
					break;
				case JsonStringRunner.boolean:
					result = cursor.Current.Value[0] == 't';
					break;
				case JsonStringRunner.@null:
					break;
				case JsonStringRunner._string:
					result = _DeescapeString(cursor.Current.Value.Substring(1, cursor.Current.Value.Length - 2));
					break;
				default:
					throw new JsonException("Expecting a value",position,line,column);
			}
			cursor.MoveNext();
			return result!;
		}
		static JsonObject _ParseObject(IEnumerator<FAMatch> cursor)
		{
			var position = cursor.Current.Position;
			var line = cursor.Current.Line;
			var column = cursor.Current.Column;

			var result = new JsonObject();
			_SkipWS(cursor);
			if (cursor.Current.SymbolId != JsonStringRunner.@object) throw new JsonException("Expecting a JSON object",position,line,column);
			if (!cursor.MoveNext()) throw new JsonException("Unterminated JSON object", position, line, column);
			while (cursor.Current.SymbolId != JsonStringRunner.object_end)
			{
				_SkipWS(cursor);

				position = cursor.Current.Position;
				line = cursor.Current.Line;
				column = cursor.Current.Column;
				if (cursor.Current.SymbolId != JsonStringRunner._string) throw new JsonException("Expecting a field name", position, line, column);
				var name = _DeescapeString(cursor.Current.Value.Substring(1, cursor.Current.Value.Length - 2));
				_SkipWS(cursor);
				if (!cursor.MoveNext()) throw new JsonException("Unterminated JSON field", position, line, column);
				if (cursor.Current.SymbolId != JsonStringRunner.field) throw new JsonException("Expecting a field separator", position, line, column);
				_SkipWS(cursor);
				if (!cursor.MoveNext()) throw new JsonException("JSON field missing value", position, line, column);
				object value = _ParseValue(cursor);
				result.Add(name, value);
				_SkipWS(cursor);
				if (cursor.Current.SymbolId == JsonStringRunner.comma)
				{
					cursor.MoveNext();
				}
			}
			return result;
		}
		static string _EscapeString(string str)
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
		static void _WriteValue(object value, TextWriter writer, int depth = 0, bool minimized = false)
		{
			if (value == null)
			{
				writer.Write("null");
			}
			else if (value is string)
			{
				writer.Write("\"");
				writer.Write(_EscapeString((string)value));
				writer.Write("\"");
			}
			else if (value is double)
			{
				writer.Write(((double)value).ToString("r"));
			}
			else if (value is bool)
			{
				writer.Write(((bool)value) ? "true" : "false");
			}
			else if (value is IDictionary<string, object>)
			{
				_WriteObject((IDictionary<string, object>)value, writer, depth, minimized);
			}
			else if (value is IList<object>)
			{
				_WriteArray((IList<object>)value, writer, depth,minimized);
			}
			else
			{
				throw new NotSupportedException("The value type cannot be written");
			}
		}
		static void _WriteArray(IList<object> result, TextWriter writer, int depth = 0, bool minimized = false)
		{
			var tabs = new string(' ', 4 * depth);
			if (!minimized)
				writer.WriteLine("[");
			else writer.Write("[");
			var innerTabs = new string(' ', 4 * (depth + 1));
			var c = result.Count;
			foreach (var value in result)
			{
				if(!minimized)
					writer.Write(innerTabs);
				_WriteValue(value, writer, depth + 1,minimized);
				--c;
				if (c > 0)
				{
					if (!minimized)
						writer.WriteLine(",");
					else writer.Write(",");
				}
				else if(!minimized)
				{
					writer.WriteLine();
				}
			}
			if (!minimized)
			{
				writer.Write(tabs);
			}
			writer.Write("]");
		}
		static void _WriteObject(IDictionary<string, object> result, TextWriter writer, int depth = 0, bool minimized = false)
		{
			var tabs = new string(' ', 4 * depth);
			if (!minimized)
				writer.WriteLine("{");
			else writer.Write("{");
			var innerTabs = new string(' ', 4 * (depth + 1));
			var c = result.Count;
			foreach (var field in result)
			{
				if (!minimized)
				{
					writer.Write(innerTabs);
				}
				_WriteValue(field.Key, writer, depth,minimized);
				if (!minimized)
				{
					writer.Write(": ");
				} else
				{
					writer.Write(':');
				}
				_WriteValue(field.Value, writer, depth + 1,minimized);
				--c;
				if (c > 0)
				{
					if (!minimized)
						writer.WriteLine(",");
					else writer.Write(',');
				}
				else if(!minimized)
				{
					writer.WriteLine();
				}
			}
			if(!minimized)
				writer.Write(tabs);
			writer.Write("}");
		}
		private static object? _Parse(FARunner runner)
		{
			var e = runner.GetEnumerator();
			if (e.MoveNext())
			{
				return _ParseValue(e);
			}
			throw new JsonException("No content", 0, 0, 0);
		}
		public static object? Parse(string json)
		{
			var runner = new JsonStringRunner();
			runner.Set(json);
			return _Parse(runner);
		}
		public static object? Parse(TextReader json)
		{
			var runner = new JsonTextReaderRunner();
			runner.Set(json);
			return _Parse(runner);
		}
		public static void WriteTo(object value,TextWriter output, bool minimized=false) { 
			_WriteValue(value,output,0,minimized);
		}
		public static object? Wrap(object? value)
		{
			if(value==null) return null;
			if(value is IJsonElement) return value;
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
	public sealed class JsonObject : DynamicObject, IDictionary<string, object?> , IJsonElement
	{
		IDictionary<string, object?> _inner;
		public JsonObject()
		{
			_inner = new Dictionary<string, object?>(StringComparer.Ordinal);
		}
		public JsonObject(IDictionary<string,object?> inner)
		{
			if(_inner == null) throw new ArgumentNullException(nameof(inner));
			_inner = new Dictionary<string, object?>(inner.Count, StringComparer.Ordinal);
			foreach(var entry in inner)
			{
				_inner[entry.Key] = JsonUtility.Wrap(inner);
			}
			
		}
		
		public override IEnumerable<string> GetDynamicMemberNames()
		{
			foreach(var key in _inner.Keys)
			{
				yield return key;
			}
		}
		public override bool TryGetMember(GetMemberBinder binder, out object? result)
		{
			return _inner.TryGetValue(binder.Name, out result);
		}
		public override bool TrySetMember(SetMemberBinder binder, object? value)
		{
			_inner[binder.Name] = JsonUtility.Wrap(value);
			return true;
		}
		public object? this[string key] { get => (_inner)[key]; set => (_inner)[key] = JsonUtility.Wrap(value); }

		public ICollection<string> Keys => (_inner).Keys;

		public ICollection<object?> Values => (_inner).Values;

		public int Count => (_inner).Count;

		public bool IsReadOnly => (_inner).IsReadOnly;

		public void Add(string key, object? value)
		{
			(_inner).Add(key, JsonUtility.Wrap(value));
		}

		public void Add(KeyValuePair<string, object?> item)
		{
			(_inner).Add(new KeyValuePair<string,object?>(item.Key, JsonUtility.Wrap(item.Value)));
		}

		public void Clear()
		{
			(_inner).Clear();
		}

		public bool Contains(KeyValuePair<string, object?> item)
		{
			return (_inner).Contains(item);
		}

		public bool ContainsKey(string key)
		{
			return (_inner).ContainsKey(key);
		}

		public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
		{
			(_inner).CopyTo(array, arrayIndex);
		}

		public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
		{
			return (_inner).GetEnumerator();
		}

		public bool Remove(string key)
		{
			return (_inner).Remove(key);
		}

		public bool Remove(KeyValuePair<string, object?> item)
		{
			return (_inner).Remove(item);
		}

		public bool TryGetValue(string key, [MaybeNullWhen(false)] out object? value)
		{
			return (_inner).TryGetValue(key, out value);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_inner).GetEnumerator();
		}

		public void WriteTo(TextWriter writer, bool minimized = false)
		{
			JsonUtility.WriteTo(_inner, writer, minimized);
		}

		public string ToString(string? format)
		{
			var writer = new System.IO.StringWriter();
			WriteTo(writer, format == "m");
			return writer.ToString();
		}
		public override string ToString()
		{
			return ToString(null);
		}
		public static object? Parse(string value)
		{
			if (value == null) throw new JsonException("No content", 0, 0, 0);
			return JsonUtility.Parse(value);
		}
		public static object? Parse(TextReader value)
		{
			if (value == null) throw new JsonException("No content", 0, 0, 0);
			return JsonUtility.Parse(value);
		}
	}
	public sealed class JsonArray : DynamicObject, IList<object?>, IJsonElement
	{
		IList<object?> _inner;
		public JsonArray()
		{
			_inner = new List<object?>();
		}
		public JsonArray(IList<object?> inner)
		{
			if(_inner == null) throw new ArgumentNullException(nameof(inner));
			_inner = new List<object?>(inner.Count);
			_inner = inner;
		}
		public object? this[int index] { get => _inner[index]; set => _inner[index] = JsonUtility.Wrap(value); }

		public int Count => _inner.Count;

		public bool IsReadOnly => _inner.IsReadOnly;

		public void Add(object? item)
		{
			_inner.Add(JsonUtility.Wrap(item));
		}

		public void Clear()
		{
			_inner.Clear();
		}

		public bool Contains(object? item)
		{
			return _inner.Contains(item);
		}

		public void CopyTo(object?[] array, int arrayIndex)
		{
			_inner.CopyTo(array, arrayIndex);
		}

		public IEnumerator<object?> GetEnumerator()
		{
			return _inner.GetEnumerator();
		}

		public int IndexOf(object? item)
		{
			return _inner.IndexOf(item);
		}

		public void Insert(int index, object? item)
		{
			_inner.Insert(index, JsonUtility.Wrap(item));
		}

		public bool Remove(object? item)
		{
			return _inner.Remove(item);
		}

		public void RemoveAt(int index)
		{
			_inner.RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_inner).GetEnumerator();
		}
		public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
		{
			result = default;
			if(indexes.Length != 1)
			{
				return false;
			}
			if (!(indexes[0] is int)) return false;
			var i = (int)indexes[0];
			result = _inner[i];
			return true;
		}
		public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value)
		{
			if (indexes.Length != 1)
			{
				return false;
			}
			if (!(indexes[0] is int)) return false;
			var i = (int)indexes[0];
			_inner[i] = JsonUtility.Wrap(value);
			return true;
		}
		public void WriteTo(TextWriter writer, bool minimized = false)
		{
			JsonUtility.WriteTo(_inner, writer, minimized);
		}

		public string ToString(string? format)
		{
			var writer = new System.IO.StringWriter();
			WriteTo(writer, format == "m");
			return writer.ToString();
		}
		public override string ToString()
		{
			return ToString(null);
		}
		public static object? Parse(string value)
		{
			if (value == null) throw new JsonException("No content", 0, 0, 0);
			return JsonUtility.Parse(value);
		}
		public static object? Parse(TextReader value)
		{
			if (value == null) throw new JsonException("No content", 0, 0, 0);
			return JsonUtility.Parse(value);
		}
	}
}
