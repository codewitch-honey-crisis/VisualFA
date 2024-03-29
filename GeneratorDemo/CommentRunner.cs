//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VisualFA;


[System.CodeDom.Compiler.GeneratedCodeAttribute("Visual FA", "1.0.0.0")]
internal sealed partial class CommentRunner : FAStringRunner {
    private FAMatch _BlockEnd0(ReadOnlySpan<char> s, int cp, int len, int position, int line, int column) {
    q0:
        // [\*]
        if ((cp == 42)) {
            this.Advance(s, ref cp, ref len, false);
            goto q1;
        }
        goto errorout;
    q1:
        // [\/]
        if ((cp == 47)) {
            this.Advance(s, ref cp, ref len, false);
            goto q2;
        }
        goto errorout;
    q2:
        return FAMatch.Create(0, s.Slice(position, len).ToString(), position, line, column);
    errorout:
        if ((cp == -1)) {
            return FAMatch.Create(-1, s.Slice(position, len).ToString(), position, line, column);
        }
        this.Advance(s, ref cp, ref len, false);
        goto q0;
    }
    private FAMatch NextMatchImpl(ReadOnlySpan<char> s) {
        int ch;
        int len;
        int p;
        int l;
        int c;
        ch = -1;
        len = 0;
        if ((this.position == -1)) {
            this.position = 0;
        }
        p = this.position;
        l = this.line;
        c = this.column;
        this.Advance(s, ref ch, ref len, true);
        // q0:
        // [\t-\n\r ]
        if (((((ch >= 9) 
                    && (ch <= 10)) 
                    || (ch == 13)) 
                    || (ch == 32))) {
            this.Advance(s, ref ch, ref len, false);
            goto q1;
        }
        // [\/]
        if ((ch == 47)) {
            this.Advance(s, ref ch, ref len, false);
            goto q2;
        }
        goto errorout;
    q1:
        // [\t-\n\r ]
        if (((((ch >= 9) 
                    && (ch <= 10)) 
                    || (ch == 13)) 
                    || (ch == 32))) {
            this.Advance(s, ref ch, ref len, false);
            goto q1;
        }
        return FAMatch.Create(2, s.Slice(p, len).ToString(), p, l, c);
    q2:
        // [\*]
        if ((ch == 42)) {
            this.Advance(s, ref ch, ref len, false);
            goto q3;
        }
        // [\/]
        if ((ch == 47)) {
            this.Advance(s, ref ch, ref len, false);
            goto q4;
        }
        goto errorout;
    q3:
        return _BlockEnd0(s, ch, len, p, l, c);
    q4:
        // [\0-\x10ffff]
        if (((ch >= 0) 
                    && (ch <= 1114111))) {
            this.Advance(s, ref ch, ref len, false);
            goto q4;
        }
        return FAMatch.Create(1, s.Slice(p, len).ToString(), p, l, c);
    errorout:
        if ((((((ch == -1) 
                    || ((ch >= 9) 
                    && (ch <= 10))) 
                    || (ch == 13)) 
                    || (ch == 32)) 
                    || (ch == 47))) {
            if ((len == 0)) {
                return FAMatch.Create(-2, null, 0, 0, 0);
            }
            return FAMatch.Create(-1, s.Slice(p, len).ToString(), p, l, c);
        }
        this.Advance(s, ref ch, ref len, false);
        goto errorout;
    }
    public override FAMatch NextMatch() {
        return this.NextMatchImpl(this.@string);
    }
    public const int commentBlock = 0;
    public const int commentLine = 1;
    public const int whitespace = 2;
}
