/*

Code for ToString originally adapted from
https://github.com/wolever/nfa2regex
Adaptation by honey the codewitch.

Original license for derivative work follows:
MIT License

Copyright (c) 2021 David Wolever

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System.Text;

namespace VisualFA
{
	partial class FA : IFormattable
	{
		private struct _ExpEdge
		{
			public string Exp;
			public FA From;
			public FA To;
		}
		static void _ToExpressionFillEdgesIn(IList<_ExpEdge> edges, FA node,IList<_ExpEdge> result)
		{
			for (int i = 0; i < edges.Count; ++i)
			{
				if (edges[i].To == node)
				{
					result.Add(edges[i]);
				}
			}
		}
		static void _ToExpressionFillEdgesOut(IList<_ExpEdge> edges, FA node, IList<_ExpEdge> result)
		{
			for (int i = 0; i < edges.Count; ++i)
			{
				var edge = edges[i];
				if (edge.From == node)
				{
					result.Add(edge);
				}
			}
		}
		static string _ToExpression(FA fa)
		{
			List<FA> closure = new List<FA>();
			List<_ExpEdge> fsmEdges = new List<_ExpEdge>();
			FA first, final = null;

			first = fa;
			var acc = first.FillFind(AcceptingFilter);
			if (acc.Count == 1)
			{
				final = acc[0];
			}
			else if (acc.Count > 1)
			{
				fa = fa.Clone();
				first = fa;
				acc = fa.FillFind(AcceptingFilter);
				final = new FA(acc[0].AcceptSymbol);
				for (int i = 0; i < acc.Count; ++i)
				{
					var a = acc[i];
					a.AddEpsilon(final, false);
					a.AcceptSymbol = -1;
				}
			}
			closure.Clear();
			first.FillClosure(closure);
			var sb = new StringBuilder();
			// build the machine from the FA
			var trnsgrp = new Dictionary<FA, IList<FARange>>(closure.Count);
			for (int q = 0; q < closure.Count; ++q)
			{
				var cfa = closure[q];
				trnsgrp.Clear();
				foreach (var trns in cfa.FillInputTransitionRangesGroupedByState(true,trnsgrp))
				{
					sb.Clear();
					if (trns.Value.Count == 1 && trns.Value[0].Min == trns.Value[0].Max)
					{
						var range = trns.Value[0];
						if (range.Min == -1 && range.Max == -1)
						{
							var eedge = new _ExpEdge();
							eedge.Exp = string.Empty;
							eedge.From = cfa;
							eedge.To = trns.Key;
							fsmEdges.Add(eedge);
							continue;
						}
						_AppendRangeCharTo(sb, range.Min);
					}
					else
					{
						sb.Append("[");
						_AppendRangeTo(sb, trns.Value);
						sb.Append("]");
					}
					var edge = new _ExpEdge();
					edge.Exp = sb.ToString();
					edge.From = cfa;
					edge.To = trns.Key;
					fsmEdges.Add(edge);
				}
			}
			var tmp = new FA();
			tmp.AddEpsilon(first, false);
			var q0 = first;
			first = tmp;
			tmp = new FA(final.AcceptSymbol);
			var qLast = final;
			final.AcceptSymbol = -1;
			final.AddEpsilon(tmp, false);
			final = tmp;
			// add first and final
			var newEdge = new _ExpEdge();
			newEdge.Exp = string.Empty;
			newEdge.From = first;
			newEdge.To = q0;
			fsmEdges.Add(newEdge);
			newEdge = new _ExpEdge();
			newEdge.Exp = string.Empty;
			newEdge.From = qLast;
			newEdge.To = final;
			fsmEdges.Add(newEdge);
			closure.Insert(0, first);
			closure.Add(final);
			var inEdges = new List<_ExpEdge>();
			var outEdges = new List<_ExpEdge>();
			while (closure.Count > 2)
			{
				for (int q = 1; q < closure.Count - 1; ++q)
				{
					var node = closure[q];
					var loops = new List<string>();
					inEdges.Clear();
					_ToExpressionFillEdgesIn(fsmEdges, node, inEdges);
					for (int i = 0; i < inEdges.Count; ++i)
					{
						var edge = inEdges[i];
						if (edge.From == edge.To)
						{
							loops.Add(edge.Exp);
						}
					}
					var middle = _ToExpressionKleeneStar(_ToExpressionOrJoin(loops), loops.Count > 1);
					for (int i = 0; i < inEdges.Count; ++i)
					{
						var inEdge = inEdges[i];
						if (inEdge.From == inEdge.To)
						{
							continue;
						}
						outEdges.Clear();
						_ToExpressionFillEdgesOut(fsmEdges, node, outEdges);
						for (int j = 0; j < outEdges.Count; ++j)
						{
							var outEdge = outEdges[j];
							if (outEdge.From == outEdge.To)
							{
								continue;
							}
							var expEdge = new _ExpEdge();
							expEdge.From = inEdge.From;
							expEdge.To = outEdge.To;
							expEdge.Exp = string.Concat(inEdge.Exp, middle, outEdge.Exp);
							fsmEdges.Add(expEdge);
						}
					}
					// reuse inedges since we're not using it
					inEdges.Clear();
					_ToExpressionFillEdgesOrphanState(fsmEdges, node,inEdges);
					fsmEdges.Clear();
					fsmEdges.AddRange(inEdges);
					closure.Remove(node);
				}
			}
			var result = new List<string>(fsmEdges.Count);
			for (int i = 0; i < fsmEdges.Count; ++i)
			{
				var edge = fsmEdges[i];
				result.Add(edge.Exp.ToString());
			}

			return _ToExpressionOrJoin(result);

		}
		

		static string _ToExpressionOrJoin(IList<string> strings)
		{
			if (strings.Count == 0) return string.Empty;
			if (strings.Count == 1) return strings[0];
			return string.Concat("(", string.Join("|",strings), ")");
		}
		static string _ToExpressionKleeneStar(string s, bool noWrap)
		{
			if (string.IsNullOrEmpty(s)) return "";
			if (noWrap || s.Length == 1)
			{
				return s + "*";
			}
			return string.Concat("(", s, ")*");
		}



		static void _ToExpressionFillEdgesOrphanState(IList<_ExpEdge> edges, FA node, IList<_ExpEdge> result)
		{
			for (int i = 0; i < edges.Count; ++i)
			{
				var edge = edges[i];
				if (edge.From == node || edge.To == node)
				{
					continue;
				}
				result.Add(edge);
			}
		}
		public string ToString(string format, IFormatProvider provider)
		{
			return ToString(format);
		}
		public string ToString(string format)
		{
			if (string.IsNullOrEmpty(format))
			{
				return ToString();
			}
			if (format == "e")
			{
				return _ToExpression(this);
			} else if(format=="r")
			{
				return RegexExpression.FromFA(this).Reduce().ToString();
			}
			throw new FormatException("Invalid format specifier");
		}
		
	}
}
