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
}
