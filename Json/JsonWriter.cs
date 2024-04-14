namespace Json
{
	internal static class JsonWriter
	{

		static void _WriteValue(object value, TextWriter writer, int depth = 0, bool minimized = false)
		{
			if (value == null)
			{
				writer.Write("null");
			}
			else if (value is string)
			{
				writer.Write("\"");
				writer.Write(JsonUtility.EscapeString((string)value));
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
				_WriteArray((IList<object>)value, writer, depth, minimized);
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
				if (!minimized)
					writer.Write(innerTabs);
				_WriteValue(value, writer, depth + 1, minimized);
				--c;
				if (c > 0)
				{
					if (!minimized)
						writer.WriteLine(",");
					else writer.Write(",");
				}
				else if (!minimized)
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
				_WriteValue(field.Key, writer, depth, minimized);
				if (!minimized)
				{
					writer.Write(": ");
				}
				else
				{
					writer.Write(':');
				}
				_WriteValue(field.Value, writer, depth + 1, minimized);
				--c;
				if (c > 0)
				{
					if (!minimized)
						writer.WriteLine(",");
					else writer.Write(',');
				}
				else if (!minimized)
				{
					writer.WriteLine();
				}
			}
			if (!minimized)
				writer.Write(tabs);
			writer.Write("}");
		}
		public static void WriteTo(object value, TextWriter output, bool minimized = false)
		{
			_WriteValue(value, output, 0, minimized);
		}

	}
}
