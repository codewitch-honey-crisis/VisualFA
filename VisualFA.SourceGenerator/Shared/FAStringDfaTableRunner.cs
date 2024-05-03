partial class FAStringDfaTableRunner : FAStringRunner
{
    private readonly int[] _dfa;
    private readonly int[][] _blockEnds;
    public FAStringDfaTableRunner(int[] dfa, int[][] blockEnds = null)
    {
        _dfa = dfa;
        _blockEnds = blockEnds;
    }
    public override FAMatch NextMatch()
    {
        return _NextImpl(input_string);
    }
    private FAMatch _NextImpl(
			string s
        )
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
        if (position == -1)
        {
            // first read
            ++position;
        }
        int len = 0;
        long cursor_pos = position;
        int line = this.line;
        int column = this.column;
        int ch = -1;
        Advance(s, ref ch, ref len, true);
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
                if (ch < pmin)
                {
                    state += ((prlen - (j + 1)) * 2);
                    j = prlen;
                }
                else if (ch <= pmax)
                {
                    Advance(s, ref ch, ref len, false);
                    state = tto;
                    goto start_dfa;
                }
            }
        }
        if (acc != -1)
        {
            int sym = acc;
            int[] be = (_blockEnds != null && _blockEnds.Length > acc) ? _blockEnds[acc] : null;
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
                        if (ch < pmin)
                        {
                            state += ((prlen - (j + 1)) * 2);
                            j = prlen;
                        }
                        else if (ch <= pmax)
                        {
                            Advance(s, ref ch, ref len, false);
                            state = tto;
                            goto start_be_dfa;
                        }
                    }
                }
                if (acc != -1)
                {
                    return FAMatch.Create(sym,
							s.Substring(unchecked((int)cursor_pos), len)
                        , cursor_pos, line, column);
                }
                if (ch == -1)
                {
                    return FAMatch.Create(-1,
							s.Substring(unchecked((int)cursor_pos), len)
                        , cursor_pos, line, column);
                }
                Advance(s, ref ch, ref len, false);
                state = 0;
                goto start_be_dfa;
            }
            return FAMatch.Create(acc,
							s.Substring(unchecked((int)cursor_pos), len)
                , cursor_pos, line, column);
        }
        // error. keep trying until we find a potential transition.
        while (ch != -1)
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
                    if (ch < pmin)
                    {
                        state += ((prlen - (j + 1)) * 2);
                        j = prlen;
                    }
                    else if (ch <= pmax)
                    {
                        moved = true;
                    }
                }
            }
            if (moved)
            {
                break;
            }
            Advance(s, ref ch, ref len, false);
        }
        if (len == 0)
        {
            return FAMatch.Create(-2, null, 0, 0, 0);
        }
        return FAMatch.Create(-1,
							s.Substring(unchecked((int)cursor_pos), len)
            , cursor_pos, line, column);
    }
}