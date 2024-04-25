using System;
using System.Collections.Generic;

namespace VisualFA
{
	partial class FA
	{
		/// <summary>
		/// Creates a literal machine given the UTF-32 string
		/// </summary>
		/// <remarks>Use <code>ToUtf32()</code> to compute from characters.</remarks>
		/// <param name="codepoints">The codepoints to create the literal from.</param>
		/// <param name="accept">The accepting id</param>
		/// <param name="compact">True to collapse epsilons, false to generate expanded epsilons</param>
		/// <returns>A new machine representing the literal expression</returns>
		public static FA Literal(IEnumerable<int> codepoints, int accept = 0, bool compact = true)
		{
			var result = new FA(accept);
			var current = result;
			foreach (var codepoint in codepoints)
			{
				current.AcceptSymbol = -1;
				var fa = new FA(accept);
				current.AddTransition(new FARange(codepoint, codepoint), fa);
				current = fa;
			}
			return result;
		}
		/// <summary>
		/// Creates a literal machine given the string
		/// </summary>
		/// <remarks>Use <code>ToUtf32()</code> to compute from characters.</remarks>
		/// <param name="string">The string create the literal from.</param>
		/// <param name="accept">The accepting id</param>
		/// <param name="compact">True to collapse epsilons, false to generate expanded epsilons</param>
		/// <returns>A new machine representing the literal expression</returns>
		public static FA Literal(string @string, int accept = 0, bool compact = true)
		{
			return Literal(ToUtf32(@string), accept, compact);
		}
		/// <summary>
		/// Creates a charset machine represeting the given the UTF-32 codepoint ranges
		/// </summary>
		/// <param name="ranges">The <see cref="FARange"/> codepoint ranges to create the set from.</param>
		/// <param name="accept">The accepting id</param>
		/// <param name="compact">True to collapse epsilons, false to generate expanded epsilons</param>
		/// <returns>A new machine representing the set expression</returns>
		public static FA Set(IEnumerable<FARange> ranges, int accept = 0, bool compact = true)
		{
			var result = new FA();
			var final = new FA(accept);
			foreach (var range in ranges)
				result.AddTransition(range, final);
			return result;
		}
		/// <summary>
		/// Creates a machine that is a concatenation of the given expressions
		/// </summary>
		/// <param name="exprs">The expressions to concatenate</param>
		/// <param name="accept">The accept id</param>
		/// <param name="compact">True to collapse epsilons, false to generate expanded epsilons</param>
		/// <returns>A new machine representing the concatenated expressions</returns>
		public static FA Concat(IEnumerable<FA> exprs, int accept = 0, bool compact = true)
		{
			FA result = null, left = null, right = null;
			foreach (var val in exprs)
			{
				if (null == val) continue;
				var nval = val.Clone();
				if (null == left)
				{
					if (null == result)
						result = nval;
					left = nval;
					continue;
				}
				if (null == right)
				{
					right = nval;
				}
				nval = right.Clone();
				_Concat(left, nval, compact);
				right = null;
				left = nval;
			}
			if (null != right)
			{
				var acc = right.FillFind(AcceptingFilter);
				for (int ic = acc.Count, i = 0; i < ic; ++i)
					acc[i].AcceptSymbol = accept;
			}
			else
			{
				var acc = result.FillFind(AcceptingFilter);
				for (int ic = acc.Count, i = 0; i < ic; ++i)
					acc[i].AcceptSymbol = accept;
			}
			return result == null ? new FA() : result;
		}
		static void _Concat(FA lhs, FA rhs, bool compact)
		{
			var acc = lhs.FillFind(AcceptingFilter);
			for (int ic = acc.Count, i = 0; i < ic; ++i)
			{
				var f = acc[i];
				f.AcceptSymbol = -1;
				f.AddEpsilon(rhs, compact);
			}
		}
		/// <summary>
		/// Creates a machine that is a disjunction between several expressions.
		/// </summary>
		/// <param name="exprs">The expressions that represent the possible choices to match</param>
		/// <param name="accept">The accept id</param>
		/// <param name="compact">True to collapse epsilons, false to generate expanded epsilons</param>
		/// <returns>A new machine representing the or expression</returns>
		public static FA Or(IEnumerable<FA> exprs, int accept = 0, bool compact = true)
		{
			var result = new FA();
			var final = new FA(accept);
			foreach (var fa in exprs)
			{
				if (null != fa)
				{
					var nfa = fa.Clone();
					result.AddEpsilon(nfa, compact);
					var acc = nfa.FillFind(AcceptingFilter);
					for (int ic = acc.Count, i = 0; i < ic; ++i)
					{
						var nffa = acc[i];
						nffa.AcceptSymbol = -1;
						nffa.AddEpsilon(final, compact);
					}
				}
				else result.AddEpsilon(final, compact);
			}
			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="expr">The expression to make optional</param>
		/// <param name="accept">The accept id</param>
		/// <param name="compact">True to collapse epsilons, false to generate expanded epsilons</param>
		/// <returns>A new machine representing the optional expression</returns>
		public static FA Optional(FA expr, int accept = 0, bool compact = true)
		{
			var result = expr.Clone();
			if (result.IsAccepting) return result;
			var acc = result.FillFind(AcceptingFilter);
			FA final = null;
			if (acc.Count > 1)
			{
				final = new FA(accept);
				for (int ic = acc.Count, i = 0; i < ic; ++i)
				{
					var fa = acc[i];
					fa.AcceptSymbol = -1;
					fa.AddEpsilon(final, compact);
				}
			}
			else
			{
				final = acc[0];
				final.AcceptSymbol = accept;
			}
			result.AddEpsilon(final, compact);

			return result;
		}
		/// <summary>
		/// Creates a repetition of the given expression
		/// </summary>
		/// <param name="expr">The expression to repeat</param>
		/// <param name="minOccurs">The minimum number of times <paramref name="expr"/> should occur</param>
		/// <param name="maxOccurs">The maximum number of times <paramref name="expr"/> should occur. Specify 0 or -1 for unbounded.</param>
		/// <param name="accept">The accept id to use</param>
		/// <param name="compact">True to collapse epsilons, false to generate expanded epsilons</param>
		/// <returns>A new machine representing the repeated expression</returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="minOccurs"/> or <paramref name="maxOccurs"/> is an invalid value</exception>
		public static FA Repeat(FA expr,
			int minOccurs = 0,
			int maxOccurs = 0,
			int accept = 0,
			bool compact = true)
		{
			if (minOccurs < 0) minOccurs = 0;
			if (maxOccurs < 0) maxOccurs = 0;
			expr = expr.Clone();
			if (minOccurs > 0 && maxOccurs > 0 && minOccurs > maxOccurs)
				throw new ArgumentOutOfRangeException(nameof(maxOccurs));
			FA result;
			switch (minOccurs)
			{
				case 0: // lower bound unbounded. whole expression is optional
					switch (maxOccurs)
					{
						case 0:
							result = new FA(accept);
							result.AddEpsilon(expr, compact);
							foreach (var afa in expr.FillFind(AcceptingFilter))
							{
								afa.AddEpsilon(result, compact);
							}
							return result;
						case 1:
							result = Optional(expr, accept, compact);
							return result;
						default:
							var l = new List<FA>();
							expr = Optional(expr, accept, compact);
							l.Add(expr);
							for (int i = 1; i < maxOccurs; ++i)
							{
								l.Add(expr.Clone());
							}
							result = Concat(l, accept, compact);
							return result;
					}
				case 1:
					switch (maxOccurs)
					{
						case 0:
							result = Repeat(expr, 0, 0, accept, compact);
							result.AcceptSymbol = -1;
							return result;
						case 1:
							return expr;
						default:
							result = Concat(
								new FA[] { expr,
									Repeat(expr,
									0,
									maxOccurs - 1,
									accept,
									compact) },
								accept,
								compact);
							return result;
					}
				default:
					switch (maxOccurs)
					{
						case 0:
							result = Concat(
								new FA[] {
									Repeat(expr,
										minOccurs,
										minOccurs,
										accept,
										compact),
									Repeat(expr,
										0,
										0,
										accept,
										compact) },
								accept,
								compact);
							return result;
						case 1:
							throw new ArgumentOutOfRangeException(nameof(maxOccurs));
						default:
							if (minOccurs == maxOccurs)
							{
								var l = new List<FA>();
								l.Add(expr);
								for (int i = 1; i < minOccurs; ++i)
								{
									var e = expr.Clone();
									l.Add(e);
								}
								result = Concat(l, accept);
								return result;
							}
							result = Concat(new FA[] {
								Repeat(
									expr,
									minOccurs,
									minOccurs,
									accept,
									compact),
								Repeat(
									Optional(
										expr,
										accept,
										compact),
									maxOccurs - minOccurs,
									maxOccurs - minOccurs,
									accept,
									compact) },
								accept,
								compact);
							return result;
					}
			}
		}
		/// <summary>
		/// Makes a machine case insensitive
		/// </summary>
		/// <param name="expr">The expression to make case insensitive</param>
		/// <returns>A case insensitive copy of the machine</returns>
		/// <exception cref="NotSupportedException">A range could not be made case insensitive because one of the codepoints was not a letter.</exception>
		public static FA CaseInsensitive(FA expr)
		{
			const string emsg = "Attempt to make an invalid range case insensitive";
			var result = expr.Clone();
			var closure = new List<FA>();
			result.FillClosure(closure);
			for (int ic = closure.Count, i = 0; i < ic; ++i)
			{
				var fa = closure[i];
				var t = new List<FATransition>(fa._transitions);
				fa.ClearTransitions();
				foreach (var trns in t)
				{
					var f = char.ConvertFromUtf32(trns.Min);
					var l = char.ConvertFromUtf32(trns.Max);
					if (char.IsLower(f, 0))
					{
						if (!char.IsLower(l, 0))
							throw new NotSupportedException(emsg);
						fa.AddTransition(new FARange(
								trns.Min,
								trns.Max),
							trns.To);
						f = f.ToUpperInvariant();
						l = l.ToUpperInvariant();
						fa.AddTransition(new FARange(
								char.ConvertToUtf32(f, 0),
								char.ConvertToUtf32(l, 0)),
							trns.To);

					}
					else if (char.IsUpper(f, 0))
					{
						if (!char.IsUpper(l, 0))
							throw new NotSupportedException(emsg);
						fa.AddTransition(new FARange(trns.Min, trns.Max), trns.To);
						f = f.ToLowerInvariant();
						l = l.ToLowerInvariant();
						fa.AddTransition(new FARange(
								char.ConvertToUtf32(f, 0),
								char.ConvertToUtf32(l, 0)),
							trns.To);
					}
					else
					{
						fa.AddTransition(new FARange(
								trns.Min,
								trns.Max),
							trns.To);
					}
				}
			}
			return result;
		}
	}
}
