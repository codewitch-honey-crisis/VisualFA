using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace VisualFA
{
	public static class FACompiler
	{
#if DEBUG
		static readonly MethodInfo _ZDbgWL = typeof(Console).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
		static readonly MethodInfo _ZDbgW = typeof(Console).GetMethod("Write", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
		private static void _DbgWriteLine(ILGenerator il, string format, params object[] args)
		{
			var str = (args.Length == 0) ? format : string.Format(format, args);
			il.Emit(OpCodes.Ldstr, str);
			System.Diagnostics.Debug.Assert(_ZDbgWL != null);
			il.EmitCall(OpCodes.Call, _ZDbgWL, null);
		}
		private static void _DbgWrite(ILGenerator il, string format, params object[] args)
		{
			var str = (args.Length == 0) ? format : string.Format(format, args);
			il.Emit(OpCodes.Ldstr, str);
			System.Diagnostics.Debug.Assert(_ZDbgW != null);
			il.EmitCall(OpCodes.Call, _ZDbgW, null);
		}
		/*private static void _DbgOutCP(ILGenerator il, bool ismatch)
		{
			var cw = typeof(Console).GetMethod("Write", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string) }, null);
			var cvt = typeof(char).GetMethod("ConvertFromUtf32");
			il.Emit(ismatch ? OpCodes.Ldloc_0 : OpCodes.Ldloc_3);
			il.EmitCall(OpCodes.Call, cvt, null);
			il.EmitCall(OpCodes.Call, cw, null);
		}*/
#endif
		static readonly MethodInfo _RocharspanCnv = typeof(string).GetMethod("op_Implicit", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
		static readonly FieldInfo _string = typeof(FAStringRunner).GetField("input_string", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
		static readonly MethodInfo _advStr = typeof(FAStringRunner).GetMethod("Advance", BindingFlags.NonPublic | BindingFlags.Instance);
		static readonly MethodInfo _advRdr = typeof(FATextReaderRunner).GetMethod("Advance", BindingFlags.NonPublic | BindingFlags.Instance);
		static readonly FieldInfo _pos = typeof(FARunner).GetField("position", BindingFlags.NonPublic | BindingFlags.Instance);
		static readonly FieldInfo _lin = typeof(FARunner).GetField("line", BindingFlags.NonPublic | BindingFlags.Instance);
		static readonly FieldInfo _col = typeof(FARunner).GetField("column", BindingFlags.NonPublic | BindingFlags.Instance);
		static readonly FieldInfo _cur = typeof(FATextReaderRunner).GetField("current", BindingFlags.NonPublic | BindingFlags.Instance);
		static readonly FieldInfo _cap = typeof(FATextReaderRunner).GetField("capture", BindingFlags.NonPublic | BindingFlags.Instance);
		static readonly MethodInfo _capcl = typeof(StringBuilder).GetMethod("Clear", BindingFlags.Public | BindingFlags.Instance);
		static readonly MethodInfo _caplen = typeof(StringBuilder).GetProperty("Length", BindingFlags.Public | BindingFlags.Instance)?.GetGetMethod();
		static readonly MethodInfo _capts = typeof(StringBuilder).GetMethod("ToString", BindingFlags.Instance | BindingFlags.Public, null,Type.EmptyTypes,null);
#if FALIB_SPANS
		static readonly MethodInfo _slice = typeof(ReadOnlySpan<char>).GetMethod("Slice", BindingFlags.Instance | BindingFlags.Public, null,new Type[] { typeof(int), typeof(int) },null);
		static readonly MethodInfo _sts = typeof(ReadOnlySpan<char>).GetMethod("ToString", BindingFlags.Instance | BindingFlags.Public, null,Type.EmptyTypes,null);
#endif
		static readonly MethodInfo _sbstr = typeof(string).GetMethod("Substring", BindingFlags.Instance | BindingFlags.Public, null,new Type[] { typeof(int), typeof(int) },null);
		static readonly MethodInfo _fmcr = typeof(FAMatch).GetMethod("Create", BindingFlags.Static | BindingFlags.Public);
		static readonly MethodInfo _nextMatchBase = typeof(FARunner).GetMethod("NextMatch", BindingFlags.Instance | BindingFlags.Public);
		static readonly Type _ReturnType = typeof(FAMatch);
		private static void _EmitConst(ILGenerator il, int i)
		{
			switch (i)
			{
				case -1:
					il.Emit(OpCodes.Ldc_I4_M1);
					break;
				case 0:
					il.Emit(OpCodes.Ldc_I4_0);
					break;
				case 1:
					il.Emit(OpCodes.Ldc_I4_1);
					break;
				case 2:
					il.Emit(OpCodes.Ldc_I4_2);
					break;
				case 3:
					il.Emit(OpCodes.Ldc_I4_3);
					break;
				case 4:
					il.Emit(OpCodes.Ldc_I4_4);
					break;
				case 5:
					il.Emit(OpCodes.Ldc_I4_5);
					break;
				case 6:
					il.Emit(OpCodes.Ldc_I4_6);
					break;
				case 7:
					il.Emit(OpCodes.Ldc_I4_7);
					break;
				case 8:
					il.Emit(OpCodes.Ldc_I4_8);
					break;
				default:

					il.Emit(OpCodes.Ldc_I4, i);

					break;
			}
		}
		private static IList<MethodBuilder> _GenerateTextBlockEnds(TypeBuilder type, IList<FA> blockEnds, IProgress<int> progress)
		{
			System.Diagnostics.Debug.Assert(_cur != null);
			System.Diagnostics.Debug.Assert(_cap != null);
			System.Diagnostics.Debug.Assert(_capts != null);
			System.Diagnostics.Debug.Assert(_advRdr != null);
			System.Diagnostics.Debug.Assert(_fmcr != null);
			if (blockEnds == null)
			{
				return new MethodBuilder[0];
			}
			var result = new List<MethodBuilder>(blockEnds.Count);

			for (int i = 0; i < blockEnds.Count; ++i)
			{
				var be = blockEnds[i];
				if (be != null)
				{
					be = be.ToMinimizedDfa(progress);
					var closure = be.FillClosure();
					var mb = type.DefineMethod("BlockEnd" + i.ToString(), MethodAttributes.Private, typeof(FAMatch), new Type[]  {
						typeof(int) , // pos	1
						typeof(int) , // line	2
						typeof(int)  // column	3
					});
					result.Add(mb);
					var il = mb.GetILGenerator();
					il.DeclareLocal(typeof(int)); // ch 0
					var states = _DeclareLabelsForStates(il, closure);
					var error = il.DefineLabel();
					for (var q = 0; q < closure.Count; ++q)
					{
						var fromState = closure[q];
						il.MarkLabel(states[q]);
						var trnsgrp = fromState.FillInputTransitionRangesGroupedByState();
						foreach (var trn in trnsgrp)
						{
							var foundMatch = il.DefineLabel();
							var tryNextStateRanges = il.DefineLabel();
							il.Emit(OpCodes.Ldarg_0);
							il.Emit(OpCodes.Ldfld, _cur);
							il.Emit(OpCodes.Stloc_0); // ch = current;
							_GenerateRangesExpression(il, foundMatch, tryNextStateRanges, trn.Value);
							il.MarkLabel(foundMatch);
							// matched
							var si = closure.IndexOf(trn.Key);

							il.Emit(OpCodes.Ldarg_0);
							il.EmitCall(OpCodes.Call, _advRdr, null);
							il.Emit(OpCodes.Br, states[si]);
							il.MarkLabel(tryNextStateRanges);
						}
						// not matched
						if (fromState.IsAccepting)
						{
							_EmitConst(il, i);
							il.Emit(OpCodes.Ldarg_0);
							il.Emit(OpCodes.Ldfld, _cap);
							il.EmitCall(OpCodes.Callvirt, _capts, null);
							il.Emit(OpCodes.Ldarg_1);
							il.Emit(OpCodes.Conv_I8);
							il.Emit(OpCodes.Ldarg_2);
							il.Emit(OpCodes.Ldarg_3);
							il.EmitCall(OpCodes.Call, _fmcr, null);
							il.Emit(OpCodes.Ret);

						}
						else
						{
							il.Emit(OpCodes.Br, error);
						}
					}
					var error_eof = il.DefineLabel();
					il.MarkLabel(error);
					_EmitConst(il, -1);
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldfld, _cur);
					il.Emit(OpCodes.Beq_S, error_eof);
					il.Emit(OpCodes.Ldarg_0);
					il.EmitCall(OpCodes.Call, _advRdr, null);
					il.Emit(OpCodes.Br, states[0]);
					il.MarkLabel(error_eof);
					_EmitConst(il, -1);
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldfld, _cap);
					il.EmitCall(OpCodes.Callvirt, _capts, null);
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Conv_I8);
					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Ldarg_3);
					il.EmitCall(OpCodes.Call, _fmcr, null);
					il.Emit(OpCodes.Ret);
				}
				else
				{
					result.Add(null);
				}
			}
			return result;
		}
		private static IList<MethodBuilder> _GenerateStringBlockEnds(TypeBuilder type, IList<FA> blockEnds, IProgress<int> progress)
		{
			System.Diagnostics.Debug.Assert(_advStr != null);
			Type itype;
#if FALIB_SPANS
			if (FAStringRunner.UsingSpans)
			{
				itype = typeof(ReadOnlySpan<char>);
				System.Diagnostics.Debug.Assert(_slice != null);
				System.Diagnostics.Debug.Assert(_sts != null);
			} else
#endif
			{
				itype = typeof(string);
				System.Diagnostics.Debug.Assert(_sbstr != null);
			}

			System.Diagnostics.Debug.Assert(_fmcr != null);
			if (blockEnds == null)
			{
				return new MethodBuilder[0];
			}
			var result = new List<MethodBuilder>(blockEnds.Count);

			for (int i = 0; i < blockEnds.Count; ++i)
			{
				var be = blockEnds[i];
				if (be != null)
				{
					be = be.ToMinimizedDfa(progress);
					var closure = be.FillClosure();
					var mb = type.DefineMethod("BlockEnd" + i.ToString(), MethodAttributes.Private, typeof(FAMatch), new Type[]  { 
						itype, // span
						typeof(int), // cp		2
						typeof(int) , // len	3
						typeof(int) , // pos	4
						typeof(int) , // line	5
						typeof(int)  // column	6
					});
					result.Add(mb);
					var il = mb.GetILGenerator();
					il.DeclareLocal(typeof(int)); // ch 0
#if FALIB_SPANS
					if (FAStringRunner.UsingSpans)
					{
						il.DeclareLocal(typeof(ReadOnlySpan<char>)); // tmp 1
					}
#endif
					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Stloc_0); // ch = cp;
					var start = il.DefineLabel();
					il.MarkLabel(start);
					var states = _DeclareLabelsForStates(il, closure);
					il.Emit(OpCodes.Ldloc_0);
					_EmitConst(il, -1);
					il.Emit(OpCodes.Bne_Un, states[0]);
					var ret_err = il.DefineLabel();
					il.MarkLabel(ret_err);
					_EmitConst(il, -1);
#if FALIB_SPANS
					if (FAStringRunner.UsingSpans)
					{
						il.Emit(OpCodes.Ldarga_S, 1); // span&
					} else
#endif
					{
						il.Emit(OpCodes.Ldarg_1); // string
					}
					il.Emit(OpCodes.Ldarg_S, 4); // pos
					il.Emit(OpCodes.Ldarg_3); // len
#if FALIB_SPANS
					if (FAStringRunner.UsingSpans)
					{
						il.EmitCall(OpCodes.Callvirt, _slice, null);
						il.Emit(OpCodes.Stloc_1);
						il.Emit(OpCodes.Ldloca_S, 1);
						il.Emit(OpCodes.Constrained, typeof(ReadOnlySpan<char>));
						il.EmitCall(OpCodes.Callvirt, _sts, null);
					} else
#endif
					{
						il.EmitCall(OpCodes.Callvirt, _sbstr, null);
					}
					il.Emit(OpCodes.Ldarg_S, 4); //pos
					il.Emit(OpCodes.Conv_I8);
					il.Emit(OpCodes.Ldarg_S, 5); // line
					il.Emit(OpCodes.Ldarg_S, 6); // col
					il.EmitCall(OpCodes.Call, _fmcr, null);
					il.Emit(OpCodes.Ret);
					var error = il.DefineLabel();

					for (var q = 0; q < closure.Count; ++q)
					{
						var fromState = closure[q];
						il.MarkLabel(states[q]);
						var trnsgrp = fromState.FillInputTransitionRangesGroupedByState();
						foreach (var trn in trnsgrp)
						{
							var foundMatch = il.DefineLabel();
							var tryNextStateRanges = il.DefineLabel();
							_GenerateRangesExpression(il, foundMatch, tryNextStateRanges, trn.Value);
							il.MarkLabel(foundMatch);
							// matched
							var si = closure.IndexOf(trn.Key);

							il.Emit(OpCodes.Ldarg_0);
							il.Emit(OpCodes.Ldarg_1);
							il.Emit(OpCodes.Ldloca_S, 0);
							il.Emit(OpCodes.Ldarga_S, 3);
							il.Emit(OpCodes.Ldc_I4_0);
							il.EmitCall(OpCodes.Call, _advStr, null);
							il.Emit(OpCodes.Br, states[si]);
							il.MarkLabel(tryNextStateRanges);
						}
						// not matched
						if (fromState.IsAccepting)
						{
							_EmitConst(il, i);
							il.Emit(OpCodes.Ldarga,1); // span
							il.Emit(OpCodes.Ldarg_S, 4); // pos
							il.Emit(OpCodes.Ldarg_3);
#if FALIB_SPANS
							if (FAStringRunner.UsingSpans)
							{
								il.EmitCall(OpCodes.Callvirt, _slice, null);
								il.Emit(OpCodes.Stloc_1);
								il.Emit(OpCodes.Ldloca_S, 1);
								il.Emit(OpCodes.Constrained, typeof(ReadOnlySpan<char>));
								il.EmitCall(OpCodes.Callvirt, _sts, null);
							} else
#endif
							{
								il.EmitCall(OpCodes.Callvirt, _sbstr, null);
							}
							il.Emit(OpCodes.Ldarg_S, 4);
							il.Emit(OpCodes.Conv_I8);
							il.Emit(OpCodes.Ldarg_S, 5);
							il.Emit(OpCodes.Ldarg_S, 6);
							il.EmitCall(OpCodes.Call, _fmcr, null);
							il.Emit(OpCodes.Ret);

						}
						else
						{
							il.Emit(OpCodes.Br, error);
						}
					}
					il.MarkLabel(error);
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldloca_S, 0);
					il.Emit(OpCodes.Ldarga_S, 3);
					_EmitConst(il, 0);
					il.EmitCall(OpCodes.Call, _advStr, null);
					il.Emit(OpCodes.Br,start);
				}
				else
				{
					result.Add(null);
				}
			}
			return result;
		}
		private static void _GenerateRangesExpression(ILGenerator il, Label trueLabel, Label falseLabel, IList<FARange> ranges)
		{
			for (int i = 0; i < ranges.Count; ++i)
			{
				int first = ranges[i].Min;
				int last = ranges[i].Max;
				var next = il.DefineLabel();
				if (first != last)
				{
					il.Emit(OpCodes.Ldloc_0);
					_EmitConst(il, first);
					il.Emit(OpCodes.Blt, falseLabel);
					il.Emit(OpCodes.Ldloc_0);
					_EmitConst(il, last);
					il.Emit(OpCodes.Ble, trueLabel);
				}
				else
				{
					il.Emit(OpCodes.Ldloc_0);
					_EmitConst(il, first);
					il.Emit(OpCodes.Beq, trueLabel);

				}
				il.MarkLabel(next);
			}
			il.Emit(OpCodes.Br, falseLabel);
		}
		private static void _GenerateStringNextMatchImplBody(IList<FA> closure, IList<MethodBuilder> blockEnds, ILGenerator il)
		{
			System.Diagnostics.Debug.Assert(_pos != null);
			System.Diagnostics.Debug.Assert(_lin != null);
			System.Diagnostics.Debug.Assert(_col != null);
			System.Diagnostics.Debug.Assert(_advStr != null);
#if FALIB_SPANS
			if (FAStringRunner.UsingSpans)
			{
				System.Diagnostics.Debug.Assert(_slice != null);
				System.Diagnostics.Debug.Assert(_sts != null);
			} else
#endif
			{
				System.Diagnostics.Debug.Assert(_sbstr != null);
			}
			System.Diagnostics.Debug.Assert(_fmcr != null);
			il.DeclareLocal(typeof(int)); // 0 ch
			il.DeclareLocal(typeof(int)); // 1 len
			il.DeclareLocal(typeof(int)); // 2 p
			il.DeclareLocal(typeof(int)); // 3 l
			il.DeclareLocal(typeof(int)); // 4 c
#if FALIB_SPANS
			if (FAStringRunner.UsingSpans)
			{
				il.DeclareLocal(typeof(ReadOnlySpan<char>)); // 5 tmp
			}
#endif
			_EmitConst(il, -1);
			il.Emit(OpCodes.Stloc_0); // ch = -1
			var init = il.DefineLabel();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, _pos);
			_EmitConst(il, -1);
			il.Emit(OpCodes.Bgt, init); // if(position>-1) goto init;
			il.Emit(OpCodes.Ldarg_0);
			_EmitConst(il, 0);
			il.Emit(OpCodes.Stfld,_pos);
			il.MarkLabel(init);
			_EmitConst(il, 0);
			il.Emit(OpCodes.Stloc_1); // len = 0
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, _pos);
			il.Emit(OpCodes.Stloc_2); // p = position
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, _lin);
			il.Emit(OpCodes.Stloc_3); // l = line
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, _col);
			il.Emit(OpCodes.Stloc_S,4); // c = column

			il.Emit(OpCodes.Ldarg_0);
			il.Emit (OpCodes.Ldarg_1); // s
			il.Emit(OpCodes.Ldloca_S, 0); // &ch
			il.Emit(OpCodes.Ldloca_S, 1); // &len
			_EmitConst(il, 1); // true
			il.EmitCall(OpCodes.Callvirt, _advStr, null);
			var error = il.DefineLabel();
			var states = _DeclareLabelsForStates(il, closure);
			var q0trans = new List<FARange>();
			for (var q = 0; q < closure.Count; ++q)
			{
				var fromState = closure[q];
				il.MarkLabel(states[q]);
				var trnsgrp = fromState.FillInputTransitionRangesGroupedByState();
				foreach (var trn in trnsgrp)
				{
					if (q == 0)
					{
						q0trans.AddRange(trn.Value);
					}
					var foundMatch = il.DefineLabel();
					var tryNextStateRanges = il.DefineLabel();
					_GenerateRangesExpression(il, foundMatch, tryNextStateRanges, trn.Value);
					il.MarkLabel(foundMatch);
					// matched
					var si = closure.IndexOf(trn.Key);

					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldloca_S, 0);
					il.Emit(OpCodes.Ldloca_S, 1);
					_EmitConst(il, 0);
					il.EmitCall(OpCodes.Call, _advStr, null);
					il.Emit(OpCodes.Br, states[si]);
					il.MarkLabel(tryNextStateRanges);
				}
				// not matched
				if (fromState.IsAccepting)
				{
					MethodBuilder mi = blockEnds != null && blockEnds.Count > fromState.AcceptSymbol ? blockEnds[fromState.AcceptSymbol] : null;
					if (mi != null)
					{
						il.Emit(OpCodes.Ldarg_0);
						il.Emit(OpCodes.Ldarg_1); // span
						il.Emit(OpCodes.Ldloc_0); // ch
						il.Emit(OpCodes.Ldloc_1); // len
						il.Emit(OpCodes.Ldloc_2); // p
						il.Emit(OpCodes.Ldloc_3); // l
						il.Emit(OpCodes.Ldloc_S, 4); // c
						il.EmitCall(OpCodes.Call, mi, null);
						il.Emit(OpCodes.Ret);
					}
					else
					{
						_EmitConst(il, fromState.AcceptSymbol);
						if (FAStringRunner.UsingSpans)
						{
							il.Emit(OpCodes.Ldarga_S, 1); // span.
						} else
						{
							il.Emit(OpCodes.Ldarg_1); // string
						}
						il.Emit(OpCodes.Ldloc_2); // p
						il.Emit(OpCodes.Ldloc_1); // len
#if FALIB_SPANS
						if (FAStringRunner.UsingSpans)
						{
							il.EmitCall(OpCodes.Callvirt, _slice, null);
							il.Emit(OpCodes.Stloc_S, 5);
							il.Emit(OpCodes.Ldloca_S, 5);
							il.Emit(OpCodes.Constrained, typeof(ReadOnlySpan<char>));
							il.EmitCall(OpCodes.Callvirt, _sts, null);
						} else
#endif
						{
							il.EmitCall(OpCodes.Callvirt, _sbstr, null);
						}
						il.Emit(OpCodes.Ldloc_2);
						il.Emit(OpCodes.Conv_I8);
						il.Emit(OpCodes.Ldloc_3);
						il.Emit(OpCodes.Ldloc, 4);
						il.EmitCall(OpCodes.Call, _fmcr, null);
						il.Emit(OpCodes.Ret);
					}

				}
				else
				{
					il.Emit(OpCodes.Br, error);
				}
			}
			il.MarkLabel(error);
			var matchQ0 = il.DefineLabel();
			var nextError = il.DefineLabel();
			var notEmptyErr = il.DefineLabel();
			_EmitConst(il, 0);
			il.Emit(OpCodes.Ldloc_1); // if (len!=0) goto notEmptyErr;
			il.Emit(OpCodes.Bne_Un_S, notEmptyErr);
			il.Emit(OpCodes.Ldloc_0);
			_EmitConst(il, -1); // if(ch>-1) goto notEmptyErr;
			il.Emit(OpCodes.Bgt, notEmptyErr);
			// EOF. return FAMatch.Create(-2,null,0,0,0);
			_EmitConst(il, -2);
			il.Emit(OpCodes.Ldnull);
			_EmitConst(il, 0);
			il.Emit(OpCodes.Conv_I8);
			_EmitConst(il, 0);
			_EmitConst(il, 0);
			il.EmitCall(OpCodes.Call, _fmcr, null);
			il.Emit(OpCodes.Ret);
			il.MarkLabel(notEmptyErr);
			_GenerateRangesExpression(il, matchQ0, nextError, q0trans);
			il.MarkLabel(matchQ0);
			_EmitConst(il, -1);
