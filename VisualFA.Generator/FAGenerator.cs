﻿using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;

namespace VisualFA
{

#if FALIB
	public
#endif
	enum FAGeneratorDependencies
	{
		/// <summary>
		/// No dependent code will be generated
		/// </summary>
		None,
		/// <summary>
		/// Shared code will be generated
		/// </summary>
		GenerateSharedCode,
		/// <summary>
		/// The runtime library will be referenced
		/// </summary>
		UseRuntime
	}
#if FALIB
	public
#endif
	partial class FAGeneratorOptions
	{
		public FAGeneratorDependencies Dependencies { get; set; } = FAGeneratorDependencies.None;

		public bool GenerateTables { get; set; } = false;
		public bool GenerateTextReaderRunner { get; set; } = false;
		// not always supported
#if FALIB_SPANS
		public bool UseSpans { get; set; } = FAStringRunner.UsingSpans;
#endif
		public string ClassName { get; set; } = "GeneratedRunner";
		public string Namespace { get; set; } = "";
	}
#if FALIB
	public
#endif
	static partial class FAGenerator
	{
		static CodeBinaryOperatorExpression _GenerateRangesExpression(CodeExpression codepoint, IList<FARange> ranges)
		{
			CodeBinaryOperatorExpression result = null;
			for (int i = 0; i < ranges.Count ; ++i)
			{
				var first = ranges[i].Min;
				int last = ranges[i].Max;
				var fp = new CodePrimitiveExpression(first);

				if (first != last)
				{
					var lp = new CodePrimitiveExpression(last);
					var exp = new CodeBinaryOperatorExpression(codepoint, CodeBinaryOperatorType.GreaterThanOrEqual, fp);
					exp = new CodeBinaryOperatorExpression(exp, CodeBinaryOperatorType.BooleanAnd, new CodeBinaryOperatorExpression(codepoint, CodeBinaryOperatorType.LessThanOrEqual, lp));
					if (result == null)
					{
						result = exp;
					}
					else
					{
						result = new CodeBinaryOperatorExpression(result, CodeBinaryOperatorType.BooleanOr, exp);
					}


				}
				else
				{
					var exp = new CodeBinaryOperatorExpression(codepoint, CodeBinaryOperatorType.ValueEquality, fp);
					if (result == null)
					{
						result = exp;
					}
					else
					{
						result = new CodeBinaryOperatorExpression(result, CodeBinaryOperatorType.BooleanOr, exp);
					}
				}
			}
			return result;
		}
		static void _GenerateMatchImplBody(bool textReader, IList<FA> closure, IList<FA> blockEnds, CodeStatementCollection dest, FAGeneratorOptions opts)
		{
			var lcapturebuffer = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "capture");
			var lccbts = new CodeMethodInvokeExpression(lcapturebuffer, "ToString");
			var crm = new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("FAMatch"), "Create");
			CodeStatement adv;
			CodeExpression tostr = null;
			if (!textReader)
			{
#if FALIB_SPANS
				if(opts.UseSpans)
				{
					tostr = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("s"), "Slice", new CodeVariableReferenceExpression("p"), new CodeVariableReferenceExpression("len")), "ToString"));
				}
