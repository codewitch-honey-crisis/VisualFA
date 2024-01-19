/*

Code for ToString originally adapted from
https://github.com/wolever/nfa2regex
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
	partial class FA
	{
		private class _ExpEdge
		{
			public string Exp;
			public FA From;
			public FA To;
		}
		private class _ExpMachine
		{
		}
		static IList<_ExpEdge> _ToExpressionEdgesIn(IList<_ExpEdge> edges, FA node)
		{
			var result = new List<_ExpEdge>();
			for (int i = 0; i < edges.Count; ++i)
			{
				if (edges[i].To == node)
				{
					result.Add(edges[i]);
				}
			}
			return result;
		}
		static IList<_ExpEdge> _ToExpressionEdgesOut(IList<_ExpEdge> edges, FA node)
		{
			var result = new List<_ExpEdge>();
			for (int i = 0; i < edges.Count; ++i)
			{
				var edge = edges[i];
				if (edge.From == node)
				{
					result.Add(edge);
				}
			}
			return result;
		}
		static string _ToExpression(FA fa)
		{
			List<FA> Closure = new List<FA>();
			List<_ExpEdge> Edges = new List<_ExpEdge>();
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
			Closure.Clear();
			first.FillClosure(Closure);
			var sb = new StringBuilder();
			// build the machine from the FA
			for (int q = 0; q < Closure.Count; ++q)
			{
				var cfa = Closure[q];

				foreach (var trns in cfa.FillInputTransitionRangesGroupedByState(true))
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
							Edges.Add(eedge);
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
					Edges.Add(edge);
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
			Edges.Add(newEdge);
			newEdge = new _ExpEdge();
			newEdge.Exp = string.Empty;
			newEdge.From = qLast;
			newEdge.To = final; ;
			Edges.Add(newEdge);
			Closure.Insert(0, first);
			Closure.Add(final);

			while (Closure.Count > 2)
			{
				for (int q = 1; q < Closure.Count - 1; ++q)
				{
					var node = Closure[q];
					var loops = new List<string>();
					var inEdges = _ToExpressionEdgesIn(Edges, node);
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
						var outEdges = _ToExpressionEdgesOut(Edges, node);
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
							Edges.Add(expEdge);
						}
					}
					var newEdges = _ToExpressionOrphanState(Edges, node);
					Edges.Clear();
					Edges.AddRange(newEdges);
					Closure.Remove(node);
				}
			}
			var result = new List<string>(Edges.Count);
			for (int i = 0; i < Edges.Count; ++i)
			{
				var edge = Edges[i];
				result.Add(edge.Exp.ToString());
			}

			return _ToExpressionOrJoin(result);

		}


		static string _ToExpressionOrJoin(IList<string> strings)
		{
			if (strings.Count == 0) return string.Empty;
			if (strings.Count == 1) return strings[0];
			return string.Concat("(", string.Join("|", strings), ")");
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



		static IList<_ExpEdge> _ToExpressionOrphanState(IList<_ExpEdge> edges, FA node)
		{
			var newEdges = new List<_ExpEdge>(edges.Count);
			for (int i = 0; i < edges.Count; ++i)
			{
				var edge = edges[i];
				if (edge.From == node || edge.To == node)
				{
					continue;
				}
				newEdges.Add(edge);
			}
			return newEdges;
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
				
			}
			throw new FormatException("Invalid format specifier");
		}
		/// <summary>
		/// Converts the state machine to a regular expression.
		/// </summary>
		/// <returns>The expression</returns>
		public override string ToString()
		{
			if (Id > -1)
			{
				return String.Concat("q", Id.ToString());
			}
			else
			{
				return base.ToString();
			}
			//var mach = _RxMachine.FromFA(this, null);
			//return mach.Convert();
		}
	}
}
