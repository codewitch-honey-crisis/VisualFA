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