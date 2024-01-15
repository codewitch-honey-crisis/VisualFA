using System;
using System.CodeDom;

namespace VisualFA
{
	static partial class Deslanged
	{
		public static CodeCompileUnit GetFAMatch(bool spans)
		{
			if(spans)
			{

#if FALIB_SPANS
				return DeslangedSpan.FAMatch;
#else
				throw new NotSupportedException("Spans are not supported");
#endif
			}
			return DeslangedString.FAMatch;
		}
		public static CodeCompileUnit FAMatch {
			get {
#if FALIB_SPANS
				if (VisualFA.FAStringRunner.UsingSpans)
				{
					return DeslangedSpan.FAMatch;
				}
				else
				{
					return DeslangedString.FAMatch;
				}
#else
				return DeslangedString.FAMatch;
#endif
			}
		}
		public static CodeCompileUnit GetFARunner(bool spans)
		{
			if (spans)
			{

#if FALIB_SPANS
				return DeslangedSpan.FARunnerSpan;
#else
				throw new NotSupportedException("Spans are not supported");
#endif
			}
			return DeslangedString.FARunnerString;
		}
		public static CodeCompileUnit FARunner {
			get {
#if FALIB_SPANS
				if (VisualFA.FAStringRunner.UsingSpans)
				{
					return DeslangedSpan.FARunnerSpan;
				}
				else
				{
					return DeslangedString.FARunnerString;
				}
#else
				return DeslangedString.FARunnerString;
#endif
			}
		}
		public static CodeCompileUnit GetFADfaTableRunner(bool spans)
		{
			if (spans)
			{

#if FALIB_SPANS
				return DeslangedSpan.FADfaTableRunnerSpan;
#else
				throw new NotSupportedException("Spans are not supported");
#endif
			}
			return DeslangedString.FADfaTableRunnerString;
		}
		public static CodeCompileUnit FADfaTableRunner {
			get {
#if FALIB_SPANS
				if (VisualFA.FAStringRunner.UsingSpans)
				{
					return DeslangedSpan.FADfaTableRunnerSpan;
				}
				else
				{
					return DeslangedString.FADfaTableRunnerString;
				}
#else
				return DeslangedString.FADfaTableRunnerString;
#endif
			}
		}
	}
}