#endif
				if(tostr==null)
				{
					tostr = new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("s"), "Substring", new CodeVariableReferenceExpression("p"), new CodeVariableReferenceExpression("len"));
				}
				adv = new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "Advance"), new CodeExpression[]
				{
					new CodeArgumentReferenceExpression("s"),
					new CodeDirectionExpression(FieldDirection.Ref, new CodeVariableReferenceExpression("ch")),
					new CodeDirectionExpression(FieldDirection.Ref, new CodeVariableReferenceExpression("len")),
					new CodePrimitiveExpression(false)

				}));
				
			} else
			{
				tostr = lccbts;
				adv = new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "Advance")));
			}
			dest.Add(new CodeVariableDeclarationStatement(typeof(int), "ch"));
			if (!textReader)
			{
				dest.Add(new CodeVariableDeclarationStatement(typeof(int), "len"));
			}
			dest.Add(new CodeVariableDeclarationStatement(typeof(int), "p"));
			dest.Add(new CodeVariableDeclarationStatement(typeof(int), "l"));
			dest.Add(new CodeVariableDeclarationStatement(typeof(int), "c"));
			dest.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("ch"), new CodePrimitiveExpression(-1)));
			if (textReader)
			{
				dest.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(lcapturebuffer, "Clear"))));
				dest.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "current"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(-2)),
					new CodeStatement[] {
						new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(),"Advance"))
					}));
			} else
			{
				dest.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("len"), new CodePrimitiveExpression(0)));
				dest.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "position"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(-1)),
					new CodeStatement[] {
						new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),new CodePrimitiveExpression(0))
					}));
			}
			dest.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("p"), new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "position")));
			dest.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("l"), new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "line")));
			dest.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("c"), new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "column")));
			if (!textReader)
			{
				dest.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "Advance"), new CodeExpression[]
				{
					new CodeArgumentReferenceExpression("s"),
					new CodeDirectionExpression(FieldDirection.Ref, new CodeVariableReferenceExpression("ch")),
					new CodeDirectionExpression(FieldDirection.Ref, new CodeVariableReferenceExpression("len")),
					new CodePrimitiveExpression(true)

				})));
			}
			CodeExpression cmp;
			if (textReader)
			{
				cmp = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "current");
			}
			else
			{
				cmp = new CodeVariableReferenceExpression("ch");
			}
			var q0ranges = new List<FARange>();
			q0ranges.Add(new FARange(-1, -1));
			for (var q = 0; q < closure.Count; ++q)
			{
				var fromState = closure[q];

				CodeLabeledStatement state = null;
				if (q > 0 || FA.IsLoop(closure))
				{
					state = new CodeLabeledStatement("q" + q.ToString());
					dest.Add(state);
				} else if(q==0)
				{
					
					dest.Add(new CodeCommentStatement("q0:"));
				}
				var trnsgrp = fromState.FillInputTransitionRangesGroupedByState();
				var attachedlabel = false;

				foreach (var trn in trnsgrp)
				{
					if (q == 0)
					{
						q0ranges.AddRange(trn.Value);
					}

					var iftrans = new CodeConditionStatement(_GenerateRangesExpression(cmp, trn.Value));
					if (!attachedlabel)
					{
						attachedlabel = true;
						if (state != null)
						{
							state.Statement = iftrans;
						} else
						{
							dest.Add(iftrans);
						}
					}
					else
					{
						dest.Add(iftrans);
					}
					iftrans.TrueStatements.AddRange(new CodeStatement[] {
						adv,
						new CodeGotoStatement("q"+closure.IndexOf(trn.Key).ToString())
					});

				}
				if (fromState.IsAccepting)
				{
					if (blockEnds != null && blockEnds.Count > fromState.AcceptSymbol && blockEnds[fromState.AcceptSymbol] != null)
					{
						CodeMethodReturnStatement retbe = new CodeMethodReturnStatement();
						if (textReader) {
							retbe.Expression = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(null, "_BlockEnd" + fromState.AcceptSymbol.ToString()),
								new CodeExpression[] {
								new CodeVariableReferenceExpression("p"),
								new CodeVariableReferenceExpression("l"),
								new CodeVariableReferenceExpression("c")});
						} else
						{
							retbe.Expression = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(null, "_BlockEnd" + fromState.AcceptSymbol.ToString()),
								new CodeExpression[] {
								new CodeArgumentReferenceExpression("s"),
								new CodeVariableReferenceExpression("ch"),
								new CodeVariableReferenceExpression("len"),
								new CodeVariableReferenceExpression("p"),
								new CodeVariableReferenceExpression("l"),
								new CodeVariableReferenceExpression("c")});
						}
						if (!attachedlabel)
						{
							attachedlabel = true;
							if (state != null)
							{
								state.Statement = retbe;
							} else
							{
								dest.Add(retbe);
							}
						}
						else
						{
							dest.Add(retbe);
						}
					}
					else
					{
						var retmatch = new CodeMethodReturnStatement();
							retmatch.Expression = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("FAMatch"), "Create"),
							new CodeExpression[] {
								new CodePrimitiveExpression(fromState.AcceptSymbol),
								tostr,
								new CodeVariableReferenceExpression("p"),
								new CodeVariableReferenceExpression("l"),
								new CodeVariableReferenceExpression("c")
							});
						
						if (!attachedlabel)
						{
							attachedlabel = true;
							if (state != null)
							{
								state.Statement = retmatch;
							} else
							{
								dest.Add(retmatch);
							}
						}
						else
						{
							dest.Add(retmatch);
						}
					}
				}
				else
				{
					var gerror = new CodeGotoStatement("errorout");
					if (!attachedlabel)
					{
						attachedlabel = true;
						if (state != null)
						{
							state.Statement = gerror;
						} else
						{
							dest.Add(gerror);
						}
					}
					else
					{
						dest.Add(gerror);
					}
				}
			}
			var error = new CodeLabeledStatement("errorout");
	
			var ifq0match = new CodeConditionStatement(_GenerateRangesExpression(cmp, q0ranges.ToArray()));
			dest.Add(error);
			
			error.Statement = ifq0match;
			
			
			CodeExpression isEmpty;
			if (textReader) {
				isEmpty = new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"capture"),"Length"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(0));
			} else
			{
				isEmpty = new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("len"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(0));
			}
			var checkIfEnd = new CodeConditionStatement(isEmpty);
			checkIfEnd.TrueStatements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("FAMatch"), "Create"),
						new CodeExpression[] {
							new CodePrimitiveExpression(-2),
							new CodePrimitiveExpression(null),
							new CodePrimitiveExpression(0),
							new CodePrimitiveExpression(0),
							new CodePrimitiveExpression(0)
						})));
			ifq0match.TrueStatements.AddRange(new CodeStatement[] {
				checkIfEnd,
				new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("FAMatch"), "Create"),
						new CodeExpression[] {
							new CodePrimitiveExpression(-1),
							tostr,
							new CodeVariableReferenceExpression("p"),
							new CodeVariableReferenceExpression("l"),
							new CodeVariableReferenceExpression("c")
						}))
			});
			dest.Add(adv);
			if(textReader)
			{
				dest.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("ch"), new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "current")));
			}
			dest.Add(new CodeGotoStatement(error.Label));
		}
		private static void _GenerateBlockEnds(bool textReader, CodeTypeDeclaration type, IList<FA> blockEnds, FAGeneratorOptions opts)
		{
			if (blockEnds == null)
			{
				return;
			}
			CodeStatement adv;
			CodeExpression tostr = null;
			if (!textReader)
			{
#if FALIB_SPANS
				if (opts.UseSpans)
				{
					tostr = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("s"), "Slice", new CodeArgumentReferenceExpression("position"), new CodeArgumentReferenceExpression("len")), "ToString"));
				}
