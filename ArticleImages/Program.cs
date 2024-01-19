using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VisualFA;

namespace ArticleImages
{
	internal static class Program
	{
		static void RenderCPFile(this FA fa, string file, FADotGraphOptions options = null, int width = 640)
		{
			RenderCPFile(fa.RenderToStream("png", false, options), file, width);
		}
		static void RenderCPFile(Stream stream, string file, int width = 640)
		{
			using (var img = Image.FromStream(stream))
			{
				double mult = 1;
				var size = img.Size;
				if (size.Width > width)
				{
					mult = ((double)width)/ size.Width;
					using (var bmp = new Bitmap(img, width, (int)(size.Height * mult)))
					{
						bmp.Save(file, System.Drawing.Imaging.ImageFormat.Png);
					}
				}
				else
				{
					img.Save(file, System.Drawing.Imaging.ImageFormat.Png);
				}
			}
			stream.Close();
		}
		static void Main(string[] args)
		{
			var commentBlock = FA.Parse(@"\/\*", 0, false);
			var commentBlockEnd = FA.Parse(@"\*\/", 0, false);
			var commentLine = FA.Parse(@"\/\/[^\n]*", 1, false);
			var wspace = FA.Parse("[ \\t\\r\\n]+", 2, false);
			var ident = FA.Parse("[A-Za-z_][A-Za-z0-9_]*", 3, false);
			var intNum = FA.Parse("0|\\-?[1-9][0-9]*", 4, false);
			var realNum = FA.Parse("0|\\-?[1-9][0-9]*(\\.[0-9]+([Ee]\\-?[1-9][0-9]*)?)?", 5, false);
			var exprs = new FA[] { commentBlock, commentLine, wspace, ident, intNum, realNum };
			var blockEnds = new FA[] { commentBlockEnd };
			var syms = new string[] { "cblock", "cline", "wspace", "ident", "intNum", "realNum" };
			var opts = new FADotGraphOptions();
			opts.Vertical = true;
			opts.BlockEnds = blockEnds;
			opts.AcceptSymbolNames = syms;
			var lexer_nfa = FA.ToLexer(exprs, false, false);
			lexer_nfa.RenderCPFile(@"..\..\lexer_nfa.png", opts);
			var lexer_cnfa = lexer_nfa.Clone();
			lexer_cnfa.Compact();
			lexer_cnfa.RenderCPFile(@"..\..\lexer_compact_nfa.png", opts);
			var exprsMinDfa = new FA[] { commentBlock.ToMinimizedDfa(), commentLine.ToMinimizedDfa(), wspace.ToMinimizedDfa(), ident.ToMinimizedDfa(), intNum.ToMinimizedDfa(), realNum.ToMinimizedDfa() };
			var blockEndsMinDfa = new FA[] { commentBlockEnd.ToMinimizedDfa() };
			var lexer_mdfa = FA.ToLexer(exprsMinDfa);
			opts.BlockEnds = blockEndsMinDfa;
			lexer_mdfa.RenderCPFile(@"..\..\lexer_min_dfa.png", opts);
			opts.BlockEnds = null;
			opts.HideAcceptSymbolIds = true;
			lexer_nfa.ToLinearized(true,false).Key.RenderCPFile(@"..\..\lexer_linearized.png", opts);
			opts.Vertical = false;
			opts.AcceptSymbolNames = null;
			opts.HideAcceptSymbolIds = true;
			opts.BlockEnds = null;
			ident.RenderCPFile(@"..\..\ident_nfa.png", opts);
			ident.ToMinimizedDfa().RenderCPFile(@"..\..\ident_dfa.png", opts);
			intNum.RenderCPFile(@"..\..\intNum_nfa.png", opts);
			intNum.ToMinimizedDfa().RenderCPFile(@"..\..\intNum_dfa.png", opts);
			var ABC = FA.Literal("ABC");
			var ABCset = FA.Set(new FARange[] {new FARange('A','C')});
			var DEF = FA.Literal("DEF");
			var foo = FA.Literal("foo");
			var bar = FA.Literal("bar");
			ABC.RenderCPFile(@"..\..\ABC.png", opts);
			FA.Repeat(ABC, 3, 3).RenderCPFile(@"..\..\ABCx3.png",opts);
			FA.Repeat(ABC, 2, 3,0,false).RenderCPFile(@"..\..\ABCx2or3.png", opts);
			FA.Parse("[ABC]").RenderCPFile(@"..\..\ABCset.png", opts);
			FA.Or(new FA[] { ABC, DEF }, 0, false).RenderCPFile(@"..\..\ABCorDEF.png", opts);
			var ABCloop = FA.Repeat(ABC, 0, 0, 0, false);
			ABCloop.AcceptSymbol = -1;
			ABCloop.RenderCPFile(@"..\..\ABCloop.png", opts);
			FA.Optional(ABC, 0, false).RenderCPFile(@"..\..\ABCopt.png", opts); ;
			FA.Concat(new FA[] { ABC, DEF }, 0, false).RenderCPFile(@"..\..\ABC_DEF.png",opts);
			var fooOrBar = FA.Or(new FA[] { foo, bar }, 0, false);
			var fooOrBarDfa = FA.Or(new FA[] { foo, bar }).ToMinimizedDfa();
			fooOrBar.RenderCPFile(@"..\..\fooOrBar.png", opts);
			fooOrBarDfa.RenderCPFile(@"..\..\fooOrBar_dfa.png",opts);
			var fooOrBarLoop = fooOrBar.Clone();
			var fooOrBarLoopTerm = fooOrBarLoop.FindFirst(FA.AcceptingFilter);
			fooOrBarLoopTerm.AddEpsilon(fooOrBarLoop, false);
			fooOrBarLoop.RenderCPFile(@"..\..\fooOrBarLoop.png", opts);
			fooOrBarLoop.ToMinimizedDfa().RenderCPFile(@"..\..\fooOrBarLoop_min_dfa.png", opts);
			opts.DebugSourceNfa = fooOrBarLoop;
			opts.DebugShowNfa = true;
			opts.Vertical = true;
			fooOrBarLoop.ToDfa().RenderCPFile(@"..\..\fooOrBarLoop_dfa.png", opts);

			var ambig = FA.ToLexer(new FA[] { commentBlock, commentLine }, false,false);
			opts.DebugShowNfa = false;
			opts.DebugSourceNfa = null;
			opts.Vertical = false;
			opts.HideAcceptSymbolIds = false;
			opts.AcceptSymbolNames = syms;
			ambig.RenderCPFile(@"..\..\ambig_nfa.png", opts);
			ambig.Compact();
			ambig.RenderCPFile(@"..\..\ambig_compact_nfa.png",opts);
			var ambigDfa = ambig.ToDfa();
			opts.DebugShowNfa = true;
			opts.DebugSourceNfa = ambig;
			ambigDfa.RenderCPFile(@"..\..\ambig_dfa.png",opts);
			var ambigMdfa = ambigDfa.ToMinimizedDfa();
			ambigMdfa.RenderCPFile(@"..\..\ambig_min_dfa.png", opts);


		}
	}
}
