abstract partial class FAStringRunner : FARunner
{
    public static readonly bool UsingSpans = false;
    protected string @string = "";
    public void Set(string @string)
    {
        if (@string == null) throw new ArgumentNullException(nameof(@string));
        this.@string = @string;
        position = -1;
        line = 1;
        column = 1;
    }
    public override void Reset()
    {
        position = -1;
        line = 1;
        column = 1;
    }
    // much bigger, but faster code
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    protected void Advance(
			string s
        , ref int ch, ref int len, bool first)
    {
        if (!first)
        {
            switch (ch)
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
                    if (ch > 31)
                    {
                        ++column;
                    }
                    break;
            }
            ++len;
            if (ch > 65535)
            {
                ++position;
                ++len;
            }
            ++position;
        }
        if (position < s.Length)
        {
            char ch1 = s[position];
            if (char.IsHighSurrogate(ch1))
            {
                ++position;
                if (position >= s.Length)
                {
                    ThrowUnicode(position);
                }
                char ch2 = s[position];
                ch = char.ConvertToUtf32(ch1, ch2);
            }
            else
            {
                ch = ch1;
            }
        }
        else
        {
            ch = -1;
        }
    }
}