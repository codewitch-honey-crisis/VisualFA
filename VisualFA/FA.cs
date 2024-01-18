using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace VisualFA
{
	#region FAMatch
	/// <summary>
	/// Represents a match from <code>FARunner.MatchNext()</code>
	/// </summary>
#if FALIB
	public
#endif
	partial struct FAMatch
	{
		/// <summary>
		/// The matched symbol - this is the accept id, or less than zero if the text did not match an expression
		/// </summary>
		public int SymbolId;
		/// <summary>
		/// The matched value
		/// </summary>
		public string Value;
		/// <summary>
		/// The position of the match within the codepoint series - this may not be the same as the character position due to surrogates
		/// </summary>
		public long Position;
		/// <summary>
		/// The one based line number
		/// </summary>
		public int Line;
		/// <summary>
		/// The one based column
		/// </summary>
		public int Column;
		/// <summary>
		/// Indicates whether the text matched the expression
		/// </summary>
		/// <remarks>Non matches are returned with negative accept symbols. You can use this property to determine if the text therein was part of a match.</remarks>
		public bool IsSuccess {
			get {
				return SymbolId > -1;
			}
		}
		/// <summary>
		/// Constructs a new instance
		/// </summary>
		/// <param name="symbolId">The symbol id</param>
		/// <param name="value">The matched value</param>
		/// <param name="position">The absolute codepoint position</param>
		/// <param name="line">The line</param>
		/// <param name="column">The column</param>

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAMatch Create(int symbolId, string value, long position, int line, int column)
		{
			FAMatch result = default(FAMatch);
			result.SymbolId = symbolId;
			result.Value = value;
			result.Position = position;
			result.Line = line;
			result.Column = column;
			return result;
		}
	}
	#endregion // FAMatch

	#region FARange
	/// <summary>
	/// Represents a range of codepoints
	/// </summary>
#if FALIB
	public 
#endif
	struct FARange {
		public int Min;
		public int Max;
		public FARange(int min, int max)
		{
			Min = min;
			Max = max;
		}
		/// <summary>
		/// Indicates whether this range intersects with another range
		/// </summary>
		/// <param name="rhs">The range to compare</param>
		/// <returns></returns>
		public bool Intersects(FARange rhs)
		{
			return (rhs.Min >= Min && rhs.Min <= Max) ||
				rhs.Max >= Min && rhs.Max <= Max;
		}
		/// <summary>
		/// Indicates whether or not this codepoint intersects this range
		/// </summary>
		/// <param name="codepoint">The codepoint</param>
		/// <returns>True if the codepoint is part of the range, otherwise false</returns>
		public bool Intersects(int codepoint)
		{
			return codepoint >= Min && codepoint <= Max;
		}
		/// <summary>
		/// Turns packed ranges into unpacked ranges
		/// </summary>
		/// <param name="packedRanges">The ranges to unpack</param>
		/// <returns>The unpacked ranges</returns>
		public static FARange[] ToUnpacked(int[] packedRanges)
		{
			var result = new FARange[packedRanges.Length / 2];
			for (var i = 0; i < result.Length; ++i)
			{
				var j = i * 2;
				result[i] = new FARange(packedRanges[j], packedRanges[j + 1]);
			}
			return result;
		}
		/// <summary>
		/// Packs a series of ranges
		/// </summary>
		/// <param name="pairs">The ranges to pack</param>
		/// <returns>The packed ranges</returns>
		public static int[] ToPacked(IList<FARange> pairs)
		{
			var result = new int[pairs.Count * 2];
			for (int ic = pairs.Count, i = 0; i < ic; ++i)
			{
				var pair = pairs[i];
				var j = i * 2;
				result[j] = pair.Min;
				result[j + 1] = pair.Max;
			}
			return result;
		}
		/// <summary>
		/// Inverts a set of unpacked ranges
		/// </summary>
		/// <param name="ranges">The ranges to invert</param>
		/// <returns>The inverted ranges</returns>
		public static IEnumerable<FARange> ToNotRanges(IEnumerable<FARange> ranges)
		{
			// expects ranges to be normalized
			var last = 0x10ffff;
			using (var e = ranges.GetEnumerator())
			{
				if (!e.MoveNext())
				{
					yield return new FARange(0x0, 0x10ffff);
					yield break;
				}
				if (e.Current.Min > 0)
				{
					yield return new FARange(0, unchecked(e.Current.Min - 1));
					last = e.Current.Max;
					if (0x10ffff <= last)
						yield break;
				}
				else if (e.Current.Min == 0)
				{
					last = e.Current.Max;
					if (0x10ffff <= last)
						yield break;
				}
				while (e.MoveNext())
				{
					if (0x10ffff <= last)
						yield break;
					if (unchecked(last + 1) < e.Current.Min)
						yield return new FARange(unchecked(last + 1), unchecked((e.Current.Min - 1)));
					last = e.Current.Max;
				}
				if (0x10ffff > last)
					yield return new FARange(unchecked((last + 1)), 0x10ffff);

			}
		}
		public override string ToString()
		{
			if (Min == Max)
			{
				return string.Concat("[", char.ConvertFromUtf32(Min), "]");
			}
			return string.Concat("[", char.ConvertFromUtf32(Min), "-", char.ConvertFromUtf32(Max), "]");
		}
		public bool Equals(FARange rhs)
		{
			return rhs.Min == Min && rhs.Max == Max;
		}
		public override bool Equals(object rhs)
		{
			if (ReferenceEquals(null, rhs)) return false;
			if(rhs is FARange)
			{
				return Equals((FARange)rhs);
			}
			return base.Equals(rhs);
		}
		public override int GetHashCode()
		{
			return Min.GetHashCode() ^ Max.GetHashCode();
		}
	}
	#endregion // FARange
	#region FATransition
#if FALIB
	public 
#endif
	struct FATransition
	{
		public int Min;
		public int Max;
		public FA To;
		public FATransition(FA to, int min = -1, int max =-1)
		{
			Min = min;
			Max = max;
			To = to;
		}
		public bool IsEpsilon { get { return Min == -1 && Max == -1; } }
		public override string ToString()
		{
			if(IsEpsilon)
			{
				return string.Concat("-> ", To.ToString());
			}
			if (Min == Max)
			{
				return string.Concat("[", char.ConvertFromUtf32(Min), "]-> ", To.ToString());
			}
			return string.Concat("[", char.ConvertFromUtf32(Min),"-", char.ConvertFromUtf32(Max), "]-> ", To.ToString());
		}
	}
	#endregion // FATransition
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
	#region FAException
	[Serializable]
#if FALIB
	public
#endif
	class FAException : Exception
	{
		public FAException(SerializationInfo serializationEntries, StreamingContext context) : base(serializationEntries, context) { }
		public FAException(string message) : base(message)
		{

		}
		public FAException(string message, Exception innerException) : base(message,innerException)
		{

		}
	}
	#endregion FAException
	#region FAParseException
	/// <summary>
	/// Indicates an exception occurred parsing a regular expression
	/// </summary>
	[Serializable]
#if FALIB
	public
#endif
	sealed class FAParseException : FAException
	{
		/// <summary>
		/// Indicates the strings that were expected
		/// </summary>
		public string[] Expecting { get; } = null;
		/// <summary>
		/// Indicates the position where the error occurred
		/// </summary>
		public int Position { get; }
		/// <summary>
		/// For crossing serialization boundaries
		/// </summary>
		/// <param name="serializationEntries">The data</param>
		/// <param name="context">The context</param>
		public FAParseException(SerializationInfo serializationEntries, StreamingContext context) : base(serializationEntries, context) { }
		/// <summary>
		/// Constructs a new instance
		/// </summary>
		/// <param name="message">The error</param>
		/// <param name="position">The position</param>
		public FAParseException(string message, int position) : base(message)
		{
			Position = position;
		}
		/// <summary>
		/// Constructs a new instance
		/// </summary>
		/// <param name="message">The error</param>
		/// <param name="position">The position</param>
		/// <param name="innerException">The inner exception</param>
		public FAParseException(string message, int position, Exception innerException) : base(message, innerException)
		{
			Position = position;
		}
		/// <summary>
		/// Constructs a new instance
		/// </summary>
		/// <param name="message">The error</param>
		/// <param name="position">The position</param>
		/// <param name="expecting">The strings that were expected</param>
		public FAParseException(string message, int position, string[] expecting) : base(message)
		{
			Position = position;
			Expecting = expecting;
		}
	}
	#endregion FAException
#if FALIB
	public 
#endif
	sealed partial class FA : ICloneable
	{
		// caching for find and closure functions
		[ThreadStatic] static HashSet<FA> _Seen = new HashSet<FA>();
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

		/// <summary>
		/// Constructs a non-accepting state
		/// </summary>
		public FA()
		{

		}
		/// <summary>
		/// Constructs a state with the specified accept symbol
		/// </summary>
		/// <param name="accept">The accept symbol id</param>
		public FA(int accept)
		{
			AcceptSymbol = accept;
		}
		/// <summary>
		/// Indicates that this state has no transitions to multiple states on the same or no input
		/// </summary>
		public bool IsDeterministic { get; private set; } = true;
		/// <summary>
		/// Indicates that this state has no expanded epsilons
		/// </summary>
		public bool IsCompact { get; private set; } = true;
		/// <summary>
		/// Indicates the accept symbol of the state or less than zero for no accept
		/// </summary>
		public int AcceptSymbol { get; set; } = -1;
		/// <summary>
		/// Indicates the identifier of the state used for debugging
		/// </summary>
		public int Id { get; set; } = -1;
		// used by minimization
		private int _Tag = -1;
		/// <summary>
		/// Indicates the NFA states this DFA was constructed from if it was the result of a subset construction
		/// </summary>
		public FA[] FromStates { get; private set; } = null;
		/// <summary>
		/// Indicates the list of transitions to other states
		/// </summary>
		public IList<FATransition> Transitions {
			get {
				return _transitions.AsReadOnly();
			}
		}
		private List<FATransition> _transitions = new List<FATransition>();
		/// <summary>
		/// Indicates if the state is accepting
		/// </summary>
		public bool IsAccepting { get { return AcceptSymbol > -1; } }
		/// <summary>
		/// Indicates if the state has no transitions
		/// </summary>
		public bool IsFinal { get {  return _transitions.Count==0; } }
		/// <summary>
		/// Indicates that the state is a honeypot for expressions going nowhere, usually to specifically disallow certain matches.
		/// </summary>
		public bool IsTrap { get { return !IsAccepting && IsFinal; } }
		/// <summary>
		/// Indicates if the state is neutral in that it does not change the accepted language
		/// </summary>
		public bool IsNeutral { get { return !IsAccepting && _transitions.Count == 1 && _transitions[0].IsEpsilon; } }
		/// <summary>
		/// Adds an epsilon transition to the state
		/// </summary>
		/// <param name="to">The state to transition to</param>
		/// <param name="compact">True to collapse epsilon transitions onto this state, otherwise false</param>
		public void AddEpsilon(FA to, bool compact = true)
		{
			if(to==null) throw new ArgumentNullException(nameof(to));
			if (compact)
			{
				if (!IsDeterministic && !IsCompact)
				{
					_transitions.AddRange(to._transitions);
				} else
				{
					for (int i = 0; i < to._transitions.Count; ++i)
					{
						var fat = to._transitions[i];
						if (!fat.IsEpsilon)
						{
							AddTransition(new FARange(fat.Min, fat.Max), fat.To);
						}
						else
						{
							AddEpsilon(fat.To, compact);
						}
					}
				}
				if(!IsAccepting & to.IsAccepting)
				{
					AcceptSymbol = to.AcceptSymbol;
				}
			}
			else
			{
				_transitions.Add(new FATransition(to));
				IsCompact = false;
				IsDeterministic = false;
			}
			
		}
		/// <summary>
		/// Adds an input transition
		/// </summary>
		/// <param name="range">The range of input codepoints to transition on</param>
		/// <param name="to">The state to transition to</param>
		/// <exception cref="ArgumentNullException"><paramref name="to"/> was null</exception>
		/// <exception cref="ArgumentException"><paramref name="range"/> indicated an epsilon transition</exception>
		public void AddTransition(FARange range,FA to)
		{
			if (to == null) throw new ArgumentNullException(nameof(to));
			if(range.Min==-1&&range.Max==-1)
			{
				throw new ArgumentException("Attempt to add an epsilon using the wrong method");
			}
			var insert = -1;
			for (int i = 0; i < _transitions.Count; ++i)
			{
				var fat = _transitions[i];
				if(range.Max>fat.Max)
				{
					insert = i;
				}
				if (IsDeterministic)
				{
					if (fat.To != to)
					{
						if (range.Intersects(new FARange(fat.Min, fat.Max)))
						{
							IsDeterministic = false;
						}
					}
				}
				if (!IsDeterministic && range.Max < fat.Max) 
				{
					break;
				}
			}
			
			_transitions.Insert(insert+1,new FATransition(to, range.Min, range.Max));
			
		}
		public void ClearTransitions()
		{
			_transitions.Clear();
			IsDeterministic = true;
			IsCompact = true;
		}
		/// <summary>
		/// Ensures that the machine has no incoming transitions to the starting state, as well as only one final state.
		/// </summary>
		/// <param name="start">The start state, in case a new one needs to be created.</param>
		/// <param name="end">The end state, in case a new one needs to be created</param>
		/// <param name="compact">True to compact any generated epsilons, otherwise false</param>
		/// <param name="flattenAccepting">Move the accepting status of any found accepting states to the new final state</param>
		/// <returns>True if the machine was modified, otherwise it was already linear</returns>
		public bool Linearize(out FA start, out FA end, bool flattenAccepting=false, bool compact = true)
		{
			bool result = false;
			FA final = null;
			var acc = this.FillFind(AcceptingFilter);
			var traps = this.FillFind(TrapFilter);
			end = null;
			if(acc.Count+traps.Count>0)
			{
				if(acc.Count>0)
				{
					end = acc[0];
				} else
				{
					end = traps[0];
				}
			} 
			start = this;
			var closure = FillClosure();
			if (IsLoop(closure))
			{
				start = new FA();
				start.AddEpsilon(this, compact);
				result = true;
			}
			if(traps.Count>0)
			{
				final = new FA();
				for (int i = 0; i < traps.Count; ++i)
				{
					result = true;
					end = final;
					traps[i].AddEpsilon(final);
				}
			}
			
			if (final!=null || acc.Count >1 || (acc.Count>0&&!acc[0].IsFinal))
			{
				if (final == null)
				{
					final = new FA();
				}
				for (int i = 0; i < acc.Count; ++i)
				{
					result = true;
					end = final;
					acc[i].AddEpsilon(final, compact);
				}
			}
			if(flattenAccepting && acc.Count>0 && final!=null)
			{
				if(traps.Count>0)
				{
					throw new FAException("Cannot flatten accepting symbols without changing the language due to the presence of trap states.");
				} else
				{
					final.AcceptSymbol = GetFirstAcceptSymbol(acc);
					result = true;
					for(int i = 0;i<acc.Count;++i)
					{
						acc[i].AcceptSymbol = -1;
					}
				}
				
			}
			return result;
		}
		/// <summary>
		/// A convenience method for returning a new <see cref="Linearize(out FA, bool)"/>ed copy of this machine
		/// </summary>
		/// <param name="flattenAccepting">Move the accepting status of any found accepting states to the new final state</param>
		/// <param name="compact">True to compact any created epsilons, otherwise false</param>
		/// <returns>A <see cref="KeyValuePair{FA, FA}"/> where the key is the new start, and the value is the new end of a new equivelent machine that has no incoming transitions to q0 and only one final state</returns>
		public KeyValuePair<FA,FA> ToLinearized(bool flattenAccepting = false,bool compact = true)
		{
			FA fa = Clone();
			FA start, end;
			fa.Linearize(out start, out end, flattenAccepting, compact);
			return new KeyValuePair<FA, FA>(start,end);
		}
		/// <summary>
		/// Indicates whether q0 of the machine indicated by <paramref name="closure"/> is looping.
		/// </summary>
		/// <param name="closure">The closure of the machine</param>
		/// <returns>True if q0 has incoming transitions, otherwise false</returns>
		public static bool IsLoop(IList<FA> closure)
		{
			var fa = closure[0];
			for (int q = 0; q < closure.Count; ++q)
			{
				var cfa = closure[q];
				for (int i = 0; i < cfa._transitions.Count; ++i)
				{
					if (cfa._transitions[i].To == fa) return true;
				}
			}
			return false;
		}
		/// <summary>
		/// Indicates whether this machine loops back to its root state (q0).
		/// </summary>
		/// <returns>True if q0 (this state) has incoming transitions, otherwise false</returns>
		public bool IsLoop()
		{
			return IsLoop(FillClosure());
		}
		/// <summary>
		/// Set the ids for each state in this machine
		/// </summary>
		public void SetIds()
		{
			var cls = new List<FA>();
			FillClosure(cls);
			var closure = cls.ToArray();
			for (int i = 0; i < closure.Length; ++i)
			{
				closure[i].Id = i;
			}
		}
		/// <summary>
		/// Converts the state to a string.
		/// </summary>
		/// <remarks>If the id is set, this will report it.</remarks>
		/// <returns></returns>
		public override string ToString()
		{
			if (Id < 0)
			{
				return base.ToString();
			}
			else
			{
				return "q" + Id.ToString();
			}
		}
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
			_Seen.Clear();
			_Closure(result);
			_Seen.Clear();
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
			for (int ic = _transitions.Count, i = 0; i < ic; ++i)
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
			_Seen.Clear();
			_Find(filter, result);
			_Seen.Clear();
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
			_Seen.Clear();
			var result = _FindFirst(filter);
			_Seen.Clear();
			return result;
		}
		IList<FA> _FillEpsilonClosureImpl(IList<FA> result, HashSet<FA> seen)
		{
			if(!seen.Add(this))
			{
				return result;
			}
			result.Add(this);
			for (int ic = _transitions.Count, i = 0; i < ic; ++i)
			{
				var t = _transitions[i];
				if(t.IsEpsilon)
				{
					if (t.To.IsDeterministic || t.To.IsCompact)
					{
						if (seen.Add(t.To))
						{
							result.Add(t.To);
						}
					}
					else
					{
						t.To._FillEpsilonClosureImpl(result, seen);
					}
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
		public static IList<FA> FillEpsilonClosure(IEnumerable<FA> states, IList<FA> result=null)
		{
			if (null == result)
				result = new List<FA>();
			_Seen.Clear();
			foreach (var fa in states)
			{
				fa._FillEpsilonClosureImpl(result,_Seen);
			}
			return result;
		}
		/// <summary>
		/// Computes state indices that represent the path to a given state, excluding other states.
		/// </summary>
		/// <param name="to">The state to traverse to</param>
		/// <returns>An array of indices this machine which lead to <paramref name="to"/></returns>
		public int[] PathToIndices(FA to)
		{
			var closure = FillClosure();
			var result = new List<int>(closure.Count);
			for (int i = 0; i < closure.Count; ++i)
			{
				var fa = closure[i];
				if (fa.FindFirst((ffa) => { return (ffa == to); }) != null)
				{
					result.Add(i);
				}
			}
			return result.ToArray();
		}
		/// <summary>
		/// Creates a new set of states that represent the path to a given state, excluding other states.
		/// </summary>
		/// <param name="to">The state to traverse to</param>
		/// <returns>A new set of states from cloned from this machine which lead to <paramref name="to"/></returns>
		public FA ClonePathTo(FA to)
		{
			var closure = FillClosure();
			var nclosure = new FA[closure.Count];
			for (var i = 0; i < nclosure.Length; i++)
			{
				nclosure[i] = new FA(closure[i].AcceptSymbol);
				nclosure[i]._Tag = closure[i]._Tag;
				nclosure[i].Id = closure[i].Id;
				nclosure[i].IsDeterministic = closure[i].IsDeterministic;
				nclosure[i].IsCompact = closure[i].IsCompact;
				nclosure[i].FromStates = closure[i].FromStates;
			}
			for (var i = 0; i < nclosure.Length; i++)
			{
				var t = nclosure[i]._transitions;
				foreach (var trns in closure[i]._transitions)
				{
					if (trns.To.FindFirst((fa) => { return (fa == to); }) != null)
					{
						var id = closure.IndexOf(trns.To);
						t.Add(new FATransition(nclosure[id], trns.Min, trns.Max));
					}
				}
			}
			return nclosure[0];
		}
		/// <summary>
		/// Creates a deep copy of the current machine
		/// </summary>
		/// <returns>A new machine that is a duplicate of this machine</returns>
		public FA Clone() { return Clone(FillClosure()); }
		object ICloneable.Clone() { return Clone(); }
		/// <summary>
		/// Creates a deep copy of the closure
		/// </summary>
		/// <param name="closure">The closure to copy</param>
		/// <returns>A new machine that has a deep copy of the given closure</returns>
		public static FA Clone(IList<FA> closure)
		{
			var nclosure = new FA[closure.Count];
			for (var i = 0; i < nclosure.Length; i++)
			{
				var fa = closure[i];
				var nfa = new FA();
				nfa.AcceptSymbol = fa.AcceptSymbol;
				nfa.IsDeterministic = fa.IsDeterministic;
				nfa.IsCompact = fa.IsCompact;
				nfa.FromStates = fa.FromStates;
				nfa.Id = fa.Id;
				nfa._Tag = fa._Tag;
				nclosure[i] = nfa;
			}
			for (var i = 0; i < nclosure.Length; i++)
			{
				var fa = closure[i];
				var nfa = nclosure[i];
				for (int jc = fa._transitions.Count, j = 0; j < jc; ++j)
				{
					var fat = fa._transitions[j];
					nfa._transitions.Add(new FATransition(nclosure[closure.IndexOf(fat.To)], fat.Min, fat.Max));
				}
			}
			return nclosure[0];
		}
		/// <summary>
		/// Converts a series of characters into a series of UTF-32 codepoints
		/// </summary>
		/// <param name="string">The series of characters to convert</param>
		/// <returns>The series of UTF-32 codepoints</returns>
		/// <exception cref="IOException">The characters had a sequence that was not valid unicode</exception>
		public static IEnumerable<int> ToUtf32(IEnumerable<char> @string)
		{
			int chh = -1;
			foreach (var ch in @string)
			{
				if (char.IsHighSurrogate(ch))
				{
					chh = ch;
					continue;
				}
				else
					chh = -1;
				if (-1 != chh)
				{
					if (!char.IsLowSurrogate(ch))
						throw new IOException("Unterminated Unicode surrogate pair found in string.");
					yield return char.ConvertToUtf32(unchecked((char)chh), ch);
					chh = -1;
					continue;
				}
				yield return ch;
			}
		}
		
		/// <summary>
		/// Indicates whether or not the collection of states contains an accepting state
		/// </summary>
		/// <param name="states">The states to check</param>
		/// <returns>True if one or more of the states is accepting, otherwise false</returns>
		public static int GetFirstAcceptSymbol(IEnumerable<FA> states)
		{
			foreach (var state in states)
			{
				if (state.IsAccepting) return state.AcceptSymbol;
			}
			return -1;
		}
		/// <summary>
		/// Fills a list with all of the new states after moving from a given set of states along a given input. (NFA-move)
		/// </summary>
		/// <param name="states">The current states</param>
		/// <param name="codepoint">The codepoint to move on</param>
		/// <param name="result">A list to hold the next states. If null, one will be created.</param>
		/// <returns>The list of next states</returns>
		public static IList<FA> FillMove(IEnumerable<FA> states, int codepoint, IList<FA> result = null)
		{
			_Seen.Clear();
			if (result == null) result = new List<FA>();
			foreach (var state in states)
			{
				for (int i = 0; i < state._transitions.Count; ++i)
				{
					var fat = state._transitions[i];
					if (fat.Min == -1 && fat.Max == -1)
					{
						continue;
					}
					if (fat.Min <= codepoint && codepoint <= fat.Max)
					{
						if (_Seen.Add(fat.To))
						{
							result.Add(fat.To);
						}

					}
				}
			}
			_Seen.Clear();
			return FillEpsilonClosure(result, null);
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
			for(int i = 0;i<_transitions.Count;++i)
			{
				var fat = _transitions[i];
				if(codepoint<fat.Min)
				{
					return null;
				}
				if(codepoint<=fat.Max)
				{
					return fat.To;
				}
			}
			return null;
		}
		#region Thompson construction methods
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
			var result = new FA();
			var current = result;
			foreach (var codepoint in codepoints)
			{
				current.AcceptSymbol = -1;
				var fa = new FA();
				fa.AcceptSymbol = accept;
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
			var sortedRanges = new List<FARange>(ranges);
			sortedRanges.Sort((x, y) => { var c = x.Min.CompareTo(y.Min); if (0 != c) return c; return x.Max.CompareTo(y.Max); });
			foreach (var range in sortedRanges)
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
				// shut the code analysis up.
				System.Diagnostics.Debug.Assert(result != null);
				var acc = result.FillFind(AcceptingFilter);
				for (int ic = acc.Count, i = 0; i < ic; ++i)
					acc[i].AcceptSymbol = accept;
			}
			return result==null?new FA():result;
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
			var acc = result.FillFind(AcceptingFilter);
			for (int ic = acc.Count, i = 0; i < ic; ++i)
			{
				var fa = acc[i];
				fa.AcceptSymbol = accept;
				result.AddEpsilon(fa, compact);
			}
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
		public static FA Repeat(FA expr, int minOccurs = -1, int maxOccurs = -1, int accept = 0, bool compact = true)
		{
			expr = expr.Clone();
			if (minOccurs > 0 && maxOccurs > 0 && minOccurs > maxOccurs)
				throw new ArgumentOutOfRangeException(nameof(maxOccurs));
			FA result;
			switch (minOccurs)
			{
				case -1:
				case 0:
					switch (maxOccurs)
					{
						case -1:
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
						case -1:
						case 0:
							result = Concat(new FA[] { expr, Repeat(expr, 0, 0, accept, compact) }, accept, compact);
							return result;
						case 1:
							return expr;
						default:
							// TODO: Can make this more compact
							result = Concat(new FA[] { expr, Repeat(expr, 0, maxOccurs - 1, accept, compact) }, accept, compact);
							return result;
					}
				default:
					switch (maxOccurs)
					{
						case -1:
						case 0:
							result = Concat(new FA[] { Repeat(expr, minOccurs, minOccurs, accept, compact), Repeat(expr, 0, 0, accept, compact) }, accept, compact);
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
							result = Concat(new FA[] { Repeat(expr, minOccurs, minOccurs, accept, compact), Repeat(Optional(expr, accept, compact), maxOccurs - minOccurs, maxOccurs - minOccurs, accept, compact) }, accept, compact);
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
							throw new NotSupportedException("Attempt to make an invalid range case insensitive");
						fa.AddTransition(new FARange(trns.Min, trns.Max), trns.To);
						f = f.ToUpperInvariant();
						l = l.ToUpperInvariant();
						fa.AddTransition(new FARange(char.ConvertToUtf32(f, 0), char.ConvertToUtf32(l, 0)), trns.To);

					}
					else if (char.IsUpper(f, 0))
					{
						if (!char.IsUpper(l, 0))
							throw new NotSupportedException("Attempt to make an invalid range case insensitive");
						fa.AddTransition(new FARange(trns.Min, trns.Max), trns.To);
						f = f.ToLowerInvariant();
						l = l.ToLowerInvariant();
						fa.AddTransition(new FARange(char.ConvertToUtf32(f, 0), char.ConvertToUtf32(l, 0)), trns.To);
					}
					else
					{
						fa.AddTransition(new FARange(trns.Min, trns.Max), trns.To);
					}
				}
			}
			return result;
		}
		#endregion // Thompson construction methods
		/// <summary>
		/// Creates a lexer from a series of expressions
		/// </summary>
		/// <param name="tokens">The expressions to add. They typically each have different accept states.</param>
		/// <param name="makeDfa">Make the lexer a DFA. The first disjunction is converted to a DFA and the rest of the state machine is minimized.</param>
		/// <param name="compact">True to compact epsilons, otherwise false. Does nothing if <paramref name="makeDfa"/> is true.</param>
		/// <param name="progress">The progress converting to a lexer (DFA and minimization takes time). Only applies if <paramref name="makeDfa"/> is true.</param>
		/// <returns>The lexer machine</returns>
		public static FA ToLexer(IEnumerable<FA> tokens, bool makeDfa = true, bool compact = true, IProgress<int> progress = null)
		{
			var toks = new List<FA>(tokens);
			if (makeDfa)
			{
				for (int i = 0; i < toks.Count; i++)
				{
					toks[i] = toks[i].ToMinimizedDfa(progress);
				}
			}
			var result = new FA();
			for (int i = 0; i < toks.Count; i++)
			{
				result.AddEpsilon(toks[i], compact);
			}
			if (makeDfa)
			{
				return result.ToDfa(progress);
			}
			else
			{
				return result;
			}
		}
		#region Parse
		static FA _ParseModifier(FA expr, StringCursor pc, int accept, bool compact)
		{
			var position = pc.Position;
			switch (pc.Codepoint)
			{
				case '*':
					expr = Repeat(expr, 0, 0, accept, compact);
					pc.Advance();
					break;
				case '+':
					expr = Repeat(expr, 1, 0, accept, compact);
					pc.Advance();
					break;
				case '?':
					expr = Optional(expr, accept, compact);
					pc.Advance();
					break;
				case '{':
					pc.Advance();
					pc.TrySkipWhiteSpace();
					pc.Expecting('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ',', '}');
					var min = -1;
					var max = -1;
					if (',' != pc.Codepoint && '}' != pc.Codepoint)
					{
						var l = pc.CaptureBuffer.Length;
						pc.TryReadDigits();
						min = int.Parse(pc.GetCapture(l), CultureInfo.InvariantCulture.NumberFormat);
						pc.TrySkipWhiteSpace();
					}
					if (',' == pc.Codepoint)
					{
						pc.Advance();
						pc.TrySkipWhiteSpace();
						pc.Expecting('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '}');
						if ('}' != pc.Codepoint)
						{
							var l = pc.CaptureBuffer.Length;
							pc.TryReadDigits();
							max = int.Parse(pc.GetCapture(l), CultureInfo.InvariantCulture.NumberFormat);
							pc.TrySkipWhiteSpace();
						}
					}
					else { max = min; }
					pc.Expecting('}');
					pc.Advance();
					expr = Repeat(expr, min, max, accept, compact);
					break;
			}
			return expr;
		}
		static byte _FromHexChar(int hex)
		{
			if (':' > hex && '/' < hex)
				return (byte)(hex - '0');
			if ('G' > hex && '@' < hex)
				return (byte)(hex - '7'); // 'A'-10
			if ('g' > hex && '`' < hex)
				return (byte)(hex - 'W'); // 'a'-10
			throw new ArgumentException("The value was not hex.", "hex");
		}
		static bool _IsHexChar(int hex)
		{
			if (':' > hex && '/' < hex)
				return true;
			if ('G' > hex && '@' < hex)
				return true;
			if ('g' > hex && '`' < hex)
				return true;
			return false;
		}
		// return type is either char or ranges. this is kind of a union return type.
		static int _ParseEscapePart(StringCursor pc)
		{
			if (-1 == pc.Codepoint) return -1;
			switch (pc.Codepoint)
			{
				case 'f':
					pc.Advance();
					return '\f';
				case 'v':
					pc.Advance();
					return '\v';
				case 't':
					pc.Advance();
					return '\t';
				case 'n':
					pc.Advance();
					return '\n';
				case 'r':
					pc.Advance();
					return '\r';
				case 'x':
					if (-1 == pc.Advance() || !_IsHexChar(pc.Codepoint))
						return 'x';
					byte b = _FromHexChar(pc.Codepoint);
					if (-1 == pc.Advance() || !_IsHexChar(pc.Codepoint))
						return unchecked(b);
					b <<= 4;
					b |= _FromHexChar(pc.Codepoint);
					if (-1 == pc.Advance() || !_IsHexChar(pc.Codepoint))
						return unchecked(b);
					b <<= 4;
					b |= _FromHexChar(pc.Codepoint);
					if (-1 == pc.Advance() || !_IsHexChar(pc.Codepoint))
						return unchecked(b);
					b <<= 4;
					b |= _FromHexChar(pc.Codepoint);
					return unchecked(b);
				case 'u':
					if (-1 == pc.Advance())
						return 'u';
					ushort u = _FromHexChar(pc.Codepoint);
					u <<= 4;
					if (-1 == pc.Advance())
						return unchecked(u);
					u |= _FromHexChar(pc.Codepoint);
					u <<= 4;
					if (-1 == pc.Advance())
						return unchecked(u);
					u |= _FromHexChar(pc.Codepoint);
					u <<= 4;
					if (-1 == pc.Advance())
						return unchecked(u);
					u |= _FromHexChar(pc.Codepoint);
					return unchecked(u);
				default:
					int i = pc.Codepoint;
					pc.Advance();
					return i;
			}
		}
		static int _ParseRangeEscapePart(StringCursor pc)
		{
			if (-1 == pc.Codepoint)
				return -1;
			switch (pc.Codepoint)
			{
				case '0':
					pc.Advance();
					return '\0';
				case 'f':
					pc.Advance();
					return '\f';
				case 'v':
					pc.Advance();
					return '\v';
				case 't':
					pc.Advance();
					return '\t';
				case 'n':
					pc.Advance();
					return '\n';
				case 'r':
					pc.Advance();
					return '\r';
				case 'x':
					if (-1 == pc.Advance() || !_IsHexChar(pc.Codepoint))
						return 'x';
					byte b = _FromHexChar(pc.Codepoint);
					if (-1 == pc.Advance() || !_IsHexChar(pc.Codepoint))
						return unchecked(b);
					b <<= 4;
					b |= _FromHexChar(pc.Codepoint);
					if (-1 == pc.Advance() || !_IsHexChar(pc.Codepoint))
						return unchecked(b);
					b <<= 4;
					b |= _FromHexChar(pc.Codepoint);
					if (-1 == pc.Advance() || !_IsHexChar(pc.Codepoint))
						return unchecked(b);
					b <<= 4;
					b |= _FromHexChar(pc.Codepoint);
					return unchecked(b);
				case 'u':
					if (-1 == pc.Advance())
						return 'u';
					ushort u = _FromHexChar(pc.Codepoint);
					u <<= 4;
					if (-1 == pc.Advance())
						return unchecked(u);
					u |= _FromHexChar(pc.Codepoint);
					u <<= 4;
					if (-1 == pc.Advance())
						return unchecked(u);
					u |= _FromHexChar(pc.Codepoint);
					u <<= 4;
					if (-1 == pc.Advance())
						return unchecked(u);
					u |= _FromHexChar(pc.Codepoint);
					return unchecked(u);
				default:
					int i = pc.Codepoint;
					pc.Advance();
					return i;
			}
		}
		static KeyValuePair<bool, FARange[]> _ParseSet(StringCursor pc)
		{
			var result = new List<FARange>();
			pc.EnsureStarted();
			pc.Expecting('[');
			pc.Advance();
			pc.Expecting();
			var isNot = false;
			if ('^' == pc.Codepoint)
			{
				isNot = true;
				pc.Advance();
				pc.Expecting();
			}
			var firstRead = true;
			int firstChar = '\0';
			var readFirstChar = false;
			var wantRange = false;
			while (-1 != pc.Codepoint && (firstRead || ']' != pc.Codepoint))
			{
				if (!wantRange)
				{
					// can be a single char,
					// a range
					// or a named character class
					if ('[' == pc.Codepoint) // named char class
					{
						int epos = pc.Position;
						pc.Advance();
						pc.Expecting();
						if (':' != pc.Codepoint)
						{
							firstChar = '[';
							readFirstChar = true;
						}
						else
						{
							firstRead = false;	
							pc.Advance();
							pc.Expecting();
							var ll = pc.CaptureBuffer.Length;
							if (!pc.TryReadUntil(':', false))
								throw new FAParseException("Expecting character class", pc.Position);
							pc.Expecting(':');
							pc.Advance();
							pc.Expecting(']');
							pc.Advance();
							var cls = pc.GetCapture(ll);
							int[] ranges;
							if (!CharacterClasses.Known.TryGetValue(cls, out ranges))
								throw new FAParseException("Unknown character class \"" + cls + "\" specified", epos);
							if (ranges != null)
							{
								result.AddRange(FARange.ToUnpacked(ranges));
							}
							readFirstChar = false;
							wantRange = false;
							firstRead = false;
							continue;
						}
					}
					if (!readFirstChar)
					{
						if ('\\' == pc.Codepoint)
						{
							pc.Advance();
							firstChar = _ParseRangeEscapePart(pc);
						}
						else
						{
							firstChar = pc.Codepoint;
							pc.Advance();
							pc.Expecting();
						}
						readFirstChar = true;

					}
					else
					{
						if ('-' == pc.Codepoint)
						{
							pc.Advance();
							pc.Expecting();
							wantRange = true;
						}
						else
						{
							result.Add(new FARange(firstChar,firstChar));
							readFirstChar = false;
						}
					}
					firstRead = false;
				}
				else
				{
					if ('\\' != pc.Codepoint)
					{
						var ch = pc.Codepoint;
						pc.Advance();
						pc.Expecting();
						result.Add(new FARange(firstChar, ch));
					}
					else
					{
						var min = firstChar;
						pc.Advance();
						result.Add(new FARange(min,_ParseRangeEscapePart(pc)));
					}
					wantRange = false;
					readFirstChar = false;
				}

			}
			if (readFirstChar)
			{
				result.Add(new FARange(firstChar, firstChar));
				if (wantRange)
				{
					result.Add(new FARange('-', '-'));
				}
			}
			pc.Expecting(']');
			pc.Advance();
			return new KeyValuePair<bool, FARange[]>(isNot, result.ToArray());
		}
		static FA _Parse(StringCursor pc, int accept, bool compact)
		{

			FA result = null;
			FA next = null;
			int ich;
			pc.EnsureStarted();
			while (true)
			{
				switch (pc.Codepoint)
				{
					case -1:
						//result = result.ToMinimized();
						System.Diagnostics.Debug.Assert(result != null);
						return result;
					case '.':
						var dot = FA.Set(new FARange[] { new FARange(0, 0x10ffff) }, accept, compact);
						if (null == result)
							result = dot;
						else
						{
							result = FA.Concat(new FA[] { result, dot }, accept, compact);
						}
						pc.Advance();
						result = _ParseModifier(result, pc, accept, compact);
						break;
					case '\\':

						pc.Advance();
						pc.Expecting();
						var isNot = false;
						switch (pc.Codepoint)
						{
							case 'P':
								isNot = true;
								goto case 'p';
							case 'p':
								pc.Advance();
								pc.Expecting('{');
								var uc = new StringBuilder();
								while (-1 != pc.Advance() && '}' != pc.Codepoint)
									uc.Append(char.ConvertFromUtf32(pc.Codepoint));
								pc.Expecting('}');
								pc.Advance();
								int uci = 0;
								switch (uc.ToString())
								{
									case "Pe":
										uci = 21;
										break;
									case "Pc":
										uci = 19;
										break;
									case "Cc":
										uci = 14;
										break;
									case "Sc":
										uci = 26;
										break;
									case "Pd":
										uci = 19;
										break;
									case "Nd":
										uci = 8;
										break;
									case "Me":
										uci = 7;
										break;
									case "Pf":
										uci = 23;
										break;
									case "Cf":
										uci = 15;
										break;
									case "Pi":
										uci = 22;
										break;
									case "Nl":
										uci = 9;
										break;
									case "Zl":
										uci = 12;
										break;
									case "Ll":
										uci = 1;
										break;
									case "Sm":
										uci = 25;
										break;
									case "Lm":
										uci = 3;
										break;
									case "Sk":
										uci = 27;
										break;
									case "Mn":
										uci = 5;
										break;
									case "Ps":
										uci = 20;
										break;
									case "Lo":
										uci = 4;
										break;
									case "Cn":
										uci = 29;
										break;
									case "No":
										uci = 10;
										break;
									case "Po":
										uci = 24;
										break;
									case "So":
										uci = 28;
										break;
									case "Zp":
										uci = 13;
										break;
									case "Co":
										uci = 17;
										break;
									case "Zs":
										uci = 11;
										break;
									case "Mc":
										uci = 6;
										break;
									case "Cs":
										uci = 16;
										break;
									case "Lt":
										uci = 2;
										break;
									case "Lu":
										uci = 0;
										break;
								}
								if (isNot)
								{
									next = FA.Set(FARange.ToUnpacked(CharacterClasses.UnicodeCategories[uci]), accept, compact);
								}
								else
									next = FA.Set(FARange.ToUnpacked(CharacterClasses.NotUnicodeCategories[uci]), accept, compact);
								break;
							case 'd':
								next = FA.Set(FARange.ToUnpacked(CharacterClasses.digit), accept, compact);
								pc.Advance();
								break;
							case 'D':
								next = FA.Set(FARange.ToNotRanges(FARange.ToUnpacked(CharacterClasses.digit)), accept, compact);
								pc.Advance();
								break;

							case 's':
								next = FA.Set(FARange.ToUnpacked(CharacterClasses.space), accept, compact);
								pc.Advance();
								break;
							case 'S':
								next = FA.Set(FARange.ToNotRanges(FARange.ToUnpacked(CharacterClasses.space)), accept, compact);
								pc.Advance();
								break;
							case 'w':
								next = FA.Set(FARange.ToUnpacked(CharacterClasses.word), accept, compact);
								pc.Advance();
								break;
							case 'W':
								next = FA.Set(FARange.ToNotRanges(FARange.ToUnpacked(CharacterClasses.word)), accept, compact);
								pc.Advance();
								break;
							default:
								if (-1 != (ich = _ParseEscapePart(pc)))
								{
									next = FA.Literal(new int[] { ich }, accept, compact);

								}
								else
								{
									pc.Expecting(); // throw an error
									System.Diagnostics.Debug.Assert(false);
								}
								break;
						}
						next = _ParseModifier(next, pc, accept, compact);
						if (null != result)
						{
							result = FA.Concat(new FA[] { result, next }, accept, compact);
						}
						else
							result = next;
						break;
					case ')':
						//result = result.ToMinimized();
						System.Diagnostics.Debug.Assert(result != null);
						return result;
					case '(':
						pc.Advance();
						if(pc.Codepoint=='?')
						{
							pc.Advance();
							pc.Expecting(':');
							pc.Advance();
						}
						pc.Expecting();
						next = _Parse(pc, accept, compact);
						pc.Expecting(')');
						pc.Advance();
						next = _ParseModifier(next, pc, accept, compact);
						if (null == result)
							result = next;
						else
						{
							result = FA.Concat(new FA[] { result, next }, accept, compact);
						}
						break;
					case '|':
						if (-1 != pc.Advance())
						{
							System.Diagnostics.Debug.Assert(result != null);
							next = _Parse(pc, accept, compact);
							result = FA.Or(new FA[] { result, next }, accept, compact);
						}
						else
						{
							System.Diagnostics.Debug.Assert(result != null);
							result = FA.Optional(result, accept, compact);
						}
						break;
					case '[':
						var seti = _ParseSet(pc);
						IEnumerable<FARange> set;
						if (seti.Key)
							set = FARange.ToNotRanges(seti.Value);
						else
							set = seti.Value;
						next = FA.Set(set, accept);
						next = _ParseModifier(next, pc, accept, compact);

						if (null == result)
							result = next;
						else
						{
							result = FA.Concat(new FA[] { result, next }, accept, compact);

						}
						break;
					default:
						ich = pc.Codepoint;
						next = FA.Literal(new int[] { ich }, accept, compact);
						pc.Advance();
						next = _ParseModifier(next, pc, accept, compact);
						if (null == result)
							result = next;
						else
						{
							result = FA.Concat(new FA[] { result, next }, accept, compact);
						}
						break;
				}
			}
		}
		public static FA Parse(string text, int accept = 0, bool compact = true)
		{
			StringCursor pc = new StringCursor();
			pc.Input = text;
			return _Parse(pc, accept, compact);
		}
		#endregion // Parse
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
			for(int q = 0; q<closure.Count;++q)
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
		/// Creates a packed state table as a series of integers
		/// </summary>
		/// <returns>An integer array representing the machine</returns>
		public int[] ToArray()
		{
			var working = new List<int>();
			var closure = new List<FA>();
			FillClosure(closure);
			var stateIndices = new int[closure.Count];
			for (var i = 0; i < closure.Count; ++i)
			{
				var cfa = closure[i];
				stateIndices[i] = working.Count;
				// add the accept
				working.Add(cfa.IsAccepting ? cfa.AcceptSymbol : -1);
				var itrgp = cfa.FillInputTransitionRangesGroupedByState(true);
				// add the number of transitions
				working.Add(itrgp.Count);
				foreach (var itr in itrgp)
				{
					// We have to fill in the following after the fact
					// We don't have enough info here
					// for now just drop the state index as a placeholder
					working.Add(closure.IndexOf(itr.Key));
					// add the number of packed ranges
					working.Add(itr.Value.Count);
					// add the packed ranges
					working.AddRange(FARange.ToPacked(itr.Value));
				}
			}
			var result = working.ToArray();
			var state = 0;
			while (state < result.Length)
			{
				++state;
				var tlen = result[state++];
				for (var i = 0; i < tlen; ++i)
				{
					// patch the destination
					result[state] = stateIndices[result[state]];
					++state;
					var prlen = result[state++];
					state += prlen * 2;
				}
			}
			return result;
		}
		/// <summary>
		/// Builds a state machine based on the packed state table
		/// </summary>
		/// <param name="fa">The state table to build from</param>
		/// <returns>A new machine that represents the given packed state table</returns>
		public static FA FromArray(int[] fa)
		{
			if (null == fa) throw new ArgumentNullException(nameof(fa));
			if (fa.Length == 0)
			{
				var result = new FA();
				result.IsDeterministic = true;
				result.IsCompact = true;
				return result;
			}
			var si = 0;
			var states = new Dictionary<int, FA>();
			while (si < fa.Length)
			{
				var newfa = new FA();
				states.Add(si, newfa);
				newfa.AcceptSymbol = fa[si++];
				var tlen = fa[si++];
				for (var i = 0; i < tlen; ++i)
				{
					++si; // tto
					var prlen = fa[si++];
					si += prlen * 2;
				}
			}
			si = 0;
			var sid = 0;
			while (si < fa.Length)
			{
				var newfa = states[si];
				newfa.IsCompact = true;
				newfa.IsDeterministic = true;
				newfa.AcceptSymbol = fa[si++];
				var tlen = fa[si++];
				for (var i = 0; i < tlen; ++i)
				{
					var tto = fa[si++];
					var to = states[tto];
					var prlen = fa[si++];
					for (var j = 0; j < prlen; ++j)
					{
						var pmin = fa[si++];
						var pmax = fa[si++];
						if (pmin == -1 && pmax == -1)
						{
							newfa.IsCompact = false;
							newfa.IsDeterministic = false;
						}
						else 
						{
							var newRange = new FARange(pmin, pmax);
							for(var k = 0;k< newfa._transitions.Count;++k)
							{
								var fat = newfa._transitions[k];
								if(newRange.Intersects(new FARange(fat.Min, fat.Max)))
								{
									newfa.IsDeterministic = false;
									break;
								}
							}
						}
						newfa.AddTransition(new FARange(pmin, pmax), to);
					}
				}
				++sid;
			}
			return states[0];
		}
		
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
			int prog = 0;
			progress?.Report(prog);
			var p = new HashSet<int>();
			var closure = new List<FA>();
			fa.FillClosure(closure);
			fa.SetIds();
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
			_Seen.Clear();
			fa._FillEpsilonClosureImpl(epscl, _Seen);
			foreach (var efa in epscl)
			{
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
			dfaMap.Add(initial, result);
			while (working.Count > 0)
			{
				var s = working.Dequeue();
				FA dfa = dfaMap[s];
				foreach (FA q in s)
				{
					if (q.IsAccepting)
					{
						dfa.AcceptSymbol = q.AcceptSymbol;
						break;
					}
				}

				for (var i = 0; i < points.Length; i++)
				{
					var pnt = points[i];
					var set = new _KeySet<FA>();
					foreach (FA c in s)
					{
						_Seen.Clear();
						var ecs = new List<FA>();
						c._FillEpsilonClosureImpl(ecs,_Seen);
						foreach (var efa in ecs)
						{
							foreach (var trns in efa._transitions)
							{
								if (trns.Min == -1 && trns.Max == -1)
								{
									continue;
								}
								if (trns.Min <= pnt && pnt <= trns.Max)
								{
									_Seen.Clear();
									var efcs = new List<FA>();
									trns.To._FillEpsilonClosureImpl(efcs, _Seen);
									foreach (var eefa in efcs) 
									{
										set.Add(eefa);
									}
								}
							}
						}
						_Seen.Clear();
					}
					if (!sets.ContainsKey(set))
					{
						sets.Add(set, set);
						working.Enqueue(set);
						var newfa = new FA();
						newfa.IsDeterministic = true;
						dfaMap.Add(set, newfa);
						var fas = new List<FA>(set);
						newfa.FromStates = fas.ToArray();
					}

					FA dst = dfaMap[set];
					int first = pnt;
					int last;
					if (i + 1 < points.Length)
						last = (points[i + 1] - 1);
					else
						last = 0x10ffff;
					dfa._transitions.Add(new FATransition(dst,first, last));
					++prog;
					progress?.Report(prog);
				}
				++prog;
				progress?.Report(prog);

			}
			// remove dead transitions
			foreach (var ffa in result.FillClosure())
			{
				var itrns = new List<FATransition>(ffa._transitions);
				foreach (var trns in itrns)
				{
					var acc = trns.To.FillFind(AcceptingFilter);
					if (0 == acc.Count)
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
		#region Totalize()
		/// <summary>
		/// For this machine, fills and sorts transitions such that any missing range now points to an empty non-accepting state
		/// </summary>
		public void Totalize()
		{
			Totalize(FillClosure());
		}

		static int _TransitionComparison(FATransition x, FATransition y)
		{
			var c = x.Min.CompareTo(y.Min); if (0 != c) return c; return x.Max.CompareTo(y.Max);
		}
		/// <summary>
		/// For this closure, fills and sorts transitions such that any missing range now points to an empty non-accepting state
		/// </summary>
		/// <param name="closure">The closure to totalize</param>
		public static void Totalize(IList<FA> closure)
		{
			var s = new FA();
			s._transitions.Add(new FATransition(s,0, 0x10ffff));
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
						p._transitions.Add(new FATransition(s,maxi, (t.Min - 1)));
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
				q._Tag = number;
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
				block[qq._Tag] = j;
				for (int x = 0; x < sigma.Length; x++)
				{
					var y = sigma[x];
					var p = qq._Step(y);
					System.Diagnostics.Debug.Assert(p != null);
					var pn = p._Tag;
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
						if (reverseNonempty[qq._Tag, x])
						{
							active2[qq._Tag, x] = active[j, x].Add(qq);
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
					foreach (var s in reverse[m.State._Tag][x])
					{
						if (!split2[s._Tag])
						{
							split2[s._Tag] = true;
							split.Add(s);
							int j = block[s._Tag];
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
							block[s._Tag] = k;
							for (int c = 0; c < sigma.Length; c++)
							{
								_FListNode sn = active2[s._Tag, c];
								if (sn != null && sn.StateList == active[j, c])
								{
									sn.Remove();
									active2[s._Tag, c] = active[k, c].Add(s);
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
						split2[s._Tag] = false;
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
					s._Tag = q._Tag; // Select representative.				
					q._Tag = n;
				}
				++prog;
				progress?.Report(prog);
			}

			// Build transitions and set acceptance.
			foreach (var s in newstates)
			{
				var st = states[s._Tag];
				s.AcceptSymbol = st.AcceptSymbol;
				foreach (var t in st._transitions)
				{
					s._transitions.Add(new FATransition(newstates[t.To._Tag],t.Min, t.Max));
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
		
		/// <summary>
		/// Returns a <see cref="FARunner"/> over the input
		/// </summary>
		/// <param name="string">The string to evaluate</param>
		/// <param name="blockEnds">The block end expressions</param>
		/// <returns>A new runner that can match strings given the current instance</returns>
		public FARunner Run(string @string, FA[] blockEnds = null)
		{
			var result = new FAStringStateRunner(this, blockEnds);
			result.Set(@string);
			return result;
		}
		/// <summary>
		/// Returns a <see cref="FARunner"/> over the input
		/// </summary>
		/// <param name="reader">The text to evaluate</param>
		/// <param name="blockEnds">The block end expressions</param>
		/// <returns>A new runner that can match text given the current instance</returns>
		public FARunner Run(TextReader reader, FA[] blockEnds = null)
		{
			var result = new FATextReaderStateRunner(this, blockEnds);
			result.Set(reader);
			return result;
		}

		/// <summary>
		/// Returns a <see cref="FARunner"/> over the input
		/// </summary>
		/// <param name="string">The string to evaluate</param>
		/// <param name="dfa">The dfa array to use</param>
		/// <param name="blockEnds">The block end expression arrays</param>
		/// <returns>A new runner that can match strings given the current instance</returns>
		public static FARunner Run(string @string, int[] dfa, int[][] blockEnds = null)
		{
			var result = new FAStringDfaTableRunner(dfa, blockEnds);
			result.Set(@string);
			return result;
		}

		/// <summary>
		/// Returns a <see cref="FARunner"/> over the input
		/// </summary>
		/// <param name="reader">The text to evaluate</param>
		/// <param name="dfa">The dfa array to use</param>
		/// <param name="blockEnds">The block end expression arrays</param>
		/// <returns>A new runner that can match strings given the current instance</returns>
		public static FARunner Run(TextReader reader, int[] dfa, int[][] blockEnds = null)
		{
			var result = new FATextReaderDfaTableRunner(dfa, blockEnds);
			result.Set(reader);
			return result;
		}

		#region StringCursor
		internal sealed class StringCursor
		{
			const int BeforeInput = -2;
			const int EndOfInput = -1;
			public string Input = null;
			public int Position = -1;
			public int Codepoint = -2;
			public StringBuilder CaptureBuffer { get; } = new StringBuilder();
			public void Capture()
			{
				if(Codepoint<0)
				{
					return;
				}
				CaptureBuffer.Append(char.ConvertFromUtf32(Codepoint));
			}
			public string GetCapture(int index = 0, int length = -1)
			{
				if (length == -1)
				{
					if(index==0)
					{
						return CaptureBuffer.ToString();
					}
					return CaptureBuffer.ToString(index, CaptureBuffer.Length - index);
				}
				return CaptureBuffer.ToString(index, length);
			}
			public void EnsureStarted()
			{
				if(Codepoint == -2 )
				{
					Advance();
				}
			}
			public int Advance()
			{
				if (Input == null)
				{
					Codepoint = -1;
					return -1;
				}
				if(++Position>=Input.Length)
				{
					Codepoint = -1;
					return -1;
				}
				Codepoint = Input[Position];
				if (Codepoint <= 0xFFFF && char.IsHighSurrogate((char)Codepoint))
				{
					if(++Position >= Input.Length)
					{
						throw new IOException("Unexpected end of input in Unicode stream");
					}
					var tmp = Input[Position];
					if (tmp > 0xFFFF || !char.IsLowSurrogate((char)tmp))
					{
						throw new IOException("Unterminated surrogate Unicode stream");
					}
					Codepoint = char.ConvertToUtf32((char)Codepoint, (char)tmp);
				}
				return Codepoint;
			}
			public void Expecting(params int[] codepoints)
			{
				if (BeforeInput == Codepoint)
					throw new FAParseException("The cursor is before the beginning of the input", Position);
				switch (codepoints.Length)
				{
					case 0:
						if (EndOfInput == Codepoint)
							throw new FAParseException("Unexpected end of input", Position);
						break;
					case 1:
						if (codepoints[0] != Codepoint)
							throw new FAParseException(_GetErrorMessage(codepoints), Position, _GetErrorExpecting(codepoints));
						break;
					default:
						if (0 > Array.IndexOf(codepoints, Codepoint))
							throw new FAParseException(_GetErrorMessage(codepoints), Position,_GetErrorExpecting(codepoints));
						break;
				}
			}
			string[] _GetErrorExpecting(int[] codepoints)
			{
				var result = new string[codepoints.Length];
				for (var i = 0; i < codepoints.Length; i++)
				{
					if (-1 != codepoints[i])
						result[i] = char.ConvertFromUtf32(codepoints[i]);
					else
						result[i] = "end of input";
				}
				return result;
			}
			string _GetErrorMessage(int[] expecting)
			{
				StringBuilder sb = null;
				switch (expecting.Length)
				{
					case 0:
						break;
					case 1:
						sb = new StringBuilder();
						if (-1 == expecting[0])
							sb.Append("end of input");
						else
						{
							sb.Append("\"");
							sb.Append(char.ConvertFromUtf32(expecting[0]));
							sb.Append("\"");
						}
						break;
					case 2:
						sb = new StringBuilder();
						if (-1 == expecting[0])
							sb.Append("end of input");
						else
						{
							sb.Append("\"");
							sb.Append(char.ConvertFromUtf32(expecting[0]));
							sb.Append("\"");
						}
						sb.Append(" or ");
						if (-1 == expecting[1])
							sb.Append("end of input");
						else
						{
							sb.Append("\"");
							sb.Append(char.ConvertFromUtf32(expecting[1]));
							sb.Append("\"");
						}
						break;
					default: // length > 2
						sb = new StringBuilder();
						if (-1 == expecting[0])
							sb.Append("end of input");
						else
						{
							sb.Append("\"");
							sb.Append(char.ConvertFromUtf32(expecting[0]));
							sb.Append("\"");
						}
						int l = expecting.Length - 1;
						int i = 1;
						for (; i < l; ++i)
						{
							sb.Append(", ");
							if (-1 == expecting[i])
								sb.Append("end of input");
							else
							{
								sb.Append("\"");
								sb.Append(char.ConvertFromUtf32(expecting[1]));
								sb.Append("\"");
							}
						}
						sb.Append(", or ");
						if (-1 == expecting[i])
							sb.Append("end of input");
						else
						{
							sb.Append("\"");
							sb.Append(char.ConvertFromUtf32(expecting[i]));
							sb.Append("\"");
						}
						break;
				}
				if (-1 == Codepoint)
				{
					if (0 == expecting.Length)
						return "Unexpected end of input";
					System.Diagnostics.Debug.Assert(sb != null); // shut up code analysis
					return string.Concat("Unexpected end of input. Expecting ", sb.ToString());
				}
				if (0 == expecting.Length)
					return string.Concat("Unexpected character \"", (char)Codepoint, "\" in input");
				System.Diagnostics.Debug.Assert(sb != null); // shut up code analysis
				return string.Concat("Unexpected character \"", (char)Codepoint, "\" in input. Expecting ", sb.ToString());
			}
			public bool TrySkipWhiteSpace()
			{
				EnsureStarted();
				if(Input==null || Position>=Input.Length) return false;
				if (!char.IsWhiteSpace(Input,Position))
					return false;
				++Position;
				if (Position<Input.Length && char.IsLowSurrogate(Input, Position)) ++Position;
				while (Position < Input.Length && char.IsWhiteSpace(Input,Position))
				{
					++Position;
					if (Position < Input.Length && char.IsLowSurrogate(Input, Position)) ++Position;
				}
				return true;
			}
			public bool TryReadDigits()
			{
				EnsureStarted();
				if (Input == null || Position >= Input.Length) return false;
				if (!char.IsDigit(Input, Position))
					return false;
				Capture();
				++Position;
				if (Position < Input.Length && char.IsLowSurrogate(Input, Position)) ++Position;
				while (Position < Input.Length && char.IsDigit(Input, Position))
				{
					Capture();
					++Position;
					if (Position < Input.Length && char.IsLowSurrogate(Input, Position)) ++Position;
				}
				return true;
			}
			public bool TryReadUntil(int character, bool readCharacter = true)
			{
				EnsureStarted();
				if (0 > character) character = -1;
				Capture();
				if (Codepoint == character)
				{
					return true;
				}
				while (-1 != Advance() && Codepoint!= character)
					Capture();
				//
				if (Codepoint == character)
				{
					if (readCharacter)
					{
						Capture();
						Advance();
					}
					return true;
				}
				return false;
			}
		}
		#endregion // StringCursor
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
	}
	
}
