partial class FATextReaderDfaTableRunner : FATextReaderRunner
{
    private readonly int[] _dfa;
    private readonly int[][]? _blockEnds;
    public FATextReaderDfaTableRunner(int[] dfa, int[][]? blockEnds = null)
    {
        _dfa = dfa;
        _blockEnds = blockEnds;
    }
    public override FAMatch NextMatch()
    {
        int tlen;
        int tto;
        int prlen;
        int pmin;
        int pmax;
        int i;
        int j;
        int state = 0;
        int acc;
        capture.Clear();
        if (current == -2)
        {
            Advance();
        }
        int len = 0;
        long cursor_pos = position;
        int line = this.line;
        int column = this.column;
    start_dfa:
        acc = _dfa[state];
        ++state;
        tlen = _dfa[state];
        ++state;
        for (i = 0; i < tlen; ++i)
        {
            tto = _dfa[state];
            ++state;
            prlen = _dfa[state];
            ++state;
            for (j = 0; j < prlen; ++j)
            {
                pmin = _dfa[state];
                ++state;
                pmax = _dfa[state];
                ++state;
                if (current < pmin)
                {
                    state += ((prlen - (j + 1)) * 2);
                    j = prlen;
                }
                else if (current <= pmax)
                {
                    Advance();
                    state = tto;
                    goto start_dfa;
                }
            }
        }
        if (acc != -1)
        {
            int sym = acc;
            int[]? be = (_blockEnds != null && _blockEnds.Length > acc) ? _blockEnds[acc] : null;
            if (be != null)
            {
                state = 0;
            start_be_dfa:
                acc = be[state];
                ++state;
                tlen = be[state];
                ++state;
                for (i = 0; i < tlen; ++i)
                {
                    tto = be[state];
                    ++state;
                    prlen = be[state];
                    ++state;
                    for (j = 0; j < prlen; ++j)
                    {
                        pmin = be[state];
                        ++state;
                        pmax = be[state];
                        ++state;
                        if (current < pmin)
                        {
                            state += ((prlen - (j + 1)) * 2);
                            j = prlen;
                        }
                        else if (current <= pmax)
                        {
                            Advance();
                            state = tto;
                            goto start_be_dfa;
                        }
                    }
                }
                if (acc != -1)
                {
                    return FAMatch.Create(sym, capture.ToString(), cursor_pos, line, column);
                }
                if (current == -1)
                {
                    return FAMatch.Create(-1, capture.ToString(), cursor_pos, line, column);
                }
                Advance();
                state = 0;
                goto start_be_dfa;
            }
            return FAMatch.Create(acc, capture.ToString(), cursor_pos, line, column);
        }
        // error. keep trying until we find a potential transition.
        while (current != -1)
        {
            var moved = false;
            state = 1;
            tlen = _dfa[state];
            ++state;
            for (i = 0; i < tlen; ++i)
            {
                ++state;
                prlen = _dfa[state];
                ++state;
                for (j = 0; j < prlen; ++j)
                {
                    pmin = _dfa[state];
                    ++state;
                    pmax = _dfa[state];
                    ++state;
                    if (current < pmin)
                    {
                        state += ((prlen - (j + 1)) * 2);
                        j = prlen;
                    }
                    else if (current <= pmax)
                    {
                        moved = true;
                    }
                }
            }
            if (moved)
            {
                break;
            }
            Advance();
        }
        if (len == 0)
        {
            return FAMatch.Create(-2, null, 0, 0, 0);
        }
        return FAMatch.Create(-1, capture.ToString(), cursor_pos, line, column);
    }
}