/// <summary>
/// Represents a match from <see cref="FARunner"/></code>
/// </summary>
internal partial struct FAMatch
{
	private int _symbolId;
	private string _value;
	private long _position;
	private int _line;
	private int _column;
	/// <summary>
	/// The matched symbol - this is the accept id
	/// </summary>
	public int SymbolId { get { return _symbolId; } }
	/// <summary>
	/// The matched value
	/// </summary>
	public string Value { get { return _value; } }
	/// <summary>
	/// The position of the match within the input
	/// </summary>
	public long Position { get { return _position; } }
	/// <summary>
	/// The one based line number
	/// </summary>
	public int Line { get { return _line; } }
	/// <summary>
	/// The one based column
	/// </summary>
	public int Column { get { return _column; } }
	/// <summary>
	/// Constructs a new instance
	/// </summary>
	/// <param name="symbolId">The symbol id</param>
	/// <param name="value">The matched value</param>
	/// <param name="position">The match position</param>
	/// <param name="line">The line</param>
	/// <param name="column">The column</param>
	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public static FAMatch Create(int symbolId, string value, long position, int line, int column)
	{
		FAMatch result = default(FAMatch);
		result._symbolId = symbolId;
		result._value = value;
		result._position = position;
		result._line = line;
		result._column = column;
		return result;
	}
}
