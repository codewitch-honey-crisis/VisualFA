
/// <summary>
/// Represents a match from <code>FARunner.MatchNext()</code>
/// </summary>
partial struct FAMatch
{
    /// <summary>
    /// The matched symbol - this is the accept id, or less than zero if the text did not match an expression
    /// </summary>
    public int SymbolId;
    /// <summary>
    /// The matched value
    /// </summary>
    public string Value;
    /// <summary>
    /// The position of the match within the codepoint series - this may not be the same as the character position due to surrogates
    /// </summary>
    public long Position;
    /// <summary>
    /// The one based line number
    /// </summary>
    public int Line;
    /// <summary>
    /// The one based column
    /// </summary>
    public int Column;
    /// <summary>
    /// Indicates whether the text matched the expression
    /// </summary>
    /// <remarks>Non matches are returned with negative accept symbols. You can use this property to determine if the text therein was part of a match.</remarks>
    public bool IsSuccess
    {
        get
        {
            return SymbolId > -1;
        }
    }
    /// <summary>
    /// Provides a string representation of the match
    /// </summary>
    /// <returns>A string containing match information</returns>
    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        sb.Append("[SymbolId: ");
        sb.Append(SymbolId);
        sb.Append(", Value: ");
        if (Value != null)
        {
            sb.Append("\"");
            sb.Append(Value.Replace("\r", "\\r").Replace("\t", "\\t").Replace("\n", "\\n").Replace("\v", "\\v"));
            sb.Append("\", Position: ");
        }
        else
        {
            sb.Append("null, Position: ");
        }
        sb.Append(Position);
        sb.Append(" (");
        sb.Append(Line);
        sb.Append(", ");
        sb.Append(Column);
        sb.Append(")]");
        return sb.ToString();
    }
    /// <summary>
    /// Constructs a new instance
    /// </summary>
    /// <param name="symbolId">The symbol id</param>
    /// <param name="value">The matched value</param>
    /// <param name="position">The absolute codepoint position</param>
    /// <param name="line">The line</param>
    /// <param name="column">The column</param>

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static FAMatch Create(int symbolId, string value, long position, int line, int column)
    {
        FAMatch result = default(FAMatch);
        result.SymbolId = symbolId;
        result.Value = value;
        result.Position = position;
        result.Line = line;
        result.Column = column;
        return result;
    }
}
