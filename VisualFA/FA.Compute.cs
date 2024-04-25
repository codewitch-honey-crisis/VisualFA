using System;
using System.Collections.Generic;
namespace VisualFA
{
	#region FAFindFilter
	/// <summary>
	/// The filter predicate delegate for <see cref="FA.FindFirst(FAFindFilter)" and <see cref="FA.FillFind(FAFindFilter, IList{FA})"/>/>
	/// </summary>
	/// <param name="state">The state to check</param>
	/// <returns>True if matched, otherwise false</returns>
#if FALIB
	public
#endif
	delegate bool FAFindFilter(FA state);
	#endregion // FAFindFilter
	partial class FA
	{
		// caching for find and closure functions
		[ThreadStatic] static HashSet<FA> _Seen = new HashSet<FA>();
		void _Closure(IList<FA> result)
		{
			if (!_Seen.Add(this))
			{
				return;
			}
			result.Add(this);
			for (int ic = _transitions.Count, i = 0; i < ic; ++i)
			{
				var t = _transitions[i];
				t.To._Closure(result);
			}
		}
		/// <summary>
		/// Computes the closure of this state into a list.
		/// </summary>
		/// <remarks>The closure is the list of states reachable from this state including itself. It essentially lists the states that make up the machine. This state is always the first state in the list.</remarks>
		/// <param name="result">The list to fill</param>
		/// <returns>A list filled with the closure. If <paramref name="result"/> is specified, that instance will be filled and returned. Otherwise a new list is filled and returned.</returns>
		public IList<FA> FillClosure(IList<FA> result = null)
		{
			if (null == result)
				result = new List<FA>();
			if (_Seen == null) { _Seen = new HashSet<FA>(); }
			_Seen.Clear();
			_Closure(result);
			return result;
		}
		IList<FA> _EpsilonClosure(IList<FA> result, HashSet<FA> seen)
		{
			if (!seen.Add(this))
			{
				return result;
			}
			result.Add(this);
			if (IsCompact)
			{
				return result;
			}
			for (int i = 0; i < _transitions.Count; ++i)
			{
				var t = _transitions[i];
				if (t.Min == -1 && t.Max == -1)
				{
					if (t.To.IsCompact)
					{
						if (seen.Add(t.To))
						{
							result.Add(t.To);
						}
					}
					else
					{
						t.To._EpsilonClosure(result, seen);
					}
				}
				else
				{
					// end of epsilons. we're done
					break;
				}
			}
			return result;
		}
		/// <summary>
		/// Computes the total epsilon closure of a list of states
		/// </summary>
		/// <remarks>The epsilon closure is the list of all states reachable from these states on no input.</remarks>
		/// <param name="states">The states to compute on</param>
		/// <param name="result">The result to fill, or null if a new list is to be returned. This parameter is required in order to disambiguate with the instance method of the same name.</param>
		/// <returns></returns>
		public static IList<FA> FillEpsilonClosure(IList<FA> states, IList<FA> result = null)
		{
			if (null == result)
				result = new List<FA>();
			if (_Seen == null) { _Seen = new HashSet<FA>(); }
			_Seen.Clear();
			for (int i = 0; i < states.Count; ++i)
			{
				var fa = states[i];
				fa._EpsilonClosure(result, _Seen);
			}
			return result;
		}
		/// <summary>
		/// Computes the total epsilon closure of a list of states
		/// </summary>
		/// <remarks>The epsilon closure is the list of all states reachable from these states on no input.</remarks>
		/// <param name="state">The state to compute on</param>
		/// <param name="result">The result to fill, or null if a new list is to be returned. This parameter is required in order to disambiguate with the instance method of the same name.</param>
		/// <returns></returns>
		public static IList<FA> FillEpsilonClosure(FA state, IList<FA> result = null)
		{
			if (null == result)
				result = new List<FA>();
			if (_Seen == null) { _Seen = new HashSet<FA>(); }
			_Seen.Clear();
			state._EpsilonClosure(result, _Seen);
			return result;
		}
		void _Find(FAFindFilter filter, IList<FA> result)
		{
			if (!_Seen.Add(this))
			{
				return;
			}
			if (filter(this))
			{
				result.Add(this);
			}
			for (int i = 0; i < _transitions.Count; ++i)
			{
				var t = _transitions[i];
				t.To._Find(filter, result);
			}
		}
		/// <summary>
		/// Finds states within the closure that match the filter criteria
		/// </summary>
		/// <param name="filter">The filter predicate to use.</param>
		/// <param name="result">The result to fill</param>
		/// <returns>A list filled with the result of the find. If <paramref name="result"/> is specified, that instance will be filled and returned. Otherwise a new list is filled and returned.</returns>
		public IList<FA> FillFind(FAFindFilter filter, IList<FA> result = null)
		{
			if (null == result)
				result = new List<FA>();
			if (_Seen == null) { _Seen = new HashSet<FA>(); }
			_Seen.Clear();
			_Find(filter, result);
			return result;
		}
		FA _FindFirst(FAFindFilter filter)
		{
			if (!_Seen.Add(this))
			{
				return null;
			}
			if (filter(this))
			{
				return this;
			}
			for (int ic = _transitions.Count, i = 0; i < ic; ++i)
			{
				var t = _transitions[i];
				var fa = t.To._FindFirst(filter);
				if (null != fa)
				{
					return fa;
				}
			}
			return null;
		}
		/// <summary>
		/// Finds the first state within the closure that matches the filter critera
		/// </summary>
		/// <param name="filter">The filter predicate to use</param>
		/// <returns>The first state that matches the filter criteria, or null if not found.</returns>
		public FA FindFirst(FAFindFilter filter)
		{
			if (_Seen == null) { _Seen = new HashSet<FA>(); }
			_Seen.Clear();
			var result = _FindFirst(filter);
			return result;
		}
		/// <summary>
		/// Performs subset construction to turn an NFA machine into a DFA machine
		/// </summary>
		/// <remarks>The result may have duplicate states. Use ToMinimizedDfa() to return a DFA with duplicates removed. On a deteriministic machine, this method merely clones.</remarks>
		/// <param name="progress">The progress indicator</param>
		/// <returns>A deterministic copy of this machine that accepts the same input</returns>
		public FA ToDfa(IProgress<int> progress = null)
		{
			return _Determinize(this, progress);
		}
		/// <summary>
		/// Indicates whether the entire machine is deterministic or not
		/// </summary>
		/// <param name="closure">The closure of the machine</param>
		/// <returns>True if the machine is deterministic, otherwise false</returns>
		public static bool IsDfa(IList<FA> closure)
		{
			for (int q = 0; q < closure.Count; ++q)
			{
				if (!closure[q].IsDeterministic)
				{
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// Indiciates whether the entire machine is deterministic or not
		/// </summary>
		/// <returns></returns>
		public bool IsDfa()
		{
			return FindFirst(new FAFindFilter((fa) => !IsDeterministic)) == null;
		}
		/// <summary>
		/// Performs subset construction to turn an NFA or DFA machine into a minimized DFA machine
		/// </summary>
		/// <remarks>The result will not have duplicate states.</remarks>
		/// <param name="progress"></param>
		/// <returns>A deterministic copy of this machine that accepts the same input</returns>
		public FA ToMinimizedDfa(IProgress<int> progress = null)
		{
			return _Minimize(this, progress);
		}
		/// <summary>
		/// Returns the NFA equivelent of this DFA
		/// </summary>
		/// <param name="compact">True to produce a compact NFA, false to produce an expanded NFA</param>
		/// <returns>A new NFA state machine that is the equivelent of this machine</returns>
		/// <remarks>This will flatten a lexer. This can be used to expand a compact NFA as well as a DFA.</remarks>
		public FA ToNfa(bool compact = true)
		{
			var exp = _ToExpression(this);
			var accept = FA.GetFirstAcceptSymbol(FillClosure());
			return FA.Parse(exp, accept, compact);
		}
		#region Totalize()
		/// <summary>
		/// For this machine, fills and sorts transitions such that any missing range now points to an empty non-accepting state
		/// </summary>
		public void Totalize()
		{
			Totalize(FillClosure());
		}
		/// <summary>
		/// For this closure, fills and sorts transitions such that any missing range now points to an empty non-accepting state
		/// </summary>
		/// <param name="closure">The closure to totalize</param>
		public static void Totalize(IList<FA> closure)
		{
			var s = new FA();
			s._transitions.Add(new FATransition(s, 0, 0x10ffff));
			foreach (FA p in closure)
			{
				int maxi = 0;
				var sortedTrans = new List<FATransition>(p._transitions);
				sortedTrans.Sort(_TransitionComparison);
				foreach (var t in sortedTrans)
				{
					if (t.Min == -1 && t.Max == -1)
					{
						continue;
					}
					if (t.Min > maxi)
					{
						p._transitions.Add(new FATransition(s, maxi, (t.Min - 1)));
					}

					if (t.Max + 1 > maxi)
					{
						maxi = t.Max + 1;
					}
				}

				if (maxi <= 0x10ffff)
				{
					p._transitions.Add(new FATransition(s, maxi, 0x10ffff));
				}
			}
		}

		#endregion //Totalize()
		#region _Minimize()
		static void _Init<T>(IList<T> list, int count)
		{
			for (int i = 0; i < count; ++i)
			{
				list.Add(default(T));
			}
		}
		static FA _Minimize(FA a, IProgress<int> progress)
		{
			int prog = 0;
			progress?.Report(prog);

			a = a.ToDfa(progress);
			var tr = a._transitions;
			if (1 == tr.Count)
			{
				FATransition t = tr[0];
				if (t.To == a && t.Min == 0 && t.Max == 0x10ffff)
				{
					return a;
				}
			}

			a.Totalize();
			prog = 1;
			progress?.Report(prog);
			// Make arrays for numbered states and effective alphabet.
			var cl = a.FillClosure();
			var states = new FA[cl.Count];
			int number = 0;
			foreach (var q in cl)
			{
				states[number] = q;
				q._MinimizationTag = number;
				++number;
			}

			var pp = new List<int>();
			for (int ic = cl.Count, i = 0; i < ic; ++i)
			{
				var ffa = cl[i];
				pp.Add(0);
				foreach (var t in ffa._transitions)
				{
					pp.Add(t.Min);
					if (t.Max < 0x10ffff)
					{
						pp.Add((t.Max + 1));
					}
				}
			}

			var sigma = new int[pp.Count];
			pp.CopyTo(sigma, 0);
			Array.Sort(sigma);

			// Initialize data structures.
			var reverse = new List<List<Queue<FA>>>();
			foreach (var s in states)
			{
				var v = new List<Queue<FA>>();
				_Init(v, sigma.Length);
				reverse.Add(v);
			}
			prog = 2;
			if (progress != null) { progress.Report(prog); }
			var reverseNonempty = new bool[states.Length, sigma.Length];

			var partition = new List<LinkedList<FA>>();
			_Init(partition, states.Length);
			prog = 3;
			if (progress != null) { progress.Report(prog); }
			var block = new int[states.Length];
			var active = new _FList[states.Length, sigma.Length];
			var active2 = new _FListNode[states.Length, sigma.Length];
			var pending = new Queue<KeyValuePair<int, int>>();
			var pending2 = new bool[sigma.Length, states.Length];
			var split = new List<FA>();
			var split2 = new bool[states.Length];
			var refine = new List<int>();
			var refine2 = new bool[states.Length];

			var splitblock = new List<List<FA>>();
			_Init(splitblock, states.Length);
			prog = 4;
			progress?.Report(prog);
			for (int q = 0; q < states.Length; q++)
			{
				splitblock[q] = new List<FA>();
				partition[q] = new LinkedList<FA>();
				for (int x = 0; x < sigma.Length; x++)
				{
					reverse[q][x] = new Queue<FA>();
					active[q, x] = new _FList();
				}
			}

			// Find initial partition and reverse edges.
			foreach (var qq in states)
			{
				int j = qq.IsAccepting ? 0 : 1;

				partition[j]?.AddLast(qq);
				block[qq._MinimizationTag] = j;
				for (int x = 0; x < sigma.Length; x++)
				{
					var y = sigma[x];
					var p = qq._Step(y);
					System.Diagnostics.Debug.Assert(p != null);
					var pn = p._MinimizationTag;
					reverse[pn]?[x]?.Enqueue(qq);
					reverseNonempty[pn, x] = true;
				}
				++prog;
				progress?.Report(prog);
			}

			// Initialize active sets.
			for (int j = 0; j <= 1; j++)
			{
				for (int x = 0; x < sigma.Length; x++)
				{
					var part = partition[j];
					System.Diagnostics.Debug.Assert(part != null);
					foreach (var qq in part)
					{
						System.Diagnostics.Debug.Assert(qq != null);
						if (reverseNonempty[qq._MinimizationTag, x])
						{
							active2[qq._MinimizationTag, x] = active[j, x].Add(qq);
						}
					}
				}
				++prog;
				progress?.Report(prog);
			}

			// Initialize pending.
			for (int x = 0; x < sigma.Length; x++)
			{
				int a0 = active[0, x].Count;
				int a1 = active[1, x].Count;
				int j = a0 <= a1 ? 0 : 1;
				pending.Enqueue(new KeyValuePair<int, int>(j, x));
				pending2[x, j] = true;
			}

			// Process pending until fixed point.
			int k = 2;
			while (pending.Count > 0)
			{
				KeyValuePair<int, int> ip = pending.Dequeue();
				int p = ip.Key;
				int x = ip.Value;
				pending2[x, p] = false;

				// Find states that need to be split off their blocks.
				for (var m = active[p, x].First; m != null; m = m.Next)
				{
					System.Diagnostics.Debug.Assert(m.State != null);
					foreach (var s in reverse[m.State._MinimizationTag][x])
					{
						if (!split2[s._MinimizationTag])
						{
							split2[s._MinimizationTag] = true;
							split.Add(s);
							int j = block[s._MinimizationTag];
							splitblock[j]?.Add(s);
							if (!refine2[j])
							{
								refine2[j] = true;
								refine.Add(j);
							}
						}
					}
				}
				++prog;
				if (progress != null) { progress.Report(prog); }
				// Refine blocks.
				foreach (int j in refine)
				{
					if (splitblock[j]?.Count < partition[j]?.Count)
					{
						LinkedList<FA> b1 = partition[j];
						System.Diagnostics.Debug.Assert(b1 != null);
						LinkedList<FA> b2 = partition[k];
						System.Diagnostics.Debug.Assert(b2 != null);
						var e = splitblock[j];
						System.Diagnostics.Debug.Assert(e != null);
						foreach (var s in e)
						{
							b1.Remove(s);
							b2.AddLast(s);
							block[s._MinimizationTag] = k;
							for (int c = 0; c < sigma.Length; c++)
							{
								_FListNode sn = active2[s._MinimizationTag, c];
								if (sn != null && sn.StateList == active[j, c])
								{
									sn.Remove();
									active2[s._MinimizationTag, c] = active[k, c].Add(s);
								}
							}
						}

						// Update pending.
						for (int c = 0; c < sigma.Length; c++)
						{
							int aj = active[j, c].Count;
							int ak = active[k, c].Count;
							if (!pending2[c, j] && 0 < aj && aj <= ak)
							{
								pending2[c, j] = true;
								pending.Enqueue(new KeyValuePair<int, int>(j, c));
							}
							else
							{
								pending2[c, k] = true;
								pending.Enqueue(new KeyValuePair<int, int>(k, c));
							}
						}

						k++;
					}
					var sbj = splitblock[j];
					System.Diagnostics.Debug.Assert(sbj != null);
					foreach (var s in sbj)
					{
						split2[s._MinimizationTag] = false;
					}

					refine2[j] = false;
					//splitblock[j].Clear();
					sbj.Clear();
					++prog;
					if (progress != null) { progress.Report(prog); }
				}

				split.Clear();
				refine.Clear();
			}
			++prog;
			if (progress != null) { progress.Report(prog); }
			// Make a new state for each equivalence class, set initial state.
			var newstates = new FA[k];
			for (int n = 0; n < newstates.Length; n++)
			{
				var s = new FA();
				s.IsDeterministic = true;
				newstates[n] = s;
				var pn = partition[n];
				System.Diagnostics.Debug.Assert(pn != null);
				foreach (var q in pn)
				{
					if (q == a)
					{
						a = s;
					}
					s.Id = q.Id;
					s.AcceptSymbol = q.AcceptSymbol;
					s._MinimizationTag = q._MinimizationTag; // Select representative.				
					q._MinimizationTag = n;
				}
				++prog;
				progress?.Report(prog);
			}

			// Build transitions and set acceptance.
			foreach (var s in newstates)
			{
				var st = states[s._MinimizationTag];
				s.AcceptSymbol = st.AcceptSymbol;
				foreach (var t in st._transitions)
				{
					s._transitions.Add(new FATransition(newstates[t.To._MinimizationTag], t.Min, t.Max));
				}
				++prog;
				progress?.Report(prog);
			}
			// remove dead transitions
			foreach (var ffa in a.FillClosure())
			{
				var itrns = new List<FATransition>(ffa._transitions);
				foreach (var trns in itrns)
				{
					if (null == trns.To.FindFirst(AcceptingFilter))
					{
						ffa._transitions.Remove(trns);
					}
				}
			}
			return a;
		}
		FA _Step(int input)
		{
			for (int ic = _transitions.Count, i = 0; i < ic; ++i)
			{
				var t = _transitions[i];
				if (t.Min <= input && input <= t.Max)
					return t.To;

			}
			return null;
		}
		#endregion // _Minimize()
		#region Compact()
		/// <summary>
		/// Collapses epsilon transitions
		/// </summary>
		/// <param name="closure">The closure to collapse</param>
		public static void Compact(IList<FA> closure)
		{
			var done = false;
			while (!done)
			{
				done = true;
				for (int i = 0; i < closure.Count; ++i)
				{
					var fa = closure[i];
					for (int j = 0; j < fa._transitions.Count; ++j)
					{
						var fat = fa._transitions[j];
						if (fat.Min == -1 && fat.Max == -1)
						{
							fa._transitions.RemoveAt(j);
							--j;
							fa.AddEpsilon(fat.To);
							done = false;
							break;
						}
					}
					if (!done)
					{
						closure = closure[0].FillClosure();
						break;
					}
					fa.IsCompact = true;
				}
			}


		}
		/// <summary>
		/// Collapses the epsilons on the current state machine.
		/// </summary>
		public void Compact()
		{
			Compact(FillClosure());
		}
		#endregion // Compact()
		#region _Determinize()
		private static FA _Determinize(FA fa, IProgress<int> progress)
		{
			// initialize
			int prog = 0;
			progress?.Report(prog);
			var p = new HashSet<int>();
			var closure = new List<FA>();
			fa.FillClosure(closure);
			// gather our input alphabet
			for (int ic = closure.Count, i = 0; i < ic; ++i)
			{
				var ffa = closure[i];
				p.Add(0);
				foreach (var t in ffa._transitions)
				{
					if (t.Min == -1 && t.Max == -1)
					{
						continue;
					}
					p.Add(t.Min);
					if (t.Max < 0x10ffff)
					{
						p.Add((t.Max + 1));
					}
				}
			}

			var points = new int[p.Count];
			p.CopyTo(points, 0);
			Array.Sort(points);
			++prog;
			progress?.Report(prog);
			var sets = new Dictionary<_KeySet<FA>, _KeySet<FA>>();
			var working = new Queue<_KeySet<FA>>();
			var dfaMap = new Dictionary<_KeySet<FA>, FA>();
			var initial = new _KeySet<FA>();
			var epscl = new List<FA>();
			List<FA> ecs = new List<FA>();
			List<FA> efcs = null;
			if (_Seen == null) { _Seen = new HashSet<FA>(); }
			_Seen.Clear();
			fa._EpsilonClosure(epscl, _Seen);
			for (int i = 0; i < epscl.Count; ++i)
			{
				var efa = epscl[i];
				initial.Add(efa);
			}
			sets.Add(initial, initial);
			working.Enqueue(initial);
			var result = new FA();
			result.IsDeterministic = true;
			result.FromStates = epscl.ToArray();
			foreach (var afa in initial)
			{
				if (afa.IsAccepting)
				{
					result.AcceptSymbol = afa.AcceptSymbol;
					break;
				}
			}
			++prog;
			progress?.Report(prog);
			// powerset/subset construction
			dfaMap.Add(initial, result);
			while (working.Count > 0)
			{
				// get the next set
				var s = working.Dequeue();
				// get the next DFA out of the map
				// of (NFA states)->(dfa state)
				FA dfa = dfaMap[s];
				// find the first accepting 
				// state if any, and assign
				// it to the new DFA
				foreach (FA q in s)
				{
					if (q.IsAccepting)
					{
						dfa.AcceptSymbol = q.AcceptSymbol;
						break;
					}
				}
				// for each range in the input alphabet
				for (var i = 0; i < points.Length; i++)
				{
					var pnt = points[i];
					var set = new _KeySet<FA>();
					foreach (FA c in s)
					{
						// TODO: Might be able to eliminate the
						// epsilon closure here. Needs testing
						ecs.Clear();
						if (!c.IsCompact)
						{
							// use the internal _EpsilonClosure
							// method to avoid an extra call
							// (basically inlining it)
							if (_Seen == null) { _Seen = new HashSet<FA>(); }
							_Seen.Clear();
							c._EpsilonClosure(ecs, _Seen);
						}
						else
						{
							ecs.Add(c);
						}
						for (int j = 0; j < ecs.Count; ++j)
						{
							var efa = ecs[j];
							// basically if we intersect somewhere on the input alphabet,
							// which we should, then we add the destination state(s) to the set
							for (int k = 0; k < efa._transitions.Count; ++k)
							{
								var trns = efa._transitions[k];
								if (trns.Min == -1 && trns.Max == -1)
								{
									continue;
								}
								// TODO: can probably early out here
								// if pnt > trns.Max?
								if (trns.Min <= pnt && pnt <= trns.Max)
								{
									// skip the epsilon closure
									// we don't need it
									if (trns.To.IsCompact)
									{
										set.Add(trns.To);
									}
									else
									{
										if (efcs == null)
										{
											efcs = new List<FA>();
										}
										efcs.Clear();
										if (_Seen == null) { _Seen = new HashSet<FA>(); }
										_Seen.Clear();
										trns.To._EpsilonClosure(efcs, _Seen);
										for (int m = 0; m < efcs.Count; ++m)
										{
											set.Add(efcs[m]);
										}
									}
								}
							}
						}
						// less GC stress
						_Seen.Clear();
					}
					// is this a new set or an
					// existing one?
					if (!sets.ContainsKey(set))
					{
						sets.Add(set, set);
						// add another work item
						working.Enqueue(set);
						// make a new DFA state
						var newfa = new FA();
						newfa.IsDeterministic = true;
						newfa.IsCompact = true;
						dfaMap.Add(set, newfa);
						var fas = new List<FA>(set);
						// TODO: we should really sort fas
						newfa.FromStates = fas.ToArray();
					}

					FA dst = dfaMap[set];
					// find the first and last range to insert
					int first = pnt;
					int last;
					if (i + 1 < points.Length)
					{
						last = (points[i + 1] - 1);
					}
					else
					{
						last = 0x10ffff;
					}
					// this should already be in sorted order
					// otherwise we'd use AddTransition()
					dfa._transitions.Add(new FATransition(dst, first, last));

					++prog;
					progress?.Report(prog);
				}
				++prog;
				progress?.Report(prog);

			}
			// remove dead transitions (destinations with no accept)
			foreach (var ffa in result.FillClosure())
			{
				var itrns = new List<FATransition>(ffa._transitions);
				foreach (var trns in itrns)
				{
					var acc = trns.To.FindFirst(AcceptingFilter);
					if (acc == null)
					{
						ffa._transitions.Remove(trns);
					}
				}
				++prog;
				progress?.Report(prog);
			}
			++prog;
			progress?.Report(prog);
			return result;
		}
		#endregion // _Determinize()
		/// <summary>
		/// Retrieves a transition index given a specified UTF32 codepoint
		/// </summary>
		/// <param name="codepoint">The codepoint</param>
		/// <returns>The index of the matching transition or a negative number if no match was found.</returns>
		public int FindFirstTransitionIndex(int codepoint)
		{
			for (var i = 0; i < _transitions.Count; ++i)
			{
				var t = _transitions[i];
				if (t.Min == -1 && t.Max == -1)
				{
					continue;
				}
				if (t.Min > codepoint)
				{
					return -1;
				}
				if (t.Max >= codepoint)
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Computes a dictionary keyed by states, whose values are the ranges that lead to that state packed as an integer array.
		/// </summary>
		/// <param name="includeEpsilons">Indicates that epsilon transitions should be included in the result</param>
		/// <param name="result">The dictionary to fill, or null for a new dictionary</param>
		/// <returns>A dictionary of sorted <see cref="IList{FARange}"/> instances grouped by destination state. If <paramref name="result"/> was specified, that will be returned.</returns>
		public IDictionary<FA, IList<FARange>> FillInputTransitionRangesGroupedByState(bool includeEpsilons = false, IDictionary<FA, IList<FARange>> result = null)
		{
			if (null == result)
				result = new Dictionary<FA, IList<FARange>>();

			foreach (var trns in _transitions)
			{
				if (!includeEpsilons && (trns.Min == -1 && trns.Max == -1))
				{
					continue;
				}
				IList<FARange> l;
				if (!result.TryGetValue(trns.To, out l))
				{
					l = new List<FARange>();
					result.Add(trns.To, l);
				}
				l.Add(new FARange(trns.Min, trns.Max));
			}
			foreach (var item in result)
			{
				((List<FARange>)item.Value).Sort((x, y) => { var c = x.Min.CompareTo(y.Min); if (0 != c) return c; return x.Max.CompareTo(y.Max); });
				_NormalizeSortedRangeList(item.Value);
			}
			return result;
		}
		static void _NormalizeSortedRangeList(IList<FARange> pairs)
		{
			for (int i = 1; i < pairs.Count; ++i)
			{
				if (pairs[i - 1].Max + 1 >= pairs[i].Min)
				{
					var nr = new FARange(pairs[i - 1].Min, pairs[i].Max);
					pairs[i - 1] = nr;
					pairs.RemoveAt(i);
					--i; // compensated for by ++i in for loop
				}
			}
		}
		/// <summary>
		/// Retrieves all transition indices given a specified UTF32 codepoint
		/// </summary>
		/// <param name="codepoint">The codepoint</param>
		/// <returns>The indices of the matching transitions or empty if no match was found.</returns>
		public int[] FindTransitionIndices(int codepoint)
		{
			var result = new List<int>(_transitions.Count);
			for (var i = 0; i < _transitions.Count; ++i)
			{
				var t = _transitions[i];
				if (t.Min == -1 && t.Max == -1)
				{
					result.Add(i);
				}
				else if (t.Min <= codepoint && t.Max >= codepoint)
				{
					result.Add(i);
				}
			}
			return result.ToArray();
		}
		/// <summary>
		/// Fills a list with all of the new states after moving from a given set of states along a given input. (NFA-move)
		/// </summary>
		/// <param name="states">The current states</param>
		/// <param name="codepoint">The codepoint to move on</param>
		/// <param name="result">A list to hold the next states. If null, one will be created.</param>
		/// <returns>The list of next states</returns>
		public static IList<FA> FillMove(IList<FA> states, int codepoint, IList<FA> result = null)
		{
			if (_Seen == null) { _Seen = new HashSet<FA>(); }
			_Seen.Clear();
			if (result == null) result = new List<FA>();
			for (int q = 0; q < states.Count; ++q)
			{
				var state = states[q];
				for (int i = 0; i < state._transitions.Count; ++i)
				{
					var fat = state._transitions[i];
					// epsilon dsts should already be in states:
					if (fat.Min == -1 && fat.Max == -1)
					{
						continue;
					}
					if (codepoint < fat.Min)
					{
						break;
					}
					if (codepoint <= fat.Max)
					{
						fat.To._EpsilonClosure(result, _Seen);
					}
				}
			}
			_Seen.Clear();
			return result;
		}
		/// <summary>
		/// Returns the next state
		/// </summary>
		/// <param name="codepoint">The codepoint to move on</param>
		/// <returns>The next state, or null if there was no valid move.</returns>
		/// <remarks>The transition ranges must be in sorted order. This machine must be a DFA or this will error. Use FillMove() to work with any (slower).</remarks>
		public FA Move(int codepoint)
		{
			if (!IsDeterministic)
			{
				throw new InvalidOperationException("The state machine must be deterministic");
			}
			for (int i = 0; i < _transitions.Count; ++i)
			{
				var fat = _transitions[i];
				if (codepoint < fat.Min)
				{
					return null;
				}
				if (codepoint <= fat.Max)
				{
					return fat.To;
				}
			}
			return null;
		}
		#region _KeySet
		private sealed class _KeySet<T> : ISet<T>, IEquatable<_KeySet<T>>
		{
			HashSet<T> _inner;
			int _hashCode;
			public _KeySet(IEqualityComparer<T> comparer)
			{
				_inner = new HashSet<T>(comparer);
				_hashCode = 0;
			}
			public _KeySet()
			{
				_inner = new HashSet<T>();
				_hashCode = 0;
			}
			public int Count => _inner.Count;

			public bool IsReadOnly => true;

			// hack - we allow this method so the set can be filled
			public bool Add(T item)
			{
				if (null != item)
					_hashCode ^= item.GetHashCode();
				return _inner.Add(item);
			}
			bool ISet<T>.Add(T item)
			{
				_ThrowReadOnly();
				return false;
			}
			public void Clear()
			{
				_ThrowReadOnly();
			}

			public bool Contains(T item)
			{
				return _inner.Contains(item);
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				_inner.CopyTo(array, arrayIndex);
			}

			void ISet<T>.ExceptWith(IEnumerable<T> other)
			{
				_ThrowReadOnly();
			}

			public IEnumerator<T> GetEnumerator()
			{
				return _inner.GetEnumerator();
			}

			void ISet<T>.IntersectWith(IEnumerable<T> other)
			{
				_ThrowReadOnly();
			}

			public bool IsProperSubsetOf(IEnumerable<T> other)
			{
				return _inner.IsProperSubsetOf(other);
			}

			public bool IsProperSupersetOf(IEnumerable<T> other)
			{
				return _inner.IsProperSupersetOf(other);
			}

			public bool IsSubsetOf(IEnumerable<T> other)
			{
				return _inner.IsSubsetOf(other);
			}

			public bool IsSupersetOf(IEnumerable<T> other)
			{
				return _inner.IsSupersetOf(other);
			}

			public bool Overlaps(IEnumerable<T> other)
			{
				return _inner.Overlaps(other);
			}

			bool ICollection<T>.Remove(T item)
			{
				_ThrowReadOnly();
				return false;
			}

			public bool SetEquals(IEnumerable<T> other)
			{
				return _inner.SetEquals(other);
			}

			void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
			{
				_ThrowReadOnly();
			}

			void ISet<T>.UnionWith(IEnumerable<T> other)
			{
				_ThrowReadOnly();
			}

			void ICollection<T>.Add(T item)
			{
				_ThrowReadOnly();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return _inner.GetEnumerator();
			}
			static void _ThrowReadOnly()
			{
				throw new NotSupportedException("The set is read only");
			}
			public bool Equals(_KeySet<T> rhs)
			{
				if (ReferenceEquals(this, rhs))
					return true;
				if (ReferenceEquals(rhs, null))
					return false;
				if (rhs._hashCode != _hashCode)
					return false;
				var ic = _inner.Count;
				if (ic != rhs._inner.Count)
					return false;
				return _inner.SetEquals(rhs._inner);
			}
			public override int GetHashCode()
			{
				return _hashCode;
			}
		}
		#endregion // _KeySet
		#region _FList
		private sealed class _FListNode
		{
			public _FListNode(FA q, _FList sl)
			{
				State = q;
				StateList = sl;
				if (sl.Count++ == 0)
				{
					sl.First = sl.Last = this;
				}
				else
				{
					System.Diagnostics.Debug.Assert(sl.Last != null);
					sl.Last.Next = this;
					Prev = sl.Last;
					sl.Last = this;

				}
			}

			public _FListNode Next { get; private set; }

			private _FListNode Prev { get; set; }

			public _FList StateList { get; private set; }

			public FA State { get; private set; }

			public void Remove()
			{
				System.Diagnostics.Debug.Assert(StateList != null);
				StateList.Count--;
				if (StateList.First == this)
				{
					StateList.First = Next;
				}
				else
				{
					System.Diagnostics.Debug.Assert(Prev != null);
					Prev.Next = Next;
				}

				if (StateList.Last == this)
				{
					StateList.Last = Prev;
				}
				else
				{
					System.Diagnostics.Debug.Assert(Next != null);
					Next.Prev = Prev;
				}
			}
		}
		private sealed class _FList
		{
			public int Count { get; set; }

			public _FListNode First { get; set; }

			public _FListNode Last { get; set; }

			public _FListNode Add(FA q)
			{
				return new _FListNode(q, this);
			}
		}
		#endregion // _FList
		/// <summary>
		/// A filter that returns any accepting state
		/// </summary>
		public static readonly FAFindFilter AcceptingFilter = new FAFindFilter((FA state) => { return state.IsAccepting; });
		/// <summary>
		/// A filter that returns any final states
		/// </summary>
		public static readonly FAFindFilter FinalFilter = new FAFindFilter((FA state) => { return state.IsFinal; });
		/// <summary>
		/// A filter that returns any neutral states
		/// </summary>
		public static readonly FAFindFilter NeutralFilter = new FAFindFilter((FA state) => { return state.IsNeutral; });
		/// <summary>
		/// A filter that returns any trap states
		/// </summary>
		public static readonly FAFindFilter TrapFilter = new FAFindFilter((FA state) => { return state.IsTrap; });

	}
}
