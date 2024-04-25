using System;
using System.Collections.Generic;

namespace VisualFA
{
	/// <summary>
	/// Represents a range of codepoints
	/// </summary>
#if FALIB
	public
#endif
	struct FARange
	{
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
				if (0x10ffff >= last)
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
			if (rhs is FARange)
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

}
