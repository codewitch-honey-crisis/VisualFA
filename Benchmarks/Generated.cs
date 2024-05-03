
using System;
using System.Transactions;

using VisualFA;
#if FALIB
	public
#endif


partial class BenchFAStringRunner : FAStringRunner
{
	public override FAMatch NextMatch()
	{
		return _NextImpl(input_string);
	}
	private FAMatch _NextImpl(
#if FALIB_SPANS
		ReadOnlySpan<char> s
#else
		string s
#endif
		)
	{

		int ch = -1;
		int len = 0;
		if (position < 0)
		{
			position = 0; // first read
		}
		long p = position;
		int l = line;
		int c = column;

		Advance(s, ref ch, ref len, true);

		//q0:
		if (((((ch >= 9)
					&& (ch <= 10))
					|| (ch == 13))
					|| (ch == 32)))
		{
			Advance(s, ref ch, ref len, false);
			goto q1;
		}
		if ((ch == 45))
		{
			Advance(s, ref ch, ref len, false);
			goto q2;
		}
		if ((ch == 48))
		{
			Advance(s, ref ch, ref len, false);
			goto q9;
		}
		if (((ch >= 49)
					&& (ch <= 57)))
		{
			Advance(s, ref ch, ref len, false);
			goto q3;
		}
		if (((((ch >= 65)
					&& (ch <= 90))
					|| (ch == 95))
					|| ((ch >= 97)
					&& (ch <= 122))))
		{
			Advance(s, ref ch, ref len, false);
			goto q10;
		}
		goto errorout;
		q1:
		if (((((ch >= 9)
					&& (ch <= 10))
					|| (ch == 13))
					|| (ch == 32)))
		{
			Advance(s, ref ch, ref len, false);
			goto q1;
		}
		return FAMatch.Create(2,
#if FALIB_SPANS
			s.Slice(unchecked((int)p), len).ToString()
#else
			s.Substring(unchecked((int)p),len)
#endif
			, p, l, c);
		q2:
		if (((ch >= 49)
					&& (ch <= 57)))
		{
			Advance(s, ref ch, ref len, false);
			goto q3;
		}
		goto errorout;
		q3:
		if ((ch == 46))
		{
			Advance(s, ref ch, ref len, false);
			goto q4;
		}
		if (((ch >= 48)
					&& (ch <= 57)))
		{
			Advance(s, ref ch, ref len, false);
			goto q3;
		}
		if (((ch == 69)
					|| (ch == 101)))
		{
			Advance(s, ref ch, ref len, false);
			goto q6;
		}
		return FAMatch.Create(1,
#if FALIB_SPANS
			s.Slice(unchecked((int)p), len).ToString()
#else
			s.Substring(unchecked((int)p), len)
#endif
			, p, l, c);
		q4:
		if (((ch >= 48)
					&& (ch <= 57)))
		{
			Advance(s, ref ch, ref len, false);
			goto q5;
		}
		goto errorout;
		q5:
		if (((ch >= 48)
					&& (ch <= 57)))
		{
			Advance(s, ref ch, ref len, false);
			goto q5;
		}
		if (((ch == 69)
					|| (ch == 101)))
		{
			Advance(s, ref ch, ref len, false);
			goto q6;
		}
		return FAMatch.Create(1,
#if FALIB_SPANS
			s.Slice(unchecked((int)p), len).ToString()
#else
			s.Substring(unchecked((int)p), len)
#endif
			, p, l, c);
		q6:
		if ((ch == 45))
		{
			Advance(s, ref ch, ref len, false);
			goto q7;
		}
		if (((ch >= 49)
					&& (ch <= 57)))
		{
			Advance(s, ref ch, ref len, false);
			goto q8;
		}
		goto errorout;
		q7:
		if (((ch >= 49)
					&& (ch <= 57)))
		{
			Advance(s, ref ch, ref len, false);
			goto q8;
		}
		goto errorout;
		q8:
		if (((ch >= 48)
					&& (ch <= 57)))
		{
			Advance(s, ref ch, ref len, false);
			goto q8;
		}
		return FAMatch.Create(1,
#if FALIB_SPANS
			s.Slice(unchecked((int)p), len).ToString()
#else
			s.Substring(unchecked((int)p), len)
#endif
			, p, l, c);
		q9:
		return FAMatch.Create(1,
#if FALIB_SPANS
			s.Slice(unchecked((int)p), len).ToString()
#else
			s.Substring(unchecked((int)p), len)
#endif
			, p, l, c);
		q10:
		if ((((((ch >= 48)
					&& (ch <= 57))
					|| ((ch >= 65)
					&& (ch <= 90)))
					|| (ch == 95))
					|| ((ch >= 97)
					&& (ch <= 122))))
		{
			Advance(s, ref ch, ref len, false);
			goto q10;
		}
		return FAMatch.Create(0,
#if FALIB_SPANS
			s.Slice(unchecked((int)p), len).ToString()
#else
			s.Substring(unchecked((int)p), len)
#endif
			, p, l, c);
		errorout:
		if (((((((((((ch == -1)
					|| ((ch >= 9)
					&& (ch <= 10)))
					|| (ch == 13))
					|| (ch == 32))
					|| (ch == 45))
					|| (ch == 48))
					|| ((ch >= 49)
					&& (ch <= 57)))
					|| ((ch >= 65)
					&& (ch <= 90)))
					|| (ch == 95))
					|| ((ch >= 97)
					&& (ch <= 122))))
		{
			if (len == 0)
			{
				return FAMatch.Create(-2, null, 0, 0, 0);
			}
			return FAMatch.Create(-1,
#if FALIB_SPANS
			s.Slice(unchecked((int)p), len).ToString()
#else
			s.Substring(unchecked((int)p), len)
#endif
				, p, l, c);
		}

		if (position < s.Length)
		{
			Advance(s, ref ch, ref len, false);
		}
		else
		{
			return FAMatch.Create(-1,
#if FALIB_SPANS
			s.Slice(unchecked((int)p), len).ToString()
#else
			s.Substring(unchecked((int)p), len)
#endif
				, p, l, c);
		}
		goto errorout;
	}
}
partial class BenchFATextReaderRunner : FATextReaderRunner
{
	public override FAMatch NextMatch()
	{

		capture.Clear();
		if (current == -2)
		{
			Advance();
		}
		long p = position;
		int l = line;
		int c = column;

		//q0:
		if (((((current >= 9)
					&& (current <= 10))
					|| (current == 13))
					|| (current == 32)))
		{
			Advance();
			goto q1;
		}
		if ((current == 45))
		{
			Advance();
			goto q2;
		}
		if ((current == 48))
		{
			Advance();
			goto q9;
		}
		if (((current >= 49)
					&& (current <= 57)))
		{
			Advance();
			goto q3;
		}
		if (((((current >= 65)
					&& (current <= 90))
					|| (current == 95))
					|| ((current >= 97)
					&& (current <= 122))))
		{
			Advance();
			goto q10;
		}
		goto errorout;
		q1:
		if (((((current >= 9)
					&& (current <= 10))
					|| (current == 13))
					|| (current == 32)))
		{
			Advance();
			goto q1;
		}
		return FAMatch.Create(2, capture.ToString(), p, l, c);
		q2:
		if (((current >= 49)
					&& (current <= 57)))
		{
			Advance();
			goto q3;
		}
		goto errorout;
		q3:
		if ((current == 46))
		{
			Advance();
			goto q4;
		}
		if (((current >= 48)
					&& (current <= 57)))
		{
			Advance();
			goto q3;
		}
		if (((current == 69)
					|| (current == 101)))
		{
			Advance();
			goto q6;
		}
		return FAMatch.Create(1, capture.ToString(), p, l, c);
		q4:
		if (((current >= 48)
					&& (current <= 57)))
		{
			Advance();
			goto q5;
		}
		goto errorout;
		q5:
		if (((current >= 48)
					&& (current <= 57)))
		{
			Advance();
			goto q5;
		}
		if (((current == 69)
					|| (current == 101)))
		{
			Advance();
			goto q6;
		}
		return FAMatch.Create(1, capture.ToString(), p, l, c);
		q6:
		if ((current == 45))
		{
			Advance();
			goto q7;
		}
		if (((current >= 49)
					&& (current <= 57)))
		{
			Advance();
			goto q8;
		}
		goto errorout;
		q7:
		if (((current >= 49)
					&& (current <= 57)))
		{
			Advance();
			goto q8;
		}
		goto errorout;
		q8:
		if (((current >= 48)
					&& (current <= 57)))
		{
			Advance();
			goto q8;
		}
		return FAMatch.Create(1, capture.ToString(), p, l, c);
		q9:
		return FAMatch.Create(1, capture.ToString(), p, l, c);
		q10:
		if ((((((current >= 48)
					&& (current <= 57))
					|| ((current >= 65)
					&& (current <= 90)))
					|| (current == 95))
					|| ((current >= 97)
					&& (current <= 122))))
		{
			Advance();
			goto q10;
		}
		return FAMatch.Create(0, capture.ToString(), p, l, c);
		errorout:
		if (((((((((((current == -1)
					|| ((current >= 9)
					&& (current <= 10)))
					|| (current == 13))
					|| (current == 32))
					|| (current == 45))
					|| (current == 48))
					|| ((current >= 49)
					&& (current <= 57)))
					|| ((current >= 65)
					&& (current <= 90)))
					|| (current == 95))
					|| ((current >= 97)
					&& (current <= 122))))
		{
			if (capture.Length == 0)
			{
				return FAMatch.Create(-2, null, 0, 0, 0);
			}
			return FAMatch.Create(-1, capture.ToString(), p, l, c);
		}
		Advance();
		if (current == -1)
		{
			return FAMatch.Create(-1, capture.ToString(), p, l, c);
		}
		goto errorout;
	}
}
partial class CommentFAStringRunner : FAStringRunner
{
	public override FAMatch NextMatch()
	{
		return _NextImpl(input_string);
	}
	private FAMatch _BlockEnd0(
#if FALIB_SPANS
		ReadOnlySpan<char> s
#else
		string s
#endif
		, int cp, int len, int p, int l, int c)
	{
		int ch = cp;
		if (ch == -1)
		{
			return FAMatch.Create(-1,
#if FALIB_SPANS
			s.Slice(unchecked((int)p), len).ToString()
#else
			s.Substring(unchecked((int)p), len)
#endif
				, p, l, c);
		}
		q0:
		if (ch == '*')
		{
			Advance(s, ref ch, ref len, false);
			goto q1;
		}
		goto errorout;
		q1:
		if (ch == '/')
		{
			Advance(s, ref ch, ref len, false);
			goto q2;
		}
		goto errorout;
		q2:
		return FAMatch.Create(0,
#if FALIB_SPANS
			s.Slice(unchecked((int)p), len).ToString()
#else
			s.Substring(unchecked((int)p), len)
#endif
			, p, l, c);
		errorout:
		Advance(s, ref ch, ref len, false);
		if (ch == -1)
		{
			return FAMatch.Create(-1,
#if FALIB_SPANS
			s.Slice(unchecked((int)p), len).ToString()
#else
			s.Substring(unchecked((int)p), len)
#endif
				, p, l, c);
		}
		goto q0;
	}
	private FAMatch _NextImpl(
#if FALIB_SPANS
		ReadOnlySpan<char> s
#else
		string s
#endif
		)
	{

		int ch = -1;
		int len = 0;
		if (position < 0)
		{
			position = 0; // first read
		}
		int p = position;
		int l = line;
		int c = column;

		Advance(s, ref ch, ref len, true);

		//q0:
		if ((ch == '/'))

		{
			Advance(s, ref ch, ref len, false);
			goto q1;
		}

		goto errorout;
		q1:
		if ((ch == '/'))
		{
			Advance(s, ref ch, ref len, false);
			goto q3;
		}
		if ((ch == '*'))
		{
			Advance(s, ref ch, ref len, false);
			goto q2;
		}
		goto errorout;
		q2:
		return _BlockEnd0(s, ch, len, p, l, c);
		q3:
		if (ch!=-1 && ((ch != '\r') && (ch != '\n')))
		{
			Advance(s, ref ch, ref len, false);
			goto q3;
		}
		return FAMatch.Create(1,
#if FALIB_SPANS
			s.Slice(unchecked((int)p), len).ToString()
#else
			s.Substring(unchecked((int)p), len)
#endif
			, p, l, c);
		errorout:
		if ((ch == -1) ||(ch=='/'))
		{
			if (len == 0)
			{
				return FAMatch.Create(-2, null, 0, 0, 0);
			}
			return FAMatch.Create(-1,
#if FALIB_SPANS
			s.Slice(unchecked((int)p), len).ToString()
#else
			s.Substring(unchecked((int)p), len)
#endif
, p, l, c);
		}

		if (position < s.Length)
		{
			Advance(s, ref ch, ref len, false);
		}
		else
		{
			return FAMatch.Create(-1,
#if FALIB_SPANS
			s.Slice(unchecked((int)p), len).ToString()
#else
			s.Substring(unchecked((int)p), len)
#endif
				, p, l, c);
		}
		goto errorout;
	}
}
partial class CommentFATextRunner : FATextReaderRunner
{
	public override FAMatch NextMatch()
	{
		capture.Clear();
		if (current == -2)
		{
			Advance();
		}
		int ch;
		int p = position;
		int l = line;
		int c = column;
		//q0:
		if ((current == '/'))

		{
			Advance();
			goto q1;
		}

		goto errorout;
		q1:
		if ((current == '/'))
		{
			Advance();
			goto q3;
		}
		if ((current == '*'))
		{
			Advance();
			goto q2;
		}
		goto errorout;
		q2:
		return _BlockEnd0(p, l, c);
		q3:
		if (current!=-1 && ((current != '\r') && (current != '\n')))
		{
			Advance();
			goto q3;
		}
		return FAMatch.Create(1, capture.ToString(), p, l, c);
		errorout:
		ch = current;
		if (ch== -1)
		{
			if (capture.Length == 0)
			{
				return FAMatch.Create(-2, null, 0, 0, 0);
			}
			return FAMatch.Create(-1, capture.ToString(), p, l, c);
		}
		Advance();
		ch= current;
		goto errorout;
	}
	private FAMatch _BlockEnd0(int p, int l, int c)
	{
		q0:
		if (current == '*')
		{
			Advance();
			goto q1;
		}
		goto errorout;
		q1:
		if (current == '/')
		{
			Advance();
			goto q2;
		}
		goto errorout;
		q2:
		return FAMatch.Create(0, capture.ToString(), p, l, c);
		errorout:
		if (current == -1)
		{
			return FAMatch.Create(-1, capture.ToString(), p, l, c);
		}
		Advance();

		goto q0;
	}
	
}