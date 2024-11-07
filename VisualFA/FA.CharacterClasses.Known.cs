using System;
using System.Collections.Generic;

namespace VisualFA
{
	partial class FA
	{
		public partial class CharacterClasses
		{
			static Lazy<IDictionary<string, int[]>> _Known = new Lazy<IDictionary<string, int[]>>(_GetKnown);
			static IDictionary<string, int[]> _GetKnown()
			{
				var result = new Dictionary<string, int[]>();
				var fa = typeof(CharacterClasses).GetFields();
				for (var i = 0; i < fa.Length; i++)
				{
					var f = fa[i];
					if (f.FieldType == typeof(int[]))
					{
						var a = (int[])f.GetValue(null);
						System.Diagnostics.Debug.Assert(a != null);
						result.Add(f.Name, a);
					}

				}
				return result;
			}
			/// <summary>
			/// A dictionary of all the defined character sets
			/// </summary>
			public static IDictionary<string, int[]> Known { get { return _Known.Value; } }
		}
	}
}
