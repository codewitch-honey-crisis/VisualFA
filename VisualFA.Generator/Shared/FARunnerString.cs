using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.IO;
using System.Text;

internal abstract partial class FARunner : Object, IEnumerable<FAMatch>
{
	protected internal FARunner()
	{
		position = -1;
		line = 1;
		column = 1;
		_tabWidth = 4;
	}
	public class Enumerator : Object, IEnumerator<FAMatch>
	{
		private int _state;
		private FAMatch _current;
		private WeakReference<FARunner> _parent;
		public Enumerator(FARunner parent)
		{
			_parent = new WeakReference<FARunner>(parent);
			_state = -2;
		}
		public FAMatch Current {
			get {
				if (_state == -3)
				{
					throw new ObjectDisposedException("Enumerator");
				}
				if (_state < 0)
				{
					throw new InvalidOperationException("The enumerator is not positioned on an element");
				}
				return _current;
			}
		}
		bool System.Collections.IEnumerator.MoveNext() { return MoveNext(); }
		object System.Collections.IEnumerator.Current { get { return Current; } }
		void System.Collections.IEnumerator.Reset() { Reset(); }
		void IDisposable.Dispose() { _state = -3; }
		public bool MoveNext()
		{
			if (_state == -3)
			{
				throw new ObjectDisposedException("Enumerator");
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
				throw new ObjectDisposedException("Enumerator");
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
	public int TabWidth {
		get {
			return _tabWidth;
		}
		set {
			if (value < 1) { throw new ArgumentOutOfRangeException(); }
			_tabWidth = value;
		}
	}
	private int _tabWidth;
	protected int position;
	protected int line;
	protected int column;
	protected static void ThrowUnicode(int pos)
	{
		throw new IOException(string.Concat("Unicode error in stream at position ", pos.ToString()));
	}

	public abstract FAMatch NextMatch();
	public abstract void Reset();
	public IEnumerator<FAMatch> GetEnumerator() { return new Enumerator(this); }
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return new Enumerator(this); }
}

internal abstract partial class FAStringRunner : FARunner
{
	public static bool UsingSpans { get { return false; } }
	protected string @string;
	public void Set(string @string)
	{
		this.@string = @string;
		position = -1;
		line = 1;
		column = 0;
	}
	public override void Reset()
	{
		position = -1;
		line = 1;
		column = 0;
	}
	// much bigger, but faster code
	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	protected void Advance(string str, ref int ch, ref int len, bool first)
	{
		if (!first)
		{
			++len;
			if (ch > 65535)
			{
				++len;
			}
			++position;
		}
		if (position < str.Length)
		{
			char ch1 = str[position];
			if (char.IsHighSurrogate(ch1))
			{
				++position;
				if (position >= str.Length)
				{
					ThrowUnicode(position);
				}
				char ch2 = str[position];
				ch = char.ConvertToUtf32(ch1, ch2);
			}
			else
			{
				ch = System.Convert.ToInt32(ch1);
			}
			if (!first)
			{
				if (ch == 10)
				{
					++line;
					column = 0;
				}
				else if (ch == 13)
				{
					column = 0;
				}
				else if (ch == 9)
				{

					column = ((column - 1) / TabWidth) * (TabWidth + 1);
				}
				else if (ch > 31)
				{
					++column;
				}
			}
		}
		else
		{
			ch = -1;
		}
	}
}
internal abstract partial class FATextReaderRunner : FARunner
{
	protected TextReader reader;
	protected StringBuilder capture;
	protected int current;
	protected FATextReaderRunner()
	{
		capture = new StringBuilder();
	}
	public void Set(TextReader reader)
	{
		this.reader = reader;
		current = -2;
		position = -1;
		line = 1;
		column = 0;
	}
	public override void Reset()
	{
		throw new NotSupportedException();
	}
	protected void Advance()
	{
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
		char ch1 = Convert.ToChar(current);
		if (char.IsHighSurrogate(ch1))
		{
			current = reader.Read();
			if (current == -1)
			{
				ThrowUnicode(position);
			}
			char ch2 = Convert.ToChar(current);
			current = char.ConvertToUtf32(ch1, ch2);
			++position;
		}
		if (current == 10)
		{
			++line;
			column = 0;
		}
		else if (current == 13)
		{
			column = 0;
		}
		else if (current == 9)
		{

			column = ((column - 1) / TabWidth) * (TabWidth + 1);
		}
		else if (current > 31)
		{
			++column;
		}
	}
}

