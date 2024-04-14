namespace Json
{
	internal static class JsonParser
	{
		static void _SkipWS(IEnumerator<FAMatch> cursor)
		{
			while (cursor.Current.SymbolId == JsonStringRunner.WhiteSpace && cursor.MoveNext()) ;
		}
		static JsonArray _ParseArray(IEnumerator<FAMatch> cursor)
		{
			var position = cursor.Current.Position;
			var line = cursor.Current.Line;
			var column = cursor.Current.Column;
			var result = new JsonArray();
			_SkipWS(cursor);
			if (cursor.Current.SymbolId != JsonStringRunner.Array) throw new Exception("Expected an array");
			if (!cursor.MoveNext()) throw new JsonException("Unterminated array", position, line, column);
			while (cursor.Current.SymbolId != JsonStringRunner.ArrayEnd)
			{
				result.Add(_ParseValue(cursor));
				_SkipWS(cursor);
				if (cursor.Current.SymbolId == JsonStringRunner.Comma)
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
				case JsonStringRunner.Object:
					result = _ParseObject(cursor);
					break;
				case JsonStringRunner.Array:
					result = _ParseArray(cursor);
					break;
				case JsonStringRunner.Number:
					result = double.Parse(cursor.Current.Value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
					break;
				case JsonStringRunner.Boolean:
					result = cursor.Current.Value[0] == 't';
					break;
				case JsonStringRunner.Null:
					break;
				case JsonStringRunner.String:
					result = JsonUtility.DeescapeString(cursor.Current.Value.Substring(1, cursor.Current.Value.Length - 2));
					break;
				default:
					throw new JsonException("Expecting a value", position, line, column);
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
			if (cursor.Current.SymbolId != JsonStringRunner.Object) throw new JsonException("Expecting a JSON object", position, line, column);
			if (!cursor.MoveNext()) throw new JsonException("Unterminated JSON object", position, line, column);
			while (cursor.Current.SymbolId != JsonStringRunner.ObjectEnd)
			{
				_SkipWS(cursor);

				position = cursor.Current.Position;
				line = cursor.Current.Line;
				column = cursor.Current.Column;
				if (cursor.Current.SymbolId != JsonStringRunner.String) throw new JsonException("Expecting a field name", position, line, column);
				var name = JsonUtility.DeescapeString(cursor.Current.Value.Substring(1, cursor.Current.Value.Length - 2));
				_SkipWS(cursor);
				if (!cursor.MoveNext()) throw new JsonException("Unterminated JSON field", position, line, column);
				if (cursor.Current.SymbolId != JsonStringRunner.FieldSeparator) throw new JsonException("Expecting a field separator", position, line, column);
				_SkipWS(cursor);
				if (!cursor.MoveNext()) throw new JsonException("JSON field missing value", position, line, column);
				object value = _ParseValue(cursor);
				result.Add(name, value);
				_SkipWS(cursor);
				if (cursor.Current.SymbolId == JsonStringRunner.Comma)
				{
					cursor.MoveNext();
				}
			}
			return result;
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
	}
}
