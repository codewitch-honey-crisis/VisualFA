//HintName: FARunnerShared.g.cs
namespace Tests
{

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
        public string? Value;
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
        public static FAMatch Create(int symbolId, string? value, long position, int line, int column)
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
    abstract partial class FARunner : IEnumerable<FAMatch>
    {
        protected internal FARunner()
        {
            position = -1;
            line = 1;
            column = 1;
            tabWidth = 4;
        }
        public sealed class Enumerator : IEnumerator<FAMatch>
        {
            int _state;
            FAMatch _current;
            WeakReference<FARunner> _parent;
            public Enumerator(FARunner parent)
            {
                _parent = new WeakReference<FARunner>(parent);
                _state = -2;
            }
            public FAMatch Current
            {
                get
                {
                    if (_state == -3)
                    {
                        throw new ObjectDisposedException(nameof(Enumerator));
                    }
                    if (_state < 0)
                    {
                        throw new InvalidOperationException("The enumerator is not positioned on an element");
                    }
                    return _current;
                }
            }

            object System.Collections.IEnumerator.Current { get { return Current; } }
            void IDisposable.Dispose() { _state = -3; }
            public bool MoveNext()
            {
                if (_state == -3)
                {
                    throw new ObjectDisposedException(nameof(Enumerator));
                }
                if (_state == -1)
                {
                    return false;
                }
                _state = 0;
                FARunner parent;
                if (!_parent.TryGetTarget(out parent))
                {
                    throw new InvalidOperationException("The parent was destroyed");
                }
                _current = parent.NextMatch();
                if (_current.SymbolId == -2)
                {
                    _state = -2;
                    return false;
                }
                return true;
            }
            public void Reset()
            {
                if (_state == -3)
                {
                    throw new ObjectDisposedException(nameof(Enumerator));
                }
                FARunner parent;
                if (!_parent.TryGetTarget(out parent))
                {
                    throw new InvalidOperationException("The parent was destroyed");
                }
                parent.Reset();
                _state = -2;
            }
        }
        /// <summary>
        /// Indicates the width of a tab, in columns
        /// </summary>
        public int TabWidth
        {
            get
            {
                return tabWidth;
            }
            set
            {
                if (value < 1) { throw new ArgumentOutOfRangeException(); }
                tabWidth = value;
            }
        }
        protected int tabWidth;
        protected int position;
        protected int line;
        protected int column;
        protected static void ThrowUnicode(int pos)
        {
            throw new IOException("Unicode error in stream at position " + pos.ToString());
        }

        public abstract FAMatch NextMatch();
        public abstract void Reset();
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }
        IEnumerator<FAMatch> IEnumerable<FAMatch>.GetEnumerator() { return GetEnumerator(); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
    abstract partial class FATextReaderRunner : FARunner
    {
        protected TextReader reader = TextReader.Null;
        protected System.Text.StringBuilder capture = new System.Text.StringBuilder();
        protected int current = -2;
        public void Set(TextReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            this.reader = reader;
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
            current = reader.Read();
            if (current == -1)
            {
                return;
            }
            ++position;
            char ch1 = unchecked((char)current);
            if (char.IsHighSurrogate(ch1))
            {
                current = reader.Read();
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

}
