//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Example {
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using VisualFA;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Visual FA", "1.0.0.0")]
    internal sealed partial class ExampleRunner : FATextReaderRunner {
        private FAMatch _BlockEnd6(int position, int line, int column) {
        q0:
            if ((this.current == 42)) {
                this.Advance();
                goto q1;
            }
            goto errorout;
        q1:
            if ((this.current == 47)) {
                this.Advance();
                goto q2;
            }
            goto errorout;
        q2:
            return FAMatch.Create(6, this.capture.ToString(), position, line, column);
        errorout:
            if ((this.current == -1)) {
                return FAMatch.Create(-1, this.capture.ToString(), position, line, column);
            }
            this.Advance();
            goto q0;
        }
        public override FAMatch NextMatch() {
            int ch;
            int p;
            int l;
            int c;
            ch = -1;
            this.capture.Clear();
            if ((this.current == -2)) {
                this.Advance();
            }
            p = this.position;
            l = this.line;
            c = this.column;
        q0:
            if ((((this.current >= 9) 
                        && (this.current <= 13)) 
                        || (this.current == 32))) {
                this.Advance();
                goto q1;
            }
            if (((((this.current == 37) 
                        || ((this.current >= 42) 
                        && (this.current <= 43))) 
                        || (this.current == 61)) 
                        || (this.current == 92))) {
                this.Advance();
                goto q2;
            }
            if ((this.current == 45)) {
                this.Advance();
                goto q3;
            }
            if ((this.current == 47)) {
                this.Advance();
                goto q13;
            }
            if ((this.current == 48)) {
                this.Advance();
                goto q17;
            }
            if (((this.current >= 49) 
                        && (this.current <= 57))) {
                this.Advance();
                goto q20;
            }
            if (((((this.current >= 65) 
                        && (this.current <= 90)) 
                        || (this.current == 95)) 
                        || ((this.current >= 97) 
                        && (this.current <= 122)))) {
                this.Advance();
                goto q21;
            }
            goto errorout;
        q1:
            return FAMatch.Create(3, this.capture.ToString(), p, l, c);
        q2:
            return FAMatch.Create(4, this.capture.ToString(), p, l, c);
        q3:
            if (((this.current >= 49) 
                        && (this.current <= 57))) {
                this.Advance();
                goto q4;
            }
            return FAMatch.Create(4, this.capture.ToString(), p, l, c);
        q4:
            if ((this.current == 46)) {
                this.Advance();
                goto q5;
            }
            if (((this.current >= 48) 
                        && (this.current <= 57))) {
                this.Advance();
                goto q12;
            }
            return FAMatch.Create(1, this.capture.ToString(), p, l, c);
        q5:
            if (((this.current >= 48) 
                        && (this.current <= 57))) {
                this.Advance();
                goto q6;
            }
            goto errorout;
        q6:
            if (((this.current >= 48) 
                        && (this.current <= 57))) {
                this.Advance();
                goto q7;
            }
            if (((this.current == 69) 
                        || (this.current == 101))) {
                this.Advance();
                goto q8;
            }
            return FAMatch.Create(2, this.capture.ToString(), p, l, c);
        q7:
            if (((this.current >= 48) 
                        && (this.current <= 57))) {
                this.Advance();
                goto q7;
            }
            if (((this.current == 69) 
                        || (this.current == 101))) {
                this.Advance();
                goto q8;
            }
            return FAMatch.Create(2, this.capture.ToString(), p, l, c);
        q8:
            if ((this.current == 45)) {
                this.Advance();
                goto q9;
            }
            if (((this.current >= 49) 
                        && (this.current <= 57))) {
                this.Advance();
                goto q10;
            }
            goto errorout;
        q9:
            if (((this.current >= 49) 
                        && (this.current <= 57))) {
                this.Advance();
                goto q10;
            }
            goto errorout;
        q10:
            if (((this.current >= 48) 
                        && (this.current <= 57))) {
                this.Advance();
                goto q11;
            }
            return FAMatch.Create(2, this.capture.ToString(), p, l, c);
        q11:
            if (((this.current >= 48) 
                        && (this.current <= 57))) {
                this.Advance();
                goto q11;
            }
            return FAMatch.Create(2, this.capture.ToString(), p, l, c);
        q12:
            if ((this.current == 46)) {
                this.Advance();
                goto q5;
            }
            if (((this.current >= 48) 
                        && (this.current <= 57))) {
                this.Advance();
                goto q12;
            }
            return FAMatch.Create(1, this.capture.ToString(), p, l, c);
        q13:
            if ((this.current == 42)) {
                this.Advance();
                goto q14;
            }
            if ((this.current == 47)) {
                this.Advance();
                goto q15;
            }
            goto errorout;
        q14:
            return _BlockEnd6(p, l, c);
        q15:
            if ((((this.current >= 0) 
                        && (this.current <= 9)) 
                        || ((this.current >= 11) 
                        && (this.current <= 1114111)))) {
                this.Advance();
                goto q16;
            }
            return FAMatch.Create(5, this.capture.ToString(), p, l, c);
        q16:
            if ((((this.current >= 0) 
                        && (this.current <= 9)) 
                        || ((this.current >= 11) 
                        && (this.current <= 1114111)))) {
                this.Advance();
                goto q16;
            }
            return FAMatch.Create(5, this.capture.ToString(), p, l, c);
        q17:
            if ((this.current == 46)) {
                this.Advance();
                goto q18;
            }
            if (((this.current >= 48) 
                        && (this.current <= 57))) {
                this.Advance();
                goto q19;
            }
            return FAMatch.Create(1, this.capture.ToString(), p, l, c);
        q18:
            if (((this.current >= 48) 
                        && (this.current <= 57))) {
                this.Advance();
                goto q6;
            }
            goto errorout;
        q19:
            if (((this.current >= 48) 
                        && (this.current <= 57))) {
                this.Advance();
                goto q11;
            }
            return FAMatch.Create(2, this.capture.ToString(), p, l, c);
        q20:
            if ((this.current == 46)) {
                this.Advance();
                goto q5;
            }
            if (((this.current >= 48) 
                        && (this.current <= 57))) {
                this.Advance();
                goto q12;
            }
            return FAMatch.Create(1, this.capture.ToString(), p, l, c);
        q21:
            if ((((((this.current >= 48) 
                        && (this.current <= 57)) 
                        || ((this.current >= 65) 
                        && (this.current <= 90))) 
                        || (this.current == 95)) 
                        || ((this.current >= 97) 
                        && (this.current <= 122)))) {
                this.Advance();
                goto q22;
            }
            return FAMatch.Create(0, this.capture.ToString(), p, l, c);
        q22:
            if ((((((this.current >= 48) 
                        && (this.current <= 57)) 
                        || ((this.current >= 65) 
                        && (this.current <= 90))) 
                        || (this.current == 95)) 
                        || ((this.current >= 97) 
                        && (this.current <= 122)))) {
                this.Advance();
                goto q22;
            }
            return FAMatch.Create(0, this.capture.ToString(), p, l, c);
        errorout:
            if (((((((((((((((this.current == -1) 
                        || ((this.current >= 9) 
                        && (this.current <= 13))) 
                        || (this.current == 32)) 
                        || (this.current == 37)) 
                        || ((this.current >= 42) 
                        && (this.current <= 43))) 
                        || (this.current == 61)) 
                        || (this.current == 92)) 
                        || (this.current == 45)) 
                        || (this.current == 47)) 
                        || (this.current == 48)) 
                        || ((this.current >= 49) 
                        && (this.current <= 57))) 
                        || ((this.current >= 65) 
                        && (this.current <= 90))) 
                        || (this.current == 95)) 
                        || ((this.current >= 97) 
                        && (this.current <= 122)))) {
                if ((this.capture.Length == 0)) {
                    return FAMatch.Create(-2, null, 0, 0, 0);
                }
                return FAMatch.Create(-1, this.capture.ToString(), p, l, c);
            }
            this.Advance();
            ch = this.current;
            goto errorout;
        }
    }
}