#endif
				if (tostr == null)
				{
					tostr = new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("s"), "Substring", new CodeArgumentReferenceExpression("position"), new CodeArgumentReferenceExpression("len"));
				}
				adv = new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "Advance"), new CodeExpression[]
				{
					new CodeArgumentReferenceExpression("s"),
					new CodeDirectionExpression(FieldDirection.Ref, new CodeArgumentReferenceExpression("cp")),
					new CodeDirectionExpression(FieldDirection.Ref, new CodeArgumentReferenceExpression("len")),
					new CodePrimitiveExpression(false)

				}));
				
			}
			else
			{
				tostr = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "capture"), "ToString"));
				adv = new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "Advance")));
			}
			var position = new CodeArgumentReferenceExpression("position");
			var line = new CodeArgumentReferenceExpression("line");
			var column = new CodeArgumentReferenceExpression("column");
			var lcapturebuffer = new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "capture");
			var lccbts = new CodeMethodInvokeExpression(lcapturebuffer, "ToString");
			var crm = new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(FAMatch).Name), "Create");

			for (int i = 0; i < blockEnds.Count; ++i)
			{
				var be = blockEnds[i];
				if (be != null)
				{
					var closure = be.FillClosure();
					var ch = new CodeVariableReferenceExpression("ch");

					var mb = new CodeMemberMethod();
					mb.Name = "_BlockEnd" + i.ToString();
					mb.Attributes = MemberAttributes.Private;
					mb.ReturnType = new CodeTypeReference(typeof(FAMatch).Name);
					if (!textReader)
					{
#if FALIB_SPANS
						if (opts.UseSpans)
						{
							mb.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference("ReadOnlySpan`1", new CodeTypeReference[] { new CodeTypeReference(typeof(char)) }), "s"));
						}
#endif
						if (mb.Parameters.Count == 0)
						{
							mb.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "s"));
						}
						mb.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "cp"));
						mb.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)), "len"));
					}
					mb.Parameters.AddRange(new CodeParameterDeclarationExpression[] {
						new CodeParameterDeclarationExpression(typeof(int),"position"),
						new CodeParameterDeclarationExpression(typeof(int),"line"),
						new CodeParameterDeclarationExpression(typeof(int),"column")
					});
					type.Members.Add(mb);
					var dest = mb.Statements;
					
					CodeExpression cmp;
					if (textReader)
					{
						cmp = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "current");
					} else
					{
						cmp = new CodeArgumentReferenceExpression("cp");
					}
					for (var q = 0; q < closure.Count; ++q)
					{
						var fromState = closure[q];
						var state = new CodeLabeledStatement("q" + q.ToString());
						dest.Add(state);
						var trnsgrp = fromState.FillInputTransitionRangesGroupedByState();
						var attachedlabel = false;
						foreach (var trn in trnsgrp)
						{
							
							var iftrans = new CodeConditionStatement(_GenerateRangesExpression(cmp, trn.Value));
							if (!attachedlabel)
							{
								attachedlabel = true;
								state.Statement = iftrans;
							}
							else
							{
								dest.Add(iftrans);
							}
							iftrans.TrueStatements.AddRange(new CodeStatement[] {
								adv,
								new CodeGotoStatement("q"+closure.IndexOf(trn.Key).ToString())
							});
						}
						if (fromState.IsAccepting)
						{

							var retmatch = new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(FAMatch).Name), "Create"),
								new CodeExpression[] {
							new CodePrimitiveExpression(fromState.AcceptSymbol),
							tostr,
									position,
							line,
							column
								}));
							if (!attachedlabel)
							{
								attachedlabel = true;
								state.Statement = retmatch;
							}
							else
							{
								dest.Add(retmatch);
							}

						}
						else
						{
							var gerror = new CodeGotoStatement("errorout");
							if (!attachedlabel)
							{
								attachedlabel = true;
								state.Statement = gerror;
							}
							else
							{
								dest.Add(gerror);
							}
						}
					}
					var error = new CodeLabeledStatement("errorout");
					
					var ifEnd = new CodeConditionStatement(new CodeBinaryOperatorExpression(cmp, CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(-1)), new CodeStatement[] {
						new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(FAMatch).Name), "Create"),
								new CodeExpression[] {
							new CodePrimitiveExpression(-1),
							tostr,
							position,
							line,
							column
								}))
				}) ;

					error.Statement = ifEnd;
					dest.Add(error);
					dest.Add(adv);
					dest.Add(new CodeGotoStatement("q0"));
				}
			}
		}

		private static CodeTypeDeclaration _GenerateRunner(bool textReader, IList<FA> closure, IList<FA> blockEnds, FAGeneratorOptions options)
		{
			var result = new CodeTypeDeclaration(options.ClassName);
			result.TypeAttributes = TypeAttributes.NotPublic | TypeAttributes.Sealed;
			result.BaseTypes.Add(new CodeTypeReference(textReader ? typeof(FATextReaderRunner).Name : typeof(FAStringRunner).Name));
			result.IsClass = true;
			result.IsPartial = true;
			var nextMatch = new CodeMemberMethod();
			nextMatch.Name = "NextMatch";
			nextMatch.Attributes = MemberAttributes.Public | MemberAttributes.Override;
			nextMatch.ReturnType = new CodeTypeReference(typeof(FAMatch).Name);
			CodeStatementCollection target;
			_GenerateBlockEnds(textReader, result, blockEnds,options);
			if (!options.GenerateTextReaderRunner)
			{
				var nextMatchImpl = new CodeMemberMethod();
				nextMatchImpl.Name = "NextMatchImpl";
				CodeTypeReference pt = null;
#if FALIB_SPANS
				if (options.UseSpans)
				{
					pt = new CodeTypeReference("ReadOnlySpan`1", new CodeTypeReference[] { new CodeTypeReference(typeof(char)) });
				}
#endif
				if(pt==null)
				{
					pt = new CodeTypeReference(typeof(string));
				}
				nextMatchImpl.Parameters.Add(new CodeParameterDeclarationExpression(pt, "s"));
				nextMatchImpl.ReturnType = new CodeTypeReference(typeof(FAMatch).Name);
				result.Members.Add(nextMatchImpl);
				target = nextMatchImpl.Statements;
				nextMatch.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), nextMatchImpl.Name), new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "string"))));
			} else
			{
				target = nextMatch.Statements;
			}
			
			_GenerateMatchImplBody(textReader,closure, blockEnds, target, options);
			result.Members.Add(nextMatch);
			return result;
		}
		private static CodeArrayCreateExpression _CreateDfaArray(int[] dfa)
		{
			var result = new CodeArrayCreateExpression(new CodeTypeReference(typeof(int)));
			for (int i = 0; i < dfa.Length; ++i)
			{
				result.Initializers.Add(new CodePrimitiveExpression(dfa[i]));
			}
			return result;
		}
		private static CodeArrayCreateExpression _CreateBlockEndArray(IList<int[]> blockEnds)
		{
			var intArrays = new CodeTypeReference(new CodeTypeReference(new CodeTypeReference(typeof(int)), 1), 1);
			var result = new CodeArrayCreateExpression(intArrays);
			var nullExp = new CodePrimitiveExpression(null);
			for (int i = 0; i < blockEnds.Count; ++i)
			{
				if (blockEnds[i] == null)
				{
					result.Initializers.Add(nullExp);
				}
				else
				{
					result.Initializers.Add(_CreateDfaArray(blockEnds[i]));
				}
			}
			return result;
		}
		private static CodeTypeDeclaration _GenerateTableRunner(bool textReader,IList<FA> closure, IList<FA> blockEnds, FAGeneratorOptions options)
		{
			var result = new CodeTypeDeclaration(options.ClassName);
			var hasBlockEnds = (blockEnds != null && blockEnds.Count > 0);

			result.TypeAttributes = TypeAttributes.NotPublic | TypeAttributes.Sealed;
			result.BaseTypes.Add(new CodeTypeReference(textReader? typeof(FATextReaderDfaTableRunner).Name : typeof(FAStringDfaTableRunner).Name));
			var dfaField = new CodeMemberField(new CodeTypeReference(new CodeTypeReference(typeof(int)), 1), "_dfa");
			dfaField.Attributes = MemberAttributes.Private | MemberAttributes.Static;
			dfaField.InitExpression = _CreateDfaArray(closure[0].ToArray());
			result.Members.Add(dfaField);
			if (hasBlockEnds)
			{
				var blockEndsField = new CodeMemberField(new CodeTypeReference(new CodeTypeReference(new CodeTypeReference(typeof(int)), 1), 1), "_blockEnds");
				blockEndsField.Attributes = MemberAttributes.Private | MemberAttributes.Static;
				var belist = new List<int[]>(blockEnds.Count);
				for (int i = 0; i < blockEnds.Count; ++i)
				{
					if (blockEnds[i] != null)
					{
						belist.Add(blockEnds[i].ToArray());
					}
					else
					{
						belist.Add(null);
					}
				}
				blockEndsField.InitExpression = _CreateBlockEndArray(belist);
				result.Members.Add(blockEndsField);
			}
			var ctor = new CodeConstructor();
			ctor.Attributes = MemberAttributes.Public;
			ctor.BaseConstructorArgs.Add(new CodeFieldReferenceExpression(null, "_dfa"));
			ctor.BaseConstructorArgs.Add(hasBlockEnds ? (CodeExpression)new CodeFieldReferenceExpression(null, "_blockEnds") : (CodeExpression)new CodePrimitiveExpression(null));
			result.Members.Add(ctor);
			result.IsClass = true;
			result.IsPartial = true;
			return result;
		}
		public static CodeCompileUnit Generate(this FA fa, IList<FA> blockEnds = null, FAGeneratorOptions options = null, IProgress<int> progress = null)
		{
			if (options == null)
			{
				options = new FAGeneratorOptions();
			}
			if (!fa.IsDeterministic)
			{
				fa = fa.ToDfa(progress);
			}
			if (blockEnds != null)
			{
				for (int i = 0; i < blockEnds.Count; ++i)
				{
					var be = blockEnds[i];
					if (be != null)
					{
						blockEnds[i] = be.ToMinimizedDfa(progress);
					}
				}
			}

			var closure = fa.FillClosure();
			var result = new CodeCompileUnit();
			var ns = new CodeNamespace();
			if (options.Namespace != null)
			{
				ns.Name = options.Namespace;
			}
			switch (options.Dependencies)
			{
				case FAGeneratorDependencies.None:
					result.ReferencedAssemblies.Add(typeof(WeakReference<>).Assembly.FullName);
					ns.Imports.AddRange(new CodeNamespaceImport[] { new CodeNamespaceImport("System"), new CodeNamespaceImport("System.IO"), new CodeNamespaceImport("System.Text"), new CodeNamespaceImport("System.Collections.Generic")});
					break;
				case FAGeneratorDependencies.UseRuntime:
					result.ReferencedAssemblies.Add(typeof(WeakReference<>).Assembly.FullName);
					result.ReferencedAssemblies.Add(typeof(FA).Assembly.FullName);
					ns.Imports.AddRange(new CodeNamespaceImport[] { new CodeNamespaceImport("System"), new CodeNamespaceImport("System.IO"), new CodeNamespaceImport("System.Text"), new CodeNamespaceImport("System.Collections.Generic"), new CodeNamespaceImport("VisualFA") });
					break;
				case FAGeneratorDependencies.GenerateSharedCode:

					ns.Imports.AddRange(new CodeNamespaceImport[] { new CodeNamespaceImport("System"), new CodeNamespaceImport("System.IO"), new CodeNamespaceImport("System.Text"), new CodeNamespaceImport("System.Collections.Generic") });
#if FALIB_SPANS
					ns.Types.AddRange(Deslanged.GetFAMatch(options.UseSpans).Namespaces[0].Types);
					ns.Types.AddRange(Deslanged.GetFARunner(options.UseSpans).Namespaces[0].Types);
#else
					ns.Types.AddRange(Deslanged.GetFAMatch(false).Namespaces[0].Types);
					ns.Types.AddRange(Deslanged.GetFARunner(false).Namespaces[0].Types);
#endif
					if (options.GenerateTables)
					{
#if FALIB_SPANS
						ns.Types.AddRange(Deslanged.GetFADfaTableRunner(options.UseSpans).Namespaces[0].Types);
#else
						ns.Types.AddRange(Deslanged.GetFADfaTableRunner(false).Namespaces[0].Types);
#endif
					}
					break;
			}			
			result.Namespaces.Add(ns);
			if (options.GenerateTables)
			{
				if (options.GenerateTextReaderRunner)
				{
					ns.Types.Add(_GenerateTableRunner(true, closure, blockEnds, options));
				}
				else
				{
					ns.Types.Add(_GenerateTableRunner(false, closure, blockEnds, options));
				}
				
			}
			else
			{
				if (options.GenerateTextReaderRunner)
				{
					ns.Types.Add(_GenerateRunner(true, closure, blockEnds, options));
				}
				else
				{
					ns.Types.Add(_GenerateRunner(false, closure, blockEnds, options));
				}	
			}
			var ver = typeof(FA).Assembly.GetName().Version.ToString();
			var gendecl = new CodeAttributeDeclaration(new CodeTypeReference(typeof(GeneratedCodeAttribute)), new CodeAttributeArgument(new CodePrimitiveExpression("Visual FA")), new CodeAttributeArgument(new CodePrimitiveExpression(ver)));
			foreach (CodeTypeDeclaration type in ns.Types)
			{
				type.CustomAttributes.Add(gendecl);
			}
			return result;
		}
	}
}