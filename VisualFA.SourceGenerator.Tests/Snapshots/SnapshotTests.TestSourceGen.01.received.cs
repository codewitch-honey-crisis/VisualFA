//HintName: TestSourceCalc.g.cs
namespace Tests
{
    partial class TestSourceCalcTextReaderRunner
        : Tests.FATextReaderRunner
    {
        /// <summary>Matched the expression: \/\*.*?\*\/</summary>
        public const int commentBlock = 0;
        /// <summary>Matched the expression: //[^\n]*</summary>
        public const int lineComment = 1;
        /// <summary>Matched the expression: [ \t\r\n]+</summary>
        public const int whiteSpace = 2;
        /// <summary>Matched the expression: [A-Za-z_][A-Za-z0-9_]*</summary>
        public const int identifier = 3;
        /// <summary>Matched the expression: (0|([1-9][0-9]*))((\.[0-9]+[Ee]\-?[1-9][0-9]*)?|\.[0-9]+)</summary>
        public const int number = 4;
        /// <summary>Matched the expression: \+</summary>
        public const int plus = 5;
        /// <summary>Matched the expression: \-</summary>
        public const int minus = 6;
        /// <summary>Matched the expression: \*</summary>
        public const int multiply = 7;
        /// <summary>Matched the expression: \/</summary>
        public const int divide = 8;
        /// <summary>Matched the expression: %</summary>
        public const int modulo = 9;
        Tests.FAMatch _BlockEnd0(int p, int l, int c)
        {
        q0:
            // [\*]
            if (current == 42)
            {
                Advance();
                goto q1;
            }
            goto errorout;
        q1:
            // [\/]
            if (current == 47)
            {
                Advance();
                goto q2;
            }
            goto errorout;
        q2:
            return Tests.FAMatch.Create(0, capture.ToString(), p, l, c);
        errorout:
            if (current == -1)
            {
                return Tests.FAMatch.Create(-1, capture.ToString(), p, l, c);
            }
            Advance();
            goto q0;
            
        }
        public override Tests.FAMatch NextMatch()
        {
            int p;
            int l;
            int c;
            capture.Clear();
            if (current == -2)
            {
                Advance();
            }
            p = position;
            l = line;
            c = column;
        // q0:
            // [\t-\n\r ]
            if (current == 9 || current == 10 || current == 13 || current == 32)
            {
                Advance();
                goto q1;
            }
            // [%]
            if (current == 37)
            {
                Advance();
                goto q2;
            }
            // [\*]
            if (current == 42)
            {
                Advance();
                goto q3;
            }
            // [\+]
            if (current == 43)
            {
                Advance();
                goto q4;
            }
            // [\-]
            if (current == 45)
            {
                Advance();
                goto q5;
            }
            // [\/]
            if (current == 47)
            {
                Advance();
                goto q6;
            }
            // [0]
            if (current == 48)
            {
                Advance();
                goto q9;
            }
            // [1-9]
            if ((current >= 49 && current <= 57))
            {
                Advance();
                goto q15;
            }
            // [A-Z_a-z]
            if ((current >= 65 && current <= 90) || current == 95 || (current >= 97 && current <= 122))
            {
                Advance();
                goto q16;
            }
            goto errorout;
        q1:
            // [\t-\n\r ]
            if (current == 9 || current == 10 || current == 13 || current == 32)
            {
                Advance();
                goto q1;
            }
            return Tests.FAMatch.Create(2, capture.ToString(), p, l, c);
        q2:
            return Tests.FAMatch.Create(9, capture.ToString(), p, l, c);
        q3:
            return Tests.FAMatch.Create(7, capture.ToString(), p, l, c);
        q4:
            return Tests.FAMatch.Create(5, capture.ToString(), p, l, c);
        q5:
            return Tests.FAMatch.Create(6, capture.ToString(), p, l, c);
        q6:
            // [\*]
            if (current == 42)
            {
                Advance();
                goto q7;
            }
            // [\/]
            if (current == 47)
            {
                Advance();
                goto q8;
            }
            return Tests.FAMatch.Create(8, capture.ToString(), p, l, c);
        q7:
            return _BlockEnd0(p, l, c);
        q8:
            // [\0-\t\v-\x10ffff]
            if ((current >= 0 && current <= 9) || (current >= 11 && current <= 1114111))
            {
                Advance();
                goto q8;
            }
            return Tests.FAMatch.Create(1, capture.ToString(), p, l, c);
        q9:
            // [\.]
            if (current == 46)
            {
                Advance();
                goto q10;
            }
            return Tests.FAMatch.Create(4, capture.ToString(), p, l, c);
        q10:
            // [0-9]
            if ((current >= 48 && current <= 57))
            {
                Advance();
                goto q11;
            }
            goto errorout;
        q11:
            // [0-9]
            if ((current >= 48 && current <= 57))
            {
                Advance();
                goto q11;
            }
            // [Ee]
            if (current == 69 || current == 101)
            {
                Advance();
                goto q12;
            }
            return Tests.FAMatch.Create(4, capture.ToString(), p, l, c);
        q12:
            // [\-]
            if (current == 45)
            {
                Advance();
                goto q13;
            }
            // [1-9]
            if ((current >= 49 && current <= 57))
            {
                Advance();
                goto q14;
            }
            goto errorout;
        q13:
            // [1-9]
            if ((current >= 49 && current <= 57))
            {
                Advance();
                goto q14;
            }
            goto errorout;
        q14:
            // [0-9]
            if ((current >= 48 && current <= 57))
            {
                Advance();
                goto q14;
            }
            return Tests.FAMatch.Create(4, capture.ToString(), p, l, c);
        q15:
            // [\.]
            if (current == 46)
            {
                Advance();
                goto q10;
            }
            // [0-9]
            if ((current >= 48 && current <= 57))
            {
                Advance();
                goto q15;
            }
            return Tests.FAMatch.Create(4, capture.ToString(), p, l, c);
        q16:
            // [0-9A-Z_a-z]
            if ((current >= 48 && current <= 57) || (current >= 65 && current <= 90) || current == 95 || (current >= 97 && current <= 122))
            {
                Advance();
                goto q16;
            }
            return Tests.FAMatch.Create(3, capture.ToString(), p, l, c);
        errorout:
            if (current == -1 || current == 9 || current == 10 || current == 13 || current == 32 || current == 37 || current == 42 || current == 43 || current == 45 || current == 47 || current == 48 || (current >= 49 && current <= 57) || (current >= 65 && current <= 90) || current == 95 || (current >= 97 && current <= 122))
            {
                if (capture.Length == 0)
                {
                    return Tests.FAMatch.Create(-2, null, 0, 0, 0);
                }
                return Tests.FAMatch.Create(-1, capture.ToString(), p, l, c);
            }
            Advance();
            goto errorout;
        }
    }
}