#if FALIB_SPANS
			if (FAStringRunner.UsingSpans)
			{
				il.Emit(OpCodes.Ldarga_S, 1); // span.
			} else
#endif
			{
				il.Emit(OpCodes.Ldarg_1);
			}
			il.Emit(OpCodes.Ldloc_2); // p
			il.Emit(OpCodes.Ldloc_1); // len
#if FALIB_SPANS
			if (FAStringRunner.UsingSpans)
			{
				il.EmitCall(OpCodes.Callvirt, _slice, null);
				il.Emit(OpCodes.Stloc_S, 5);
				il.Emit(OpCodes.Ldloca_S, 5);
				il.Emit(OpCodes.Constrained, typeof(ReadOnlySpan<char>));
				il.EmitCall(OpCodes.Callvirt, _sts, null);
			} else
#endif
			{
				il.EmitCall(OpCodes.Callvirt, _sbstr, null);
			}
			il.Emit(OpCodes.Ldloc_2);
			il.Emit(OpCodes.Conv_I8);
			il.Emit(OpCodes.Ldloc_3);
			il.Emit(OpCodes.Ldloc_S, 4);
			il.EmitCall(OpCodes.Call, _fmcr, null);
			il.Emit(OpCodes.Ret);
			il.MarkLabel(nextError);
			_EmitConst(il, 0);
			il.Emit(OpCodes.Ldloc_0);
			il.Emit(OpCodes.Bgt, matchQ0);
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldloca_S, 0);
			il.Emit(OpCodes.Ldloca_S, 1);
			_EmitConst(il, 0);
			il.EmitCall(OpCodes.Call, _advStr, null);
			il.Emit(OpCodes.Br, error);

		}
		private static void _GenerateTextNextMatchBody(IList<FA> closure, IList<MethodBuilder> blockEnds, ILGenerator il)
		{
			System.Diagnostics.Debug.Assert(_pos != null);
			System.Diagnostics.Debug.Assert(_lin != null);
			System.Diagnostics.Debug.Assert(_col != null);
			System.Diagnostics.Debug.Assert(_advRdr != null);
			System.Diagnostics.Debug.Assert(_cap != null);
			System.Diagnostics.Debug.Assert(_caplen != null);
			System.Diagnostics.Debug.Assert(_cur != null);
			System.Diagnostics.Debug.Assert(_capts != null);
			System.Diagnostics.Debug.Assert(_capcl != null);
			System.Diagnostics.Debug.Assert(_fmcr != null);
			il.DeclareLocal(typeof(int)); // 0 ch
			il.DeclareLocal(typeof(int)); // 1 p
			il.DeclareLocal(typeof(int)); // 2 l
			il.DeclareLocal(typeof(int)); // 3 c
			il.DeclareLocal(typeof(FAMatch)); // 4 tmp
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, _cap);
			il.Emit(OpCodes.Callvirt, _capcl);
			il.Emit(OpCodes.Pop);
			var init = il.DefineLabel();
			var ret = il.DefineLabel();
			_EmitConst(il, -1);
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, _cur);
			il.Emit(OpCodes.Ble, init);
			il.Emit(OpCodes.Ldarg_0);
			il.EmitCall(OpCodes.Callvirt, _advRdr, null);
			il.MarkLabel(init);
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, _pos);
			il.Emit(OpCodes.Stloc_1); // p = position
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, _lin);
			il.Emit(OpCodes.Stloc_2); // l = line
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, _col);
			il.Emit(OpCodes.Stloc_3); // c = column

			var error = il.DefineLabel();
			var states = _DeclareLabelsForStates(il, closure);
			var q0trans = new List<FARange>();
			q0trans.Add(new FARange(-1, -1));
			for (var q = 0; q < closure.Count; ++q)
			{
				var fromState = closure[q];
				il.MarkLabel(states[q]);
				var trnsgrp = fromState.FillInputTransitionRangesGroupedByState();
				foreach (var trn in trnsgrp)
				{
					if (q == 0)
					{
						q0trans.AddRange(trn.Value);
					}
					var foundMatch = il.DefineLabel();
					var tryNextStateRanges = il.DefineLabel();
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldfld, _cur);
					il.Emit(OpCodes.Stloc_0); // ch = current

					_GenerateRangesExpression(il, foundMatch, tryNextStateRanges, trn.Value);
					il.MarkLabel(foundMatch);
					// matched
					var si = closure.IndexOf(trn.Key);

					il.Emit(OpCodes.Ldarg_0);
					il.EmitCall(OpCodes.Call, _advRdr, null);
					il.Emit(OpCodes.Br, states[si]);
					il.MarkLabel(tryNextStateRanges);
				}
				// not matched
				if (fromState.IsAccepting)
				{
					MethodBuilder mi = blockEnds != null && blockEnds.Count > fromState.AcceptSymbol ? blockEnds[fromState.AcceptSymbol] : null;
					if (mi != null)
					{
						il.Emit(OpCodes.Ldarg_0);
						il.Emit(OpCodes.Ldloc_1); // p
						il.Emit(OpCodes.Ldloc_2); // l
						il.Emit(OpCodes.Ldloc_3); // c
						il.EmitCall(OpCodes.Callvirt, mi, null);
						il.Emit(OpCodes.Ret);
					}
					else
					{
						_EmitConst(il, fromState.AcceptSymbol);
						il.Emit(OpCodes.Ldarg_0);
						il.Emit(OpCodes.Ldfld, _cap);
						il.EmitCall(OpCodes.Callvirt, _capts, null);
						il.Emit(OpCodes.Ldloc_1);
						il.Emit(OpCodes.Conv_I8);
						il.Emit(OpCodes.Ldloc_2);
						il.Emit(OpCodes.Ldloc_3);
						il.EmitCall(OpCodes.Call, _fmcr, null);
						il.Emit(OpCodes.Stloc_S, 4);
						il.Emit(OpCodes.Br, ret);
					}

				}
				else
				{
					il.Emit(OpCodes.Br, error);
				}
			}
			il.MarkLabel(error);
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, _cur);
			il.Emit(OpCodes.Stloc_0); // ch = current
			var matchQ0 = il.DefineLabel();
			var nextError = il.DefineLabel();
			var notEmptyErr = il.DefineLabel();
			_GenerateRangesExpression(il, matchQ0, nextError, q0trans);
			il.MarkLabel(matchQ0);
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, _cap);
			il.EmitCall(OpCodes.Callvirt, _caplen, null);
			_EmitConst(il, 0);
			il.Emit(OpCodes.Bgt, notEmptyErr);
			// EOF. return FAMatch.Create(-2,null,0,0,0);
			_EmitConst(il, -2);
			il.Emit(OpCodes.Ldnull);
			_EmitConst(il, 0);
			il.Emit(OpCodes.Conv_I8);
			_EmitConst(il, 0);
			_EmitConst(il, 0);
			il.EmitCall(OpCodes.Call, _fmcr, null);
			il.Emit(OpCodes.Stloc_S, 4);
			il.Emit(OpCodes.Br, ret);

			il.MarkLabel(notEmptyErr);

			_EmitConst(il, -1);
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, _cap);
			il.EmitCall(OpCodes.Callvirt, _capts, null);
			il.Emit(OpCodes.Ldloc_1);
			il.Emit(OpCodes.Conv_I8);
			il.Emit(OpCodes.Ldloc_2);
			il.Emit(OpCodes.Ldloc_3);
			il.EmitCall(OpCodes.Call, _fmcr, null);
			il.Emit(OpCodes.Ret);
			il.MarkLabel(nextError);
			il.Emit(OpCodes.Ldarg_0);
			il.EmitCall(OpCodes.Call, _advRdr, null);
			il.Emit(OpCodes.Br, error);
			il.MarkLabel(ret);
			il.Emit(OpCodes.Ldloc_S, 4);
			il.Emit(OpCodes.Ret);
		}
		private static Label[] _DeclareLabelsForStates(ILGenerator il, IList<FA> closure)
		{
			var result = new Label[closure.Count];
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = il.DefineLabel();
			}
			return result;
		}
		
		public static FAStringRunner CompileString(this FA fa, FA[] blockEnds = null, IProgress<int> progress = null)
		{
			System.Diagnostics.Debug.Assert(_RocharspanCnv!= null);
			System.Diagnostics.Debug.Assert(_string != null);
			System.Diagnostics.Debug.Assert(_nextMatchBase!= null);
			if (fa == null) throw new ArgumentNullException(nameof(fa));
			fa = fa.ToDfa(progress);
			IList<FA> closure = fa.FillClosure();
			string name = "FAStringRunner" + fa.GetHashCode();
			AssemblyName asmName = new AssemblyName(name);
			AssemblyBuilder asm = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
			ModuleBuilder mod = asm.DefineDynamicModule("M" + name);
			TypeBuilder type = mod.DefineType(name, TypeAttributes.Public | TypeAttributes.Sealed, typeof(FAStringRunner));
			IList<MethodBuilder> bems = _GenerateStringBlockEnds(type, blockEnds, progress);
			MethodBuilder nextMatchImpl = type.DefineMethod("_NextMatchImpl",
				MethodAttributes.Private,
				_ReturnType, new Type[] { 
#if FALIB_SPANS
					FAStringRunner.UsingSpans?typeof(ReadOnlySpan<char>): typeof(string)
#else
					typeof(string)
#endif
				});
			ILGenerator il = nextMatchImpl.GetILGenerator();
			_GenerateStringNextMatchImplBody(closure, bems, il);
			MethodBuilder nextMatch = type.DefineMethod("NextMatch",
				MethodAttributes.Public | MethodAttributes.ReuseSlot |
				MethodAttributes.Virtual | MethodAttributes.HideBySig,
				_ReturnType,Type.EmptyTypes);
			il = nextMatch.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld,_string);
			if (FAStringRunner.UsingSpans)
			{
				il.Emit(OpCodes.Call, _RocharspanCnv);
			}
			il.Emit(OpCodes.Call, nextMatchImpl);
			il.Emit(OpCodes.Ret);
			type.DefineMethodOverride(nextMatch, _nextMatchBase);
			Type newType = type.CreateType();
			System.Diagnostics.Debug.Assert(newType != null);
			var result = (FAStringRunner)Activator.CreateInstance(newType);
			System.Diagnostics.Debug.Assert(result != null);
			return result;
		}
		public static FATextReaderRunner CompileTextReader(this FA fa, FA[] blockEnds = null, IProgress<int> progress = null)
		{
			System.Diagnostics.Debug.Assert(_nextMatchBase != null);
			if (fa == null) throw new ArgumentNullException(nameof(fa));
			fa = fa.ToDfa(progress);
			IList<FA> closure = fa.FillClosure();
			string name = "FATextReaderRunner" + fa.GetHashCode();
			AssemblyName asmName = new AssemblyName(name);
			AssemblyBuilder asm = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
			ModuleBuilder mod = asm.DefineDynamicModule("M" + name);
			TypeBuilder type = mod.DefineType(name, TypeAttributes.Public | TypeAttributes.Sealed, typeof(FATextReaderRunner));
			IList<MethodBuilder> bems = _GenerateTextBlockEnds(type, blockEnds, progress);
			MethodBuilder nextMatch = type.DefineMethod("NextMatch",
				MethodAttributes.Public | MethodAttributes.ReuseSlot |
				MethodAttributes.Virtual | MethodAttributes.HideBySig,
				_ReturnType, Type.EmptyTypes);
			ILGenerator il = nextMatch.GetILGenerator();
			_GenerateTextNextMatchBody(closure, bems, il);
			type.DefineMethodOverride(nextMatch,_nextMatchBase);	
			Type newType = type.CreateType();
			System.Diagnostics.Debug.Assert(newType != null);
			var result = (FATextReaderRunner)Activator.CreateInstance(newType);
			System.Diagnostics.Debug.Assert(result != null);
			return result;
		}
		/// <summary>
		/// Returns a <see cref="FARunner"/> over the input
		/// </summary>
		/// <param name="fa">The state to run</param>
		/// <param name="string">The string to evaluate</param>
		/// <param name="blockEnds">The block end expressions</param>
		/// <param name="compiled">Indicates whether or not this expression should be compiled</param>
		/// <returns>A new runner that can match strings given the current instance</returns>
		public static FAStringRunner Run(this FA fa, string @string, FA[] blockEnds, bool compiled)
		{
			if(compiled)
			{
				var result = CompileString(fa, blockEnds, null);
				result.Set(@string);
				return result;
			}
			else
			{
				return fa.Run(@string, blockEnds);
			}
		}
		/// <summary>
		/// Returns a <see cref="FARunner"/> over the input
		/// </summary>
		/// <param name="fa">The state to run</param>
		/// <param name="reader">The text to evaluate</param>
		/// <param name="blockEnds">The block end expressions</param>
		/// <param name="compiled">Indicates whether or not this expression should be compiled</param>
		/// <returns>A new runner that can match text given the current instance</returns>
		public static FATextReaderRunner Run(this FA fa, TextReader reader, FA[] blockEnds, bool compiled)
		{
			if (compiled)
			{
				var result = CompileTextReader(fa, blockEnds, null);
				result.Set(reader);
				return result;
			}
			else
			{
				return fa.Run(reader, blockEnds);
			}
		}
	}
}