using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VisualFA;
namespace Tests
{

    partial class TestSource3
    {
        [FARule(@"\/\*", Symbol = "commentBlock", BlockEnd = @"\*\/")]
        [FARule(@"\/\/[^\n]*", Symbol = "lineComment")]
        [FARule(@"[ \t\r\n]+", Symbol = "whiteSpace")]
        [FARule(@"[A-Za-z_][A-Za-z0-9_]*", Symbol = "identifier")]
        internal static partial FAStringRunner Calc(string text);
        
    }
}
