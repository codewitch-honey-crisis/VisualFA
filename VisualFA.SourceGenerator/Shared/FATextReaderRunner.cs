abstract partial class FATextReaderRunner : FARunner
{
    protected TextReader input_reader = TextReader.Null;
    protected System.Text.StringBuilder capture = new System.Text.StringBuilder();
    protected int current = -2;
    public void Set(TextReader reader)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));
        this.input_reader = reader;
        current = -2;
        position = -1;
        line = 1;
        column = 1;
    }
    public override void Reset()
    {
        throw new NotSupportedException();
    }
    protected void Advance()
    {
        switch (this.current)
        {
            case '\n':
                ++line;
                column = 1;
                break;
            case '\r':
                column = 1;
                break;
            case '\t':
                column = ((column - 1) / tabWidth) * (tabWidth + 1);
                break;
            default:
                if (this.current > 31)
                {
                    ++column;
                }
                break;
        }
        if (current > -1)
        {
            capture.Append(char.ConvertFromUtf32(current));
        }
        current = input_reader.Read();
        if (current == -1)
        {
            return;
        }
        ++position;
        char ch1 = unchecked((char)current);
        if (char.IsHighSurrogate(ch1))
        {
            current = input_reader.Read();
            if (current == -1)
            {
                ThrowUnicode(position);
            }
            char ch2 = unchecked((char)current);
            current = char.ConvertToUtf32(ch1, ch2);
            ++position;
        }
    }
}