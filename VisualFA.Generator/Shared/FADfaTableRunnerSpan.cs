using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.IO;
using System.Text;

internal partial class FAStringDfaTableRunner : FAStringRunner
{
	private int[] _dfa;
	private int[][] _blockEnds;
	public FAStringDfaTableRunner(int[] dfa)
	{
		_dfa = dfa;
		_blockEnds = null;
	}
	public FAStringDfaTableRunner(int[] dfa, int[][] blockEnds)
	{
		_dfa = dfa;
		_blockEnds = blockEnds;
	}
	public override FAMatch NextMatch()
	{
		return _NextImpl(@string);
	}
	private FAMatch _NextImpl(ReadOnlySpan<char> span)
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
		Advance(span, ref ch, ref len, true);
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
					Advance(span, ref ch, ref len, false);
					state = tto;
					goto start_dfa;
				}
			}
		}
		if (acc != -1)
		{
			int sym = acc;
			int[] be = null;
			if (_blockEnds != null && _blockEnds.Length > acc)
			{
				be = _blockEnds[acc];
			}
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
							Advance(span, ref ch, ref len, false);
							state = tto;
							goto start_be_dfa;
						}
					}
				}
				if (acc != -1)
				{
					return FAMatch.Create(sym, span.Slice((int)cursor_pos, len).ToString(), cursor_pos, line, column);
				}
				if (ch == -1)
				{
					return FAMatch.Create(-1, span.Slice((int)cursor_pos, len).ToString(), cursor_pos, line, column);
				}
				Advance(span, ref ch, ref len, false);
				state = 0;
				goto start_be_dfa;
			}
			return FAMatch.Create(acc, span.Slice((int)cursor_pos, len).ToString(), cursor_pos, line, column);
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
				goto break_loop;
			}
			Advance(span, ref ch, ref len, false);
		}
		break_loop:
		if (len == 0)
		{
			return FAMatch.Create(-2, null, 0, 0, 0);
		}
		return FAMatch.Create(-1, span.Slice((int)cursor_pos, len).ToString(), cursor_pos, line, column);
	}
}
internal partial class FATextReaderDfaTableRunner : FATextReaderRunner
{
	private int[] _dfa;
	private int[][] _blockEnds;
	public FATextReaderDfaTableRunner(int[] dfa)
	{
		_dfa = dfa;
		_blockEnds = null;
	}
	public FATextReaderDfaTableRunner(int[] dfa, int[][] blockEnds)
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
			int[] be = null;
			if (_blockEnds != null && _blockEnds.Length > acc)
			{
				be = _blockEnds[acc];
			}
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
			bool moved = false;
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
				goto break_loop;
			}
			Advance();
		}
		break_loop:
		if (len == 0)
		{
			return FAMatch.Create(-2, null, 0, 0, 0);
		}
		return FAMatch.Create(-1, capture.ToString(), cursor_pos, line, column);
	}
}
