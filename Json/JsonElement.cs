using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;

namespace Json
{
	public interface IJsonElement
	{
		public void WriteTo(TextWriter writer, bool minimized = false);
		public string ToString(string? format);
		public string ToString();
	}

	public sealed class JsonObject : DynamicObject, IDictionary<string, object?>, IJsonElement
	{
		IDictionary<string, object?> _inner;
		public JsonObject()
		{
			_inner = new Dictionary<string, object?>(StringComparer.Ordinal);
		}
		public JsonObject(IDictionary<string, object?> inner)
		{
			if (_inner == null) throw new ArgumentNullException(nameof(inner));
			_inner = new Dictionary<string, object?>(inner.Count, StringComparer.Ordinal);
			foreach (var entry in inner)
			{
				_inner[entry.Key] = JsonUtility.Wrap(inner);
			}

		}

		public override IEnumerable<string> GetDynamicMemberNames()
		{
			foreach (var key in _inner.Keys)
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
			(_inner).Add(new KeyValuePair<string, object?>(item.Key, JsonUtility.Wrap(item.Value)));
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
			JsonWriter.WriteTo(_inner, writer, minimized);
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
			return JsonParser.Parse(value);
		}
		public static object? Parse(TextReader value)
		{
			if (value == null) throw new JsonException("No content", 0, 0, 0);
			return JsonParser.Parse(value);
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
			if (_inner == null) throw new ArgumentNullException(nameof(inner));
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
			if (indexes.Length != 1)
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
			JsonWriter.WriteTo(_inner, writer, minimized);
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
			return JsonParser.Parse(value);
		}
		public static object? Parse(TextReader value)
		{
			if (value == null) throw new JsonException("No content", 0, 0, 0);
			return JsonParser.Parse(value);
		}
	}
}
