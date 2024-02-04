using System.Collections.Immutable;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace VisualFA
{
    [Generator]
    public class FASourceGenerator : IIncrementalGenerator
    {
        const string NullableOff = @"#nullable disable
";
        const string AttributeImpl = @"#nullable disable
namespace VisualFA
{
    [System.AttributeUsage(System.AttributeTargets.Method|System.AttributeTargets.Class,AllowMultiple = true,Inherited = false)]
    class FARuleAttribute : System.Attribute
    {
        public FARuleAttribute() { }
        public FARuleAttribute(string expression)
        { 
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (expression.Length == 0) throw new ArgumentException(""The expression must not be empty"", nameof(expression));
            Expression = expression;
        }
        public string Expression { get; set; } = """";
        public string BlockEnd { get; set; } = null;
        public int Id { get; set; } = -1;
        public string Symbol { get; set; } = null;
    }
}";
        private const string FARuleAttributeFullName = "VisualFA.FARuleAttribute";
        private const string FAMatchFullName = "VisualFA.FAMatch";
        private const string FAMatchName = "FAMatch";
        private const string FARunnerFullName = "VisualFA.FARunner";
        private const string FARunnerName = "FARunner";
        private const string FAStringRunnerFullName = "VisualFA.FAStringRunner";
        private const string FAStringRunnerName = "FAStringRunner";
        private const string FAStringDfaTableRunnerFullName = "VisualFA.FAStringDfaTableRunner";
        private const string FAStringDfaTableRunnerName = "FAStringDfaTableRunner";
        private const string FATextReaderRunnerFullName = "VisualFA.FATextReaderRunner";
        private const string FATextReaderRunnerName = "FATextReaderRunner";
        private const string FATextReaderDfaTableRunnerFullName = "VisualFA.FATextReaderDfaTableRunner";
        private const string FATextReaderDfaTableRunnerName = "FATextReaderDfaTableRunner";

        private const string TextReaderName = "TextReader";
        private const string TextReaderFullName = "System.IO.TextReader";
        private const string StringFullName = "string";
        static bool _IsSyntaxTargetForGeneration(SyntaxNode node)
        => ((node is MethodDeclarationSyntax m && m.AttributeLists.Count > 0)|| (node is TypeDeclarationSyntax t && t.AttributeLists.Count > 0));
        static MemberDeclarationSyntax _GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            // we know the node is a MethodDeclarationSyntax thanks to IsSyntaxTargetForGeneration
            var memberDeclarationSyntax = (MemberDeclarationSyntax)context.Node;

            // loop through all the attributes on the method
            foreach (AttributeListSyntax attributeListSyntax in memberDeclarationSyntax.AttributeLists)
            {
                foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
                {
                    if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                    {
                        // weird, we couldn't get the symbol, ignore it
                        continue;
                    }

                    INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                    string fullName = attributeContainingTypeSymbol.ToDisplayString();

                    // Is the attribute the [FARule] attribute?
                    if (fullName == FARuleAttributeFullName)
                    {
                        // return the member
                        return memberDeclarationSyntax;
                    }
                }
            }

            // we didn't find the attribute we were looking for
            return null;
        }
        static string _GetNamespace(BaseTypeDeclarationSyntax syntax)
        {
            // If we don't have a namespace at all we'll return an empty string
            // This accounts for the "default namespace" case
            string nameSpace = string.Empty;

            // Get the containing syntax node for the type declaration
            // (could be a nested type, for example)
            SyntaxNode potentialNamespaceParent = syntax.Parent;

            // Keep moving "out" of nested classes etc until we get to a namespace
            // or until we run out of parents
            while (potentialNamespaceParent != null &&
                    potentialNamespaceParent is not NamespaceDeclarationSyntax
                    && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
            {
                potentialNamespaceParent = potentialNamespaceParent.Parent;
            }

            // Build up the final namespace by looping until we no longer have a namespace declaration
            if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
            {
                // We have a namespace. Use that as the type
                nameSpace = namespaceParent.Name.ToString();

                // Keep moving "out" of the namespace declarations until we 
                // run out of nested namespace declarations
                while (true)
                {
                    if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                    {
                        break;
                    }

                    // Add the outer namespace as a prefix to the final namespace
                    nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                    namespaceParent = parent;
                }
            }

            // return the final namespace
            return nameSpace;
        }
        static string _MakeSafeName(string name)
        {
            StringBuilder sb;
            if (char.IsDigit(name[0]))
            {
                sb = new StringBuilder(name.Length + 1);
                sb.Append('_');
            }
            else
            {
                sb = new StringBuilder(name.Length);
            }
            for (var i = 0; i < name.Length; ++i)
            {
                var ch = name[i];
                if ('_' == ch || char.IsLetterOrDigit(ch))
                    sb.Append(ch);
                else
                    sb.Append('_');
            }
            return sb.ToString();
        }
        static void _GenerateRangesExpression(string cmp,IList<FARange> ranges,StringBuilder sb)
        {
            for (int i = 0;i<ranges.Count;++i) 
            {
                if (i!=0)
                {
                    sb.Append(" || ");
                }
                var range = ranges[i];
                switch(range.Max-range.Min)
                {
                    case 0:
                        sb.Append(cmp);
                        sb.Append(" == ");
                        sb.Append(range.Min.ToString()); 
                    break;
                    case 1:
                        sb.Append(cmp);
                        sb.Append(" == ");
                        sb.Append(range.Min.ToString());
                        sb.Append(" || ");
                        sb.Append(cmp);
                        sb.Append(" == ");
                        sb.Append(range.Max.ToString());
                        break;
                    default:
                        sb.Append('(');
                        sb.Append(cmp);
                        sb.Append(" >= ");
                        sb.Append(range.Min.ToString());
                        sb.Append(" && ");
                        sb.Append(cmp);
                        sb.Append(" <= ");
                        sb.Append(range.Max.ToString());
                        sb.Append(')');
                        break;
                }
            }
        }
        static void _GenerateBlockEnd(bool isReader,int accept, FA blockEnd, string faMatch, string tab, StringBuilder sb)
        {
            tab += "    ";
            if (isReader)
            {
                sb.AppendLine(tab + faMatch + " _BlockEnd" + accept.ToString() + "(int p, int l, int c)");
            }
            else
            {
                sb.AppendLine(tab + faMatch + " _BlockEnd" + accept.ToString() + "(string s, int ch, int len, int p, int l, int c)");
            }
            sb.AppendLine(tab + "{");
            var closure = blockEnd.FillClosure();
            for(var i = 0; i < closure.Count; ++i)
            {
                var q = closure[i];
                sb.AppendLine(tab+"q"+i.ToString()+":");
                var rnggrps = q.FillInputTransitionRangesGroupedByState();
                foreach(var rnggrp in rnggrps)
                {
                    var tmp = new RegexCharsetExpression();
                    foreach (var rng in rnggrp.Value)
                    {

                        if (rng.Min == rng.Max)
                        {
                            tmp.Entries.Add(new RegexCharsetCharEntry(rng.Min));
                        }
                        else
                        {
                            tmp.Entries.Add(new RegexCharsetRangeEntry(rng.Min, rng.Max));
                        }
                    }
                    sb.AppendLine(tab + "    // " + tmp.ToString());
                    sb.Append(tab + "    if (");
                    _GenerateRangesExpression(isReader?"current":"ch",rnggrp.Value,sb); ;
                    sb.AppendLine(")");
                    sb.AppendLine(tab + "    {");
                    if (isReader)
                    {
                        sb.AppendLine(tab + "        Advance();");
                    }
                    else
                    {
                        sb.AppendLine(tab + "        Advance(s, ref ch, ref len, false);");
                    }
                    sb.AppendLine(tab + "        goto q"+closure.IndexOf(rnggrp.Key).ToString()+";");
                    sb.AppendLine(tab + "    }");
                }
                if (q.IsAccepting)
                {
                    if (isReader)
                    {
                        sb.AppendLine(tab + "    return " + faMatch + ".Create(" + accept.ToString() + ", capture.ToString(), p, l, c);");
                    }
                    else
                    {
                        sb.AppendLine(tab + "    return " + faMatch + ".Create(" + accept.ToString() + ", s.Substring(p, len).ToString(), p, l, c);");
                    }
                }
                else
                {
                    sb.AppendLine(tab + "    goto errorout;");

                }
                
            }
            sb.AppendLine(tab + "errorout:");
            if (isReader)
            {
                sb.AppendLine(tab + "    if (current == -1)");
            }
            else
            {
                sb.AppendLine(tab + "    if (ch == -1)");
            } 
            sb.AppendLine(tab + "    {");
            if (isReader)
            {
                sb.AppendLine(tab + "        return " + faMatch + ".Create(-1, capture.ToString(), p, l, c);");
            }
            else
            {
                sb.AppendLine(tab + "        return " + faMatch + ".Create(-1, s.Substring(p, len).ToString(), p, l, c);");
            }
            sb.AppendLine(tab + "    }");
            if (isReader)
            {
                sb.AppendLine(tab + "    Advance();");
            }
            else
            {
                sb.AppendLine(tab + "    Advance(s, ref ch, ref len, false);");
            }
            sb.AppendLine(tab + "    goto q0;");
            sb.AppendLine(tab + "    ");
            sb.AppendLine(tab + "}");
        }

        static void _ImplementCompiledRunner(_LexEntry member, string faRunnerBase, string faMatch, SourceProductionContext context)
        {
            int maxBe;
            FA lexer = _MethodToLexer(member, out maxBe);
            var closure = lexer.FillClosure();
            var sb = new StringBuilder();
            var ns = _GetNamespace(member.TypeDecl);
            string tab = "";
            if (!string.IsNullOrEmpty(ns))
            {
                tab = "    ";
                sb.AppendLine("namespace " + ns);
                sb.AppendLine("{");
            }
            var name = member.MethodDecl != null ? member.QName + (member.IsReader ? "TextReader" : "String") + "Runner" : member.QName;
            //var name = member.QName + (member.IsReader ? "TextReader" : "String") + "Runner";
            sb.AppendLine(tab + "partial class " + name);
            sb.AppendLine(tab + "    : " + faRunnerBase);
            sb.AppendLine(tab + "{");
            for (int i = 0; i < member.Rules.Length; i++)
            {
                var rule = member.Rules[i];
                if (!string.IsNullOrEmpty(rule.Symbol))
                {
                    sb.Append(tab + "    /// <summary>Matched the expression: " + _Escape(rule.Expression!));
                    if (!string.IsNullOrEmpty(rule.BlockEnd))
                    {
                        sb.Append(".*?");
                        sb.Append(_Escape(rule.BlockEnd!));
                    }
                    sb.AppendLine("</summary>");
                    sb.AppendLine(tab + "    public const int " + _MakeSafeName(rule.Symbol!) + " = " + rule.Id.ToString() + ";");
                }
            }
            FA[] bes = _MemberToBlockEnds(member, maxBe);
            for (int i = 0; i < bes!.Length; i++)
            {
                var be = bes[i];
                if (be != null)
                {
                    _GenerateBlockEnd(member.IsReader, i, be, faMatch, tab, sb);
                }
            }
            sb.AppendLine(tab + "    public override " + faMatch + " NextMatch()");
            sb.AppendLine(tab + "    {");
            if (!member.IsReader)
            {
                sb.AppendLine(tab + "        int ch = -1;");
                sb.AppendLine(tab + "        int len = 0;");
            }
            sb.AppendLine(tab + "        int p;");
            sb.AppendLine(tab + "        int l;");
            sb.AppendLine(tab + "        int c;");
            if (member.IsReader)
            {
                sb.AppendLine(tab + "        capture.Clear();");
                sb.AppendLine(tab + "        if (current == -2)");
                sb.AppendLine(tab + "        {");
                sb.AppendLine(tab + "            Advance();");
                sb.AppendLine(tab + "        }");
            }
            else
            {
                sb.AppendLine(tab + "        if ((position == -1))");
                sb.AppendLine(tab + "        {");
                sb.AppendLine(tab + "            position = 0; // first move");
                sb.AppendLine(tab + "        }");
            }
            sb.AppendLine(tab + "        p = position;");
            sb.AppendLine(tab + "        l = line;");
            sb.AppendLine(tab + "        c = column;");
            if (!member.IsReader)
            {
                sb.AppendLine(tab + "        Advance(@string, ref ch, ref len, true);");
            }
            var q0ranges = new List<FARange>();
            q0ranges.Add(new FARange(-1, -1));
            for (int i = 0; i < closure.Count; i++)
            {
                var q = closure[i];
                if (i == 0)
                {
                    if (FA.IsLoop(closure))
                    {
                        sb.AppendLine(tab + "    q0:");
                    }
                    else
                    {
                        sb.AppendLine(tab + "    // q0:");
                    }
                }
                else
                {
                    sb.AppendLine(tab + "    q" + i.ToString() + ":");
                }
                var rnggrps = q.FillInputTransitionRangesGroupedByState();

                foreach (var rnggrp in rnggrps)
                {
                    if (i == 0)
                    {
                        q0ranges.AddRange(rnggrp.Value);
                    }
                    var tmp = new RegexCharsetExpression();
                    foreach (var rng in rnggrp.Value)
                    {

                        if (rng.Min == rng.Max)
                        {
                            tmp.Entries.Add(new RegexCharsetCharEntry(rng.Min));
                        }
                        else
                        {
                            tmp.Entries.Add(new RegexCharsetRangeEntry(rng.Min, rng.Max));
                        }
                    }
                    sb.AppendLine(tab + "        // " + tmp.ToString());
                    sb.Append(tab + "        if (");
                    _GenerateRangesExpression(member.IsReader ? "current" : "ch", rnggrp.Value, sb);
                    sb.AppendLine(")");
                    sb.AppendLine(tab + "        {");
                    if (member.IsReader)
                    {
                        sb.AppendLine(tab + "            Advance();");
                    }
                    else
                    {
                        sb.AppendLine(tab + "            Advance(@string, ref ch, ref len, false);");
                    }
                    sb.AppendLine(tab + "            goto q" + closure.IndexOf(rnggrp.Key).ToString() + ";");
                    sb.AppendLine(tab + "        }");
                }
                // not matched
                if (q.IsAccepting)
                {
                    if (bes != null && bes.Length > q.AcceptSymbol && bes[q.AcceptSymbol] != null)
                    {
                        if (member.IsReader)
                        {
                            sb.AppendLine(tab + "        return _BlockEnd" + q.AcceptSymbol.ToString() + "(p, l, c);");
                        }
                        else
                        {
                            sb.AppendLine(tab + "        return _BlockEnd" + q.AcceptSymbol.ToString() + "(@string, ch, len, p, l, c);");
                        }
                    }
                    else
                    {
                        if (member.IsReader)
                        {
                            sb.AppendLine(tab + "        return " + faMatch + ".Create(" + q.AcceptSymbol.ToString() + ", capture.ToString(), p, l, c);");
                        }
                        else
                        {
                            sb.AppendLine(tab + "        return " + faMatch + ".Create(" + q.AcceptSymbol.ToString() + ", @string.Substring(p,len), p, l, c);");
                        }
                    }
                }
                else
                {
                    sb.AppendLine(tab + "        goto errorout;");
                }

            }
            sb.AppendLine(tab + "    errorout:");
            sb.Append(tab + "        if (");
            _GenerateRangesExpression(member.IsReader ? "current" : "ch", q0ranges, sb);
            sb.AppendLine(")");
            sb.AppendLine(tab + "        {");
            if (member.IsReader)
            {
                sb.AppendLine(tab + "            if (capture.Length == 0)");
            }
            else
            {
                sb.AppendLine(tab + "            if (len == 0)");
            }
            sb.AppendLine(tab + "            {");
            sb.AppendLine(tab + "                return " + faMatch + ".Create(-2, null, 0, 0, 0);");
            sb.AppendLine(tab + "            }");
            if (member.IsReader)
            {
                sb.AppendLine(tab + "            return " + faMatch + ".Create(-1, capture.ToString(), p, l, c);");
            }
            else
            {
                sb.AppendLine(tab + "            return " + faMatch + ".Create(-1, @string.Substring(p, len), p, l, c);");
            }
            sb.AppendLine(tab + "        }");
            if (member.IsReader)
            {
                sb.AppendLine(tab + "        Advance();");
            }
            else
            {
                sb.AppendLine(tab + "        Advance(@string, ref ch, ref len, false);");
            }
            sb.AppendLine(tab + "        goto errorout;");
            sb.AppendLine(tab + "    }");
            sb.AppendLine(tab + "}");
            if (!string.IsNullOrEmpty(ns))
            {
                sb.AppendLine("}");
            }
            context.AddSource(member.QName + ".g.cs", sb.ToString());
        }
        static void _ImplementTableRunner(_LexEntry member, bool vfaReffed,bool isReader, string faMatch, SourceProductionContext context)
        {
            int maxBe;
            FA lexer = _MethodToLexer(member, out maxBe);
            var closure = lexer.FillClosure();
            var sb = new StringBuilder();
            var ns = _GetNamespace(member.TypeDecl);
            string tab = "";
            if (!string.IsNullOrEmpty(ns))
            {
                tab = "    ";
                sb.AppendLine("namespace " + ns);
                sb.AppendLine("{");
            }
            var name = member.QName;// + (member.IsReader ? "TextReader" : "String") + "Runner";
            sb.AppendLine(tab + "partial class " + name);
            if (!vfaReffed)
            {
                sb.AppendLine(tab + "    : " + (isReader ? FATextReaderDfaTableRunnerName : FAStringDfaTableRunnerName));
            } else
            {
                sb.AppendLine(tab + "    : " + (isReader ? FATextReaderDfaTableRunnerFullName : FAStringDfaTableRunnerFullName));
            }
            sb.AppendLine(tab + "{");
            for (int i = 0; i < member.Rules.Length; i++)
            {
                var rule = member.Rules[i];
                if (!string.IsNullOrEmpty(rule.Symbol))
                {
                    sb.Append(tab + "    /// <summary>Matched the expression: " + _Escape(rule.Expression!));
                    if (!string.IsNullOrEmpty(rule.BlockEnd))
                    {
                        sb.Append(".*?");
                        sb.Append(_Escape(rule.BlockEnd!));
                    }
                    sb.AppendLine("</summary>");
                    sb.AppendLine(tab + "    public const int " + _MakeSafeName(rule.Symbol!) + " = " + rule.Id.ToString() + ";");
                }
            }
            var lexeri = lexer.ToArray();
            FA[] bes = _MemberToBlockEnds(member, maxBe);
            var beis = new int[bes.Length][];
            for (int i = 0; i < bes!.Length; i++)
            {
                var be = bes[i];
                if (be != null)
                {
                    beis[i] = be.ToArray();
                }
            }
            sb.Append(tab + "    private static readonly int[] _dfa = new int[] { ");
            _GenerateIntArrayInit(tab + "        ", lexeri, sb);
            sb.AppendLine("};");
            sb.Append(tab + "    public static readonly int[][] _blockEndDfas = new int[][] { ");
            _GenerateIntIntArrayInit(tab + "        ", beis, sb);
            sb.AppendLine("};");
            sb.AppendLine(tab + "    public " + name + "()");
            sb.AppendLine(tab + "        : base(_dfa, _blockEndDfas) { }");
            sb.AppendLine(tab + "}");
            if (!string.IsNullOrEmpty(ns))
            {
                sb.AppendLine("};");
            }
            context.AddSource(member.QName + ".g.cs", sb.ToString());
        }

        private static FA[] _MemberToBlockEnds(_LexEntry method, int maxBe)
        {
            FA[] bes = null;
            if (maxBe > 0)
            {
                bes = new FA[maxBe];
                for (int i = 0; i < method.Rules.Length; i++)
                {
                    var rule = method.Rules[i];
                    if (!string.IsNullOrEmpty(rule.BlockEnd))
                    {
                        try
                        {
                            bes[i] = FA.Parse(rule.BlockEnd!).ToMinimizedDfa();
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidProgramException("Error parsing regex block end: " + _Escape(rule.BlockEnd), ex);
                        }

                    }
                }
            }

            return bes;
        }

        private static FA _MethodToLexer(_LexEntry method, out int maxBe)
        {
            var fas = new FA[method.Rules.Length];
            maxBe = 0;
            for (int i = 0; i < method.Rules.Length; i++)
            {
                var rule = method.Rules[i];
                try
                {
                    fas[i] = FA.Parse(rule.Expression!, rule.Id);
                }
                catch (Exception ex)
                {
                    throw new InvalidProgramException("Error parsing regex expression: " + _Escape(rule.BlockEnd), ex);
                }
                if (!string.IsNullOrEmpty(rule.BlockEnd))
                {
                    var newMax = rule.Id + 1;
                    if (newMax > maxBe)
                    {
                        maxBe = newMax;
                    }
                }
            }
            return FA.ToLexer(fas, true, true);

        }

        static void _GenerateIntArrayInit(string tab, int[] values, StringBuilder sb)
        {
            int cols = 0;
            if(values==null)
            {
                sb.Append("null");
                return;
            }
            for(var i = 0;i<values.Length;i++)
            {
                var value = values[i].ToString();
                cols += values.Length;
                sb.Append(value);
                if (i < values.Length-1)
                {
                    cols += 2;
                    sb.Append(", ");
                }
                if(cols> 80)
                {
                    sb.AppendLine();
                    sb.Append(tab);
                    cols = 0;
                }
            }
            if(cols!=0)
            {
                sb.AppendLine();
            }
        }
        static void _GenerateIntIntArrayInit(string tab, int[][] values, StringBuilder sb)
        {
            int cols = 0;
            if (values == null)
            {
                sb.AppendLine(tab+"null");
                return;
            }
            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                if(value==null)
                {
                    sb.Append(tab+"null");
                } else
                {
                    sb.Append(tab);
                    sb.Append("new int[] { ");
                    _GenerateIntArrayInit(tab + "    ", value, sb);
                    sb.AppendLine("}");
                }
                if (i < values.Length - 1)
                {
                    sb.Append(",");
                }
                sb.AppendLine();
            }
            if (cols != 0)
            {
                sb.AppendLine();
            }
        }
        static void _Execute(Compilation compilation, ImmutableArray<MemberDeclarationSyntax> members, SourceProductionContext context)
        {

            if (members.IsDefaultOrEmpty)
            {
                // nothing to do yet
                return;
            }
            bool vfaReffed = _IsVisualFAReferenced(compilation);

            string sharedCodeRunnerNS, sharedCodeStringNS, sharedCodeStringTableNS, sharedCodeReaderNS, sharedCodeReaderTableNS, faMatch;
            Dictionary<string, StringBuilder> sharedNsMap;
            _ProcessExistingSharedCode(compilation, 
                ref members, 
                ref vfaReffed, 
                out sharedCodeRunnerNS, 
                out sharedCodeStringNS, 
                out sharedCodeStringTableNS, 
                out sharedCodeReaderNS, 
                out sharedCodeReaderTableNS, 
                out faMatch, 
                out sharedNsMap);

            // Create a dummy diagnostic, just for demonstration purposes
            // context.ReportDiagnostic(CreateDiagnostic(enums[0]));

            // I'm not sure if this is actually necessary, but `[LoggerMessage]` does it, so seems like a good idea!
            IEnumerable<MemberDeclarationSyntax> distinctMembers = members.Distinct();
            string stringRunnerBase = null;
            string textRunnerBase = null;

            // Convert each EnumDeclarationSyntax to an EnumToGenerate
            var membersToGenerate = _GetLexMembers(compilation, context,distinctMembers, vfaReffed, sharedCodeRunnerNS, sharedCodeStringNS, sharedCodeReaderNS, sharedCodeStringTableNS, sharedCodeReaderTableNS, context.CancellationToken);
            if (!vfaReffed)
            {
                var needsReaderTable = false;
                var needsStringTable = false;
                var needsString = false;
                var needsReader = false;
                var needsCore = false;
                for (int i = 0; i < membersToGenerate.Count; ++i)
                {
                    var lm = membersToGenerate[i];
                    if (lm.IsReader)
                    {
                        if (lm.IsTable)
                        {
                            needsReaderTable = true;
                        }
                        needsReader = true;
                        needsCore = true;
                    }
                    else
                    {
                        if (lm.IsTable)
                        {
                            needsStringTable = true;
                        }
                        needsString = true;
                        needsCore = true;
                    }

                }
                if(needsCore)
                {
                    var cls = membersToGenerate[0].TypeDecl as ClassDeclarationSyntax;
                    if (cls != null)
                    {
                        var ns = _GetNamespace(cls);
                        StringBuilder sharedSb;
                        sharedNsMap.TryGetValue(ns, out sharedSb);
                        if (sharedSb == null)
                        {
                            sharedSb = new StringBuilder();
                            sharedNsMap.Add(ns, sharedSb);
                        }
                        sharedCodeRunnerNS = ns;
                        var tab = "";
                        if (!string.IsNullOrEmpty(ns))
                        {
                            tab = "    ";
                        }
                        using (var sr = _GetResource("FAMatch.cs"))
                        {
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                sharedSb.AppendLine(tab + line);
                            }
                        }
                        using (var sr = _GetResource("FARunner.cs"))
                        {
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                sharedSb.AppendLine(tab + line);
                            }
                        }
                        faMatch = string.IsNullOrEmpty(ns) ? FAMatchName : ns + "." + FAMatchName;
                    }

                }
            
                if (sharedCodeStringNS == null && needsString)
                {
                    var cls = membersToGenerate[0].TypeDecl as ClassDeclarationSyntax;
                    if (cls != null)
                    {
                        var ns = _GetNamespace(cls);
                        StringBuilder sharedSb;
                        sharedNsMap.TryGetValue(ns, out sharedSb);
                        if (sharedSb == null)
                        {
                            sharedSb = new StringBuilder();
                            sharedNsMap.Add(ns, sharedSb);
                        }
                        sharedCodeStringNS = ns;
                        var tab = "";
                        if (!string.IsNullOrEmpty(ns))
                        {
                            tab = "    ";

                        }
                        using (var sr = _GetResource("FAStringRunner.cs"))
                        {
                            stringRunnerBase = string.IsNullOrEmpty(ns) ? FAStringRunnerName : ns + "." + FAStringRunnerName;
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                sharedSb.AppendLine(tab + line);
                            }
                        }

                    }
                }
                if (sharedCodeStringTableNS == null && needsStringTable)
                {
                    var cls = membersToGenerate[0].TypeDecl as ClassDeclarationSyntax;
                    if (cls != null)
                    {
                        var ns = _GetNamespace(cls);
                        StringBuilder sharedSb;
                        sharedNsMap.TryGetValue(ns, out sharedSb);
                        if (sharedSb == null)
                        {
                            sharedSb = new StringBuilder();
                            sharedNsMap.Add(ns, sharedSb);
                        }
                        sharedCodeStringTableNS = ns;
                        var tab = "";
                        if (!string.IsNullOrEmpty(ns))
                        {
                            tab = "    ";

                        }
                        using (var sr = _GetResource("FAStringDfaTableRunner.cs"))
                        {
                            stringRunnerBase = string.IsNullOrEmpty(ns) ? FAStringRunnerName : ns + "." + FAStringRunnerName;
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                sharedSb.AppendLine(tab + line);
                            }
                        }

                    }
                }
                if (sharedCodeReaderNS == null && needsReader)
                {
                    var cls = membersToGenerate[0].TypeDecl as ClassDeclarationSyntax;
                    if (cls != null)
                    {
                        var ns = _GetNamespace(cls);
                        StringBuilder sharedSb;
                        sharedNsMap.TryGetValue(ns, out sharedSb);
                        if (sharedSb == null)
                        {
                            sharedSb = new StringBuilder();
                            sharedNsMap.Add(ns, sharedSb);
                        }
                        sharedCodeReaderNS = ns;
                        var tab = "";
                        if (!string.IsNullOrEmpty(ns))
                        {
                            tab = "    ";

                        }
                        using (var sr = _GetResource("FATextReaderRunner.cs"))
                        {
                            textRunnerBase = string.IsNullOrEmpty(ns) ? FATextReaderRunnerName : ns + "." + FATextReaderRunnerName;
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                sharedSb.AppendLine(tab + line);
                            }
                        }

                    }
                }
                if (sharedCodeReaderTableNS == null && needsReaderTable)
                {
                    var cls = membersToGenerate[0].TypeDecl as ClassDeclarationSyntax;
                    if (cls != null)
                    {
                        var ns = _GetNamespace(cls);
                        StringBuilder sharedSb;
                        sharedNsMap.TryGetValue(ns, out sharedSb);
                        if (sharedSb == null)
                        {
                            sharedSb = new StringBuilder();
                            sharedNsMap.Add(ns, sharedSb);
                        }
                        sharedCodeReaderTableNS = ns;
                        var tab = "";
                        if (!string.IsNullOrEmpty(ns))
                        {
                            tab = "    ";

                        }
                        using (var sr = _GetResource("FATextReaderDfaTableRunner.cs"))
                        {
                            textRunnerBase = string.IsNullOrEmpty(ns) ? FATextReaderRunnerName : ns + "." + FATextReaderRunnerName;
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                sharedSb.AppendLine(tab + line);
                            }
                        }

                    }
                }
            }
            else
            {
                faMatch = FAMatchFullName;
                stringRunnerBase = FAStringRunnerFullName;
                textRunnerBase = FATextReaderRunnerFullName;
            }
            // make sure we have something to generate
            if (membersToGenerate.Count > 0)
            {
                string oldType = null;
                string type = null;
                string ns = null;
                string oldNS = null;
                var clstab = "";
                var mtab = "    ";
                var sb = new StringBuilder();
                var trailingBrace = false;
                var genMethods = false;
                for (int i = 0; i < membersToGenerate.Count; ++i)
                {
                    var lm = membersToGenerate[i];
                    if (!lm.IsTable )
                    {
                        _ImplementCompiledRunner(lm, lm.IsReader ? textRunnerBase! : stringRunnerBase!, faMatch!, context);
                    }
                    if (lm.MethodDecl != null)
                    {
                        genMethods = true;
                        ns = _GetNamespace(lm.TypeDecl);
                        if (ns != oldNS)
                        {
                            if (!string.IsNullOrEmpty(oldNS))
                            {
                                sb.AppendLine("}");
                                trailingBrace = false;
                            }
                            if (ns.Length > 0)
                            {
                                sb.AppendLine("namespace " + ns);
                                sb.AppendLine("{");
                                clstab = "    ";
                                mtab = clstab + clstab;
                                trailingBrace = true;
                            }
                        }
                        type = lm.TypeDecl.Identifier.ToFullString();
                        if (oldType != type)
                        {
                            if (oldType != null)
                            {
                                sb.AppendLine(clstab + "}");
                                sb.AppendLine();
                            }
                            sb.AppendLine(clstab + "partial class " + lm.TypeDecl.Identifier.ToString());
                            sb.AppendLine(clstab + "{");

                        }
                        for (int j = 0; j < lm.Rules.Length; j++)
                        {
                            var lr = lm.Rules[j];
                            sb.Append(mtab + "// [LexRule(@\"");
                            sb.Append(_Escape(lr.Expression!));
                            sb.Append('\"');
                            if (lr.Id > -1)
                            {
                                sb.Append(", Id = ");
                                sb.Append(lr.Id);
                            }
                            if (!string.IsNullOrEmpty(lr.Symbol))
                            {
                                sb.Append(", Symbol = @\"");
                                sb.Append(lr.Symbol);
                                sb.Append('\"');
                            }
                            sb.AppendLine(")]");
                        }
                        var access = "public";

                        var pub = false;
                        foreach (var mod in lm.MethodDecl.Modifiers)
                        {
                            if (mod.Text == "public")
                            {
                                pub = true;
                                break;
                            }
                        }
                        var intr = false;
                        foreach (var mod in lm.MethodDecl.Modifiers)
                        {
                            if (mod.Text == "internal")
                            {
                                intr = true;
                                break;
                            }
                        }
                        if (!pub)
                        {
                            access = intr ? "internal" : "private";
                        }

                        sb.Append(mtab + access + " static partial " + lm.ReturnTypeName + " " + lm.MethodDecl.Identifier.ToString());
                        sb.Append('(');
                        if (lm.HasArg)
                        {
                            sb.Append(lm.IsReader ? TextReaderFullName + " text" : StringFullName + " text");
                        }
                        sb.AppendLine(")");
                        sb.AppendLine(mtab + "{");
                        if (!lm.IsTable)
                        {
                            var tname = lm.QName + (lm.IsReader ? "TextReader" : "String") + "Runner";
                            sb.AppendLine(mtab + "    var result = new " + tname + "();");
                        }
                        else
                        {
                            var tns = lm.IsReader ? sharedCodeReaderTableNS : sharedCodeStringTableNS;
                            if (vfaReffed)
                            {
                                tns = "VisualFA";
                            }
                            if (!string.IsNullOrEmpty(tns))
                            {
                                tns += ".";
                            }
                            var tname = tns + "FA" + (lm.IsReader ? "TextReader" : "String") + "DfaTableRunner";
                            sb.AppendLine(mtab + "    var result = new " + tname + "(");
                            sb.Append(mtab + "        new int[] { ");
                            int maxBe;
                            var lexer = _MethodToLexer(lm, out maxBe);
                            _GenerateIntArrayInit(mtab + "            ", lexer.ToArray(), sb);
                            sb.AppendLine(mtab + mtab + "        },");
                            var bes = _MemberToBlockEnds(lm, maxBe);
                            var beis = new int[bes.Length][];
                            for (var j = 0; j < bes.Length; j++)
                            {
                                var be = bes[j];
                                if (be != null)
                                {
                                    beis[j] = be.ToArray();
                                }
                            }
                            sb.Append(mtab + "        new int[][] { ");
                            _GenerateIntIntArrayInit(mtab + "            ", beis, sb);
                            sb.AppendLine(mtab + mtab + "        });");
                        }
                        if (lm.HasArg)
                        {
                            sb.AppendLine(mtab + "    result.Set(text);");
                        }
                        sb.AppendLine(mtab + "    return result;");
                        sb.AppendLine(mtab + "}");
                        oldType = type;
                        oldNS = ns;
                    } else
                    {
                        if(lm.IsTable)
                        {
                            _ImplementTableRunner(lm, vfaReffed, lm.IsReader , faMatch!, context);
                        }
                    }
                }
                if (membersToGenerate.Count > 0)
                {
                    sb.AppendLine(clstab + "}");
                    sb.AppendLine();
                }
                if (trailingBrace)
                {
                    sb.AppendLine("}");
                    sb.AppendLine();
                }
                // generate the source code and add it to the output
                if (sharedNsMap.Count > 0)
                {
                    var nssb = new StringBuilder();
                    nssb.Append(NullableOff);
                    foreach (var kvp in sharedNsMap)
                    {
                        if (!string.IsNullOrEmpty(kvp.Key))
                        {
                            nssb.AppendLine("namespace " + kvp.Key);
                            nssb.AppendLine("{");
                            nssb.AppendLine(kvp.Value.ToString());
                            nssb.AppendLine("}");
                        }
                        else
                        {
                            nssb.AppendLine(kvp.Value.ToString());
                        }
                    }

                    context.AddSource("FARunnerShared.g.cs", SourceText.From(nssb.ToString(), Encoding.UTF8));
                }
                if (genMethods)
                {
                    context.AddSource("FARunnerMethods.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
                }
                
            }
        }

        private static void _ProcessExistingSharedCode(Compilation compilation, ref ImmutableArray<MemberDeclarationSyntax> members, ref bool vfaReffed, out string sharedCodeRunnerNS, out string sharedCodeStringNS, out string sharedCodeStringTableNS, out string sharedCodeReaderNS, out string sharedCodeReaderTableNS, out string faMatch, out Dictionary<string, StringBuilder> sharedNsMap)
        {
            
            sharedCodeRunnerNS = null;
            sharedCodeStringNS = null;
            sharedCodeStringTableNS = null;
            sharedCodeReaderNS = null;
            sharedCodeReaderTableNS = null;
            faMatch = null;
            sharedNsMap = new Dictionary<string, StringBuilder>();
            return;
            if (!vfaReffed)
            {
                var vfarunner = compilation.GetTypeByMetadataName(FARunnerFullName);
                if (vfarunner == null)
                {
                    IEnumerable<SyntaxNode> allNodes = compilation.SyntaxTrees.SelectMany(s => s.GetRoot().DescendantNodes());
                    foreach (var node in allNodes)
                    {
                        if (node.IsKind(SyntaxKind.ClassDeclaration))
                        {
                            var classDecl = node as ClassDeclarationSyntax;
                            if (classDecl!.Identifier.Text == FAStringRunnerName)
                            {
                                var ns = _GetNamespace(classDecl);
                                if (ns == "VisualFA")
                                {
                                    vfaReffed = true;
                                    sharedCodeReaderNS = null;
                                    sharedCodeStringNS = null;
                                    sharedCodeRunnerNS = null;
                                    sharedCodeStringTableNS = null;
                                    sharedCodeReaderTableNS = null;
                                    break;
                                }
                                sharedCodeStringNS = ns;
                                break;
                            }
                            else if (classDecl!.Identifier.Text == FATextReaderRunnerName)
                            {
                                var ns = _GetNamespace(classDecl);
                                if (ns == "VisualFA")
                                {
                                    vfaReffed = true;
                                    sharedCodeReaderNS = null;
                                    sharedCodeStringNS = null;
                                    sharedCodeRunnerNS = null;
                                    sharedCodeStringTableNS = null;
                                    sharedCodeReaderTableNS = null;
                                    break;
                                }
                                sharedCodeReaderNS = ns;
                                break;
                            }
                            else if (classDecl!.Identifier.Text == FARunnerName)
                            {
                                var ns = _GetNamespace(classDecl);
                                if (ns == "VisualFA")
                                {
                                    vfaReffed = true;
                                    sharedCodeReaderNS = null;
                                    sharedCodeStringNS = null;
                                    sharedCodeRunnerNS = null;
                                    sharedCodeStringTableNS = null;
                                    sharedCodeReaderTableNS = null;
                                    break;
                                }
                                sharedCodeRunnerNS = ns;
                                break;
                            }
                            if (sharedCodeStringNS != null && sharedCodeReaderNS != null && sharedCodeRunnerNS != null)
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    vfaReffed = true;
                }
                
                if (sharedCodeRunnerNS == null)
                {
                    var bt = members[0] as BaseTypeDeclarationSyntax;
                    if (bt != null)
                    {
                        var ns = _GetNamespace(bt);
                        StringBuilder sharedSb;
                        sharedNsMap.TryGetValue(ns, out sharedSb);
                        if (sharedSb == null)
                        {
                            sharedSb = new StringBuilder();
                            sharedNsMap.Add(ns, sharedSb);
                        }
                        sharedCodeRunnerNS = ns;
                        var tab = "";
                        if (!string.IsNullOrEmpty(ns))
                        {
                            tab = "    ";
                        }
                        using (var sr = _GetResource("FAMatch.cs"))
                        {
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                sharedSb.AppendLine(tab + line);
                            }
                        }
                        using (var sr = _GetResource("FARunner.cs"))
                        {
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                sharedSb.AppendLine(tab + line);
                            }
                        }
                        faMatch = string.IsNullOrEmpty(ns) ? FAMatchName : ns + "." + FAMatchName;
                    }

                }
            }
        }

        private static bool _IsVisualFAReferenced(Compilation compilation)
        {
            var vfaReffed = false;
            var asms = compilation.SourceModule.ReferencedAssemblySymbols;
            foreach (var sym in asms)
            {
                var s = sym.Identity.Name;
                if (s == "VisualFA")
                {
                    vfaReffed = true;
                    break;
                }
            }

            return vfaReffed;
        }

        struct _LexRule
        {
            public string Expression;
            public string BlockEnd;
            public int Id;
            public string Symbol;
        }
        struct _LexEntry
        {
            public string QName;
            public TypeDeclarationSyntax TypeDecl;
            public MethodDeclarationSyntax MethodDecl;
            public bool IsReader;
            public bool IsTable;
            public bool HasArg;
            public string ReturnTypeName;
            public _LexRule[] Rules;
        }
        static StreamReader _GetResource(string name)
        {
            var stm = Assembly.GetExecutingAssembly().GetManifestResourceStream("VisualFA.Shared." + name);
            if (stm!=null)
            {
                return new StreamReader(stm, Encoding.UTF8);
            }
            throw new KeyNotFoundException(name);
        }
        
        static List<_LexEntry> _GetLexMembers(Compilation compilation, SourceProductionContext context, IEnumerable<MemberDeclarationSyntax> members, bool vfaReffed,string sharedRunnerNS,string sharedStringNS,string sharedReaderNS,string sharedStringTableNS,string sharedReaderTableNS, CancellationToken ct)
        {
            var result = new List<_LexEntry>();
            INamedTypeSymbol faRuleAttribute = compilation.GetTypeByMetadataName(FARuleAttributeFullName);
            if (faRuleAttribute == null)
            {
                // nothing to do if this type isn't available
                return result;
            }
            var rtcmp = new List<string>();
            var rtstrcmp = new List<string>();
            var rtspeccmp = new List<string>();
            var rtrdrcmp = new List<string>();
            if (vfaReffed)
            {
                rtcmp.Add(FARunnerFullName);
                rtstrcmp.Add(FAStringRunnerFullName);
                rtrdrcmp.Add(FATextReaderRunnerFullName);
                rtstrcmp.Add(FAStringDfaTableRunnerFullName);
                rtrdrcmp.Add(FATextReaderDfaTableRunnerFullName);
                rtspeccmp.Add(FAStringRunnerFullName);
                rtspeccmp.Add(FATextReaderRunnerFullName);
                rtspeccmp.Add(FAStringDfaTableRunnerFullName);
                rtspeccmp.Add(FATextReaderDfaTableRunnerFullName);

            } else
            {
                if (sharedRunnerNS!=null)
                {
                    if (sharedRunnerNS.Length == 0)
                    {
                        rtcmp.Add(FARunnerName);
                    } else
                    {
                        rtcmp.Add(sharedRunnerNS+"."+FARunnerName);
                    }
                } else
                {
                    rtcmp.Add(FARunnerName);
                }
                if (sharedStringNS != null)
                {
                    if (sharedStringNS.Length == 0)
                    {
                        rtstrcmp.Add(FAStringRunnerName);
                        rtspeccmp.Add(FAStringRunnerName);
                    }
                    else
                    {
                        rtstrcmp.Add(sharedStringNS + "." + FAStringRunnerName);
                        rtspeccmp.Add(sharedStringNS + "." + FAStringRunnerName);
                    }
                } else
                {
                    rtstrcmp.Add(FAStringRunnerName);
                    rtspeccmp.Add(FAStringRunnerName);
                }
                if (sharedStringTableNS != null)
                {
                    if (sharedStringTableNS.Length == 0)
                    {
                        rtstrcmp.Add(FAStringDfaTableRunnerName);
                        rtspeccmp.Add(FAStringDfaTableRunnerName);
                    }
                    else
                    {
                        rtstrcmp.Add(sharedStringTableNS + "." + FAStringDfaTableRunnerName);
                        rtspeccmp.Add(sharedStringTableNS + "." + FAStringDfaTableRunnerName);
                    }
                }
                else
                {
                    rtstrcmp.Add(FAStringDfaTableRunnerName);
                    rtspeccmp.Add(FAStringDfaTableRunnerName);
                }
                if (sharedReaderNS != null)
                {
                    if (sharedReaderNS.Length == 0)
                    {
                        rtrdrcmp.Add(FATextReaderRunnerName);
                        rtspeccmp.Add(FATextReaderRunnerName);
                    }
                    else
                    {
                        rtrdrcmp.Add(sharedReaderNS + "." + FATextReaderRunnerName);
                        rtspeccmp.Add(sharedReaderNS + "." + FATextReaderRunnerName);
                    }
                }
                else
                {
                    rtrdrcmp.Add(FATextReaderRunnerName);
                    rtspeccmp.Add(FATextReaderRunnerName);
                }
                if (sharedReaderTableNS != null)
                {
                    if (sharedReaderTableNS.Length == 0)
                    {
                        rtrdrcmp.Add(FATextReaderDfaTableRunnerName);
                        rtspeccmp.Add(FATextReaderDfaTableRunnerName);
                    }
                    else
                    {
                        rtrdrcmp.Add(sharedReaderTableNS + "." + FATextReaderDfaTableRunnerName);
                        rtspeccmp.Add(sharedReaderTableNS + "." + FATextReaderDfaTableRunnerName);
                    }
                }
                else
                {
                    rtrdrcmp.Add(FATextReaderDfaTableRunnerName);
                    rtspeccmp.Add(FATextReaderDfaTableRunnerName);
                }
            }
            foreach (var memberDeclSyntax in members)
            {
                // stop if we're asked to
                ct.ThrowIfCancellationRequested();
                SemanticModel semanticModel = compilation.GetSemanticModel(memberDeclSyntax.SyntaxTree);
                ISymbol memberSymbol = semanticModel.GetDeclaredSymbol(memberDeclSyntax);
                MethodDeclarationSyntax methodDecl = memberDeclSyntax as MethodDeclarationSyntax;
                var isMethod = methodDecl != null;
                var lexMember = default(_LexEntry);
                var cls = isMethod?memberDeclSyntax.Parent as TypeDeclarationSyntax:memberDeclSyntax as TypeDeclarationSyntax;
                if (cls==null && isMethod)
                {
                    throw new InvalidProgramException("Unable to find containing class for "+methodDecl.Identifier.ToString());
                }
                lexMember.QName = isMethod?cls.Identifier.ToString()+methodDecl.Identifier.ToString():cls.Identifier.ToString();
                lexMember.MethodDecl = isMethod?methodDecl:null;
                lexMember.TypeDecl = cls;
                if (isMethod)
                {
                    var methSym = memberSymbol as IMethodSymbol;
                    if (methSym == null) { continue; }
                    string methodName = methSym.ToString();
                    var rtfn = methSym.ReturnType.ToDisplayString();
                    var mns = _GetNamespace(lexMember.TypeDecl);
                    lexMember.ReturnTypeName = rtfn;
                    if (methSym.Parameters.Length == 0)
                    {
                        lexMember.HasArg = false;
                        if (!rtspeccmp.Contains(rtfn))
                        {
                            var found = false;
                            if (vfaReffed)
                            {
                                if (rtspeccmp.Contains("VisualFA." + rtfn))
                                {
                                    found = true;
                                }
                                else if (rtspeccmp.Contains(rtfn))
                                {
                                    found = true;
                                }
                            }
                            else if (!string.IsNullOrEmpty(mns) && (mns == sharedReaderNS || mns == sharedStringNS || mns == sharedStringTableNS || mns == sharedReaderTableNS))
                            {
                                if (rtspeccmp.Contains(mns + "." + rtfn))
                                {
                                    found = true;
                                }
                            }
                            else
                            {
                                if (rtspeccmp.Contains(rtfn))
                                {
                                    found = true;
                                }
                            }
                            if (!found)
                            {
                                throw new AmbiguousMatchException("[FARule] method must specify FATextReaderRunner or FAStringRunner as a return type or take an argument");
                            }
                            if (rtfn == FAStringDfaTableRunnerFullName || rtfn == FAStringDfaTableRunnerName ||
                                rtfn == FATextReaderDfaTableRunnerFullName || rtfn == FATextReaderDfaTableRunnerName)
                            {
                                lexMember.IsTable = true;
                            }
                        }
                        lexMember.IsReader = rtrdrcmp.Contains(rtfn);
                    }
                    else if (methSym.Parameters.Length == 1)
                    {
                        lexMember.HasArg = true;
                        if (rtspeccmp.Contains(rtfn))
                        {
                            if (rtrdrcmp.Contains(rtfn))
                            {
                                if (methSym.Parameters[0].Type.ToDisplayString() != TextReaderFullName)
                                {
                                    throw new InvalidProgramException("[FARule] method returning an FATextReaderRunner must take no arguments, or a single TextReader argument");
                                }
                                lexMember.IsReader = true;
                            }
                            else
                            {
                                var dn = methSym.Parameters[0].Type.ToDisplayString();
                                if (dn != StringFullName)
                                {
                                    throw new InvalidProgramException("[FARule] method returning an FAStringRunner must take no arguments, or a single string argument");
                                }
                                lexMember.IsReader = false;
                            }
                            if (rtfn == FAStringDfaTableRunnerFullName || rtfn == FAStringDfaTableRunnerName ||
                                rtfn == FATextReaderDfaTableRunnerFullName || rtfn == FATextReaderDfaTableRunnerName)
                            {
                                lexMember.IsTable = true;
                            }
                        }
                        else
                        {
                            var found = false;
                            if (!string.IsNullOrEmpty(mns) && (mns == sharedReaderNS || mns == sharedStringNS))
                            {
                                if (rtspeccmp.Contains(mns + "." + rtfn))
                                {
                                    lexMember.IsReader = rtrdrcmp.Contains(mns + "." + rtfn);
                                    found = true;
                                }
                            }
                            else
                            {
                                if (rtspeccmp.Contains(rtfn))
                                {
                                    lexMember.IsReader = rtrdrcmp.Contains(rtfn);
                                    found = true;
                                }

                            }
                            if (!found)
                            {
                                throw new InvalidProgramException("[FARule] method must return an FARunner derivative");
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidProgramException("[FARule] method has too many parameters");
                    }
                } else // is type
                {
                    // figure out the base type
                    var found = false;
                    foreach(var node in lexMember.TypeDecl.BaseList.ChildNodes())
                    {
                        var str = node.ToString();
                        if(str == FATextReaderRunnerName || str == FATextReaderRunnerFullName || str == FATextReaderDfaTableRunnerName || str == FATextReaderDfaTableRunnerName)
                        {
                            found = true;
                            lexMember.IsReader = true;
                        } else if (str == FAStringRunnerName || str == FAStringRunnerFullName || str == FAStringDfaTableRunnerName || str == FAStringDfaTableRunnerName)
                        {
                            found = true;
                        }
                        if (str == FAStringDfaTableRunnerName || str == FAStringDfaTableRunnerFullName || str == FATextReaderDfaTableRunnerName || str == FATextReaderDfaTableRunnerFullName)
                        {
                            lexMember.IsTable = true;
                            found = true;
                        } else if (str == FAStringRunnerName || str == FAStringRunnerFullName || str == FATextReaderRunnerName || str == FATextReaderRunnerFullName)
                        {
                            lexMember.IsTable = false;
                            found = true;
                        }
                        if(found)
                        {
                            break;
                        }
                    }

                }
                var attrs = memberSymbol.GetAttributes();
                var rules = new List<_LexRule>();
                foreach (var attr in attrs)
                {
                    if (attr.AttributeClass != null && attr.AttributeClass.ToDisplayString() == FARuleAttributeFullName)
                    {
                        var rule = default(_LexRule);
                        rule.Expression = null;
                        rule.Id = -1;
                        rule.Symbol = null;
                        if (attr.ConstructorArguments != null && attr.ConstructorArguments.Length == 1)
                        {
                            rule.Expression = attr.ConstructorArguments[0].Value?.ToString();
                        }
                        foreach (KeyValuePair<string, TypedConstant> namedArgument in attr.NamedArguments)
                        {
                            if (namedArgument.Key == "Expression"
                                && namedArgument.Value.Value?.ToString() is { } n)
                            {
                                rule.Expression = n;
                            }
                            else if (namedArgument.Key == "BlockEnd"
                                && namedArgument.Value.Value?.ToString() is { } b)
                            {
                                rule.BlockEnd = b;
                            }
                            else if (namedArgument.Key == "Id"
                                && namedArgument.Value.Value is int i)
                            {
                                rule.Id = i;
                            }
                            else if (namedArgument.Key == "Symbol" && namedArgument.Value.Value?.ToString() is { } s)
                            {
                                rule.Symbol = s;
                            }
                        }
                        if (rule.Expression == null)
                        {
                            throw new InvalidProgramException("the [FARule] expression must be specified");
                        }
                        rules.Add(rule);
                    }
                }
                var ids = new HashSet<int>();
                for (int ic = rules.Count, i = 0; i < ic; ++i)
                {
                    var rule = rules[i];
                    if (rule.Id > -1 && !ids.Add(rule.Id))
                        throw new InvalidProgramException(string.Format("There is a duplicate [FARule] Id {0}", rule.Id));
                }
                var lastId = 0;
                for (int ic = rules.Count, i = 0; i < ic; ++i)
                {
                    var rule = rules[i];
                    if (rule.Id < 0)
                    {
                        rule.Id = lastId;
                        ids.Add(lastId);
                        while (ids.Contains(lastId))
                            ++lastId;
                    }
                    else
                    {
                        lastId = rule.Id;
                        while (ids.Contains(lastId))
                            ++lastId;
                    }
                    rules[i] = rule;
                }
                lexMember.Rules = rules.ToArray();
                result.Add(lexMember);
            }
            result.Sort((x,y)=>x.TypeDecl.Identifier.ToFullString().CompareTo(y.TypeDecl.Identifier.ToFullString()));
            return result;
        }
        static string _Escape(string str)
        {
            var result = new StringBuilder();
            for(int i = 0;i < str.Length;i++)
            {
                var ch = str[i];
                _AppendCharTo(result, ch);
            }
            return result.ToString();
        }
        static void _AppendCharTo(StringBuilder builder, int @char)
        {
            switch (@char)
            {
                
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
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
           "FARuleAttribute.g.cs", SourceText.From(AttributeImpl, Encoding.UTF8)));

            IncrementalValuesProvider<MemberDeclarationSyntax> methodDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => _IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => _GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null)!;

            IncrementalValueProvider<(Compilation, ImmutableArray<MemberDeclarationSyntax>)> compilationAndMethods
                = context.CompilationProvider.Combine(methodDeclarations.Collect());
           context.RegisterSourceOutput(compilationAndMethods,
           static (spc, source) => _Execute(source.Item1, source.Item2, spc));
        }
    }
}
