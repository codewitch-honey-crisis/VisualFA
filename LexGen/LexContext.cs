using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LexGen
{
	/// <summary>
	/// Represents an exception encountered while lexing or parsing from an input source
	/// </summary>
#if LCLIB
	public 
#endif
	sealed class ExpectingException : Exception
	{
		/// <summary>
		/// Creates a new <see cref="ExpectingException" />
		/// </summary>
		/// <param name="message">The error message - this will be appended with the location information</param>
		/// <param name="line">The one based line number where the exception occured</param>
		/// <param name="column">The one based column number where the exception occured</param>
		/// <param name="position">The zero based position where the exception occured</param>
		/// <param name="fileOrUrl">The file or URL in which the exception occured</param>
		/// <param name="expecting">A list of expected symbols or characters</param>
		public ExpectingException(string message, int line, int column,long index, long position,string fileOrUrl, params string[] expecting) : base(
			_GetMessage(message,line,column,position,fileOrUrl))
		{
			Description = message;
			Line = line;
			Column = column;
			Index = index;
			Position = position;
			FileOrUrl = fileOrUrl;
			Expecting = expecting;
		}
		/// <summary>
		/// Indicates the message without the position information
		/// </summary>
		public string Description { get; }
		/// <summary>
		/// Indicates the one based line where the exception occured
		/// </summary>
		public int Line { get; }
		/// <summary>
		/// Indicates the one based column where the exception occured
		/// </summary>
		public int Column { get; }
		/// <summary>
		/// Indicates the zero based character index where the exception occured
		/// </summary>
		public long Index { get; }
		/// <summary>
		/// Indicates the zero based position where the exception occured
		/// </summary>
		public long Position { get; }
		/// <summary>
		/// Indicates the file or URL in which the exception occured
		/// </summary>
		public string FileOrUrl { get; }
		/// <summary>
		/// Indicates a list of expecting characters or symbols
		/// </summary>
		public string[] Expecting { get; }
		static string _GetMessage(string m,int l,int c,long p,string f)
		{
			var sb = new StringBuilder();
			sb.Append(m);
			sb.Append(" at line ");
			sb.Append(l.ToString());
			sb.Append(", column ");
			sb.Append(c.ToString());
			sb.Append(", position ");
			sb.Append(p.ToString());
			if (!string.IsNullOrEmpty(f))
			{
				sb.Append(", in ");
				sb.Append(f);
			}
			return sb.ToString();
		}
	}
	/// <summary>
	/// Provides error reporting, location tracking, lifetime and start/end management over an input cursor
	/// </summary>
	public abstract partial class LexContext : IDisposable
	{
		/// <summary>
		/// Indicates the default tab width of an input device
		/// </summary>
		public const int DefaultTabWidth = 4;
		/// <summary>
		/// Represents the end of input symbol
		/// </summary>
		public const int EndOfInput = -1;
		/// <summary>
		/// Represents the before input symbol
		/// </summary>
		public const int BeforeInput = -2;
		/// <summary>
		/// Represents a symbol for the disposed state
		/// </summary>
		public const int Disposed = -3;

		int _current = BeforeInput;
		int _line;
		int _column;
		long _index;
		long _position;
		string _fileOrUrl;
		StringBuilder _captureBuffer = new StringBuilder();
		/// <summary>
		/// Indicates the tab width of the input device
		/// </summary>
		public int TabWidth { get; set; } = DefaultTabWidth;
		/// <summary>
		/// Indicates the current one based line number
		/// </summary>
		public int Line { get { return _line;  } }
		/// <summary>
		/// Indicates the current one based column number
		/// </summary>
		public int Column { get { return _column; } }
		/// <summary>
		/// Indicates the current zero based characer index
		/// </summary>
		/// <remarks>This may be different than the position due to Unicode surrogates</remarks>
		public long Index { get { return _index; } }
		/// <summary>
		/// Indicates the current zero based position
		/// </summary>
		public long Position { get { return _position; } }
		/// <summary>
		/// Indicates the current filename or URL, if any could be discerned
		/// </summary>
		public string FileOrUrl { get { return _fileOrUrl; } }
		/// <summary>
		/// Provides access to the capture buffer, a <see cref="StringBuilder" />
		/// </summary>
		public StringBuilder CaptureBuffer { get { return _captureBuffer; } }
		/// <summary>
		/// Gets the current character under the cursor or <see cref="BeforeInput"/>, <see cref="EndOfInput" />, or <see cref="Disposed" />
		/// </summary>
		public int Current {  get { return _current; } }
		internal LexContext()
		{
			_line = 1;
			_column = 0;
			_index = -1L;
			_position = -1L;
		}
		~LexContext()
 		{
		   Close();
		}
		/// <summary>
		/// Creates a <see cref="LexContext" /> over an enumeration of characters, which can be a string, character array, or other source
		/// </summary>
		/// <param name="input">The input characters</param>
		/// <returns>A new <see cref="LexContext" /> over the input</returns>
		public static LexContext Create(IEnumerable<char> input)
		{
			return new _CharEnumeratorLexContext(input);
		}
		/// <summary>
		/// Creates a <see cref="LexContext" /> over a <see cref="TextReader"/>
		/// </summary>
		/// <param name="input">The input reader</param>
		/// <returns>A new <see cref="LexContext" /> over the input</returns>
		public static LexContext CreateFrom(TextReader input)
		{
			// try to get a filename off the text reader
			string fn = null;
			var sr = input as StreamReader;
			if(null!=sr)
			{
				var fstm = sr.BaseStream as FileStream;
				if(null!=fstm)
				{
					fn = fstm.Name;
				}
			}
			var result = new _TextReaderLexContext(input);
			if (!string.IsNullOrEmpty(fn))
				result._fileOrUrl = fn;
			return result;
		}
		/// <summary>
		/// Creates a <see cref="LexContext" /> over a file
		/// </summary>
		/// <param name="filename">The file</param>
		/// <returns>A new <see cref="LexContext" /> over the file</returns>
		public static LexContext CreateFrom(string filename) {
			return CreateFrom(new StreamReader(filename));
		}
		/// <summary>
		/// Closes the current instance and releases any resources being held
		/// </summary>
		public void Close()
		{
			if (Disposed != _current)
			{
				_current = Disposed;
				GC.SuppressFinalize(this);
				CloseInner();
				CaptureBuffer.Clear();
			}
		}
		/// <summary>
		/// Sets the location information for this instance
		/// </summary>
		/// <param name="line">The one based line number</param>
		/// <param name="column">The one based column number</param>
		/// <param name="position">The zero based position</param>
		/// <param name="fileOrUrl">The file or URL</param>
		public void SetLocation(int line, int column, long index, long position, string fileOrUrl)
		{
			_line = line;
			_column = column;
			_index = index;
			_position = position;
			_fileOrUrl = fileOrUrl;
		}
		/// <summary>
		/// Gets all or a subset of the current capture buffer
		/// </summary>
		/// <param name="startIndex">The start index</param>
		/// <param name="length">The number of characters to retrieve, or zero to retrieve the remainder of the buffer</param>
		/// <returns>A string containing the specified subset of the capture buffer</returns>
		public string GetCapture(int startIndex = 0,int length = 0)
		{
			_CheckDisposed();
			if (0 == length)
				length = CaptureBuffer.Length - startIndex;
			return CaptureBuffer.ToString(startIndex, length);
		}
		/// <summary>
		/// Clears the capture buffer
		/// </summary>
		public void ClearCapture()
		{
			_CheckDisposed();
			CaptureBuffer.Clear();
		}
		/// <summary>
		/// Captures the current character under the cursor, if any
		/// </summary>
		public void Capture()
		{
			_CheckDisposed();
			if (_current > 0)
			{
				CaptureBuffer.Append(char.ConvertFromUtf32(_current));
			}
		}
		/// <summary>
		/// Verifies that one of the specified characters is under the input cursor. If it isn't, a <see cref="ExpectingException" /> is raised.
		/// </summary>
		/// <param name="expecting">The list of expected characters. If empty, anything but end of input is accepted. If <see cref="EndOfInput" /> is included, end of input is accepted.</param>
		[System.Diagnostics.DebuggerHidden()]
		public void Expecting(params int[] expecting)
		{
			_CheckDisposed();
			if (BeforeInput == _current)
				throw new ExpectingException("The cursor is before the beginning of the input", _line, _column,_index, _position, _fileOrUrl);
			switch (expecting.Length)
			{
				case 0:
					if (EndOfInput == _current)
						throw new ExpectingException("Unexpected end of input",_line,_column,_index, _position,_fileOrUrl);
					break;
				case 1:
					if (expecting[0] != _current)
						throw new ExpectingException(_GetErrorMessage(expecting), _line, _column, _index, _position, _fileOrUrl,_GetErrorExpecting(expecting));
					break;
				default:
					if(0>Array.IndexOf(expecting,_current))
						throw new ExpectingException(_GetErrorMessage(expecting), _line, _column, _index,_position, _fileOrUrl,_GetErrorExpecting(expecting));
					break;
			}
		}
		string[] _GetErrorExpecting(int[] expecting)
		{
			var result = new string[expecting.Length];
			for (var i = 0; i < expecting.Length; i++)
			{
				if (-1 != expecting[i])
					result[i] = Convert.ToString(expecting[i]);
				else
					result[i] = "end of input";
			}
			return result;
		}
		string _GetErrorMessage(int[] expecting)
		{
			StringBuilder sb = null;
			switch (expecting.Length)
			{
				case 0:
					break;
				case 1:
					sb = new StringBuilder();
					if (-1 == expecting[0])
						sb.Append("end of input");
					else
					{
						sb.Append("\"");
						sb.Append((char)expecting[0]);
						sb.Append("\"");
					}
					break;
				case 2:
					sb = new StringBuilder();
					if (-1 == expecting[0])
						sb.Append("end of input");
					else
					{
						sb.Append("\"");
						sb.Append((char)expecting[0]);
						sb.Append("\"");
					}
					sb.Append(" or ");
					if (-1 == expecting[1])
						sb.Append("end of input");
					else
					{
						sb.Append("\"");
						sb.Append((char)expecting[1]);
						sb.Append("\"");
					}
					break;
				default: // length > 2
					sb = new StringBuilder();
					if (-1 == expecting[0])
						sb.Append("end of input");
					else
					{
						sb.Append("\"");
						sb.Append((char)expecting[0]);
						sb.Append("\"");
					}
					int l = expecting.Length - 1;
					int i = 1;
					for (; i < l; ++i)
					{
						sb.Append(", ");
						if (-1 == expecting[i])
							sb.Append("end of input");
						else
						{
							sb.Append("\"");
							sb.Append((char)expecting[i]);
							sb.Append("\"");
						}
					}
					sb.Append(", or ");
					if (-1 == expecting[i])
						sb.Append("end of input");
					else
					{
						sb.Append("\"");
						sb.Append((char)expecting[i]);
						sb.Append("\"");
					}
					break;
			}
			if (-1 == Current)
			{
				if (0 == expecting.Length)
					return "Unexpected end of input";
				return string.Concat("Unexpected end of input. Expecting ", sb.ToString());
			}
			if (0 == expecting.Length)
				return string.Concat("Unexpected character \"", (char)Current, "\" in input");
			return string.Concat("Unexpected character \"", (char)Current, "\" in input. Expecting ", sb.ToString());

		}
		void IDisposable.Dispose()
		{
			Close();
		}
		public void EnsureStarted()
		{
			_CheckDisposed();
			if (BeforeInput == _current)
				Advance();
		}
		public int Advance()
		{
			_CheckDisposed();
			_current = AdvanceInner();
			if (EndOfInput == _current)
				return EndOfInput;
			if (_current<=0xFFFF && char.IsHighSurrogate((char)_current))
			{
				int tmp = AdvanceInner();
				if(tmp==-1)
				{
					throw new IOException("Unexpected end of input in Unicode stream");
				}
				if(tmp>0xFFFF || !char.IsLowSurrogate((char)tmp))
				{
					throw new IOException("Unterminated surrogate Unicode stream");
				}
				_current = char.ConvertToUtf32((char)_current, (char)tmp);
				++_index;
			}
			switch(_current)
			{
				case '\n':
					++_line;
					_column = 0;
					break;
				case '\r':
					_column = 0;
					break;
				case '\t':
					_column = ((_column - 1) / TabWidth) * (TabWidth + 1);
					break;
				default:
					if (_current > 31)
					{
						++_column;
					}
					break;
			}
			
			++_position;
			++_index;
			return _current;
		}
		protected abstract int AdvanceInner();

		protected abstract void CloseInner();
		
		void _CheckDisposed()
		{
			if (Disposed == _current)
				throw new ObjectDisposedException(typeof(LexContext).Name);
		}
#region _TextReaderLexContext
		private sealed class _TextReaderLexContext : LexContext
		{
			TextReader _inner;
			internal _TextReaderLexContext(TextReader inner)
			{
				_inner = inner;
			}
			protected override int AdvanceInner()
			{
				return _inner.Read();
			}
			protected override void CloseInner()
			{
				_inner.Close();
			}
		}
#endregion
#region _CharEnumeratorLexContext
		private sealed class _CharEnumeratorLexContext : LexContext
		{
			IEnumerator<char> _inner;
			internal _CharEnumeratorLexContext(IEnumerable<char> inner)
			{
				_inner = inner.GetEnumerator();
			}
			protected override int AdvanceInner()
			{
				if (!_inner.MoveNext())
					return EndOfInput;
				return _inner.Current;
			}
			protected override void CloseInner()
			{
				_inner.Dispose();
			}
		}
#endregion
	}
}
