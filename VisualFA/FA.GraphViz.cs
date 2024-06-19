using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace VisualFA
{
	/// <summary>
	/// Represents optional rendering parameters for a dot graph.
	/// </summary>
#if FALIB
	public
#endif
		sealed class FADotGraphOptions
	{
		/// <summary>
		/// The resolution, in dots-per-inch to render at
		/// </summary>
		public int Dpi { get; set; } = 300;
		/// <summary>
		/// The prefix used for state labels
		/// </summary>
		public string StatePrefix { get; set; } = "q";

		/// <summary>
		/// If non-null, specifies a debug render using the specified input string.
		/// </summary>
		/// <remarks>The debug render is useful for tracking the transitions in a state machine</remarks>
		public IEnumerable<char> DebugString { get; set; } = null;
		/// <summary>
		/// Indicates the block ends to render
		/// </summary>
		/// <remarks>Does not work when debug settings are set</remarks>
		public FA[] BlockEnds { get; set; } = null;
		/// <summary>
		/// If non-null, specifies the source NFA from which this DFA was derived - used for debug view
		/// </summary>
		public FA DebugSourceNfa { get; set; } = null;
		/// <summary>
		/// True to show the NFA the DFA was sourced from
		/// </summary>
		public bool DebugShowNfa { get; set; } = false;
		/// <summary>
		/// True to hide the accept symbols, otherwise they will be shown on the accepting states
		/// </summary>
		public bool HideAcceptSymbolIds { get; set; } = false;
		/// <summary>
		/// Maps accept ids to names for display in the graph
		/// </summary>
		public string[] AcceptSymbolNames { get; set; } = null;
		/// <summary>
		/// True to generate vertical output. Otherwise it will be left to right.
		/// </summary>
		public bool Vertical { get; set; } = false;
	}
	partial class FA
	{

		static void _WriteCompoundDotTo(IList<FA> closure, TextWriter writer, FADotGraphOptions options = null)
		{
			writer.WriteLine("digraph FA {");
			var vert = true;
			if (options == null || !options.Vertical)
			{
				writer.WriteLine("rankdir=LR");
				vert = false;
			}
			writer.WriteLine("node [shape=circle]");
			var opt2 = new FADotGraphOptions();
			opt2.DebugSourceNfa = null;
			opt2.StatePrefix = options.StatePrefix;
			opt2.DebugString = options.DebugString;
			opt2.DebugShowNfa = false;
			opt2.Dpi = options.Dpi;
			opt2.AcceptSymbolNames = options.AcceptSymbolNames;
			opt2.HideAcceptSymbolIds = options.HideAcceptSymbolIds;
			opt2.BlockEnds = options.BlockEnds;
			//opt2.Vertical = options.Vertical;
			if (!vert)
			{
				_WriteDotTo(closure, writer, options, 2);
				_WriteDotTo(options.DebugSourceNfa.FillClosure(), writer, opt2, 1);
			} else
			{
				_WriteDotTo(options.DebugSourceNfa.FillClosure(), writer, opt2, 2);
				_WriteDotTo(closure, writer, options, 1);
			}
			writer.WriteLine("}");
		}
		/// <summary>
		/// Writes a Graphviz dot specification of the specified closure to the specified <see cref="TextWriter"/>
		/// </summary>
		/// <param name="closure">The closure of all states</param>
		/// <param name="writer">The writer</param>
		/// <param name="options">A <see cref="FADotGraphOptions"/> instance with any options, or null to use the defaults</param>
		static void _WriteDotTo(IList<FA> closure, TextWriter writer, FADotGraphOptions options = null, int clusterIndex = -1)
		{
			if (null == options) options = new FADotGraphOptions();
			var hasBlockEnds = options.DebugShowNfa == false && options.DebugString == null && options.BlockEnds != null;
			string spfx = null == options.StatePrefix ? "q" : options.StatePrefix;
			string pfx = "";
			if (clusterIndex != -1)
			{
				writer.WriteLine("subgraph cluster_" + clusterIndex.ToString()+" {");
				pfx = "c" + clusterIndex.ToString();
			}
			else
			{
				writer.WriteLine("digraph FA {");
			}
			if (!options.Vertical)
			{
				writer.WriteLine("rankdir=LR");
			}
			writer.WriteLine("node [shape=circle]");
			var accepting = new List<FA>();
			var finals = new List<FA>();
			var neutrals = new List<FA>();
			foreach (var ffa in closure)
			{
				if (ffa.IsAccepting)
				{
					accepting.Add(ffa);
				}
				else if (ffa.IsNeutral)
				{
					neutrals.Add(ffa);
				}
				else if (ffa.IsFinal)
				{
					finals.Add(ffa);
				}
			}

			IList<FA> fromStates = null;
			IList<FA> toStates = null;

			int tchar = -1;
			if (null != options.DebugString)
			{
				
				foreach (int ch in ToUtf32(options.DebugString))
				{
					if(fromStates==null)
					{
						_Seen.Clear();
						fromStates = new List<FA>();
						closure[0]._EpsilonClosure(fromStates, _Seen);
					} else
					{
						fromStates = toStates;
					}
					tchar = ch;
					System.Diagnostics.Debug.Assert(fromStates != null);
					toStates = FillMove(fromStates, ch);

					if (0 == toStates.Count)
						break;
				}
			}
			if(fromStates==null) 
			{
				_Seen.Clear();
				fromStates = new List<FA>();
				closure[0]._EpsilonClosure(fromStates, _Seen);
			}
			if (null != toStates)
			{
				toStates = FillEpsilonClosure(toStates, null);
			} else
			{
				toStates = fromStates;
			}
			int i = 0;
			foreach (var ffa in closure)
			{
				var isfrom = null!=fromStates && FillEpsilonClosure(fromStates,null).Contains(ffa);
				
				var rngGrps = ffa.FillInputTransitionRangesGroupedByState();
				foreach (var rngGrp in rngGrps)
				{
					var istrns = isfrom && null!=toStates && options.DebugString != null && toStates.Contains(rngGrp.Key);
					var di = closure.IndexOf(rngGrp.Key);
					writer.Write(pfx+spfx);
					writer.Write(i);
					writer.Write("->");
					writer.Write(pfx+spfx);
					writer.Write(di.ToString());
					writer.Write(" [label=\"");
					var sb = new StringBuilder();
					//var notRanges = new List<FARange>(FARange.ToNotRanges(rngGrp.Value));
					var notRanges = new List<FARange>(_InvertRanges(rngGrp.Value));
					if (notRanges.Count > rngGrp.Value.Count)
					{
						_AppendRangeTo(sb, rngGrp.Value);
					} else
					{
						sb.Append("^");
						_AppendRangeTo(sb, notRanges);
					}
					if (sb.Length != 1 || " " == sb.ToString())
					{
						writer.Write('[');
						if(sb.Length> 16)
						{
							sb.Length = 16;
							sb.Append("...");
						}
						writer.Write(_EscapeLabel(sb.ToString()));
						writer.Write(']');
					}
					else
						writer.Write(_EscapeLabel(sb.ToString()));
					if (!istrns)
					{
						writer.WriteLine("\"]");
					} else
					{
						writer.Write("\",color=green]");
					}
				}
				// do epsilons
				foreach (var fat in ffa._transitions)
				{
					if (fat.Min == -1 && fat.Max == -1)
					{
						var istrns = null != toStates && options.DebugString != null && toStates.Contains(ffa) && toStates.Contains(fat.To);
						writer.Write(pfx + spfx);
						writer.Write(i);
						writer.Write("->");
						writer.Write(pfx+spfx);
						writer.Write(closure.IndexOf(fat.To));
						if (!istrns)
						{
							writer.WriteLine(" [style=dashed,color=gray]");
						} else
						{
							writer.WriteLine(" [style=dashed,color=green]");
						}
					}
				}
				// do block ends
				if (hasBlockEnds && ffa.IsAccepting && options.BlockEnds?.Length > ffa.AcceptSymbol && options.BlockEnds[ffa.AcceptSymbol] != null) {
					writer.Write(pfx + spfx + i.ToString()+"->");
					writer.Write(pfx + "blockEnd" + ffa.AcceptSymbol.ToString() + spfx + "0");
					writer.WriteLine(" [style=dashed,label=\".*?\"]");
				}
				++i;
			}
			string delim;
			if(hasBlockEnds)
			{
				for(i = 0;i<options.BlockEnds?.Length;i++)
				{
					var bfa = options.BlockEnds[i];
					if (bfa != null) {
						var bclose = bfa.FillClosure();
						delim = "";
						for (var qi = 0; qi < bclose.Count; ++qi)
						{
							var cbfa = bclose[qi];
							var rngGrps = cbfa.FillInputTransitionRangesGroupedByState();
							foreach (var rngGrp in rngGrps)
							{
								var di = bclose.IndexOf(rngGrp.Key);
								writer.Write(pfx + "blockEnd" + i.ToString() + spfx + qi.ToString());
								writer.Write("->");
								writer.Write(pfx + "blockEnd" + i.ToString() + spfx + di.ToString());
								writer.Write(" [label=\"");
								var sb = new StringBuilder();
								_AppendRangeTo(sb, rngGrp.Value);
								if (sb.Length != 1 || " " == sb.ToString())
								{
									writer.Write('[');
									if (sb.Length > 16)
									{
										sb.Length = 16;
										sb.Append("...");
									}
									writer.Write(_EscapeLabel(sb.ToString()));
									writer.Write(']');
								}
								else
								{
									writer.Write(_EscapeLabel(sb.ToString()));
								}
								writer.WriteLine("\"]");
								
							}
							// do epsilons
							foreach (var fat in cbfa._transitions)
							{
								if (fat.Min == -1 && fat.Max == -1)
								{
									writer.Write(pfx + "blockEnd" + i.ToString() + spfx + qi.ToString());
									writer.Write("->");
									var di = bclose.IndexOf(fat.To);
									writer.Write(pfx + "blockEnd" + i.ToString() + spfx + di.ToString());
									writer.WriteLine(" [style=dashed,color=gray]");
									
								}
							}
						}
						for (var qi = 0; qi < bclose.Count; ++qi) 
						{ 
							var cbfa = bclose[qi];
							writer.Write(pfx+"blockEnd" + i.ToString()+spfx+qi.ToString()+" [label=<");
							writer.Write("<TABLE BORDER=\"0\"><TR><TD>");
							writer.Write("(be)"+spfx);
							writer.Write("<SUB>");
							writer.Write(qi);
							writer.Write("</SUB></TD></TR>");
							if (cbfa.IsAccepting && !options.HideAcceptSymbolIds)
							{
								writer.Write("<TR><TD>");
								string acc = null;
								if (options.AcceptSymbolNames != null && options.AcceptSymbolNames.Length > i)
								{
									acc = options.AcceptSymbolNames[i];
								}
								if (acc == null)
								{
									acc = Convert.ToString(i);
								}
								writer.Write(acc.Replace("\"", "&quot;"));
								writer.Write("</TD></TR>");
							}
							writer.Write("</TABLE>");
							writer.Write(">");
							if (cbfa.IsAccepting)
								writer.Write(",shape=doublecircle");
							else if (cbfa.IsFinal || cbfa.IsNeutral)
							{
								writer.Write(",color=gray");
							}
							writer.WriteLine("]");
						}
					}
				}
			}
			delim = "";
			i = 0;
			foreach (var ffa in closure)
			{
				writer.Write(pfx+spfx);
				writer.Write(i);
				writer.Write(" [");
				if (null != options.DebugString)
				{
					if (null != toStates)
					{
						IList<FA> epstates= FA.FillEpsilonClosure(toStates, null);
						if (epstates.Contains(ffa))
							writer.Write("color=green,");
						else if (epstates.Contains(ffa) && (!toStates.Contains(ffa)))
							writer.Write("color=darkgreen,");
					} else
					{
						writer.Write("color=darkgreen,");
					}
				}
				writer.Write("label=<");
				writer.Write("<TABLE BORDER=\"0\"><TR><TD>");
				writer.Write(spfx);
				writer.Write("<SUB>");
				writer.Write(i);
				writer.Write("</SUB></TD></TR>");

				if (null != options.DebugSourceNfa)
				{
					var from = ffa.FromStates;
					if (null != from)
					{
						var brk = (int)Math.Floor(Math.Sqrt(from.Length));
						if(from.Length<=3) brk = 3;
						for(int j = 0;j<from.Length;++j)
						{
							if(j == 0 )
							{
								writer.Write("<TR><TD>");
								if(j==0)
								{
									writer.Write("{ ");
								}
								delim = "";
							} else if(j % brk == 0)
							{
								delim = "";
								writer.Write("</TD></TR><TR><TD>");
							}
							var fromFA = from[j];
							writer.Write(delim);
							if (fromFA is FA)
							{
								writer.Write(delim);
								writer.Write("q<SUB>");
								writer.Write(options.DebugSourceNfa.FillClosure().IndexOf((FA)fromFA).ToString());
								writer.Write("</SUB>");
								// putting a comma here is what we'd like
								// but it breaks dot no matter how its encoded
								delim = @" ";
							}
							
							if (j==from.Length-1)
							{
								writer.Write(" }");
								writer.Write("</TD></TR>");
							}
						}
					}
				}
				if (ffa.IsAccepting && !options.HideAcceptSymbolIds && !(hasBlockEnds && options.BlockEnds?.Length > ffa.AcceptSymbol && options.BlockEnds[ffa.AcceptSymbol] != null))
				{
					writer.Write("<TR><TD>");
					string acc = null;
					if (options.AcceptSymbolNames != null && options.AcceptSymbolNames.Length > ffa.AcceptSymbol)
					{
						acc = options.AcceptSymbolNames[ffa.AcceptSymbol];
					}
					if(acc==null)
					{
						acc = Convert.ToString(ffa.AcceptSymbol);
					}
					writer.Write(acc.Replace("\"", "&quot;"));
					writer.Write("</TD></TR>");
				}
				writer.Write("</TABLE>");
				writer.Write(">");
				bool isfinal = false;
				if (accepting.Contains(ffa) && ((!hasBlockEnds || options.BlockEnds?.Length <= ffa.AcceptSymbol || null == options.BlockEnds[ffa.AcceptSymbol])))
					writer.Write(",shape=doublecircle");
				else if (isfinal || neutrals.Contains(ffa))
				{
					if ((null == fromStates || !fromStates.Contains(ffa)) &&
						(null == toStates || !toStates.Contains(ffa)))
					{
						writer.Write(",color=gray");
					}
				}
				writer.WriteLine("]");
				++i;
			}
			delim = "";
			if (0 < accepting.Count)
			{
				foreach (var ntfa in accepting)
				{
					if (!hasBlockEnds || options.BlockEnds?.Length <= ntfa.AcceptSymbol || null == options.BlockEnds?[ntfa.AcceptSymbol])
					{
						writer.Write(delim);
						writer.Write(pfx + spfx);
						writer.Write(closure.IndexOf(ntfa));
						delim = ",";
					}
				}
				if (delim != "")
				{
					writer.WriteLine(" [shape=doublecircle]");
				}
			}
			delim = "";
			if (0 < neutrals.Count)
			{

				foreach (var ntfa in neutrals)
				{
					if ((null == fromStates || !fromStates.Contains(ntfa)) &&
						(null == toStates || !toStates.Contains(ntfa))
						)
					{
						writer.Write(delim);
						writer.Write(pfx + spfx);
						writer.Write(closure.IndexOf(ntfa));
						delim = ",";
					}
				}
				writer.WriteLine(" [color=gray]");
				delim = "";
			}
			delim = "";
			if (0 < finals.Count)
			{
				foreach (var ntfa in finals)
				{
					writer.Write(delim);
					writer.Write(pfx + spfx);
					writer.Write(closure.IndexOf(ntfa));
					delim = ",";
				}
				writer.WriteLine(" [shape=circle,color=gray]");
			}

			writer.WriteLine("}");
		}
		/// <summary>
		/// Renders Graphviz output for this machine to the specified file
		/// </summary>
		/// <param name="filename">The output filename. The format to render is indicated by the file extension.</param>
		/// <param name="options">A <see cref="FADotGraphOptions"/> instance with any options, or null to use the defaults</param>
		public void RenderToFile(string filename, FADotGraphOptions options = null)
		{
			if (null == options)
				options = new FADotGraphOptions();
			string args = "-T";
			string ext = Path.GetExtension(filename);
			if(0==string.Compare(".dot", 
				ext,
				StringComparison.InvariantCultureIgnoreCase)) {
				using (var writer = new StreamWriter(filename, false))
				{
					WriteDotTo(writer, options);
					return;
				}
			} else if (0 == string.Compare(".png", 
				ext, 
				StringComparison.InvariantCultureIgnoreCase))
				args += "png";
			else if (0 == string.Compare(".jpg", 
				ext, 
				StringComparison.InvariantCultureIgnoreCase))
				args += "jpg";
			else if (0 == string.Compare(".bmp", 
				ext, 
				StringComparison.InvariantCultureIgnoreCase))
				args += "bmp";
			else if (0 == string.Compare(".svg", 
				ext, 
				StringComparison.InvariantCultureIgnoreCase))
				args += "svg";
			if (0 < options.Dpi)
				args += " -Gdpi=" + options.Dpi.ToString();

			args += " -o\"" + filename + "\"";

			var psi = new ProcessStartInfo("dot", args)
			{
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardInput = true
			};
			using (var proc = Process.Start(psi))
			{
				if (proc == null)
				{
					throw new NotSupportedException(
						"Graphviz \"dot\" application is either not installed or not in the system PATH");
				}
				WriteDotTo(proc.StandardInput, options);
				proc.StandardInput.Close();
				proc.WaitForExit();
			}
		}
		/// <summary>
		/// Writes the generated dot content to the specified <see cref="TextWriter"/>
		/// </summary>
		/// <param name="writer">The writer</param>
		/// <param name="options">The options</param>
		public void WriteDotTo(TextWriter writer, FADotGraphOptions options = null)
		{
			if (options.DebugSourceNfa != null && options.DebugShowNfa)
			{
				_WriteCompoundDotTo(FillClosure(), writer, options);
			}
			else
			{
				_WriteDotTo(FillClosure(), writer, options);
			}
		}
		/// <summary>
		/// Renders Graphviz output for this machine to a stream
		/// </summary>
		/// <param name="format">The output format. The format to render can be any supported dot output format or "dot" to render a dot file. See dot command line documation for details.</param>
		/// <param name="copy">True to copy the stream, otherwise false</param>
		/// <param name="options">A <see cref="FADotGraphOptions"/> instance with any options, or null to use the defaults</param>
		/// <returns>A stream containing the output. The caller is expected to close the stream when finished.</returns>
		public Stream RenderToStream(string format, 
			bool copy = false, 
			FADotGraphOptions options = null)
		{
			if (null == options)
				options = new FADotGraphOptions();
			if(0==string.Compare(format,
				"dot",
				StringComparison.InvariantCultureIgnoreCase))
			{
				var stm = new MemoryStream();
				using(var writer = new StreamWriter(stm)) { 
					WriteDotTo(writer, options); 
					stm.Seek(0,SeekOrigin.Begin);
					return stm;
				}
			}
			string args = "-T";
			args += string.Concat(" ", format);
			if (0 < options.Dpi)
				args += " -Gdpi=" + options.Dpi.ToString();

			var psi = new ProcessStartInfo("dot", args)
			{
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardInput = true,
				RedirectStandardOutput = true
			};
			using (var proc = Process.Start(psi))
			{
				if(proc==null)
				{
					throw new NotSupportedException(
						"Graphviz \"dot\" application is either not installed or not in the system PATH");
				}
				WriteDotTo(proc.StandardInput, options);
				proc.StandardInput.Close();
				if (!copy)
					return proc.StandardOutput.BaseStream;
				else
				{
					var stm = new MemoryStream();
					proc.StandardOutput.BaseStream.CopyTo(stm);
					proc.StandardOutput.BaseStream.Close();
					proc.WaitForExit();
					return stm;
				}
			}
		}
		static void _AppendRangeTo(StringBuilder builder, IList<FARange> ranges)
		{
			for(int i = 0;i < ranges.Count;++i)
			{
				_AppendRangeTo(builder, ranges, i);
			}
		}
		static void _AppendRangeTo(StringBuilder builder, IList<FARange> ranges, int index)
		{
			var first = ranges[index].Min;
			var last = ranges[index].Max;
			_AppendRangeCharTo(builder, first);
			if (0 == last.CompareTo(first)) return;
			if (last == first + 1) // spit out 1 and 2 length ranges as flat chars
			{
				_AppendRangeCharTo(builder, last);
				return;
			}
			else if(last == first + 2)
			{
				_AppendRangeCharTo(builder, first+1);
				_AppendRangeCharTo(builder, last);
				return;
			}
			builder.Append('-');
			_AppendRangeCharTo(builder, last);
		}
		static void _AppendCharTo(StringBuilder builder,int @char)
		{
			switch(@char)
			{
				case '.':
				case '[':
				case ']':
				case '^':
				case '-':
				case '+':
				case '?':
				case '(':
				case ')':
				case '\\':
					builder.Append('\\');
					builder.Append(char.ConvertFromUtf32(@char));
					return;
				case '\t':
					builder.Append("\\t");
					return;
				case '\n':
					builder.Append("\\n");
					return;
				case '\r':
					builder.Append("\\r");
					return;
				case '\0':
					builder.Append("\\0");
					return;
				case '\f':
					builder.Append("\\f");
					return;
				case '\v':
					builder.Append("\\v");
					return;
				case '\b':
					builder.Append("\\b");
					return;
				default:
					var s = char.ConvertFromUtf32(@char);
					if (!char.IsLetterOrDigit(s, 0) && !char.IsSeparator(s, 0) && !char.IsPunctuation(s, 0) && !char.IsSymbol(s, 0))
					{
						if (s.Length == 1)
						{
							builder.Append("\\u");
							builder.Append(unchecked((ushort)@char).ToString("x4"));
						}
						else
						{
							builder.Append("\\U");
							builder.Append(@char.ToString("x8"));
						}

					}
					else
						builder.Append(s);
					break;
			}
		}
		
		static void _AppendRangeCharTo(StringBuilder builder, int rangeChar)
		{
			switch (rangeChar)
			{
				case '.':
				case '[':
				case ']':
				case '^':
				case '-':
				case '(':
				case ')':
				case '{':
				case '}':
				case '\\':
					builder.Append('\\');
					builder.Append(char.ConvertFromUtf32(rangeChar));
					return;
				case '\t':
					builder.Append("\\t");
					return;
				case '\n':
					builder.Append("\\n");
					return;
				case '\r':
					builder.Append("\\r");
					return;
				case '\0':
					builder.Append("\\0");
					return;
				case '\f':
					builder.Append("\\f");
					return;
				case '\v':
					builder.Append("\\v");
					return;
				case '\b':
					builder.Append("\\b");
					return;
				default:
					var s = char.ConvertFromUtf32(rangeChar);
					if (!char.IsLetterOrDigit(s, 0) && !char.IsSeparator(s, 0) && !char.IsPunctuation(s, 0) && !char.IsSymbol(s, 0))
					{
						if (s.Length == 1)
						{
							builder.Append("\\u");
							builder.Append(unchecked((ushort)rangeChar).ToString("x4"));
						}
						else
						{
							builder.Append("\\U");
							builder.Append(rangeChar.ToString("x8"));
						}

					}
					else
						builder.Append(s);
					break;
			}
		}
		static string _EscapeLabel(string label)
		{
			if (string.IsNullOrEmpty(label)) return label;

			string result = label.Replace("\\", @"\\");
			result = result.Replace("\"", "\\\"");
			result = result.Replace("\n", "\\n");
			result = result.Replace("\r", "\\r");
			result = result.Replace("\0", "\\0");
			result = result.Replace("\v", "\\v");
			result = result.Replace("\t", "\\t");
			result = result.Replace("\f", "\\f");
			return result;
		}
	}
}