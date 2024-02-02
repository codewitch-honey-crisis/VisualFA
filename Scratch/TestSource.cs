using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VisualFA;
namespace Tests
{
 
    partial class TestSource
    {
        // Declare a lexer. Here we specify the rules and the type of lexer
        // as indicated by FARule attributes and the return type
        // shared dependency code is automatically generated as needed.
        // It won't be generated if your code references VisualFA.
        [FARule(@"\/\*",Symbol="commentBlock",BlockEnd=@"\*\/")]
        [FARule(@"\/\/[^\n]*", Symbol = "lineComment")]
        [FARule(@"[ \t\r\n]+", Symbol = "whiteSpace")]
        [FARule(@"[A-Za-z_][A-Za-z0-9_]*",Symbol="identifier")]
        [FARule(@"(0|([1-9][0-9]*))((\.[0-9]+[Ee]\-?[1-9][0-9]*)?|\.[0-9]+)",Symbol="number")]
        [FARule(@"\+",Symbol = "plus")]
        [FARule(@"\-", Symbol = "minus")]
        [FARule(@"\*", Symbol = "multiply")]
        [FARule(@"\/", Symbol = "divide")]
        [FARule(@"%", Symbol = "modulo")]
        internal static partial FATextReaderRunner Calc(TextReader text);
    }
}
