//HintName: FARunnerMethods.g.cs
namespace Tests
{
    partial class TestSource
    {
        // [LexRule(@"\/\*", Id = 0, Symbol = @"commentBlock")]
        // [LexRule(@"//[^\n]*", Id = 1, Symbol = @"lineComment")]
        // [LexRule(@"[ \t\r\n]+", Id = 2, Symbol = @"whiteSpace")]
        // [LexRule(@"[A-Za-z_][A-Za-z0-9_]*", Id = 3, Symbol = @"identifier")]
        // [LexRule(@"(0|([1-9][0-9]*))((\.[0-9]+[Ee]\-?[1-9][0-9]*)?|\.[0-9]+)", Id = 4, Symbol = @"number")]
        // [LexRule(@"\+", Id = 5, Symbol = @"plus")]
        // [LexRule(@"\-", Id = 6, Symbol = @"minus")]
        // [LexRule(@"\*", Id = 7, Symbol = @"multiply")]
        // [LexRule(@"\/", Id = 8, Symbol = @"divide")]
        // [LexRule(@"%", Id = 9, Symbol = @"modulo")]
        internal static partial FATextReaderRunner Calc(System.IO.TextReader text)
        {
            var result = new TestSourceCalcTextReaderRunner();
            result.Set(text);
            return result;
        }
    }

}

