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
		public bool Equals(FATransition rhs)
		{
			return To == rhs.To && Min == rhs.Min && Max == rhs.Max;
		}
		public override int GetHashCode()
		{
			if(To==null)
			{
				return Min.GetHashCode() ^ Max.GetHashCode();
			}
			return Min.GetHashCode() ^ Max.GetHashCode() ^ To.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null)) return false;
			if (!(obj is FATransition)) return false;
			FATransition rhs = (FATransition) obj;
			return To == rhs.To && Min == rhs.Min && Max == rhs.Max;
		}
	}
	#endregion // FATransition
	
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
	
#if FALIB
	public 
#endif
	sealed partial class FA : ICloneable
	{
		
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
		private int _MinimizationTag = -1;
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
				
				for (int i = 0; i < to._transitions.Count; ++i)
				{
					var fat = to._transitions[i];
					if (fat.Min!=-1 || fat.Max!=-1)
					{
						AddTransition(new FARange(fat.Min, fat.Max), fat.To);
					}
					else
					{
						AddEpsilon(fat.To, true);
					}
				}

				if (AcceptSymbol < 0 && to.AcceptSymbol > -1)
				{
					AcceptSymbol = to.AcceptSymbol;
				}
			}
			else
			{
				var found =false;
				int i;
				for (i = 0;i<_transitions.Count;++i)
				{
					var fat = _transitions[i];
					// this is why we don't use List.Contains:
					if (fat.Min != -1 || fat.Max != -1) break;
					if(fat.To==to)
					{
						found = true;
						break;
					}
				}
				if (!found)
				{
					_transitions.Insert(i,new FATransition(to));
					IsCompact = false;
					IsDeterministic = false;
				}
			}
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
		/// <summary>
		/// Adds an input transition
		/// </summary>
		/// <param name="range">The range of input codepoints to transition on</param>
		/// <param name="to">The state to transition to</param>
		/// <exception cref="ArgumentNullException"><paramref name="to"/> was null</exception>
		public void AddTransition(FARange range,FA to)
		{
			if (to == null) 
				throw new ArgumentNullException(nameof(to));
			
			if(range.Min==-1 && range.Max==-1)
			{
				AddEpsilon(to);
				return;
			}
			if(range.Min>range.Max)
			{
				int tmp = range.Min;
				range.Min = range.Max;
				range.Max = tmp;
			}
			var insert = -1;
			for (int i = 0; i < _transitions.Count; ++i)
			{
				var fat = _transitions[i];
				if (to == fat.To)
				{
					if(range.Min==fat.Min && 
						range.Max==fat.Max)
					{
						return;
					}
				}
				if (IsDeterministic)
				{
					if (range.Intersects(
						new FARange(fat.Min, fat.Max)))
					{
						IsDeterministic = false;
					}
				}
				if (range.Max>fat.Max)
				{
					insert = i;
				}
				if (!IsDeterministic && 
					range.Max < fat.Min) 
				{
					break;
				}
			}
			_transitions.Insert(insert+1,
				new FATransition(to, range.Min, range.Max));	
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
			if(final==null && acc.Count==1)
			{
				final = acc[0];
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
				nclosure[i]._MinimizationTag = closure[i]._MinimizationTag;
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
				nfa._MinimizationTag = fa._MinimizationTag;
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
		public static int GetFirstAcceptSymbol(IList<FA> states)
		{
			for(int i = 0;i< states.Count;++i)
			{
				var state = states[i];
				if (state.IsAccepting) return state.AcceptSymbol;
			}
			return -1;
		}

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
			if (makeDfa && !result.IsDeterministic)
			{
				return result.ToDfa(progress);
			}
			else
			{
				return result;
			}
		}
	
		static IEnumerable<FARange> _InvertRanges(IEnumerable<FARange> ranges)
		{
			if (ranges == null)
			{
				yield break;
			}
			var last = 0x10ffff;

			using (var e = ranges.GetEnumerator())
			{
				if (!e.MoveNext())
				{
					FARange range;
					range.Min = 0;
					range.Max = 0x10ffff;
					yield return range;
					yield break;
				}
				if (e.Current.Min > 0)
				{
					FARange range;
					range.Min = 0;
					range.Max = e.Current.Min - 1;
					yield return range;
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
					{
						FARange range;
						range.Min = unchecked(last + 1);
						range.Max = unchecked((e.Current.Min - 1));
						yield return range;
					}
					last = e.Current.Max;
				}
				if (0x10ffff > last)
				{
					FARange range;
					range.Min = unchecked((last + 1));
					range.Max = 0x10ffff;
					yield return range;
				}

			}
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
			// fill in the state information
			for (var i = 0; i < stateIndices.Length; ++i)
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
			// now fill in the state indices
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
			// create the states and build a map
			// of state indices in the array to
			// new FA instances
			var si = 0;
			var indexToStateMap = new Dictionary<int, FA>();
			while (si < fa.Length)
			{
				var newfa = new FA();
				indexToStateMap.Add(si, newfa);
				newfa.AcceptSymbol = fa[si++];
				// skip to the next state
				var tlen = fa[si++];
				for (var i = 0; i < tlen; ++i)
				{
					++si; // tto
					var prlen = fa[si++];
					si += prlen * 2;
				}
			}
			// walk the array
			si = 0;
			var sid = 0;
			while (si < fa.Length)
			{
				// get the current state
				var newfa = indexToStateMap[si];
				newfa.IsCompact = true;
				newfa.IsDeterministic = true;
				// already set above:
				// newfa.AcceptSymbol = fa[si++];
				++si;
				// transitions length
				var tlen = fa[si++];
				for (var i = 0; i < tlen; ++i)
				{
					// destination state index
					var tto = fa[si++];
					// destination state instance
					var to = indexToStateMap[tto];
					// range count
					var prlen = fa[si++];
					for (var j = 0; j < prlen; ++j)
					{
						var pmin = fa[si++];
						var pmax = fa[si++];
						if (pmin == -1 && pmax == -1)
						{
							// epsilon
							newfa.AddEpsilon(to, false);
						}
						else 
						{
							newfa.AddTransition(new FARange(pmin, pmax), to);
						}
					}
				}
				++sid;
			}
			return indexToStateMap[0];
		}
		
		static int _TransitionComparison(FATransition x, FATransition y)
		{
			var c = x.Max.CompareTo(y.Max); if (0 != c) return c; return x.Min.CompareTo(y.Min);
		}
		
		/// <summary>
		/// Returns a <see cref="FARunner"/> over the input
		/// </summary>
		/// <param name="string">The string to evaluate</param>
		/// <param name="blockEnds">The block end expressions</param>
		/// <returns>A new runner that can match strings given the current instance</returns>
		public FAStringStateRunner Run(string @string, FA[] blockEnds = null)
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
		public FATextReaderStateRunner Run(TextReader reader, FA[] blockEnds = null)
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
		public static FAStringDfaTableRunner Run(string @string, int[] dfa, int[][] blockEnds = null)
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
		public static FATextReaderDfaTableRunner Run(TextReader reader, int[] dfa, int[][] blockEnds = null)
		{
			var result = new FATextReaderDfaTableRunner(dfa, blockEnds);
			result.Set(reader);
			return result;
		}
	}
	
}
