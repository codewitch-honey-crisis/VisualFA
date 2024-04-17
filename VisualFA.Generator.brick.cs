using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
namespace VisualFA{static partial class Deslanged{public static CodeCompileUnit GetFAMatch(bool spans){if(spans){
#if FALIB_SPANS
return DeslangedSpan.FAMatch;
#else
throw new NotSupportedException("Spans are not supported");
#endif
}return DeslangedString.FAMatch;}public static CodeCompileUnit FAMatch{get{
#if FALIB_SPANS
if(VisualFA.FAStringRunner.UsingSpans){return DeslangedSpan.FAMatch;}else{return DeslangedString.FAMatch;}
#else
return DeslangedString.FAMatch;
#endif
}}public static CodeCompileUnit GetFARunner(bool spans){if(spans){
#if FALIB_SPANS
return DeslangedSpan.FARunnerSpan;
#else
throw new NotSupportedException("Spans are not supported");
#endif
}return DeslangedString.FARunnerString;}public static CodeCompileUnit FARunner{get{
#if FALIB_SPANS
if(VisualFA.FAStringRunner.UsingSpans){return DeslangedSpan.FARunnerSpan;}else{return DeslangedString.FARunnerString;}
#else
return DeslangedString.FARunnerString;
#endif
}}public static CodeCompileUnit GetFADfaTableRunner(bool spans){if(spans){
#if FALIB_SPANS
return DeslangedSpan.FADfaTableRunnerSpan;
#else
throw new NotSupportedException("Spans are not supported");
#endif
}return DeslangedString.FADfaTableRunnerString;}public static CodeCompileUnit FADfaTableRunner{get{
#if FALIB_SPANS
if(VisualFA.FAStringRunner.UsingSpans){return DeslangedSpan.FADfaTableRunnerSpan;}else{return DeslangedString.FADfaTableRunnerString;}
#else
return DeslangedString.FADfaTableRunnerString;
#endif
}}}}namespace VisualFA{using System.CodeDom;using System.Reflection;internal partial class DeslangedSpan{private static CodeCompileUnit _CompileUnit(string[]
referencedAssemblies,CodeNamespace[]namespaces,CodeAttributeDeclaration[]assemblyCustomAttributes,CodeDirective[]startDirectives,CodeDirective[]endDirectives)
{CodeCompileUnit result=new CodeCompileUnit();result.ReferencedAssemblies.AddRange(referencedAssemblies);result.Namespaces.AddRange(namespaces);result.AssemblyCustomAttributes.AddRange(assemblyCustomAttributes);
result.StartDirectives.AddRange(startDirectives);result.EndDirectives.AddRange(endDirectives);return result;}private static CodeNamespace _Namespace(string
 name,CodeNamespaceImport[]imports,CodeTypeDeclaration[]types,CodeCommentStatement[]comments){CodeNamespace result=new CodeNamespace();result.Name=name;
result.Imports.AddRange(imports);result.Types.AddRange(types);result.Comments.AddRange(comments);return result;}private static CodeTypeDeclaration _TypeDeclaration(
string name,bool isClass,bool isEnum,bool isInterface,bool isStruct,bool isPartial,MemberAttributes attributes,TypeAttributes typeAttributes,CodeTypeParameter[]
typeParameters,CodeTypeReference[]baseTypes,CodeTypeMember[]members,CodeCommentStatement[]comments,CodeAttributeDeclaration[]customAttributes,CodeDirective[]
startDirectives,CodeDirective[]endDirectives,CodeLinePragma linePragma){CodeTypeDeclaration result=new CodeTypeDeclaration(name);result.IsClass=isClass;
result.IsEnum=isEnum;result.IsInterface=isInterface;result.IsStruct=isStruct;result.IsPartial=isPartial;result.Attributes=attributes;result.TypeAttributes
=typeAttributes;result.TypeParameters.AddRange(typeParameters);result.BaseTypes.AddRange(baseTypes);result.Members.AddRange(members);result.Comments.AddRange(comments);
result.CustomAttributes.AddRange(customAttributes);result.StartDirectives.AddRange(startDirectives);result.EndDirectives.AddRange(endDirectives);result.LinePragma
=linePragma;return result;}private static CodeMemberField _MemberField(CodeTypeReference type,string name,CodeExpression initExpression,MemberAttributes
 attributes,CodeCommentStatement[]comments,CodeAttributeDeclaration[]customAttributes,CodeDirective[]startDirectives,CodeDirective[]endDirectives,CodeLinePragma
 linePragma){CodeMemberField result=new CodeMemberField(type,name);result.InitExpression=initExpression;result.Attributes=attributes;result.Comments.AddRange(comments);
result.CustomAttributes.AddRange(customAttributes);result.StartDirectives.AddRange(startDirectives);result.EndDirectives.AddRange(endDirectives);result.LinePragma
=linePragma;return result;}private static CodeMemberProperty _MemberProperty(CodeTypeReference type,string name,MemberAttributes attributes,CodeParameterDeclarationExpression[]
parameters,CodeStatement[]getStatements,CodeStatement[]setStatements,CodeTypeReference[]implementationTypes,CodeTypeReference privateImplementationType,
CodeCommentStatement[]comments,CodeAttributeDeclaration[]customAttributes,CodeDirective[]startDirectives,CodeDirective[]endDirectives,CodeLinePragma linePragma)
{CodeMemberProperty result=new CodeMemberProperty();result.Type=type;result.Name=name;result.Attributes=attributes;result.Parameters.AddRange(parameters);
result.GetStatements.AddRange(getStatements);result.SetStatements.AddRange(setStatements);result.ImplementationTypes.AddRange(implementationTypes);result.PrivateImplementationType
=privateImplementationType;result.Comments.AddRange(comments);result.CustomAttributes.AddRange(customAttributes);result.StartDirectives.AddRange(startDirectives);
result.EndDirectives.AddRange(endDirectives);result.LinePragma=linePragma;return result;}private static CodeMemberMethod _MemberMethod(CodeTypeReference
 returnType,string name,MemberAttributes attributes,CodeParameterDeclarationExpression[]parameters,CodeStatement[]statements,CodeTypeReference[]implementationTypes,
CodeTypeReference privateImplementationType,CodeCommentStatement[]comments,CodeAttributeDeclaration[]customAttributes,CodeAttributeDeclaration[]returnTypeCustomAttributes,
CodeDirective[]startDirectives,CodeDirective[]endDirectives,CodeLinePragma linePragma){CodeMemberMethod result=new CodeMemberMethod();result.ReturnType
=returnType;result.Name=name;result.Attributes=attributes;result.Parameters.AddRange(parameters);result.Statements.AddRange(statements);result.ImplementationTypes.AddRange(implementationTypes);
result.PrivateImplementationType=privateImplementationType;result.Comments.AddRange(comments);result.CustomAttributes.AddRange(customAttributes);result.ReturnTypeCustomAttributes.AddRange(returnTypeCustomAttributes);
result.StartDirectives.AddRange(startDirectives);result.EndDirectives.AddRange(endDirectives);result.LinePragma=linePragma;return result;}public static
 System.CodeDom.CodeCompileUnit FAMatch{get{return DeslangedSpan._CompileUnit(new string[]{"System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"},
new CodeNamespace[]{DeslangedSpan._Namespace("",new CodeNamespaceImport[0],new CodeTypeDeclaration[]{DeslangedSpan._TypeDeclaration("FAMatch",false,false,
false,true,true,(MemberAttributes.Final|MemberAttributes.Private),TypeAttributes.NotPublic,new CodeTypeParameter[0],new CodeTypeReference[0],new CodeTypeMember[]
{DeslangedSpan._MemberField(new CodeTypeReference(typeof(int)),"_symbolId",null,MemberAttributes.Private,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],
new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberField(new CodeTypeReference(typeof(string)),"_value",null,MemberAttributes.Private,
new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberField(new CodeTypeReference(typeof(long)),
"_position",null,MemberAttributes.Private,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),
DeslangedSpan._MemberField(new CodeTypeReference(typeof(int)),"_line",null,MemberAttributes.Private,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],
new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberField(new CodeTypeReference(typeof(int)),"_column",null,MemberAttributes.Private,
new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberProperty(new CodeTypeReference(typeof(int)),
"SymbolId",(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeMethodReturnStatement(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_symbolId"))},new CodeStatement[0],new CodeTypeReference[0],null,new CodeCommentStatement[]
{new CodeCommentStatement(" <summary>",true),new CodeCommentStatement(" The matched symbol - this is the accept id",true),new CodeCommentStatement(" </summary>",
true)},new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberProperty(new CodeTypeReference(typeof(string)),
"Value",(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeMethodReturnStatement(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_value"))},new CodeStatement[0],new CodeTypeReference[0],null,new CodeCommentStatement[]
{new CodeCommentStatement(" <summary>",true),new CodeCommentStatement(" The matched value",true),new CodeCommentStatement(" </summary>",true)},new CodeAttributeDeclaration[0],
new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberProperty(new CodeTypeReference(typeof(long)),"Position",(MemberAttributes.Final|MemberAttributes.
Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"_position"))},new CodeStatement[0],new CodeTypeReference[0],null,new CodeCommentStatement[]{new CodeCommentStatement(" <summary>",true),new CodeCommentStatement(" The position of the match within the input",
true),new CodeCommentStatement(" </summary>",true)},new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberProperty(new
 CodeTypeReference(typeof(int)),"Line",(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]{
new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_line"))},new CodeStatement[0],new CodeTypeReference[0],
null,new CodeCommentStatement[]{new CodeCommentStatement(" <summary>",true),new CodeCommentStatement(" The one based line number",true),new CodeCommentStatement(" </summary>",
true)},new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberProperty(new CodeTypeReference(typeof(int)),
"Column",(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeMethodReturnStatement(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_column"))},new CodeStatement[0],new CodeTypeReference[0],null,new CodeCommentStatement[]
{new CodeCommentStatement(" <summary>",true),new CodeCommentStatement(" The one based column",true),new CodeCommentStatement(" </summary>",true)},new CodeAttributeDeclaration[0],
new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberProperty(new CodeTypeReference(typeof(bool)),"IsSuccess",(MemberAttributes.Final|
MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeMethodReturnStatement(new CodeBinaryOperatorExpression(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_symbolId"),CodeBinaryOperatorType.GreaterThan,new CodePrimitiveExpression(-1)))},new
 CodeStatement[0],new CodeTypeReference[0],null,new CodeCommentStatement[]{new CodeCommentStatement(" <summary>",true),new CodeCommentStatement(" Indicates whether the text matched the expression",
true),new CodeCommentStatement(" </summary>",true),new CodeCommentStatement(" <remarks>Non matches are returned with negative accept symbols. You can use this"
+" property to determine if the text therein was part of a match.</remarks>",true)},new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],
null),DeslangedSpan._MemberMethod(new CodeTypeReference(typeof(string)),"ToString",(MemberAttributes.Override|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],
new CodeStatement[]{new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(System.Text.StringBuilder)),"sb",new CodeObjectCreateExpression(new
 CodeTypeReference(typeof(System.Text.StringBuilder)),new CodeExpression[0])),new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeVariableReferenceExpression("sb"),"Append"),new CodeExpression[]{new CodePrimitiveExpression("[SymbolId: ")})),new CodeExpressionStatement(new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),"Append"),new CodeExpression[]{new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),
"SymbolId")})),new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),
"Append"),new CodeExpression[]{new CodePrimitiveExpression(", Value: ")})),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new
 CodeThisReferenceExpression(),"Value"),CodeBinaryOperatorType.IdentityInequality,new CodePrimitiveExpression(null)),new CodeStatement[]{new CodeExpressionStatement(new
 CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),"Append"),new CodeExpression[]{new CodePrimitiveExpression("\"")})),
new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),"Append"),new CodeExpression[]
{new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),
"Value"),"Replace"),new CodeExpression[]{new CodePrimitiveExpression("\r"),new CodePrimitiveExpression("\\r")}),"Replace"),new CodeExpression[]{new CodePrimitiveExpression("\t"),
new CodePrimitiveExpression("\\t")}),"Replace"),new CodeExpression[]{new CodePrimitiveExpression("\n"),new CodePrimitiveExpression("\\n")}),"Replace"),
new CodeExpression[]{new CodePrimitiveExpression(""),new CodePrimitiveExpression("\\v")})})),new CodeExpressionStatement(new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),"Append"),new CodeExpression[]{new CodePrimitiveExpression("\", Position: ")}))},
new CodeStatement[]{new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),
"Append"),new CodeExpression[]{new CodePrimitiveExpression("null, Position: ")}))}),new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeVariableReferenceExpression("sb"),"Append"),new CodeExpression[]{new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),"Position")})),
new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),"Append"),new CodeExpression[]
{new CodePrimitiveExpression(" (")})),new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),
"Append"),new CodeExpression[]{new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),"Line")})),new CodeExpressionStatement(new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),"Append"),new CodeExpression[]{new CodePrimitiveExpression(", ")})),new CodeExpressionStatement(new
 CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),"Append"),new CodeExpression[]{new CodePropertyReferenceExpression(new
 CodeThisReferenceExpression(),"Column")})),new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),
"Append"),new CodeExpression[]{new CodePrimitiveExpression(")]")})),new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeVariableReferenceExpression("sb"),"ToString"),new CodeExpression[0]))},new CodeTypeReference[0],null,new CodeCommentStatement[]{new CodeCommentStatement(" <summary>",
true),new CodeCommentStatement(" Provides a string representation of the match",true),new CodeCommentStatement(" </summary>",true),new CodeCommentStatement(" <returns>A string containing match information</returns>",
true)},new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberMethod(new
 CodeTypeReference("FAMatch"),"Create",(MemberAttributes.Static|MemberAttributes.Public),new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new
 CodeTypeReference(typeof(int)),"symbolId"),new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)),"value"),new CodeParameterDeclarationExpression(new
 CodeTypeReference(typeof(long)),"position"),new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)),"line"),new CodeParameterDeclarationExpression(new
 CodeTypeReference(typeof(int)),"column")},new CodeStatement[]{new CodeVariableDeclarationStatement(new CodeTypeReference("FAMatch"),"result",new CodeDefaultValueExpression(new
 CodeTypeReference("FAMatch"))),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("result"),"_symbolId"),new
 CodeArgumentReferenceExpression("symbolId")),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("result"),"_value"),
new CodeArgumentReferenceExpression("value")),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("result"),"_position"),
new CodeArgumentReferenceExpression("position")),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("result"),
"_line"),new CodeArgumentReferenceExpression("line")),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("result"),
"_column"),new CodeArgumentReferenceExpression("column")),new CodeMethodReturnStatement(new CodeVariableReferenceExpression("result"))},new CodeTypeReference[0],
null,new CodeCommentStatement[]{new CodeCommentStatement(" <summary>",true),new CodeCommentStatement(" Constructs a new instance",true),new CodeCommentStatement(" </summary>",
true),new CodeCommentStatement(" <param name=\"symbolId\">The symbol id</param>",true),new CodeCommentStatement(" <param name=\"value\">The matched value</param>",
true),new CodeCommentStatement(" <param name=\"position\">The match position</param>",true),new CodeCommentStatement(" <param name=\"line\">The line</param>",
true),new CodeCommentStatement(" <param name=\"column\">The column</param>",true)},new CodeAttributeDeclaration[]{new CodeAttributeDeclaration(new CodeTypeReference(typeof(System.Runtime.CompilerServices.MethodImplAttribute)),
new CodeAttributeArgument[]{new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(typeof(System.Runtime.CompilerServices.MethodImplOptions))),
"AggressiveInlining"))})},new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null)},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],
new CodeDirective[0],new CodeDirective[0],null)},new CodeCommentStatement[]{new CodeCommentStatement(" <summary>",true),new CodeCommentStatement(" Represents a match from <see cref=\"FARunner\"/></code>",
true),new CodeCommentStatement(" </summary>",true)})},new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0]);}}private static CodeConstructor
 _Constructor(MemberAttributes attributes,CodeParameterDeclarationExpression[]parameters,CodeExpression[]chainedConstructorArgs,CodeExpression[]baseConstructorArgs,
CodeStatement[]statements,CodeCommentStatement[]comments,CodeAttributeDeclaration[]customAttributes,CodeDirective[]startDirectives,CodeDirective[]endDirectives,
CodeLinePragma linePragma){CodeConstructor result=new CodeConstructor();result.Attributes=attributes;result.Parameters.AddRange(parameters);result.ChainedConstructorArgs.AddRange(chainedConstructorArgs);
result.BaseConstructorArgs.AddRange(baseConstructorArgs);result.Statements.AddRange(statements);result.Comments.AddRange(comments);result.CustomAttributes.AddRange(customAttributes);
result.StartDirectives.AddRange(startDirectives);result.EndDirectives.AddRange(endDirectives);result.LinePragma=linePragma;return result;}private static
 CodeParameterDeclarationExpression _ParameterDeclarationExpression(CodeTypeReference type,string name,FieldDirection direction,CodeAttributeDeclaration[]
customAttributes){CodeParameterDeclarationExpression result=new CodeParameterDeclarationExpression(type,name);result.Direction=direction;result.CustomAttributes.AddRange(customAttributes);
return result;}public static System.CodeDom.CodeCompileUnit FARunnerSpan{get{return DeslangedSpan._CompileUnit(new string[0],new CodeNamespace[]{DeslangedSpan._Namespace("",
new CodeNamespaceImport[]{new CodeNamespaceImport("System"),new CodeNamespaceImport("System.Collections"),new CodeNamespaceImport("System.Collections.Generic"),
new CodeNamespaceImport("System.Runtime.CompilerServices"),new CodeNamespaceImport("System.IO"),new CodeNamespaceImport("System.Text")},new CodeTypeDeclaration[]
{DeslangedSpan._TypeDeclaration("FARunner",true,false,false,false,true,(MemberAttributes.Final|MemberAttributes.Private),(((TypeAttributes.AutoLayout|
TypeAttributes.AnsiClass)|TypeAttributes.Class)|TypeAttributes.Abstract),new CodeTypeParameter[0],new CodeTypeReference[]{new CodeTypeReference("Object"),
new CodeTypeReference("IEnumerable`1",new CodeTypeReference[]{new CodeTypeReference("FAMatch")})},new CodeTypeMember[]{DeslangedSpan._Constructor(MemberAttributes.FamilyOrAssembly,
new CodeParameterDeclarationExpression[0],new CodeExpression[0],new CodeExpression[0],new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"position"),new CodePrimitiveExpression(-1)),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"line"),new CodePrimitiveExpression(1)),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),new CodePrimitiveExpression(0)),
new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_tabWidth"),new CodePrimitiveExpression(4))},new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._TypeDeclaration("Enumerator",true,false,false,false,false,
(MemberAttributes.Final|MemberAttributes.Private),(((TypeAttributes.AutoLayout|TypeAttributes.AnsiClass)|TypeAttributes.Class)|TypeAttributes.NestedPublic),
new CodeTypeParameter[0],new CodeTypeReference[]{new CodeTypeReference("Object"),new CodeTypeReference("IEnumerator`1",new CodeTypeReference[]{new CodeTypeReference("FAMatch")})},
new CodeTypeMember[]{DeslangedSpan._MemberField(new CodeTypeReference(typeof(int)),"_state",null,MemberAttributes.Private,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberField(new CodeTypeReference("FAMatch"),"_current",
null,MemberAttributes.Private,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberField(new
 CodeTypeReference("WeakReference`1",new CodeTypeReference[]{new CodeTypeReference("FARunner")}),"_parent",null,MemberAttributes.Private,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._Constructor((MemberAttributes.Final|MemberAttributes.Public),
new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new CodeTypeReference("FARunner"),"parent")},new CodeExpression[0],new
 CodeExpression[0],new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_parent"),new CodeObjectCreateExpression(new
 CodeTypeReference("WeakReference`1",new CodeTypeReference[]{new CodeTypeReference("FARunner")}),new CodeExpression[]{new CodeArgumentReferenceExpression("parent")})),
new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_state"),new CodePrimitiveExpression(-2))},new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberProperty(new CodeTypeReference("FAMatch"),"Current",
(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeConditionStatement(new CodeBinaryOperatorExpression(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_state"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-3)),new CodeStatement[]
{new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference("ObjectDisposedException"),new CodeExpression[]{new CodePrimitiveExpression("Enumerator")}))},
new CodeStatement[0]),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_state"),
CodeBinaryOperatorType.LessThan,new CodePrimitiveExpression(0)),new CodeStatement[]{new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new
 CodeTypeReference("InvalidOperationException"),new CodeExpression[]{new CodePrimitiveExpression("The enumerator is not positioned on an element")}))},
new CodeStatement[0]),new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_current"))},new CodeStatement[0],
new CodeTypeReference[]{new CodeTypeReference("IEnumerator`1",new CodeTypeReference[]{new CodeTypeReference("FAMatch")})},null,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberProperty(new CodeTypeReference(typeof(object)),"Current",
((MemberAttributes)(0)),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeMethodReturnStatement(new CodePropertyReferenceExpression(new
 CodeThisReferenceExpression(),"Current"))},new CodeStatement[0],new CodeTypeReference[0],new CodeTypeReference(typeof(System.Collections.IEnumerator)),
new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberMethod(new CodeTypeReference(typeof(void)),
"Dispose",((MemberAttributes)(0)),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_state"),new CodePrimitiveExpression(-3))},new CodeTypeReference[0],new CodeTypeReference("IDisposable"),new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberMethod(new CodeTypeReference(typeof(bool)),
"MoveNext",(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeConditionStatement(new
 CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_state"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-3)),
new CodeStatement[]{new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference("ObjectDisposedException"),new CodeExpression[]
{new CodePrimitiveExpression("Enumerator")}))},new CodeStatement[0]),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_state"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-1)),new CodeStatement[]{new CodeMethodReturnStatement(new
 CodePrimitiveExpression(false))},new CodeStatement[0]),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_state"),
new CodePrimitiveExpression(0)),new CodeVariableDeclarationStatement(new CodeTypeReference("FARunner"),"parent",null),new CodeConditionStatement(new CodeBinaryOperatorExpression(new
 CodePrimitiveExpression(false),CodeBinaryOperatorType.ValueEquality,new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_parent"),"TryGetTarget"),new CodeExpression[]{new CodeDirectionExpression(FieldDirection.Out,new CodeVariableReferenceExpression("parent"))})),
new CodeStatement[]{new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference("InvalidOperationException"),new CodeExpression[]
{new CodePrimitiveExpression("The parent was destroyed")}))},new CodeStatement[0]),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"_current"),new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("parent"),"NextMatch"),new CodeExpression[0])),
new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"_current"),"SymbolId"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-2)),new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_state"),new CodePrimitiveExpression(-2)),new CodeMethodReturnStatement(new CodePrimitiveExpression(false))},new CodeStatement[0]),
new CodeMethodReturnStatement(new CodePrimitiveExpression(true))},new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberMethod(new CodeTypeReference(typeof(void)),"Reset",
(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeConditionStatement(new CodeBinaryOperatorExpression(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_state"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-3)),new CodeStatement[]
{new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference("ObjectDisposedException"),new CodeExpression[]{new CodePrimitiveExpression("Enumerator")}))},
new CodeStatement[0]),new CodeVariableDeclarationStatement(new CodeTypeReference("FARunner"),"parent",null),new CodeConditionStatement(new CodeBinaryOperatorExpression(new
 CodePrimitiveExpression(false),CodeBinaryOperatorType.ValueEquality,new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_parent"),"TryGetTarget"),new CodeExpression[]{new CodeDirectionExpression(FieldDirection.Out,new CodeVariableReferenceExpression("parent"))})),
new CodeStatement[]{new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference("InvalidOperationException"),new CodeExpression[]
{new CodePrimitiveExpression("The parent was destroyed")}))},new CodeStatement[0]),new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeVariableReferenceExpression("parent"),"Reset"),new CodeExpression[0])),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"_state"),new CodePrimitiveExpression(-2))},new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],
new CodeDirective[0],new CodeDirective[0],null)},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],
null),DeslangedSpan._MemberProperty(new CodeTypeReference(typeof(int)),"TabWidth",(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],
new CodeStatement[]{new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_tabWidth"))},new CodeStatement[]
{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePropertySetValueReferenceExpression(),CodeBinaryOperatorType.LessThan,new CodePrimitiveExpression(1)),
new CodeStatement[]{new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference("ArgumentOutOfRangeException"),new CodeExpression[0]))},
new CodeStatement[0]),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_tabWidth"),new CodePropertySetValueReferenceExpression())},
new CodeTypeReference[0],null,new CodeCommentStatement[]{new CodeCommentStatement(" <summary>",true),new CodeCommentStatement(" Indicates the width of a tab, in columns",
true),new CodeCommentStatement(" </summary>",true)},new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberField(new
 CodeTypeReference(typeof(int)),"_tabWidth",null,MemberAttributes.Private,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],
new CodeDirective[0],null),DeslangedSpan._MemberField(new CodeTypeReference(typeof(int)),"position",null,MemberAttributes.Family,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberField(new CodeTypeReference(typeof(int)),"line",null,
MemberAttributes.Family,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberField(new
 CodeTypeReference(typeof(int)),"column",null,MemberAttributes.Family,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],
new CodeDirective[0],null),DeslangedSpan._MemberMethod(new CodeTypeReference(typeof(void)),"ThrowUnicode",(MemberAttributes.Static|MemberAttributes.Family),
new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)),"pos")},new CodeStatement[]{new CodeThrowExceptionStatement(new
 CodeObjectCreateExpression(new CodeTypeReference("IOException"),new CodeExpression[]{new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeTypeReferenceExpression(new CodeTypeReference(typeof(string))),"Concat"),new CodeExpression[]{new CodePrimitiveExpression("Unicode error in stream at position "),
new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeArgumentReferenceExpression("pos"),"ToString"),new CodeExpression[0])})}))},new
 CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],
null),DeslangedSpan._MemberMethod(new CodeTypeReference("FAMatch"),"NextMatch",(MemberAttributes.Abstract|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],
new CodeStatement[0],new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],
new CodeDirective[0],null),DeslangedSpan._MemberMethod(new CodeTypeReference(typeof(void)),"Reset",(MemberAttributes.Abstract|MemberAttributes.Public),
new CodeParameterDeclarationExpression[0],new CodeStatement[0],new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberMethod(new CodeTypeReference("IEnumerator`1",new CodeTypeReference[]
{new CodeTypeReference("FAMatch")}),"GetEnumerator",(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]
{new CodeMethodReturnStatement(new CodeObjectCreateExpression(new CodeTypeReference("Enumerator"),new CodeExpression[]{new CodeThisReferenceExpression()}))},
new CodeTypeReference[0],new CodeTypeReference("IEnumerable`1",new CodeTypeReference[]{new CodeTypeReference("FAMatch")}),new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberMethod(new CodeTypeReference(typeof(System.Collections.IEnumerator)),
"GetEnumerator",((MemberAttributes)(0)),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeMethodReturnStatement(new CodeObjectCreateExpression(new
 CodeTypeReference("Enumerator"),new CodeExpression[]{new CodeThisReferenceExpression()}))},new CodeTypeReference[0],new CodeTypeReference(typeof(System.Collections.IEnumerable)),
new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null)},new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._TypeDeclaration("FAStringRunner",true,false,false,false,
true,(MemberAttributes.Final|MemberAttributes.Private),(((TypeAttributes.AutoLayout|TypeAttributes.AnsiClass)|TypeAttributes.Class)|TypeAttributes.Abstract),
new CodeTypeParameter[0],new CodeTypeReference[]{new CodeTypeReference("FARunner")},new CodeTypeMember[]{DeslangedSpan._MemberProperty(new CodeTypeReference(typeof(bool)),
"UsingSpans",(MemberAttributes.Static|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeMethodReturnStatement(new
 CodePrimitiveExpression(true))},new CodeStatement[0],new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],
new CodeDirective[0],null),DeslangedSpan._MemberField(new CodeTypeReference(typeof(string)),"string",null,MemberAttributes.Family,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberMethod(new CodeTypeReference(typeof(void)),"Set",
(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)),
"string")},new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"string"),new CodeArgumentReferenceExpression("string")),
new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),new CodePrimitiveExpression(-1)),new CodeAssignStatement(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"line"),new CodePrimitiveExpression(1)),new CodeAssignStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"column"),new CodePrimitiveExpression(1))},new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberMethod(new CodeTypeReference(typeof(void)),"Reset",
(MemberAttributes.Override|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"position"),new CodePrimitiveExpression(-1)),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"line"),new CodePrimitiveExpression(1)),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),new CodePrimitiveExpression(1))},
new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],
null),DeslangedSpan._MemberMethod(new CodeTypeReference(typeof(void)),"Advance",MemberAttributes.Family,new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new
 CodeTypeReference("ReadOnlySpan`1",new CodeTypeReference[]{new CodeTypeReference("Char")}),"span"),DeslangedSpan._ParameterDeclarationExpression(new CodeTypeReference(typeof(int)),
"ch",FieldDirection.Ref,new CodeAttributeDeclaration[0]),DeslangedSpan._ParameterDeclarationExpression(new CodeTypeReference(typeof(int)),"len",FieldDirection.Ref,
new CodeAttributeDeclaration[0]),new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(bool)),"first")},new CodeStatement[]{new CodeConditionStatement(new
 CodeBinaryOperatorExpression(new CodePrimitiveExpression(false),CodeBinaryOperatorType.ValueEquality,new CodeArgumentReferenceExpression("first")),new
 CodeStatement[]{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("ch"),CodeBinaryOperatorType.ValueEquality,
new CodePrimitiveExpression(10)),new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"line"),
new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"line"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),
new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),new CodePrimitiveExpression(1))},new CodeStatement[]
{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("ch"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(13)),
new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),new CodePrimitiveExpression(1))},
new CodeStatement[]{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("ch"),CodeBinaryOperatorType.ValueEquality,
new CodePrimitiveExpression(9)),new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),
new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"column"),CodeBinaryOperatorType.Subtract,new CodePrimitiveExpression(1)),CodeBinaryOperatorType.Divide,new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),
"TabWidth")),CodeBinaryOperatorType.Multiply,new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),"TabWidth"),
CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))))},new CodeStatement[]{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("ch"),
CodeBinaryOperatorType.GreaterThan,new CodePrimitiveExpression(31)),new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"column"),new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),CodeBinaryOperatorType.Add,new
 CodePrimitiveExpression(1)))},new CodeStatement[0])})})}),new CodeAssignStatement(new CodeArgumentReferenceExpression("len"),new CodeBinaryOperatorExpression(new
 CodeArgumentReferenceExpression("len"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeConditionStatement(new CodeBinaryOperatorExpression(new
 CodeArgumentReferenceExpression("ch"),CodeBinaryOperatorType.GreaterThan,new CodePrimitiveExpression(65535)),new CodeStatement[]{new CodeAssignStatement(new
 CodeArgumentReferenceExpression("len"),new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("len"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1)))},
new CodeStatement[0]),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),new CodeBinaryOperatorExpression(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1)))},new CodeStatement[0]),
new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),CodeBinaryOperatorType.LessThan,
new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("span"),"Length")),new CodeStatement[]{new CodeVariableDeclarationStatement(new
 CodeTypeReference(typeof(char)),"ch1",new CodeIndexerExpression(new CodeArgumentReferenceExpression("span"),new CodeExpression[]{new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"position")})),new CodeConditionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new
 CodeTypeReference(typeof(char))),"IsHighSurrogate"),new CodeExpression[]{new CodeVariableReferenceExpression("ch1")}),new CodeStatement[]{new CodeAssignStatement(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"position"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"position"),CodeBinaryOperatorType.GreaterThanOrEqual,new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("span"),
"Length")),new CodeStatement[]{new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new
 CodeTypeReference("FAStringRunner")),"ThrowUnicode"),new CodeExpression[]{new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position")}))},
new CodeStatement[0]),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(char)),"ch2",new CodeIndexerExpression(new CodeArgumentReferenceExpression("span"),
new CodeExpression[]{new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position")})),new CodeAssignStatement(new CodeArgumentReferenceExpression("ch"),
new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(typeof(char))),"ConvertToUtf32"),
new CodeExpression[]{new CodeVariableReferenceExpression("ch1"),new CodeVariableReferenceExpression("ch2")}))},new CodeStatement[]{new CodeAssignStatement(new
 CodeArgumentReferenceExpression("ch"),new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(typeof(System.Convert))),
"ToInt32"),new CodeExpression[]{new CodeVariableReferenceExpression("ch1")}))})},new CodeStatement[]{new CodeAssignStatement(new CodeArgumentReferenceExpression("ch"),
new CodePrimitiveExpression(-1))})},new CodeTypeReference[0],null,new CodeCommentStatement[]{new CodeCommentStatement(" much bigger, but faster code")},
new CodeAttributeDeclaration[]{new CodeAttributeDeclaration(new CodeTypeReference(typeof(System.Runtime.CompilerServices.MethodImplAttribute)),new CodeAttributeArgument[]
{new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(typeof(System.Runtime.CompilerServices.MethodImplOptions))),
"AggressiveInlining"))})},new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null)},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],
new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._TypeDeclaration("FATextReaderRunner",true,false,false,false,true,(MemberAttributes.Final
|MemberAttributes.Private),(((TypeAttributes.AutoLayout|TypeAttributes.AnsiClass)|TypeAttributes.Class)|TypeAttributes.Abstract),new CodeTypeParameter[0],
new CodeTypeReference[]{new CodeTypeReference("FARunner")},new CodeTypeMember[]{DeslangedSpan._MemberField(new CodeTypeReference("TextReader"),"reader",
null,MemberAttributes.Family,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberField(new
 CodeTypeReference("StringBuilder"),"capture",null,MemberAttributes.Family,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],
new CodeDirective[0],null),DeslangedSpan._MemberField(new CodeTypeReference(typeof(int)),"current",null,MemberAttributes.Family,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._Constructor(MemberAttributes.Family,new CodeParameterDeclarationExpression[0],
new CodeExpression[0],new CodeExpression[0],new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"capture"),new CodeObjectCreateExpression(new CodeTypeReference("StringBuilder"),new CodeExpression[0]))},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],
new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberMethod(new CodeTypeReference(typeof(void)),"Set",(MemberAttributes.Final|MemberAttributes.
Public),new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new CodeTypeReference("TextReader"),"reader")},new CodeStatement[]
{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"reader"),new CodeArgumentReferenceExpression("reader")),new
 CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),new CodePrimitiveExpression(-2)),new CodeAssignStatement(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),new CodePrimitiveExpression(-1)),new CodeAssignStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"line"),new CodePrimitiveExpression(1)),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"column"),new CodePrimitiveExpression(1))},new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],
new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberMethod(new CodeTypeReference(typeof(void)),"Reset",(MemberAttributes.Override|MemberAttributes.
Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference("NotSupportedException"),
new CodeExpression[0]))},new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new
 CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberMethod(new CodeTypeReference(typeof(void)),"Advance",MemberAttributes.Family,new CodeParameterDeclarationExpression[0],
new CodeStatement[]{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),
CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(10)),new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"line"),new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"line"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),new CodePrimitiveExpression(1))},
new CodeStatement[]{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),
CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(13)),new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"column"),new CodePrimitiveExpression(1))},new CodeStatement[]{new CodeConditionStatement(new CodeBinaryOperatorExpression(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(9)),new CodeStatement[]
{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new
 CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),CodeBinaryOperatorType.Subtract,new CodePrimitiveExpression(1)),
CodeBinaryOperatorType.Divide,new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),"TabWidth")),CodeBinaryOperatorType.Multiply,new CodeBinaryOperatorExpression(new
 CodePropertyReferenceExpression(new CodeThisReferenceExpression(),"TabWidth"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))))},new CodeStatement[]
{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.GreaterThan,
new CodePrimitiveExpression(31)),new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),
new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1)))},
new CodeStatement[0])})})}),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"current"),CodeBinaryOperatorType.GreaterThan,new CodePrimitiveExpression(-1)),new CodeStatement[]{new CodeExpressionStatement(new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"capture"),"Append"),new CodeExpression[]{new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(typeof(char))),"ConvertFromUtf32"),new CodeExpression[]{new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"current")})}))},new CodeStatement[0]),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"current"),new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"reader"),
"Read"),new CodeExpression[0])),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"current"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-1)),new CodeStatement[]{new CodeMethodReturnStatement(null)},new CodeStatement[0]),
new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"position"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(char)),
"ch1",new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(typeof(System.Convert))),
"ToChar"),new CodeExpression[]{new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current")})),new CodeConditionStatement(new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(typeof(char))),"IsHighSurrogate"),new CodeExpression[]{new CodeVariableReferenceExpression("ch1")}),
new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"reader"),"Read"),new CodeExpression[0])),new CodeConditionStatement(new
 CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-1)),
new CodeStatement[]{new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference("FATextReaderRunner")),
"ThrowUnicode"),new CodeExpression[]{new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position")}))},new CodeStatement[0]),new CodeVariableDeclarationStatement(new
 CodeTypeReference(typeof(char)),"ch2",new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(typeof(System.Convert))),
"ToChar"),new CodeExpression[]{new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current")})),new CodeAssignStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"current"),new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(typeof(char))),
"ConvertToUtf32"),new CodeExpression[]{new CodeVariableReferenceExpression("ch1"),new CodeVariableReferenceExpression("ch2")})),new CodeAssignStatement(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"position"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1)))},new CodeStatement[0])},new CodeTypeReference[0],null,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null)},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],
new CodeDirective[0],new CodeDirective[0],null)},new CodeCommentStatement[0])},new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0]);
}}public static System.CodeDom.CodeCompileUnit FADfaTableRunnerSpan{get{return DeslangedSpan._CompileUnit(new string[0],new CodeNamespace[]{DeslangedSpan._Namespace("",
new CodeNamespaceImport[]{new CodeNamespaceImport("System"),new CodeNamespaceImport("System.Collections"),new CodeNamespaceImport("System.Collections.Generic"),
new CodeNamespaceImport("System.Runtime.CompilerServices"),new CodeNamespaceImport("System.IO"),new CodeNamespaceImport("System.Text")},new CodeTypeDeclaration[]
{DeslangedSpan._TypeDeclaration("FAStringDfaTableRunner",true,false,false,false,true,(MemberAttributes.Final|MemberAttributes.Private),TypeAttributes.NotPublic,
new CodeTypeParameter[0],new CodeTypeReference[]{new CodeTypeReference("FAStringRunner")},new CodeTypeMember[]{DeslangedSpan._MemberField(new CodeTypeReference(new
 CodeTypeReference(typeof(int)),1),"_dfa",null,MemberAttributes.Private,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],
new CodeDirective[0],null),DeslangedSpan._MemberField(new CodeTypeReference(new CodeTypeReference(new CodeTypeReference(typeof(int)),1),1),"_blockEnds",
null,MemberAttributes.Private,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._Constructor((MemberAttributes.Final
|MemberAttributes.Public),new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new CodeTypeReference(new CodeTypeReference(typeof(int)),
1),"dfa")},new CodeExpression[0],new CodeExpression[0],new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"_dfa"),new CodeArgumentReferenceExpression("dfa")),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_blockEnds"),
new CodePrimitiveExpression(null))},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._Constructor((MemberAttributes.Final
|MemberAttributes.Public),new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new CodeTypeReference(new CodeTypeReference(typeof(int)),
1),"dfa"),new CodeParameterDeclarationExpression(new CodeTypeReference(new CodeTypeReference(new CodeTypeReference(typeof(int)),1),1),"blockEnds")},new
 CodeExpression[0],new CodeExpression[0],new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"_dfa"),new CodeArgumentReferenceExpression("dfa")),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_blockEnds"),
new CodeArgumentReferenceExpression("blockEnds"))},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],
null),DeslangedSpan._MemberMethod(new CodeTypeReference("FAMatch"),"NextMatch",(MemberAttributes.Override|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],
new CodeStatement[]{new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(),"_NextImpl"),
new CodeExpression[]{new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"string")}))},new CodeTypeReference[0],null,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberMethod(new CodeTypeReference("FAMatch"),
"_NextImpl",MemberAttributes.Private,new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new CodeTypeReference("ReadOnlySpan`1",
new CodeTypeReference[]{new CodeTypeReference(typeof(char))}),"span")},new CodeStatement[]{new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),
"tlen",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"tto",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),
"prlen",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"pmin",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),
"pmax",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"i",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),
"j",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"state",new CodePrimitiveExpression(0)),new CodeVariableDeclarationStatement(new
 CodeTypeReference(typeof(int)),"acc",null),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"position"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-1)),new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"position"),new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),
CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1)))},new CodeStatement[0]),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),
"len",new CodePrimitiveExpression(0)),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(long)),"cursor_pos",new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"position")),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"line",new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"line")),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"column",new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"column")),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"ch",new CodePrimitiveExpression(-1)),
new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(),"Advance"),new CodeExpression[]
{new CodeArgumentReferenceExpression("span"),new CodeDirectionExpression(FieldDirection.Ref,new CodeVariableReferenceExpression("ch")),new CodeDirectionExpression(FieldDirection.Ref,
new CodeVariableReferenceExpression("len")),new CodePrimitiveExpression(true)})),new CodeLabeledStatement("start_dfa",new CodeSnippetStatement("")),new
 CodeAssignStatement(new CodeVariableReferenceExpression("acc"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("tlen"),
new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeIterationStatement(new CodeAssignStatement(new CodeVariableReferenceExpression("i"),new CodePrimitiveExpression(0)),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("tlen")),
new CodeAssignStatement(new CodeVariableReferenceExpression("i"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("tto"),new CodeArrayIndexerExpression(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new
 CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new
 CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("prlen"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeIterationStatement(new
 CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodePrimitiveExpression(0)),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),
CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("prlen")),new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("pmin"),
new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("pmax"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeConditionStatement(new
 CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("ch"),CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("pmin")),new
 CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),
CodeBinaryOperatorType.Add,new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("prlen"),CodeBinaryOperatorType.Subtract,
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),CodeBinaryOperatorType.Multiply,
new CodePrimitiveExpression(2)))),new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeVariableReferenceExpression("prlen"))},new CodeStatement[]
{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("ch"),CodeBinaryOperatorType.LessThanOrEqual,new CodeVariableReferenceExpression("pmax")),
new CodeStatement[]{new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(),"Advance"),
new CodeExpression[]{new CodeArgumentReferenceExpression("span"),new CodeDirectionExpression(FieldDirection.Ref,new CodeVariableReferenceExpression("ch")),
new CodeDirectionExpression(FieldDirection.Ref,new CodeVariableReferenceExpression("len")),new CodePrimitiveExpression(false)})),new CodeAssignStatement(new
 CodeVariableReferenceExpression("state"),new CodeVariableReferenceExpression("tto")),new CodeGotoStatement("start_dfa")},new CodeStatement[0])})})}),
new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePrimitiveExpression(false),CodeBinaryOperatorType.ValueEquality,new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("acc"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-1))),new CodeStatement[]{new CodeVariableDeclarationStatement(new
 CodeTypeReference(typeof(int)),"sym",new CodeVariableReferenceExpression("acc")),new CodeVariableDeclarationStatement(new CodeTypeReference(new CodeTypeReference(typeof(int)),
1),"be",new CodePrimitiveExpression(null)),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_blockEnds"),CodeBinaryOperatorType.IdentityInequality,new CodePrimitiveExpression(null)),CodeBinaryOperatorType.BooleanAnd,
new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_blockEnds"),
"Length"),CodeBinaryOperatorType.GreaterThan,new CodeVariableReferenceExpression("acc"))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("be"),
new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_blockEnds"),new CodeExpression[]{new CodeVariableReferenceExpression("acc")}))},
new CodeStatement[0]),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("be"),CodeBinaryOperatorType.IdentityInequality,
new CodePrimitiveExpression(null)),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodePrimitiveExpression(0)),
new CodeLabeledStatement("start_be_dfa",new CodeSnippetStatement("")),new CodeAssignStatement(new CodeVariableReferenceExpression("acc"),new CodeArrayIndexerExpression(new
 CodeVariableReferenceExpression("be"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeAssignStatement(new
 CodeVariableReferenceExpression("tlen"),new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("be"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeIterationStatement(new CodeAssignStatement(new CodeVariableReferenceExpression("i"),new CodePrimitiveExpression(0)),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("tlen")),
new CodeAssignStatement(new CodeVariableReferenceExpression("i"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("tto"),new CodeArrayIndexerExpression(new
 CodeVariableReferenceExpression("be"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeAssignStatement(new
 CodeVariableReferenceExpression("prlen"),new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("be"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeIterationStatement(new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodePrimitiveExpression(0)),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("prlen")),
new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("pmin"),new CodeArrayIndexerExpression(new
 CodeVariableReferenceExpression("be"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeAssignStatement(new
 CodeVariableReferenceExpression("pmax"),new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("be"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("ch"),CodeBinaryOperatorType.LessThan,
new CodeVariableReferenceExpression("pmin")),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("prlen"),
CodeBinaryOperatorType.Subtract,new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),
CodeBinaryOperatorType.Multiply,new CodePrimitiveExpression(2)))),new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeVariableReferenceExpression("prlen"))},
new CodeStatement[]{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("ch"),CodeBinaryOperatorType.LessThanOrEqual,
new CodeVariableReferenceExpression("pmax")),new CodeStatement[]{new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeThisReferenceExpression(),"Advance"),new CodeExpression[]{new CodeArgumentReferenceExpression("span"),new CodeDirectionExpression(FieldDirection.Ref,
new CodeVariableReferenceExpression("ch")),new CodeDirectionExpression(FieldDirection.Ref,new CodeVariableReferenceExpression("len")),new CodePrimitiveExpression(false)})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeVariableReferenceExpression("tto")),new CodeGotoStatement("start_be_dfa")},
new CodeStatement[0])})})}),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePrimitiveExpression(false),CodeBinaryOperatorType.ValueEquality,
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("acc"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-1))),new
 CodeStatement[]{new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference("FAMatch")),
"Create"),new CodeExpression[]{new CodeVariableReferenceExpression("sym"),new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeArgumentReferenceExpression("span"),"Slice"),new CodeExpression[]{new CodeCastExpression(new CodeTypeReference(typeof(int)),
new CodeVariableReferenceExpression("cursor_pos")),new CodeVariableReferenceExpression("len")}),"ToString"),new CodeExpression[0]),new CodeVariableReferenceExpression("cursor_pos"),
new CodeVariableReferenceExpression("line"),new CodeVariableReferenceExpression("column")}))},new CodeStatement[0]),new CodeConditionStatement(new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("ch"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-1)),new CodeStatement[]{new CodeMethodReturnStatement(new
 CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference("FAMatch")),"Create"),new CodeExpression[]
{new CodePrimitiveExpression(-1),new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeArgumentReferenceExpression("span"),"Slice"),new CodeExpression[]{new CodeCastExpression(new CodeTypeReference(typeof(int)),new CodeVariableReferenceExpression("cursor_pos")),
new CodeVariableReferenceExpression("len")}),"ToString"),new CodeExpression[0]),new CodeVariableReferenceExpression("cursor_pos"),new CodeVariableReferenceExpression("line"),
new CodeVariableReferenceExpression("column")}))},new CodeStatement[0]),new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeThisReferenceExpression(),"Advance"),new CodeExpression[]{new CodeArgumentReferenceExpression("span"),new CodeDirectionExpression(FieldDirection.Ref,
new CodeVariableReferenceExpression("ch")),new CodeDirectionExpression(FieldDirection.Ref,new CodeVariableReferenceExpression("len")),new CodePrimitiveExpression(false)})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodePrimitiveExpression(0)),new CodeGotoStatement("start_be_dfa")},new CodeStatement[0]),
new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference("FAMatch")),
"Create"),new CodeExpression[]{new CodeVariableReferenceExpression("acc"),new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeArgumentReferenceExpression("span"),"Slice"),new CodeExpression[]{new CodeCastExpression(new CodeTypeReference(typeof(int)),
new CodeVariableReferenceExpression("cursor_pos")),new CodeVariableReferenceExpression("len")}),"ToString"),new CodeExpression[0]),new CodeVariableReferenceExpression("cursor_pos"),
new CodeVariableReferenceExpression("line"),new CodeVariableReferenceExpression("column")}))},new CodeStatement[0]),new CodeIterationStatement(new CodeSnippetStatement(""),
new CodeBinaryOperatorExpression(new CodePrimitiveExpression(false),CodeBinaryOperatorType.ValueEquality,new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("ch"),
CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-1))),new CodeSnippetStatement(""),new CodeStatement[]{new CodeVariableDeclarationStatement(new
 CodeTypeReference(typeof(bool)),"moved",new CodePrimitiveExpression(false)),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodePrimitiveExpression(1)),
new CodeAssignStatement(new CodeVariableReferenceExpression("tlen"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeIterationStatement(new CodeAssignStatement(new
 CodeVariableReferenceExpression("i"),new CodePrimitiveExpression(0)),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),CodeBinaryOperatorType.LessThan,
new CodeVariableReferenceExpression("tlen")),new CodeAssignStatement(new CodeVariableReferenceExpression("i"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),
CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new
 CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeAssignStatement(new
 CodeVariableReferenceExpression("prlen"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new
 CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeIterationStatement(new CodeAssignStatement(new
 CodeVariableReferenceExpression("j"),new CodePrimitiveExpression(0)),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.LessThan,
new CodeVariableReferenceExpression("prlen")),new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),
CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("pmin"),new
 CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("pmax"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeConditionStatement(new
 CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("ch"),CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("pmin")),new
 CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),
CodeBinaryOperatorType.Add,new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("prlen"),CodeBinaryOperatorType.Subtract,
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),CodeBinaryOperatorType.Multiply,
new CodePrimitiveExpression(2)))),new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeVariableReferenceExpression("prlen"))},new CodeStatement[]
{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("ch"),CodeBinaryOperatorType.LessThanOrEqual,new CodeVariableReferenceExpression("pmax")),
new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("moved"),new CodePrimitiveExpression(true))},new CodeStatement[0])})})}),
new CodeConditionStatement(new CodeVariableReferenceExpression("moved"),new CodeStatement[]{new CodeGotoStatement("break_loop")},new CodeStatement[0]),
new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(),"Advance"),new CodeExpression[]
{new CodeArgumentReferenceExpression("span"),new CodeDirectionExpression(FieldDirection.Ref,new CodeVariableReferenceExpression("ch")),new CodeDirectionExpression(FieldDirection.Ref,
new CodeVariableReferenceExpression("len")),new CodePrimitiveExpression(false)}))}),new CodeLabeledStatement("break_loop",new CodeSnippetStatement("")),
new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("len"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(0)),
new CodeStatement[]{new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new
 CodeTypeReference("FAMatch")),"Create"),new CodeExpression[]{new CodePrimitiveExpression(-2),new CodePrimitiveExpression(null),new CodePrimitiveExpression(0),
new CodePrimitiveExpression(0),new CodePrimitiveExpression(0)}))},new CodeStatement[0]),new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference("FAMatch")),"Create"),new CodeExpression[]{new CodePrimitiveExpression(-1),
new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeArgumentReferenceExpression("span"),
"Slice"),new CodeExpression[]{new CodeCastExpression(new CodeTypeReference(typeof(int)),new CodeVariableReferenceExpression("cursor_pos")),new CodeVariableReferenceExpression("len")}),
"ToString"),new CodeExpression[0]),new CodeVariableReferenceExpression("cursor_pos"),new CodeVariableReferenceExpression("line"),new CodeVariableReferenceExpression("column")}))},
new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],
null)},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._TypeDeclaration("FATextReaderDfaTableRunner",
true,false,false,false,true,(MemberAttributes.Final|MemberAttributes.Private),TypeAttributes.NotPublic,new CodeTypeParameter[0],new CodeTypeReference[]
{new CodeTypeReference("FATextReaderRunner")},new CodeTypeMember[]{DeslangedSpan._MemberField(new CodeTypeReference(new CodeTypeReference(typeof(int)),
1),"_dfa",null,MemberAttributes.Private,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberField(new
 CodeTypeReference(new CodeTypeReference(new CodeTypeReference(typeof(int)),1),1),"_blockEnds",null,MemberAttributes.Private,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._Constructor((MemberAttributes.Final|MemberAttributes.Public),
new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new CodeTypeReference(new CodeTypeReference(typeof(int)),1),"dfa")},new
 CodeExpression[0],new CodeExpression[0],new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"_dfa"),new CodeArgumentReferenceExpression("dfa")),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_blockEnds"),
new CodePrimitiveExpression(null))},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._Constructor((MemberAttributes.Final
|MemberAttributes.Public),new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new CodeTypeReference(new CodeTypeReference(typeof(int)),
1),"dfa"),new CodeParameterDeclarationExpression(new CodeTypeReference(new CodeTypeReference(new CodeTypeReference(typeof(int)),1),1),"blockEnds")},new
 CodeExpression[0],new CodeExpression[0],new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"_dfa"),new CodeArgumentReferenceExpression("dfa")),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_blockEnds"),
new CodeArgumentReferenceExpression("blockEnds"))},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],
null),DeslangedSpan._MemberMethod(new CodeTypeReference("FAMatch"),"NextMatch",(MemberAttributes.Override|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],
new CodeStatement[]{new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"tlen",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),
"tto",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"prlen",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),
"pmin",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"pmax",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),
"i",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"j",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),
"state",new CodePrimitiveExpression(0)),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"acc",null),new CodeExpressionStatement(new
 CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"capture"),"Clear"),new
 CodeExpression[0])),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),
CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-2)),new CodeStatement[]{new CodeExpressionStatement(new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeThisReferenceExpression(),"Advance"),new CodeExpression[0]))},new CodeStatement[0]),new CodeVariableDeclarationStatement(new
 CodeTypeReference(typeof(long)),"cursor_pos",new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position")),new CodeVariableDeclarationStatement(new
 CodeTypeReference(typeof(int)),"line",new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"line")),new CodeVariableDeclarationStatement(new
 CodeTypeReference(typeof(int)),"column",new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column")),new CodeLabeledStatement("start_dfa",
new CodeSnippetStatement("")),new CodeAssignStatement(new CodeVariableReferenceExpression("acc"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeAssignStatement(new
 CodeVariableReferenceExpression("tlen"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new
 CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeIterationStatement(new CodeAssignStatement(new
 CodeVariableReferenceExpression("i"),new CodePrimitiveExpression(0)),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),CodeBinaryOperatorType.LessThan,
new CodeVariableReferenceExpression("tlen")),new CodeAssignStatement(new CodeVariableReferenceExpression("i"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),
CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("tto"),new
 CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("prlen"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeIterationStatement(new
 CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodePrimitiveExpression(0)),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),
CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("prlen")),new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("pmin"),
new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("pmax"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeConditionStatement(new
 CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("pmin")),
new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),
CodeBinaryOperatorType.Add,new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("prlen"),CodeBinaryOperatorType.Subtract,
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),CodeBinaryOperatorType.Multiply,
new CodePrimitiveExpression(2)))),new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeVariableReferenceExpression("prlen"))},new CodeStatement[]
{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.LessThanOrEqual,
new CodeVariableReferenceExpression("pmax")),new CodeStatement[]{new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeThisReferenceExpression(),"Advance"),new CodeExpression[0])),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeVariableReferenceExpression("tto")),
new CodeGotoStatement("start_dfa")},new CodeStatement[0])})})}),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePrimitiveExpression(false),
CodeBinaryOperatorType.ValueEquality,new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("acc"),CodeBinaryOperatorType.ValueEquality,
new CodePrimitiveExpression(-1))),new CodeStatement[]{new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"sym",new CodeVariableReferenceExpression("acc")),
new CodeVariableDeclarationStatement(new CodeTypeReference(new CodeTypeReference(typeof(int)),1),"be",new CodePrimitiveExpression(null)),new CodeConditionStatement(new
 CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_blockEnds"),CodeBinaryOperatorType.IdentityInequality,
new CodePrimitiveExpression(null)),CodeBinaryOperatorType.BooleanAnd,new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_blockEnds"),"Length"),CodeBinaryOperatorType.GreaterThan,new CodeVariableReferenceExpression("acc"))),new CodeStatement[]
{new CodeAssignStatement(new CodeVariableReferenceExpression("be"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"_blockEnds"),new CodeExpression[]{new CodeVariableReferenceExpression("acc")}))},new CodeStatement[0]),new CodeConditionStatement(new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("be"),CodeBinaryOperatorType.IdentityInequality,new CodePrimitiveExpression(null)),new CodeStatement[]{new CodeAssignStatement(new
 CodeVariableReferenceExpression("state"),new CodePrimitiveExpression(0)),new CodeLabeledStatement("start_be_dfa",new CodeSnippetStatement("")),new CodeAssignStatement(new
 CodeVariableReferenceExpression("acc"),new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("be"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("tlen"),new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("be"),
new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeIterationStatement(new CodeAssignStatement(new
 CodeVariableReferenceExpression("i"),new CodePrimitiveExpression(0)),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),CodeBinaryOperatorType.LessThan,
new CodeVariableReferenceExpression("tlen")),new CodeAssignStatement(new CodeVariableReferenceExpression("i"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),
CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("tto"),new
 CodeArrayIndexerExpression(new CodeVariableReferenceExpression("be"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new
 CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new
 CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("prlen"),new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("be"),
new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeIterationStatement(new CodeAssignStatement(new
 CodeVariableReferenceExpression("j"),new CodePrimitiveExpression(0)),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.LessThan,
new CodeVariableReferenceExpression("prlen")),new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),
CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("pmin"),new
 CodeArrayIndexerExpression(new CodeVariableReferenceExpression("be"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new
 CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new
 CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("pmax"),new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("be"),
new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeConditionStatement(new CodeBinaryOperatorExpression(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("pmin")),
new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),
CodeBinaryOperatorType.Add,new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("prlen"),CodeBinaryOperatorType.Subtract,
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),CodeBinaryOperatorType.Multiply,
new CodePrimitiveExpression(2)))),new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeVariableReferenceExpression("prlen"))},new CodeStatement[]
{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.LessThanOrEqual,
new CodeVariableReferenceExpression("pmax")),new CodeStatement[]{new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeThisReferenceExpression(),"Advance"),new CodeExpression[0])),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeVariableReferenceExpression("tto")),
new CodeGotoStatement("start_be_dfa")},new CodeStatement[0])})})}),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePrimitiveExpression(false),
CodeBinaryOperatorType.ValueEquality,new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("acc"),CodeBinaryOperatorType.ValueEquality,
new CodePrimitiveExpression(-1))),new CodeStatement[]{new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeTypeReferenceExpression(new CodeTypeReference("FAMatch")),"Create"),new CodeExpression[]{new CodeVariableReferenceExpression("sym"),new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"capture"),"ToString"),new CodeExpression[0]),new CodeVariableReferenceExpression("cursor_pos"),
new CodeVariableReferenceExpression("line"),new CodeVariableReferenceExpression("column")}))},new CodeStatement[0]),new CodeConditionStatement(new CodeBinaryOperatorExpression(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-1)),new CodeStatement[]
{new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference("FAMatch")),
"Create"),new CodeExpression[]{new CodePrimitiveExpression(-1),new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"capture"),"ToString"),new CodeExpression[0]),new CodeVariableReferenceExpression("cursor_pos"),new CodeVariableReferenceExpression("line"),
new CodeVariableReferenceExpression("column")}))},new CodeStatement[0]),new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeThisReferenceExpression(),"Advance"),new CodeExpression[0])),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodePrimitiveExpression(0)),
new CodeGotoStatement("start_be_dfa")},new CodeStatement[0]),new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeTypeReferenceExpression(new CodeTypeReference("FAMatch")),"Create"),new CodeExpression[]{new CodeVariableReferenceExpression("acc"),new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"capture"),"ToString"),new CodeExpression[0]),new CodeVariableReferenceExpression("cursor_pos"),
new CodeVariableReferenceExpression("line"),new CodeVariableReferenceExpression("column")}))},new CodeStatement[0]),new CodeIterationStatement(new CodeSnippetStatement(""),
new CodeBinaryOperatorExpression(new CodePrimitiveExpression(false),CodeBinaryOperatorType.ValueEquality,new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-1))),new CodeSnippetStatement(""),new CodeStatement[]
{new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(bool)),"moved",new CodePrimitiveExpression(false)),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodePrimitiveExpression(1)),new CodeAssignStatement(new CodeVariableReferenceExpression("tlen"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeIterationStatement(new
 CodeAssignStatement(new CodeVariableReferenceExpression("i"),new CodePrimitiveExpression(0)),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),
CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("tlen")),new CodeAssignStatement(new CodeVariableReferenceExpression("i"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("i"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeAssignStatement(new
 CodeVariableReferenceExpression("prlen"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new
 CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeIterationStatement(new CodeAssignStatement(new
 CodeVariableReferenceExpression("j"),new CodePrimitiveExpression(0)),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.LessThan,
new CodeVariableReferenceExpression("prlen")),new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),
CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("pmin"),new
 CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("pmax"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeConditionStatement(new
 CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("pmin")),
new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),
CodeBinaryOperatorType.Add,new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("prlen"),CodeBinaryOperatorType.Subtract,
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),CodeBinaryOperatorType.Multiply,
new CodePrimitiveExpression(2)))),new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeVariableReferenceExpression("prlen"))},new CodeStatement[]
{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.LessThanOrEqual,
new CodeVariableReferenceExpression("pmax")),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("moved"),new CodePrimitiveExpression(true))},
new CodeStatement[0])})})}),new CodeConditionStatement(new CodeVariableReferenceExpression("moved"),new CodeStatement[]{new CodeGotoStatement("break_loop")},
new CodeStatement[0]),new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(),"Advance"),
new CodeExpression[0]))}),new CodeLabeledStatement("break_loop",new CodeSnippetStatement("")),new CodeConditionStatement(new CodeBinaryOperatorExpression(new
 CodePropertyReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"capture"),"Length"),CodeBinaryOperatorType.ValueEquality,
new CodePrimitiveExpression(0)),new CodeStatement[]{new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeTypeReferenceExpression(new CodeTypeReference("FAMatch")),"Create"),new CodeExpression[]{new CodePrimitiveExpression(-2),new CodePrimitiveExpression(null),
new CodePrimitiveExpression(0),new CodePrimitiveExpression(0),new CodePrimitiveExpression(0)}))},new CodeStatement[0]),new CodeMethodReturnStatement(new
 CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference("FAMatch")),"Create"),new CodeExpression[]
{new CodePrimitiveExpression(-1),new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"capture"),"ToString"),new CodeExpression[0]),new CodeVariableReferenceExpression("cursor_pos"),new CodeVariableReferenceExpression("line"),new CodeVariableReferenceExpression("column")}))},
new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],
null)},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null)},new CodeCommentStatement[0])},new CodeAttributeDeclaration[0],
new CodeDirective[0],new CodeDirective[0]);}}public static System.CodeDom.CodeCompileUnit ReadOnlySpan{get{return DeslangedSpan._CompileUnit(new string[0],
new CodeNamespace[]{DeslangedSpan._Namespace("",new CodeNamespaceImport[0],new CodeTypeDeclaration[0],new CodeCommentStatement[0]),DeslangedSpan._Namespace("System",
new CodeNamespaceImport[0],new CodeTypeDeclaration[]{DeslangedSpan._TypeDeclaration("ReadOnlySpan",true,false,false,false,false,(MemberAttributes.Final
|MemberAttributes.Private),TypeAttributes.NotPublic,new CodeTypeParameter[]{new CodeTypeParameter("T")},new CodeTypeReference[0],new CodeTypeMember[]{
DeslangedSpan._MemberMethod(new CodeTypeReference(typeof(string)),"ToString",(MemberAttributes.Override|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],
new CodeStatement[]{new CodeMethodReturnStatement(new CodePrimitiveExpression(null))},new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedSpan._MemberProperty(new CodeTypeReference(typeof(int)),"Length",
(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeMethodReturnStatement(new CodePrimitiveExpression(0))},
new CodeStatement[0],new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],
null),DeslangedSpan._MemberMethod(new CodeTypeReference("ReadOnlySpan`1",new CodeTypeReference[]{new CodeTypeReference("T")}),"Slice",(MemberAttributes.Final
|MemberAttributes.Public),new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)),"position"),
new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)),"length")},new CodeStatement[]{new CodeMethodReturnStatement(new CodePrimitiveExpression(null))},
new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],
null)},new CodeCommentStatement[]{new CodeCommentStatement(" dummy for DNF so Slang can compile")},new CodeAttributeDeclaration[0],new CodeDirective[0],
new CodeDirective[0],null)},new CodeCommentStatement[0])},new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0]);}}}}namespace VisualFA
{using System.CodeDom;using System.Reflection;internal partial class DeslangedString{private static CodeCompileUnit _CompileUnit(string[]referencedAssemblies,
CodeNamespace[]namespaces,CodeAttributeDeclaration[]assemblyCustomAttributes,CodeDirective[]startDirectives,CodeDirective[]endDirectives){CodeCompileUnit
 result=new CodeCompileUnit();result.ReferencedAssemblies.AddRange(referencedAssemblies);result.Namespaces.AddRange(namespaces);result.AssemblyCustomAttributes.AddRange(assemblyCustomAttributes);
result.StartDirectives.AddRange(startDirectives);result.EndDirectives.AddRange(endDirectives);return result;}private static CodeNamespace _Namespace(string
 name,CodeNamespaceImport[]imports,CodeTypeDeclaration[]types,CodeCommentStatement[]comments){CodeNamespace result=new CodeNamespace();result.Name=name;
result.Imports.AddRange(imports);result.Types.AddRange(types);result.Comments.AddRange(comments);return result;}private static CodeTypeDeclaration _TypeDeclaration(
string name,bool isClass,bool isEnum,bool isInterface,bool isStruct,bool isPartial,MemberAttributes attributes,TypeAttributes typeAttributes,CodeTypeParameter[]
typeParameters,CodeTypeReference[]baseTypes,CodeTypeMember[]members,CodeCommentStatement[]comments,CodeAttributeDeclaration[]customAttributes,CodeDirective[]
startDirectives,CodeDirective[]endDirectives,CodeLinePragma linePragma){CodeTypeDeclaration result=new CodeTypeDeclaration(name);result.IsClass=isClass;
result.IsEnum=isEnum;result.IsInterface=isInterface;result.IsStruct=isStruct;result.IsPartial=isPartial;result.Attributes=attributes;result.TypeAttributes
=typeAttributes;result.TypeParameters.AddRange(typeParameters);result.BaseTypes.AddRange(baseTypes);result.Members.AddRange(members);result.Comments.AddRange(comments);
result.CustomAttributes.AddRange(customAttributes);result.StartDirectives.AddRange(startDirectives);result.EndDirectives.AddRange(endDirectives);result.LinePragma
=linePragma;return result;}private static CodeMemberField _MemberField(CodeTypeReference type,string name,CodeExpression initExpression,MemberAttributes
 attributes,CodeCommentStatement[]comments,CodeAttributeDeclaration[]customAttributes,CodeDirective[]startDirectives,CodeDirective[]endDirectives,CodeLinePragma
 linePragma){CodeMemberField result=new CodeMemberField(type,name);result.InitExpression=initExpression;result.Attributes=attributes;result.Comments.AddRange(comments);
result.CustomAttributes.AddRange(customAttributes);result.StartDirectives.AddRange(startDirectives);result.EndDirectives.AddRange(endDirectives);result.LinePragma
=linePragma;return result;}private static CodeMemberProperty _MemberProperty(CodeTypeReference type,string name,MemberAttributes attributes,CodeParameterDeclarationExpression[]
parameters,CodeStatement[]getStatements,CodeStatement[]setStatements,CodeTypeReference[]implementationTypes,CodeTypeReference privateImplementationType,
CodeCommentStatement[]comments,CodeAttributeDeclaration[]customAttributes,CodeDirective[]startDirectives,CodeDirective[]endDirectives,CodeLinePragma linePragma)
{CodeMemberProperty result=new CodeMemberProperty();result.Type=type;result.Name=name;result.Attributes=attributes;result.Parameters.AddRange(parameters);
result.GetStatements.AddRange(getStatements);result.SetStatements.AddRange(setStatements);result.ImplementationTypes.AddRange(implementationTypes);result.PrivateImplementationType
=privateImplementationType;result.Comments.AddRange(comments);result.CustomAttributes.AddRange(customAttributes);result.StartDirectives.AddRange(startDirectives);
result.EndDirectives.AddRange(endDirectives);result.LinePragma=linePragma;return result;}private static CodeMemberMethod _MemberMethod(CodeTypeReference
 returnType,string name,MemberAttributes attributes,CodeParameterDeclarationExpression[]parameters,CodeStatement[]statements,CodeTypeReference[]implementationTypes,
CodeTypeReference privateImplementationType,CodeCommentStatement[]comments,CodeAttributeDeclaration[]customAttributes,CodeAttributeDeclaration[]returnTypeCustomAttributes,
CodeDirective[]startDirectives,CodeDirective[]endDirectives,CodeLinePragma linePragma){CodeMemberMethod result=new CodeMemberMethod();result.ReturnType
=returnType;result.Name=name;result.Attributes=attributes;result.Parameters.AddRange(parameters);result.Statements.AddRange(statements);result.ImplementationTypes.AddRange(implementationTypes);
result.PrivateImplementationType=privateImplementationType;result.Comments.AddRange(comments);result.CustomAttributes.AddRange(customAttributes);result.ReturnTypeCustomAttributes.AddRange(returnTypeCustomAttributes);
result.StartDirectives.AddRange(startDirectives);result.EndDirectives.AddRange(endDirectives);result.LinePragma=linePragma;return result;}public static
 System.CodeDom.CodeCompileUnit FAMatch{get{return DeslangedString._CompileUnit(new string[]{"System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"},
new CodeNamespace[]{DeslangedString._Namespace("",new CodeNamespaceImport[0],new CodeTypeDeclaration[]{DeslangedString._TypeDeclaration("FAMatch",false,
false,false,true,true,(MemberAttributes.Final|MemberAttributes.Private),TypeAttributes.NotPublic,new CodeTypeParameter[0],new CodeTypeReference[0],new
 CodeTypeMember[]{DeslangedString._MemberField(new CodeTypeReference(typeof(int)),"_symbolId",null,MemberAttributes.Private,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberField(new CodeTypeReference(typeof(string)),"_value",
null,MemberAttributes.Private,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberField(new
 CodeTypeReference(typeof(long)),"_position",null,MemberAttributes.Private,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],
new CodeDirective[0],null),DeslangedString._MemberField(new CodeTypeReference(typeof(int)),"_line",null,MemberAttributes.Private,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberField(new CodeTypeReference(typeof(int)),"_column",
null,MemberAttributes.Private,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberProperty(new
 CodeTypeReference(typeof(int)),"SymbolId",(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]
{new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_symbolId"))},new CodeStatement[0],new CodeTypeReference[0],
null,new CodeCommentStatement[]{new CodeCommentStatement(" <summary>",true),new CodeCommentStatement(" The matched symbol - this is the accept id",true),
new CodeCommentStatement(" </summary>",true)},new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberProperty(new
 CodeTypeReference(typeof(string)),"Value",(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]
{new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_value"))},new CodeStatement[0],new CodeTypeReference[0],
null,new CodeCommentStatement[]{new CodeCommentStatement(" <summary>",true),new CodeCommentStatement(" The matched value",true),new CodeCommentStatement(" </summary>",
true)},new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberProperty(new CodeTypeReference(typeof(long)),
"Position",(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeMethodReturnStatement(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_position"))},new CodeStatement[0],new CodeTypeReference[0],null,new CodeCommentStatement[]
{new CodeCommentStatement(" <summary>",true),new CodeCommentStatement(" The position of the match within the input",true),new CodeCommentStatement(" </summary>",
true)},new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberProperty(new CodeTypeReference(typeof(int)),
"Line",(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeMethodReturnStatement(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_line"))},new CodeStatement[0],new CodeTypeReference[0],null,new CodeCommentStatement[]
{new CodeCommentStatement(" <summary>",true),new CodeCommentStatement(" The one based line number",true),new CodeCommentStatement(" </summary>",true)},
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberProperty(new CodeTypeReference(typeof(int)),"Column",
(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_column"))},new CodeStatement[0],new CodeTypeReference[0],null,new CodeCommentStatement[]{new CodeCommentStatement(" <summary>",
true),new CodeCommentStatement(" The one based column",true),new CodeCommentStatement(" </summary>",true)},new CodeAttributeDeclaration[0],new CodeDirective[0],
new CodeDirective[0],null),DeslangedString._MemberProperty(new CodeTypeReference(typeof(bool)),"IsSuccess",(MemberAttributes.Final|MemberAttributes.Public),
new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeMethodReturnStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_symbolId"),CodeBinaryOperatorType.GreaterThan,new CodePrimitiveExpression(-1)))},new CodeStatement[0],new CodeTypeReference[0],
null,new CodeCommentStatement[]{new CodeCommentStatement(" <summary>",true),new CodeCommentStatement(" Indicates whether the text matched the expression",
true),new CodeCommentStatement(" </summary>",true),new CodeCommentStatement(" <remarks>Non matches are returned with negative accept symbols. You can use this"
+" property to determine if the text therein was part of a match.</remarks>",true)},new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],
null),DeslangedString._MemberMethod(new CodeTypeReference(typeof(string)),"ToString",(MemberAttributes.Override|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],
new CodeStatement[]{new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(System.Text.StringBuilder)),"sb",new CodeObjectCreateExpression(new
 CodeTypeReference(typeof(System.Text.StringBuilder)),new CodeExpression[0])),new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeVariableReferenceExpression("sb"),"Append"),new CodeExpression[]{new CodePrimitiveExpression("[SymbolId: ")})),new CodeExpressionStatement(new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),"Append"),new CodeExpression[]{new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),
"SymbolId")})),new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),
"Append"),new CodeExpression[]{new CodePrimitiveExpression(", Value: ")})),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new
 CodeThisReferenceExpression(),"Value"),CodeBinaryOperatorType.IdentityInequality,new CodePrimitiveExpression(null)),new CodeStatement[]{new CodeExpressionStatement(new
 CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),"Append"),new CodeExpression[]{new CodePrimitiveExpression("\"")})),
new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),"Append"),new CodeExpression[]
{new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),
"Value"),"Replace"),new CodeExpression[]{new CodePrimitiveExpression("\r"),new CodePrimitiveExpression("\\r")}),"Replace"),new CodeExpression[]{new CodePrimitiveExpression("\t"),
new CodePrimitiveExpression("\\t")}),"Replace"),new CodeExpression[]{new CodePrimitiveExpression("\n"),new CodePrimitiveExpression("\\n")}),"Replace"),
new CodeExpression[]{new CodePrimitiveExpression(""),new CodePrimitiveExpression("\\v")})})),new CodeExpressionStatement(new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),"Append"),new CodeExpression[]{new CodePrimitiveExpression("\", Position: ")}))},
new CodeStatement[]{new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),
"Append"),new CodeExpression[]{new CodePrimitiveExpression("null, Position: ")}))}),new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeVariableReferenceExpression("sb"),"Append"),new CodeExpression[]{new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),"Position")})),
new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),"Append"),new CodeExpression[]
{new CodePrimitiveExpression(" (")})),new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),
"Append"),new CodeExpression[]{new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),"Line")})),new CodeExpressionStatement(new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),"Append"),new CodeExpression[]{new CodePrimitiveExpression(", ")})),new CodeExpressionStatement(new
 CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),"Append"),new CodeExpression[]{new CodePropertyReferenceExpression(new
 CodeThisReferenceExpression(),"Column")})),new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("sb"),
"Append"),new CodeExpression[]{new CodePrimitiveExpression(")]")})),new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeVariableReferenceExpression("sb"),"ToString"),new CodeExpression[0]))},new CodeTypeReference[0],null,new CodeCommentStatement[]{new CodeCommentStatement(" <summary>",
true),new CodeCommentStatement(" Provides a string representation of the match",true),new CodeCommentStatement(" </summary>",true),new CodeCommentStatement(" <returns>A string containing match information</returns>",
true)},new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberMethod(new
 CodeTypeReference("FAMatch"),"Create",(MemberAttributes.Static|MemberAttributes.Public),new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new
 CodeTypeReference(typeof(int)),"symbolId"),new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)),"value"),new CodeParameterDeclarationExpression(new
 CodeTypeReference(typeof(long)),"position"),new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)),"line"),new CodeParameterDeclarationExpression(new
 CodeTypeReference(typeof(int)),"column")},new CodeStatement[]{new CodeVariableDeclarationStatement(new CodeTypeReference("FAMatch"),"result",new CodeDefaultValueExpression(new
 CodeTypeReference("FAMatch"))),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("result"),"_symbolId"),new
 CodeArgumentReferenceExpression("symbolId")),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("result"),"_value"),
new CodeArgumentReferenceExpression("value")),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("result"),"_position"),
new CodeArgumentReferenceExpression("position")),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("result"),
"_line"),new CodeArgumentReferenceExpression("line")),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("result"),
"_column"),new CodeArgumentReferenceExpression("column")),new CodeMethodReturnStatement(new CodeVariableReferenceExpression("result"))},new CodeTypeReference[0],
null,new CodeCommentStatement[]{new CodeCommentStatement(" <summary>",true),new CodeCommentStatement(" Constructs a new instance",true),new CodeCommentStatement(" </summary>",
true),new CodeCommentStatement(" <param name=\"symbolId\">The symbol id</param>",true),new CodeCommentStatement(" <param name=\"value\">The matched value</param>",
true),new CodeCommentStatement(" <param name=\"position\">The match position</param>",true),new CodeCommentStatement(" <param name=\"line\">The line</param>",
true),new CodeCommentStatement(" <param name=\"column\">The column</param>",true)},new CodeAttributeDeclaration[]{new CodeAttributeDeclaration(new CodeTypeReference(typeof(System.Runtime.CompilerServices.MethodImplAttribute)),
new CodeAttributeArgument[]{new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(typeof(System.Runtime.CompilerServices.MethodImplOptions))),
"AggressiveInlining"))})},new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null)},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],
new CodeDirective[0],new CodeDirective[0],null)},new CodeCommentStatement[]{new CodeCommentStatement(" <summary>",true),new CodeCommentStatement(" Represents a match from <see cref=\"FARunner\"/></code>",
true),new CodeCommentStatement(" </summary>",true)})},new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0]);}}private static CodeConstructor
 _Constructor(MemberAttributes attributes,CodeParameterDeclarationExpression[]parameters,CodeExpression[]chainedConstructorArgs,CodeExpression[]baseConstructorArgs,
CodeStatement[]statements,CodeCommentStatement[]comments,CodeAttributeDeclaration[]customAttributes,CodeDirective[]startDirectives,CodeDirective[]endDirectives,
CodeLinePragma linePragma){CodeConstructor result=new CodeConstructor();result.Attributes=attributes;result.Parameters.AddRange(parameters);result.ChainedConstructorArgs.AddRange(chainedConstructorArgs);
result.BaseConstructorArgs.AddRange(baseConstructorArgs);result.Statements.AddRange(statements);result.Comments.AddRange(comments);result.CustomAttributes.AddRange(customAttributes);
result.StartDirectives.AddRange(startDirectives);result.EndDirectives.AddRange(endDirectives);result.LinePragma=linePragma;return result;}private static
 CodeParameterDeclarationExpression _ParameterDeclarationExpression(CodeTypeReference type,string name,FieldDirection direction,CodeAttributeDeclaration[]
customAttributes){CodeParameterDeclarationExpression result=new CodeParameterDeclarationExpression(type,name);result.Direction=direction;result.CustomAttributes.AddRange(customAttributes);
return result;}public static System.CodeDom.CodeCompileUnit FARunnerString{get{return DeslangedString._CompileUnit(new string[0],new CodeNamespace[]{DeslangedString._Namespace("",
new CodeNamespaceImport[]{new CodeNamespaceImport("System"),new CodeNamespaceImport("System.Collections"),new CodeNamespaceImport("System.Collections.Generic"),
new CodeNamespaceImport("System.Runtime.CompilerServices"),new CodeNamespaceImport("System.IO"),new CodeNamespaceImport("System.Text")},new CodeTypeDeclaration[]
{DeslangedString._TypeDeclaration("FARunner",true,false,false,false,true,(MemberAttributes.Final|MemberAttributes.Private),(((TypeAttributes.AutoLayout
|TypeAttributes.AnsiClass)|TypeAttributes.Class)|TypeAttributes.Abstract),new CodeTypeParameter[0],new CodeTypeReference[]{new CodeTypeReference("Object"),
new CodeTypeReference("IEnumerable`1",new CodeTypeReference[]{new CodeTypeReference("FAMatch")})},new CodeTypeMember[]{DeslangedString._Constructor(MemberAttributes.FamilyOrAssembly,
new CodeParameterDeclarationExpression[0],new CodeExpression[0],new CodeExpression[0],new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"position"),new CodePrimitiveExpression(-1)),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"line"),new CodePrimitiveExpression(1)),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),new CodePrimitiveExpression(1)),
new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_tabWidth"),new CodePrimitiveExpression(4))},new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._TypeDeclaration("Enumerator",true,false,false,false,false,
(MemberAttributes.Final|MemberAttributes.Private),(((TypeAttributes.AutoLayout|TypeAttributes.AnsiClass)|TypeAttributes.Class)|TypeAttributes.NestedPublic),
new CodeTypeParameter[0],new CodeTypeReference[]{new CodeTypeReference("Object"),new CodeTypeReference("IEnumerator`1",new CodeTypeReference[]{new CodeTypeReference("FAMatch")})},
new CodeTypeMember[]{DeslangedString._MemberField(new CodeTypeReference(typeof(int)),"_state",null,MemberAttributes.Private,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberField(new CodeTypeReference("FAMatch"),"_current",
null,MemberAttributes.Private,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberField(new
 CodeTypeReference("WeakReference`1",new CodeTypeReference[]{new CodeTypeReference("FARunner")}),"_parent",null,MemberAttributes.Private,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._Constructor((MemberAttributes.Final|MemberAttributes.
Public),new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new CodeTypeReference("FARunner"),"parent")},new CodeExpression[0],
new CodeExpression[0],new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_parent"),new CodeObjectCreateExpression(new
 CodeTypeReference("WeakReference`1",new CodeTypeReference[]{new CodeTypeReference("FARunner")}),new CodeExpression[]{new CodeArgumentReferenceExpression("parent")})),
new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_state"),new CodePrimitiveExpression(-2))},new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberProperty(new CodeTypeReference("FAMatch"),"Current",
(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeConditionStatement(new CodeBinaryOperatorExpression(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_state"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-3)),new CodeStatement[]
{new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference("ObjectDisposedException"),new CodeExpression[]{new CodePrimitiveExpression("Enumerator")}))},
new CodeStatement[0]),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_state"),
CodeBinaryOperatorType.LessThan,new CodePrimitiveExpression(0)),new CodeStatement[]{new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new
 CodeTypeReference("InvalidOperationException"),new CodeExpression[]{new CodePrimitiveExpression("The enumerator is not positioned on an element")}))},
new CodeStatement[0]),new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_current"))},new CodeStatement[0],
new CodeTypeReference[]{new CodeTypeReference("IEnumerator`1",new CodeTypeReference[]{new CodeTypeReference("FAMatch")})},null,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberMethod(new CodeTypeReference(typeof(bool)),"MoveNext",
((MemberAttributes)(0)),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeThisReferenceExpression(),"MoveNext"),new CodeExpression[0]))},new CodeTypeReference[0],new CodeTypeReference(typeof(System.Collections.IEnumerator)),
new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberProperty(new
 CodeTypeReference(typeof(object)),"Current",((MemberAttributes)(0)),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeMethodReturnStatement(new
 CodePropertyReferenceExpression(new CodeThisReferenceExpression(),"Current"))},new CodeStatement[0],new CodeTypeReference[0],new CodeTypeReference(typeof(System.Collections.IEnumerator)),
new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberMethod(new CodeTypeReference(typeof(void)),
"Reset",((MemberAttributes)(0)),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeExpressionStatement(new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeThisReferenceExpression(),"Reset"),new CodeExpression[0]))},new CodeTypeReference[0],new CodeTypeReference(typeof(System.Collections.IEnumerator)),
new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberMethod(new
 CodeTypeReference(typeof(void)),"Dispose",((MemberAttributes)(0)),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeAssignStatement(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_state"),new CodePrimitiveExpression(-3))},new CodeTypeReference[0],new CodeTypeReference("IDisposable"),
new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberMethod(new
 CodeTypeReference(typeof(bool)),"MoveNext",(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]
{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_state"),CodeBinaryOperatorType.ValueEquality,
new CodePrimitiveExpression(-3)),new CodeStatement[]{new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference("ObjectDisposedException"),
new CodeExpression[]{new CodePrimitiveExpression("Enumerator")}))},new CodeStatement[0]),new CodeConditionStatement(new CodeBinaryOperatorExpression(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_state"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-1)),new CodeStatement[]
{new CodeMethodReturnStatement(new CodePrimitiveExpression(false))},new CodeStatement[0]),new CodeAssignStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_state"),new CodePrimitiveExpression(0)),new CodeVariableDeclarationStatement(new CodeTypeReference("FARunner"),"parent",
null),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePrimitiveExpression(false),CodeBinaryOperatorType.ValueEquality,new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_parent"),"TryGetTarget"),new CodeExpression[]{new CodeDirectionExpression(FieldDirection.Out,
new CodeVariableReferenceExpression("parent"))})),new CodeStatement[]{new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference("InvalidOperationException"),
new CodeExpression[]{new CodePrimitiveExpression("The parent was destroyed")}))},new CodeStatement[0]),new CodeAssignStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_current"),new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("parent"),
"NextMatch"),new CodeExpression[0])),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_current"),"SymbolId"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-2)),new CodeStatement[]{new CodeAssignStatement(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_state"),new CodePrimitiveExpression(-2)),new CodeMethodReturnStatement(new CodePrimitiveExpression(false))},
new CodeStatement[0]),new CodeMethodReturnStatement(new CodePrimitiveExpression(true))},new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberMethod(new CodeTypeReference(typeof(void)),"Reset",
(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeConditionStatement(new CodeBinaryOperatorExpression(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_state"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-3)),new CodeStatement[]
{new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference("ObjectDisposedException"),new CodeExpression[]{new CodePrimitiveExpression("Enumerator")}))},
new CodeStatement[0]),new CodeVariableDeclarationStatement(new CodeTypeReference("FARunner"),"parent",null),new CodeConditionStatement(new CodeBinaryOperatorExpression(new
 CodePrimitiveExpression(false),CodeBinaryOperatorType.ValueEquality,new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_parent"),"TryGetTarget"),new CodeExpression[]{new CodeDirectionExpression(FieldDirection.Out,new CodeVariableReferenceExpression("parent"))})),
new CodeStatement[]{new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference("InvalidOperationException"),new CodeExpression[]
{new CodePrimitiveExpression("The parent was destroyed")}))},new CodeStatement[0]),new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeVariableReferenceExpression("parent"),"Reset"),new CodeExpression[0])),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"_state"),new CodePrimitiveExpression(-2))},new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],
new CodeDirective[0],new CodeDirective[0],null)},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],
null),DeslangedString._MemberProperty(new CodeTypeReference(typeof(int)),"TabWidth",(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],
new CodeStatement[]{new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_tabWidth"))},new CodeStatement[]
{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePropertySetValueReferenceExpression(),CodeBinaryOperatorType.LessThan,new CodePrimitiveExpression(1)),
new CodeStatement[]{new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference("ArgumentOutOfRangeException"),new CodeExpression[0]))},
new CodeStatement[0]),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_tabWidth"),new CodePropertySetValueReferenceExpression())},
new CodeTypeReference[0],null,new CodeCommentStatement[]{new CodeCommentStatement(" <summary>",true),new CodeCommentStatement(" Indicates the width of a tab, in columns",
true),new CodeCommentStatement(" </summary>",true)},new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberField(new
 CodeTypeReference(typeof(int)),"_tabWidth",null,MemberAttributes.Private,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],
new CodeDirective[0],null),DeslangedString._MemberField(new CodeTypeReference(typeof(int)),"position",null,MemberAttributes.Family,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberField(new CodeTypeReference(typeof(int)),"line",
null,MemberAttributes.Family,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberField(new
 CodeTypeReference(typeof(int)),"column",null,MemberAttributes.Family,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],
new CodeDirective[0],null),DeslangedString._MemberMethod(new CodeTypeReference(typeof(void)),"ThrowUnicode",(MemberAttributes.Static|MemberAttributes.
Family),new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)),"pos")},new CodeStatement[]
{new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference("IOException"),new CodeExpression[]{new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(typeof(string))),"Concat"),new CodeExpression[]{new CodePrimitiveExpression("Unicode error in stream at position "),
new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeArgumentReferenceExpression("pos"),"ToString"),new CodeExpression[0])})}))},new
 CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],
null),DeslangedString._MemberMethod(new CodeTypeReference("FAMatch"),"NextMatch",(MemberAttributes.Abstract|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],
new CodeStatement[0],new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],
new CodeDirective[0],null),DeslangedString._MemberMethod(new CodeTypeReference(typeof(void)),"Reset",(MemberAttributes.Abstract|MemberAttributes.Public),
new CodeParameterDeclarationExpression[0],new CodeStatement[0],new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberMethod(new CodeTypeReference("IEnumerator`1",new
 CodeTypeReference[]{new CodeTypeReference("FAMatch")}),"GetEnumerator",(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],
new CodeStatement[]{new CodeMethodReturnStatement(new CodeObjectCreateExpression(new CodeTypeReference("Enumerator"),new CodeExpression[]{new CodeThisReferenceExpression()}))},
new CodeTypeReference[]{new CodeTypeReference("IEnumerable`1",new CodeTypeReference[]{new CodeTypeReference("FAMatch")})},null,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberMethod(new CodeTypeReference(typeof(System.Collections.IEnumerator)),
"GetEnumerator",((MemberAttributes)(0)),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeMethodReturnStatement(new CodeObjectCreateExpression(new
 CodeTypeReference("Enumerator"),new CodeExpression[]{new CodeThisReferenceExpression()}))},new CodeTypeReference[0],new CodeTypeReference(typeof(System.Collections.IEnumerable)),
new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null)},new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._TypeDeclaration("FAStringRunner",true,false,false,false,
true,(MemberAttributes.Final|MemberAttributes.Private),(((TypeAttributes.AutoLayout|TypeAttributes.AnsiClass)|TypeAttributes.Class)|TypeAttributes.Abstract),
new CodeTypeParameter[0],new CodeTypeReference[]{new CodeTypeReference("FARunner")},new CodeTypeMember[]{DeslangedString._MemberProperty(new CodeTypeReference(typeof(bool)),
"UsingSpans",(MemberAttributes.Static|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeMethodReturnStatement(new
 CodePrimitiveExpression(false))},new CodeStatement[0],new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],
new CodeDirective[0],null),DeslangedString._MemberField(new CodeTypeReference(typeof(string)),"string",null,MemberAttributes.Family,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberMethod(new CodeTypeReference(typeof(void)),"Set",
(MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)),
"string")},new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"string"),new CodeArgumentReferenceExpression("string")),
new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),new CodePrimitiveExpression(-1)),new CodeAssignStatement(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"line"),new CodePrimitiveExpression(1)),new CodeAssignStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"column"),new CodePrimitiveExpression(1))},new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberMethod(new CodeTypeReference(typeof(void)),"Reset",
(MemberAttributes.Override|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"position"),new CodePrimitiveExpression(-1)),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"line"),new CodePrimitiveExpression(1)),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),new CodePrimitiveExpression(1))},
new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],
null),DeslangedString._MemberMethod(new CodeTypeReference(typeof(void)),"Advance",MemberAttributes.Family,new CodeParameterDeclarationExpression[]{new
 CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)),"str"),DeslangedString._ParameterDeclarationExpression(new CodeTypeReference(typeof(int)),
"ch",FieldDirection.Ref,new CodeAttributeDeclaration[0]),DeslangedString._ParameterDeclarationExpression(new CodeTypeReference(typeof(int)),"len",FieldDirection.Ref,
new CodeAttributeDeclaration[0]),new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(bool)),"first")},new CodeStatement[]{new CodeConditionStatement(new
 CodeBinaryOperatorExpression(new CodePrimitiveExpression(false),CodeBinaryOperatorType.ValueEquality,new CodeArgumentReferenceExpression("first")),new
 CodeStatement[]{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("ch"),CodeBinaryOperatorType.ValueEquality,
new CodePrimitiveExpression(10)),new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"line"),
new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"line"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),
new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),new CodePrimitiveExpression(1))},new CodeStatement[]
{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("ch"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(13)),
new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),new CodePrimitiveExpression(1))},
new CodeStatement[]{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("ch"),CodeBinaryOperatorType.ValueEquality,
new CodePrimitiveExpression(9)),new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),
new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"column"),CodeBinaryOperatorType.Subtract,new CodePrimitiveExpression(1)),CodeBinaryOperatorType.Divide,new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),
"TabWidth")),CodeBinaryOperatorType.Multiply,new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),"TabWidth"),
CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))))},new CodeStatement[]{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("ch"),
CodeBinaryOperatorType.GreaterThan,new CodePrimitiveExpression(31)),new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"column"),new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),CodeBinaryOperatorType.Add,new
 CodePrimitiveExpression(1)))},new CodeStatement[0])})})}),new CodeAssignStatement(new CodeArgumentReferenceExpression("len"),new CodeBinaryOperatorExpression(new
 CodeArgumentReferenceExpression("len"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeConditionStatement(new CodeBinaryOperatorExpression(new
 CodeArgumentReferenceExpression("ch"),CodeBinaryOperatorType.GreaterThan,new CodePrimitiveExpression(65535)),new CodeStatement[]{new CodeAssignStatement(new
 CodeArgumentReferenceExpression("len"),new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("len"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1)))},
new CodeStatement[0]),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),new CodeBinaryOperatorExpression(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1)))},new CodeStatement[0]),
new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),CodeBinaryOperatorType.LessThan,
new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("str"),"Length")),new CodeStatement[]{new CodeVariableDeclarationStatement(new
 CodeTypeReference(typeof(char)),"ch1",new CodeIndexerExpression(new CodeArgumentReferenceExpression("str"),new CodeExpression[]{new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"position")})),new CodeConditionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new
 CodeTypeReference(typeof(char))),"IsHighSurrogate"),new CodeExpression[]{new CodeVariableReferenceExpression("ch1")}),new CodeStatement[]{new CodeAssignStatement(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"position"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"position"),CodeBinaryOperatorType.GreaterThanOrEqual,new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("str"),
"Length")),new CodeStatement[]{new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new
 CodeTypeReference("FAStringRunner")),"ThrowUnicode"),new CodeExpression[]{new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position")}))},
new CodeStatement[0]),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(char)),"ch2",new CodeIndexerExpression(new CodeArgumentReferenceExpression("str"),
new CodeExpression[]{new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position")})),new CodeAssignStatement(new CodeArgumentReferenceExpression("ch"),
new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(typeof(char))),"ConvertToUtf32"),
new CodeExpression[]{new CodeVariableReferenceExpression("ch1"),new CodeVariableReferenceExpression("ch2")}))},new CodeStatement[]{new CodeAssignStatement(new
 CodeArgumentReferenceExpression("ch"),new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(typeof(System.Convert))),
"ToInt32"),new CodeExpression[]{new CodeVariableReferenceExpression("ch1")}))})},new CodeStatement[]{new CodeAssignStatement(new CodeArgumentReferenceExpression("ch"),
new CodePrimitiveExpression(-1))})},new CodeTypeReference[0],null,new CodeCommentStatement[]{new CodeCommentStatement(" much bigger, but faster code")},
new CodeAttributeDeclaration[]{new CodeAttributeDeclaration(new CodeTypeReference(typeof(System.Runtime.CompilerServices.MethodImplAttribute)),new CodeAttributeArgument[]
{new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(typeof(System.Runtime.CompilerServices.MethodImplOptions))),
"AggressiveInlining"))})},new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null)},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],
new CodeDirective[0],new CodeDirective[0],null),DeslangedString._TypeDeclaration("FATextReaderRunner",true,false,false,false,true,(MemberAttributes.Final
|MemberAttributes.Private),(((TypeAttributes.AutoLayout|TypeAttributes.AnsiClass)|TypeAttributes.Class)|TypeAttributes.Abstract),new CodeTypeParameter[0],
new CodeTypeReference[]{new CodeTypeReference("FARunner")},new CodeTypeMember[]{DeslangedString._MemberField(new CodeTypeReference("TextReader"),"reader",
null,MemberAttributes.Family,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberField(new
 CodeTypeReference("StringBuilder"),"capture",null,MemberAttributes.Family,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],
new CodeDirective[0],null),DeslangedString._MemberField(new CodeTypeReference(typeof(int)),"current",null,MemberAttributes.Family,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._Constructor(MemberAttributes.Family,new CodeParameterDeclarationExpression[0],
new CodeExpression[0],new CodeExpression[0],new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"capture"),new CodeObjectCreateExpression(new CodeTypeReference("StringBuilder"),new CodeExpression[0]))},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],
new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberMethod(new CodeTypeReference(typeof(void)),"Set",(MemberAttributes.Final|MemberAttributes.
Public),new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new CodeTypeReference("TextReader"),"reader")},new CodeStatement[]
{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"reader"),new CodeArgumentReferenceExpression("reader")),new
 CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),new CodePrimitiveExpression(-2)),new CodeAssignStatement(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),new CodePrimitiveExpression(-1)),new CodeAssignStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"line"),new CodePrimitiveExpression(1)),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"column"),new CodePrimitiveExpression(1))},new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],
new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberMethod(new CodeTypeReference(typeof(void)),"Reset",(MemberAttributes.Override|MemberAttributes.
Public),new CodeParameterDeclarationExpression[0],new CodeStatement[]{new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference("NotSupportedException"),
new CodeExpression[0]))},new CodeTypeReference[0],null,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new
 CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberMethod(new CodeTypeReference(typeof(void)),"Advance",MemberAttributes.Family,new CodeParameterDeclarationExpression[0],
new CodeStatement[]{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),
CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(10)),new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"line"),new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"line"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),new CodePrimitiveExpression(1))},
new CodeStatement[]{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),
CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(13)),new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"column"),new CodePrimitiveExpression(1))},new CodeStatement[]{new CodeConditionStatement(new CodeBinaryOperatorExpression(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(9)),new CodeStatement[]
{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new
 CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),CodeBinaryOperatorType.Subtract,new CodePrimitiveExpression(1)),
CodeBinaryOperatorType.Divide,new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),"TabWidth")),CodeBinaryOperatorType.Multiply,new CodeBinaryOperatorExpression(new
 CodePropertyReferenceExpression(new CodeThisReferenceExpression(),"TabWidth"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))))},new CodeStatement[]
{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.GreaterThan,
new CodePrimitiveExpression(31)),new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),
new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1)))},
new CodeStatement[0])})})}),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"current"),CodeBinaryOperatorType.GreaterThan,new CodePrimitiveExpression(-1)),new CodeStatement[]{new CodeExpressionStatement(new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"capture"),"Append"),new CodeExpression[]{new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(typeof(char))),"ConvertFromUtf32"),new CodeExpression[]{new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"current")})}))},new CodeStatement[0]),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"current"),new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"reader"),
"Read"),new CodeExpression[0])),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"current"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-1)),new CodeStatement[]{new CodeMethodReturnStatement(null)},new CodeStatement[0]),
new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"position"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(char)),
"ch1",new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(typeof(System.Convert))),
"ToChar"),new CodeExpression[]{new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current")})),new CodeConditionStatement(new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(typeof(char))),"IsHighSurrogate"),new CodeExpression[]{new CodeVariableReferenceExpression("ch1")}),
new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"reader"),"Read"),new CodeExpression[0])),new CodeConditionStatement(new
 CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-1)),
new CodeStatement[]{new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference("FATextReaderRunner")),
"ThrowUnicode"),new CodeExpression[]{new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position")}))},new CodeStatement[0]),new CodeVariableDeclarationStatement(new
 CodeTypeReference(typeof(char)),"ch2",new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(typeof(System.Convert))),
"ToChar"),new CodeExpression[]{new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current")})),new CodeAssignStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"current"),new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(typeof(char))),
"ConvertToUtf32"),new CodeExpression[]{new CodeVariableReferenceExpression("ch1"),new CodeVariableReferenceExpression("ch2")})),new CodeAssignStatement(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"position"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1)))},new CodeStatement[0])},new CodeTypeReference[0],null,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null)},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],
new CodeDirective[0],new CodeDirective[0],null)},new CodeCommentStatement[0])},new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0]);
}}public static System.CodeDom.CodeCompileUnit FADfaTableRunnerString{get{return DeslangedString._CompileUnit(new string[0],new CodeNamespace[]{DeslangedString._Namespace("",
new CodeNamespaceImport[]{new CodeNamespaceImport("System"),new CodeNamespaceImport("System.Collections"),new CodeNamespaceImport("System.Collections.Generic"),
new CodeNamespaceImport("System.Runtime.CompilerServices"),new CodeNamespaceImport("System.IO"),new CodeNamespaceImport("System.Text")},new CodeTypeDeclaration[]
{DeslangedString._TypeDeclaration("FAStringDfaTableRunner",true,false,false,false,true,(MemberAttributes.Final|MemberAttributes.Private),TypeAttributes.NotPublic,
new CodeTypeParameter[0],new CodeTypeReference[]{new CodeTypeReference("FAStringRunner")},new CodeTypeMember[]{DeslangedString._MemberField(new CodeTypeReference(new
 CodeTypeReference(typeof(int)),1),"_dfa",null,MemberAttributes.Private,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],
new CodeDirective[0],null),DeslangedString._MemberField(new CodeTypeReference(new CodeTypeReference(new CodeTypeReference(typeof(int)),1),1),"_blockEnds",
null,MemberAttributes.Private,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._Constructor((MemberAttributes.Final
|MemberAttributes.Public),new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new CodeTypeReference(new CodeTypeReference(typeof(int)),
1),"dfa")},new CodeExpression[0],new CodeExpression[0],new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"_dfa"),new CodeArgumentReferenceExpression("dfa")),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_blockEnds"),
new CodePrimitiveExpression(null))},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._Constructor((MemberAttributes.Final
|MemberAttributes.Public),new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new CodeTypeReference(new CodeTypeReference(typeof(int)),
1),"dfa"),new CodeParameterDeclarationExpression(new CodeTypeReference(new CodeTypeReference(new CodeTypeReference(typeof(int)),1),1),"blockEnds")},new
 CodeExpression[0],new CodeExpression[0],new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"_dfa"),new CodeArgumentReferenceExpression("dfa")),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_blockEnds"),
new CodeArgumentReferenceExpression("blockEnds"))},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],
null),DeslangedString._MemberMethod(new CodeTypeReference("FAMatch"),"NextMatch",(MemberAttributes.Override|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],
new CodeStatement[]{new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(),"_NextImpl"),
new CodeExpression[]{new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"string")}))},new CodeTypeReference[0],null,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberMethod(new CodeTypeReference("FAMatch"),
"_NextImpl",MemberAttributes.Private,new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)),
"str")},new CodeStatement[]{new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"tlen",null),new CodeVariableDeclarationStatement(new
 CodeTypeReference(typeof(int)),"tto",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"prlen",null),new CodeVariableDeclarationStatement(new
 CodeTypeReference(typeof(int)),"pmin",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"pmax",null),new CodeVariableDeclarationStatement(new
 CodeTypeReference(typeof(int)),"i",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"j",null),new CodeVariableDeclarationStatement(new
 CodeTypeReference(typeof(int)),"state",new CodePrimitiveExpression(0)),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"acc",
null),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),CodeBinaryOperatorType.ValueEquality,
new CodePrimitiveExpression(-1)),new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),
new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1)))},
new CodeStatement[0]),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"len",new CodePrimitiveExpression(0)),new CodeVariableDeclarationStatement(new
 CodeTypeReference(typeof(long)),"cursor_pos",new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position")),new CodeVariableDeclarationStatement(new
 CodeTypeReference(typeof(int)),"line",new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"line")),new CodeVariableDeclarationStatement(new
 CodeTypeReference(typeof(int)),"column",new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column")),new CodeVariableDeclarationStatement(new
 CodeTypeReference(typeof(int)),"ch",new CodePrimitiveExpression(-1)),new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeThisReferenceExpression(),"Advance"),new CodeExpression[]{new CodeArgumentReferenceExpression("str"),new CodeDirectionExpression(FieldDirection.Ref,
new CodeVariableReferenceExpression("ch")),new CodeDirectionExpression(FieldDirection.Ref,new CodeVariableReferenceExpression("len")),new CodePrimitiveExpression(true)})),
new CodeLabeledStatement("start_dfa",new CodeSnippetStatement("")),new CodeAssignStatement(new CodeVariableReferenceExpression("acc"),new CodeArrayIndexerExpression(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new
 CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new
 CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("tlen"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeIterationStatement(new
 CodeAssignStatement(new CodeVariableReferenceExpression("i"),new CodePrimitiveExpression(0)),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),
CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("tlen")),new CodeAssignStatement(new CodeVariableReferenceExpression("i"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("i"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("tto"),
new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("prlen"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeIterationStatement(new
 CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodePrimitiveExpression(0)),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),
CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("prlen")),new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("pmin"),
new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("pmax"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeConditionStatement(new
 CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("ch"),CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("pmin")),new
 CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),
CodeBinaryOperatorType.Add,new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("prlen"),CodeBinaryOperatorType.Subtract,
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),CodeBinaryOperatorType.Multiply,
new CodePrimitiveExpression(2)))),new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeVariableReferenceExpression("prlen"))},new CodeStatement[]
{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("ch"),CodeBinaryOperatorType.LessThanOrEqual,new CodeVariableReferenceExpression("pmax")),
new CodeStatement[]{new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(),"Advance"),
new CodeExpression[]{new CodeArgumentReferenceExpression("str"),new CodeDirectionExpression(FieldDirection.Ref,new CodeVariableReferenceExpression("ch")),
new CodeDirectionExpression(FieldDirection.Ref,new CodeVariableReferenceExpression("len")),new CodePrimitiveExpression(false)})),new CodeAssignStatement(new
 CodeVariableReferenceExpression("state"),new CodeVariableReferenceExpression("tto")),new CodeGotoStatement("start_dfa")},new CodeStatement[0])})})}),
new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePrimitiveExpression(false),CodeBinaryOperatorType.ValueEquality,new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("acc"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-1))),new CodeStatement[]{new CodeVariableDeclarationStatement(new
 CodeTypeReference(typeof(int)),"sym",new CodeVariableReferenceExpression("acc")),new CodeVariableDeclarationStatement(new CodeTypeReference(new CodeTypeReference(typeof(int)),
1),"be",new CodePrimitiveExpression(null)),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_blockEnds"),CodeBinaryOperatorType.IdentityInequality,new CodePrimitiveExpression(null)),CodeBinaryOperatorType.BooleanAnd,
new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_blockEnds"),
"Length"),CodeBinaryOperatorType.GreaterThan,new CodeVariableReferenceExpression("acc"))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("be"),
new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_blockEnds"),new CodeExpression[]{new CodeVariableReferenceExpression("acc")}))},
new CodeStatement[0]),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("be"),CodeBinaryOperatorType.IdentityInequality,
new CodePrimitiveExpression(null)),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodePrimitiveExpression(0)),
new CodeLabeledStatement("start_be_dfa",new CodeSnippetStatement("")),new CodeAssignStatement(new CodeVariableReferenceExpression("acc"),new CodeArrayIndexerExpression(new
 CodeVariableReferenceExpression("be"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeAssignStatement(new
 CodeVariableReferenceExpression("tlen"),new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("be"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeIterationStatement(new CodeAssignStatement(new CodeVariableReferenceExpression("i"),new CodePrimitiveExpression(0)),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("tlen")),
new CodeAssignStatement(new CodeVariableReferenceExpression("i"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("tto"),new CodeArrayIndexerExpression(new
 CodeVariableReferenceExpression("be"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeAssignStatement(new
 CodeVariableReferenceExpression("prlen"),new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("be"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeIterationStatement(new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodePrimitiveExpression(0)),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("prlen")),
new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("pmin"),new CodeArrayIndexerExpression(new
 CodeVariableReferenceExpression("be"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeAssignStatement(new
 CodeVariableReferenceExpression("pmax"),new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("be"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("ch"),CodeBinaryOperatorType.LessThan,
new CodeVariableReferenceExpression("pmin")),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("prlen"),
CodeBinaryOperatorType.Subtract,new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),
CodeBinaryOperatorType.Multiply,new CodePrimitiveExpression(2)))),new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeVariableReferenceExpression("prlen"))},
new CodeStatement[]{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("ch"),CodeBinaryOperatorType.LessThanOrEqual,
new CodeVariableReferenceExpression("pmax")),new CodeStatement[]{new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeThisReferenceExpression(),"Advance"),new CodeExpression[]{new CodeArgumentReferenceExpression("str"),new CodeDirectionExpression(FieldDirection.Ref,
new CodeVariableReferenceExpression("ch")),new CodeDirectionExpression(FieldDirection.Ref,new CodeVariableReferenceExpression("len")),new CodePrimitiveExpression(false)})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeVariableReferenceExpression("tto")),new CodeGotoStatement("start_be_dfa")},
new CodeStatement[0])})})}),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePrimitiveExpression(false),CodeBinaryOperatorType.ValueEquality,
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("acc"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-1))),new
 CodeStatement[]{new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference("FAMatch")),
"Create"),new CodeExpression[]{new CodeVariableReferenceExpression("sym"),new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeArgumentReferenceExpression("str"),
"Substring"),new CodeExpression[]{new CodeCastExpression(new CodeTypeReference(typeof(int)),new CodeVariableReferenceExpression("cursor_pos")),new CodeVariableReferenceExpression("len")}),
new CodeVariableReferenceExpression("cursor_pos"),new CodeVariableReferenceExpression("line"),new CodeVariableReferenceExpression("column")}))},new CodeStatement[0]),
new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("ch"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-1)),
new CodeStatement[]{new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new
 CodeTypeReference("FAMatch")),"Create"),new CodeExpression[]{new CodePrimitiveExpression(-1),new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeArgumentReferenceExpression("str"),"Substring"),new CodeExpression[]{new CodeCastExpression(new CodeTypeReference(typeof(int)),new CodeVariableReferenceExpression("cursor_pos")),
new CodeVariableReferenceExpression("len")}),new CodeVariableReferenceExpression("cursor_pos"),new CodeVariableReferenceExpression("line"),new CodeVariableReferenceExpression("column")}))},
new CodeStatement[0]),new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(),"Advance"),
new CodeExpression[]{new CodeArgumentReferenceExpression("str"),new CodeDirectionExpression(FieldDirection.Ref,new CodeVariableReferenceExpression("ch")),
new CodeDirectionExpression(FieldDirection.Ref,new CodeVariableReferenceExpression("len")),new CodePrimitiveExpression(false)})),new CodeAssignStatement(new
 CodeVariableReferenceExpression("state"),new CodePrimitiveExpression(0)),new CodeGotoStatement("start_be_dfa")},new CodeStatement[0]),new CodeMethodReturnStatement(new
 CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference("FAMatch")),"Create"),new CodeExpression[]
{new CodeVariableReferenceExpression("acc"),new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeArgumentReferenceExpression("str"),
"Substring"),new CodeExpression[]{new CodeCastExpression(new CodeTypeReference(typeof(int)),new CodeVariableReferenceExpression("cursor_pos")),new CodeVariableReferenceExpression("len")}),
new CodeVariableReferenceExpression("cursor_pos"),new CodeVariableReferenceExpression("line"),new CodeVariableReferenceExpression("column")}))},new CodeStatement[0]),
new CodeIterationStatement(new CodeSnippetStatement(""),new CodeBinaryOperatorExpression(new CodePrimitiveExpression(false),CodeBinaryOperatorType.ValueEquality,
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("ch"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-1))),new CodeSnippetStatement(""),
new CodeStatement[]{new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(bool)),"moved",new CodePrimitiveExpression(false)),new CodeAssignStatement(new
 CodeVariableReferenceExpression("state"),new CodePrimitiveExpression(1)),new CodeAssignStatement(new CodeVariableReferenceExpression("tlen"),new CodeArrayIndexerExpression(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new
 CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new
 CodePrimitiveExpression(1))),new CodeIterationStatement(new CodeAssignStatement(new CodeVariableReferenceExpression("i"),new CodePrimitiveExpression(0)),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("tlen")),
new CodeAssignStatement(new CodeVariableReferenceExpression("i"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("prlen"),
new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeIterationStatement(new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodePrimitiveExpression(0)),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("prlen")),
new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("pmin"),new CodeArrayIndexerExpression(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new
 CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new
 CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("pmax"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeConditionStatement(new
 CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("ch"),CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("pmin")),new
 CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),
CodeBinaryOperatorType.Add,new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("prlen"),CodeBinaryOperatorType.Subtract,
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),CodeBinaryOperatorType.Multiply,
new CodePrimitiveExpression(2)))),new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeVariableReferenceExpression("prlen"))},new CodeStatement[]
{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("ch"),CodeBinaryOperatorType.LessThanOrEqual,new CodeVariableReferenceExpression("pmax")),
new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("moved"),new CodePrimitiveExpression(true))},new CodeStatement[0])})})}),
new CodeConditionStatement(new CodeVariableReferenceExpression("moved"),new CodeStatement[]{new CodeGotoStatement("break_loop")},new CodeStatement[0]),
new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(),"Advance"),new CodeExpression[]
{new CodeArgumentReferenceExpression("str"),new CodeDirectionExpression(FieldDirection.Ref,new CodeVariableReferenceExpression("ch")),new CodeDirectionExpression(FieldDirection.Ref,
new CodeVariableReferenceExpression("len")),new CodePrimitiveExpression(false)}))}),new CodeLabeledStatement("break_loop",new CodeSnippetStatement("")),
new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("len"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(0)),
new CodeStatement[]{new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new
 CodeTypeReference("FAMatch")),"Create"),new CodeExpression[]{new CodePrimitiveExpression(-2),new CodePrimitiveExpression(null),new CodePrimitiveExpression(0),
new CodePrimitiveExpression(0),new CodePrimitiveExpression(0)}))},new CodeStatement[0]),new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference("FAMatch")),"Create"),new CodeExpression[]{new CodePrimitiveExpression(-1),
new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeArgumentReferenceExpression("str"),"Substring"),new CodeExpression[]{new CodeCastExpression(new
 CodeTypeReference(typeof(int)),new CodeVariableReferenceExpression("cursor_pos")),new CodeVariableReferenceExpression("len")}),new CodeVariableReferenceExpression("cursor_pos"),
new CodeVariableReferenceExpression("line"),new CodeVariableReferenceExpression("column")}))},new CodeTypeReference[0],null,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null)},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],
new CodeDirective[0],new CodeDirective[0],null),DeslangedString._TypeDeclaration("FATextReaderDfaTableRunner",true,false,false,false,true,(MemberAttributes.Final
|MemberAttributes.Private),TypeAttributes.NotPublic,new CodeTypeParameter[0],new CodeTypeReference[]{new CodeTypeReference("FATextReaderRunner")},new CodeTypeMember[]
{DeslangedString._MemberField(new CodeTypeReference(new CodeTypeReference(typeof(int)),1),"_dfa",null,MemberAttributes.Private,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null),DeslangedString._MemberField(new CodeTypeReference(new CodeTypeReference(new
 CodeTypeReference(typeof(int)),1),1),"_blockEnds",null,MemberAttributes.Private,new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],
new CodeDirective[0],null),DeslangedString._Constructor((MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new
 CodeTypeReference(new CodeTypeReference(typeof(int)),1),"dfa")},new CodeExpression[0],new CodeExpression[0],new CodeStatement[]{new CodeAssignStatement(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new CodeArgumentReferenceExpression("dfa")),new CodeAssignStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_blockEnds"),new CodePrimitiveExpression(null))},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],
new CodeDirective[0],null),DeslangedString._Constructor((MemberAttributes.Final|MemberAttributes.Public),new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(new
 CodeTypeReference(new CodeTypeReference(typeof(int)),1),"dfa"),new CodeParameterDeclarationExpression(new CodeTypeReference(new CodeTypeReference(new
 CodeTypeReference(typeof(int)),1),1),"blockEnds")},new CodeExpression[0],new CodeExpression[0],new CodeStatement[]{new CodeAssignStatement(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_dfa"),new CodeArgumentReferenceExpression("dfa")),new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"_blockEnds"),new CodeArgumentReferenceExpression("blockEnds"))},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],
null),DeslangedString._MemberMethod(new CodeTypeReference("FAMatch"),"NextMatch",(MemberAttributes.Override|MemberAttributes.Public),new CodeParameterDeclarationExpression[0],
new CodeStatement[]{new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"tlen",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),
"tto",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"prlen",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),
"pmin",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"pmax",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),
"i",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"j",null),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),
"state",new CodePrimitiveExpression(0)),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"acc",null),new CodeExpressionStatement(new
 CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"capture"),"Clear"),new
 CodeExpression[0])),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),
CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-2)),new CodeStatement[]{new CodeExpressionStatement(new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeThisReferenceExpression(),"Advance"),new CodeExpression[0]))},new CodeStatement[0]),new CodeVariableDeclarationStatement(new
 CodeTypeReference(typeof(int)),"len",new CodePrimitiveExpression(0)),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(long)),"cursor_pos",
new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position")),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),
"line",new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"line")),new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),
"column",new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column")),new CodeLabeledStatement("start_dfa",new CodeSnippetStatement("")),
new CodeAssignStatement(new CodeVariableReferenceExpression("acc"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("tlen"),
new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeIterationStatement(new CodeAssignStatement(new CodeVariableReferenceExpression("i"),new CodePrimitiveExpression(0)),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("tlen")),
new CodeAssignStatement(new CodeVariableReferenceExpression("i"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("tto"),new CodeArrayIndexerExpression(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new
 CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new
 CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("prlen"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeIterationStatement(new
 CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodePrimitiveExpression(0)),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),
CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("prlen")),new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("pmin"),
new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("pmax"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeConditionStatement(new
 CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("pmin")),
new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),
CodeBinaryOperatorType.Add,new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("prlen"),CodeBinaryOperatorType.Subtract,
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),CodeBinaryOperatorType.Multiply,
new CodePrimitiveExpression(2)))),new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeVariableReferenceExpression("prlen"))},new CodeStatement[]
{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.LessThanOrEqual,
new CodeVariableReferenceExpression("pmax")),new CodeStatement[]{new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeThisReferenceExpression(),"Advance"),new CodeExpression[0])),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeVariableReferenceExpression("tto")),
new CodeGotoStatement("start_dfa")},new CodeStatement[0])})})}),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePrimitiveExpression(false),
CodeBinaryOperatorType.ValueEquality,new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("acc"),CodeBinaryOperatorType.ValueEquality,
new CodePrimitiveExpression(-1))),new CodeStatement[]{new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)),"sym",new CodeVariableReferenceExpression("acc")),
new CodeVariableDeclarationStatement(new CodeTypeReference(new CodeTypeReference(typeof(int)),1),"be",new CodePrimitiveExpression(null)),new CodeConditionStatement(new
 CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_blockEnds"),CodeBinaryOperatorType.IdentityInequality,
new CodePrimitiveExpression(null)),CodeBinaryOperatorType.BooleanAnd,new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_blockEnds"),"Length"),CodeBinaryOperatorType.GreaterThan,new CodeVariableReferenceExpression("acc"))),new CodeStatement[]
{new CodeAssignStatement(new CodeVariableReferenceExpression("be"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"_blockEnds"),new CodeExpression[]{new CodeVariableReferenceExpression("acc")}))},new CodeStatement[0]),new CodeConditionStatement(new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("be"),CodeBinaryOperatorType.IdentityInequality,new CodePrimitiveExpression(null)),new CodeStatement[]{new CodeAssignStatement(new
 CodeVariableReferenceExpression("state"),new CodePrimitiveExpression(0)),new CodeLabeledStatement("start_be_dfa",new CodeSnippetStatement("")),new CodeAssignStatement(new
 CodeVariableReferenceExpression("acc"),new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("be"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("tlen"),new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("be"),
new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeIterationStatement(new CodeAssignStatement(new
 CodeVariableReferenceExpression("i"),new CodePrimitiveExpression(0)),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),CodeBinaryOperatorType.LessThan,
new CodeVariableReferenceExpression("tlen")),new CodeAssignStatement(new CodeVariableReferenceExpression("i"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),
CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("tto"),new
 CodeArrayIndexerExpression(new CodeVariableReferenceExpression("be"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new
 CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new
 CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("prlen"),new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("be"),
new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeIterationStatement(new CodeAssignStatement(new
 CodeVariableReferenceExpression("j"),new CodePrimitiveExpression(0)),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.LessThan,
new CodeVariableReferenceExpression("prlen")),new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),
CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("pmin"),new
 CodeArrayIndexerExpression(new CodeVariableReferenceExpression("be"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new
 CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new
 CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("pmax"),new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("be"),
new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeConditionStatement(new CodeBinaryOperatorExpression(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("pmin")),
new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),
CodeBinaryOperatorType.Add,new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("prlen"),CodeBinaryOperatorType.Subtract,
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),CodeBinaryOperatorType.Multiply,
new CodePrimitiveExpression(2)))),new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeVariableReferenceExpression("prlen"))},new CodeStatement[]
{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.LessThanOrEqual,
new CodeVariableReferenceExpression("pmax")),new CodeStatement[]{new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeThisReferenceExpression(),"Advance"),new CodeExpression[0])),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeVariableReferenceExpression("tto")),
new CodeGotoStatement("start_be_dfa")},new CodeStatement[0])})})}),new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePrimitiveExpression(false),
CodeBinaryOperatorType.ValueEquality,new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("acc"),CodeBinaryOperatorType.ValueEquality,
new CodePrimitiveExpression(-1))),new CodeStatement[]{new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeTypeReferenceExpression(new CodeTypeReference("FAMatch")),"Create"),new CodeExpression[]{new CodeVariableReferenceExpression("sym"),new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"capture"),"ToString"),new CodeExpression[0]),new CodeVariableReferenceExpression("cursor_pos"),
new CodeVariableReferenceExpression("line"),new CodeVariableReferenceExpression("column")}))},new CodeStatement[0]),new CodeConditionStatement(new CodeBinaryOperatorExpression(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-1)),new CodeStatement[]
{new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference("FAMatch")),
"Create"),new CodeExpression[]{new CodePrimitiveExpression(-1),new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"capture"),"ToString"),new CodeExpression[0]),new CodeVariableReferenceExpression("cursor_pos"),new CodeVariableReferenceExpression("line"),
new CodeVariableReferenceExpression("column")}))},new CodeStatement[0]),new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeThisReferenceExpression(),"Advance"),new CodeExpression[0])),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodePrimitiveExpression(0)),
new CodeGotoStatement("start_be_dfa")},new CodeStatement[0]),new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeTypeReferenceExpression(new CodeTypeReference("FAMatch")),"Create"),new CodeExpression[]{new CodeVariableReferenceExpression("acc"),new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"capture"),"ToString"),new CodeExpression[0]),new CodeVariableReferenceExpression("cursor_pos"),
new CodeVariableReferenceExpression("line"),new CodeVariableReferenceExpression("column")}))},new CodeStatement[0]),new CodeIterationStatement(new CodeSnippetStatement(""),
new CodeBinaryOperatorExpression(new CodePrimitiveExpression(false),CodeBinaryOperatorType.ValueEquality,new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-1))),new CodeSnippetStatement(""),new CodeStatement[]
{new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(bool)),"moved",new CodePrimitiveExpression(false)),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodePrimitiveExpression(1)),new CodeAssignStatement(new CodeVariableReferenceExpression("tlen"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeIterationStatement(new
 CodeAssignStatement(new CodeVariableReferenceExpression("i"),new CodePrimitiveExpression(0)),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"),
CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("tlen")),new CodeAssignStatement(new CodeVariableReferenceExpression("i"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("i"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeAssignStatement(new
 CodeVariableReferenceExpression("prlen"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new
 CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeIterationStatement(new CodeAssignStatement(new
 CodeVariableReferenceExpression("j"),new CodePrimitiveExpression(0)),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.LessThan,
new CodeVariableReferenceExpression("prlen")),new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),
CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("pmin"),new
 CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),
new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,
new CodePrimitiveExpression(1))),new CodeAssignStatement(new CodeVariableReferenceExpression("pmax"),new CodeArrayIndexerExpression(new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"_dfa"),new CodeExpression[]{new CodeVariableReferenceExpression("state")})),new CodeAssignStatement(new CodeVariableReferenceExpression("state"),
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),new CodeConditionStatement(new
 CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.LessThan,new CodeVariableReferenceExpression("pmin")),
new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("state"),new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("state"),
CodeBinaryOperatorType.Add,new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("prlen"),CodeBinaryOperatorType.Subtract,
new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("j"),CodeBinaryOperatorType.Add,new CodePrimitiveExpression(1))),CodeBinaryOperatorType.Multiply,
new CodePrimitiveExpression(2)))),new CodeAssignStatement(new CodeVariableReferenceExpression("j"),new CodeVariableReferenceExpression("prlen"))},new CodeStatement[]
{new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.LessThanOrEqual,
new CodeVariableReferenceExpression("pmax")),new CodeStatement[]{new CodeAssignStatement(new CodeVariableReferenceExpression("moved"),new CodePrimitiveExpression(true))},
new CodeStatement[0])})})}),new CodeConditionStatement(new CodeVariableReferenceExpression("moved"),new CodeStatement[]{new CodeGotoStatement("break_loop")},
new CodeStatement[0]),new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(),"Advance"),
new CodeExpression[0]))}),new CodeLabeledStatement("break_loop",new CodeSnippetStatement("")),new CodeConditionStatement(new CodeBinaryOperatorExpression(new
 CodeVariableReferenceExpression("len"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(0)),new CodeStatement[]{new CodeMethodReturnStatement(new
 CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference("FAMatch")),"Create"),new CodeExpression[]
{new CodePrimitiveExpression(-2),new CodePrimitiveExpression(null),new CodePrimitiveExpression(0),new CodePrimitiveExpression(0),new CodePrimitiveExpression(0)}))},
new CodeStatement[0]),new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new
 CodeTypeReference("FAMatch")),"Create"),new CodeExpression[]{new CodePrimitiveExpression(-1),new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new
 CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"capture"),"ToString"),new CodeExpression[0]),new CodeVariableReferenceExpression("cursor_pos"),
new CodeVariableReferenceExpression("line"),new CodeVariableReferenceExpression("column")}))},new CodeTypeReference[0],null,new CodeCommentStatement[0],
new CodeAttributeDeclaration[0],new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0],null)},new CodeCommentStatement[0],new CodeAttributeDeclaration[0],
new CodeDirective[0],new CodeDirective[0],null)},new CodeCommentStatement[0])},new CodeAttributeDeclaration[0],new CodeDirective[0],new CodeDirective[0]);
}}}}namespace VisualFA{
#if FALIB
public
#endif
enum FAGeneratorDependencies{/// <summary>
/// No dependent code will be generated
/// </summary>
None,/// <summary>
/// Shared code will be generated
/// </summary>
GenerateSharedCode,/// <summary>
/// The runtime library will be referenced
/// </summary>
UseRuntime}
#if FALIB
public
#endif
partial class FAGeneratorOptions{public FAGeneratorDependencies Dependencies{get;set;}=FAGeneratorDependencies.None;public bool GenerateTables{get;set;
}=false;public bool GenerateTextReaderRunner{get;set;}=false;public bool GenerateStringRunner{get;set;}=true;
#if FALIB_SPANS
public bool UseSpans{get;set;}=FAStringRunner.UsingSpans;
#endif
[Obsolete]public string ClassName{get;set;}="GeneratedRunner";public string StringRunnerClassName{get;set;}="GeneratedStringRunner";public string TextReaderRunnerClassName
{get;set;}="GeneratedTextReaderRunner";public string Namespace{get;set;}="";public string[]Symbols{get;set;}=null;}
#if FALIB
public
#endif
static partial class FAGenerator{static CodeBinaryOperatorExpression _GenerateRangesExpression(CodeExpression codepoint,IList<FARange>ranges){CodeBinaryOperatorExpression
 result=null;var inverted=false;var hasEof=false;for(int i=0;i<ranges.Count;++i){if(ranges[i].Min==-1){hasEof=true;break;}}if(!hasEof){var notRanges=new
 List<FARange>(FARange.ToNotRanges(ranges));if(notRanges.Count<ranges.Count){inverted=true;ranges=notRanges;}}for(int i=0;i<ranges.Count;++i){var first
=ranges[i].Min;int last=ranges[i].Max;var fp=new CodePrimitiveExpression(first);if(first!=last){var lp=new CodePrimitiveExpression(last);var exp=new CodeBinaryOperatorExpression(codepoint,
CodeBinaryOperatorType.GreaterThanOrEqual,fp);exp=new CodeBinaryOperatorExpression(exp,CodeBinaryOperatorType.BooleanAnd,new CodeBinaryOperatorExpression(codepoint,
CodeBinaryOperatorType.LessThanOrEqual,lp));if(result==null){result=exp;}else{result=new CodeBinaryOperatorExpression(result,CodeBinaryOperatorType.BooleanOr,
exp);}}else{var exp=new CodeBinaryOperatorExpression(codepoint,CodeBinaryOperatorType.ValueEquality,fp);if(result==null){result=exp;}else{result=new CodeBinaryOperatorExpression(result,
CodeBinaryOperatorType.BooleanOr,exp);}}}if(inverted){var notEof=new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(codepoint,CodeBinaryOperatorType.ValueEquality,
new CodePrimitiveExpression(-1)),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(false));var notResult=new CodeBinaryOperatorExpression(result,
CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(false));result=new CodeBinaryOperatorExpression(notEof,CodeBinaryOperatorType.BooleanAnd,
notResult);}return result;}static string _MakeSafeName(string name){StringBuilder sb;if(char.IsDigit(name[0])){sb=new StringBuilder(name.Length+1);sb.Append('_');
}else{sb=new StringBuilder(name.Length);}for(var i=0;i<name.Length;++i){var ch=name[i];if('_'==ch||char.IsLetterOrDigit(ch))sb.Append(ch);else sb.Append('_');
}return sb.ToString();}static bool _HasMember(CodeTypeDeclaration decl,string name){for(int i=0;i<decl.Members.Count;++i){var member=decl.Members[i];if(member.Name
==name){return true;}}return false;}static string _MakeUniqueName(CodeTypeDeclaration decl,string name){string result=name;int i=1;while(_HasMember(decl,result))
{++i;result=name+i.ToString();}return result;}static void _GenerateSymbols(CodeTypeDeclaration decl,FAGeneratorOptions opts){if(opts.Symbols==null){return;
}var tint=new CodeTypeReference(typeof(int));for(int i=0;i<opts.Symbols.Length;i++){var org=opts.Symbols[i];var sym=org;if(!string.IsNullOrEmpty(sym))
{sym=_MakeUniqueName(decl,_MakeSafeName(sym));var field=new CodeMemberField();field.Name=sym;field.Type=tint;field.Attributes=MemberAttributes.Public|
MemberAttributes.Const;field.InitExpression=new CodePrimitiveExpression(i);if(org!=sym){field.Comments.Add(new CodeCommentStatement(org));}decl.Members.Add(field);
}}}static void _GenerateMatchImplBody(bool textReader,IList<FA>closure,IList<FA>blockEnds,CodeStatementCollection dest,FAGeneratorOptions opts){var lcapturebuffer
=new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"capture");var lccbts=new CodeMethodInvokeExpression(lcapturebuffer,"ToString");var
 crm=new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("FAMatch"),"Create");CodeStatement adv;CodeExpression tostr=null;if(!textReader)
{
#if FALIB_SPANS
if(opts.UseSpans){tostr=new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("s"),
"Slice",new CodeVariableReferenceExpression("p"),new CodeVariableReferenceExpression("len")),"ToString"));}
#endif
if(tostr==null){tostr=new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("s"),"Substring",new CodeVariableReferenceExpression("p"),new
 CodeVariableReferenceExpression("len"));} adv=new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(),
"Advance"),new CodeExpression[]{new CodeArgumentReferenceExpression("s"),new CodeDirectionExpression(FieldDirection.Ref,new CodeVariableReferenceExpression("ch")),
new CodeDirectionExpression(FieldDirection.Ref,new CodeVariableReferenceExpression("len")),new CodePrimitiveExpression(false)}));}else{tostr=lccbts;adv
=new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(),"Advance")));} if(!textReader)
{dest.Add(new CodeVariableDeclarationStatement(typeof(int),"ch"));dest.Add(new CodeVariableDeclarationStatement(typeof(int),"len"));}dest.Add(new CodeVariableDeclarationStatement(typeof(int),
"p"));dest.Add(new CodeVariableDeclarationStatement(typeof(int),"l"));dest.Add(new CodeVariableDeclarationStatement(typeof(int),"c"));if(!textReader){
 dest.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("ch"),new CodePrimitiveExpression(-1)));}if(textReader){ dest.Add(new CodeExpressionStatement(
new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(lcapturebuffer,"Clear")))); dest.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(
new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-2)),new
 CodeStatement[]{new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(),"Advance"))}));}else{ dest.Add(new CodeAssignStatement(
new CodeVariableReferenceExpression("len"),new CodePrimitiveExpression(0))); dest.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(
new CodeThisReferenceExpression(),"position"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-1)),new CodeStatement[]{new CodeAssignStatement(
new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"),new CodePrimitiveExpression(0))}));} dest.Add(new CodeAssignStatement(new
 CodeVariableReferenceExpression("p"),new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"position"))); dest.Add(new CodeAssignStatement(
new CodeVariableReferenceExpression("l"),new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"line"))); dest.Add(new CodeAssignStatement(
new CodeVariableReferenceExpression("c"),new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"column")));if(!textReader){ dest.Add(new CodeExpressionStatement(
new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(),"Advance"),new CodeExpression[]{new CodeArgumentReferenceExpression("s"),
new CodeDirectionExpression(FieldDirection.Ref,new CodeVariableReferenceExpression("ch")),new CodeDirectionExpression(FieldDirection.Ref,new CodeVariableReferenceExpression("len")),
new CodePrimitiveExpression(true)})));}CodeExpression cmp; if(textReader){cmp=new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"current");
}else{cmp=new CodeVariableReferenceExpression("ch");} var q0ranges=new List<FARange>(); q0ranges.Add(new FARange(-1,-1)); for(var q=0;q<closure.Count;
++q){var fromState=closure[q];CodeLabeledStatement state=null;if(q>0||FA.IsLoop(closure)){state=new CodeLabeledStatement("q"+q.ToString());dest.Add(state);
}else if(q==0){dest.Add(new CodeCommentStatement("q0:"));} bool attachedlabel=_GenerateTransitions(closure,dest,adv,cmp,q0ranges,q,fromState,state);if
(fromState.IsAccepting){ if(blockEnds!=null&&blockEnds.Count>fromState.AcceptSymbol&&blockEnds[fromState.AcceptSymbol]!=null){attachedlabel=_GenerateBlockEndCall(textReader,
dest,fromState,state,attachedlabel);}else{ attachedlabel=_GenerateMatchAccept(dest,tostr,fromState,state,attachedlabel);}}else{ var gerror=new CodeGotoStatement("errorout");
if(!attachedlabel){attachedlabel=true;if(state!=null){state.Statement=gerror;}else{dest.Add(gerror);}}else{dest.Add(gerror);}}}var error=new CodeLabeledStatement("errorout");
 var ifq0match=new CodeConditionStatement(_GenerateRangesExpression(cmp,q0ranges.ToArray()));dest.Add(error); error.Statement=ifq0match;CodeExpression
 isEmpty;if(textReader){ isEmpty=new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
"capture"),"Length"),CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(0));}else{ isEmpty=new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("len"),
CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(0));}var checkIfEnd=new CodeConditionStatement(isEmpty); checkIfEnd.TrueStatements.Add(
new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("FAMatch"),"Create"),new
 CodeExpression[]{new CodePrimitiveExpression(-2),new CodePrimitiveExpression(null),new CodePrimitiveExpression(0),new CodePrimitiveExpression(0),new CodePrimitiveExpression(0)
}))); ifq0match.TrueStatements.AddRange(new CodeStatement[]{checkIfEnd,new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(
new CodeTypeReferenceExpression("FAMatch"),"Create"),new CodeExpression[]{new CodePrimitiveExpression(-1),tostr,new CodeVariableReferenceExpression("p"),
new CodeVariableReferenceExpression("l"),new CodeVariableReferenceExpression("c")}))}); dest.Add(adv); dest.Add(new CodeGotoStatement(error.Label));}private
 static bool _GenerateMatchAccept(CodeStatementCollection dest,CodeExpression tostr,FA fromState,CodeLabeledStatement state,bool attachedlabel){var retmatch
=new CodeMethodReturnStatement();retmatch.Expression=new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("FAMatch"),
"Create"),new CodeExpression[]{new CodePrimitiveExpression(fromState.AcceptSymbol),tostr,new CodeVariableReferenceExpression("p"),new CodeVariableReferenceExpression("l"),
new CodeVariableReferenceExpression("c")});if(!attachedlabel){attachedlabel=true;if(state!=null){state.Statement=retmatch;}else{dest.Add(retmatch);}}else
{dest.Add(retmatch);}return attachedlabel;}private static bool _GenerateBlockEndCall(bool textReader,CodeStatementCollection dest,FA fromState,CodeLabeledStatement
 state,bool attachedlabel){CodeMethodReturnStatement retbe=new CodeMethodReturnStatement();if(textReader){retbe.Expression=new CodeMethodInvokeExpression(
new CodeMethodReferenceExpression(null,"_BlockEnd"+fromState.AcceptSymbol.ToString()),new CodeExpression[]{new CodeVariableReferenceExpression("p"),new
 CodeVariableReferenceExpression("l"),new CodeVariableReferenceExpression("c")});}else{retbe.Expression=new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(
null,"_BlockEnd"+fromState.AcceptSymbol.ToString()),new CodeExpression[]{new CodeArgumentReferenceExpression("s"),new CodeVariableReferenceExpression("ch"),
new CodeVariableReferenceExpression("len"),new CodeVariableReferenceExpression("p"),new CodeVariableReferenceExpression("l"),new CodeVariableReferenceExpression("c")});
}if(!attachedlabel){attachedlabel=true;if(state!=null){state.Statement=retbe;}else{dest.Add(retbe);}}else{dest.Add(retbe);}return attachedlabel;}private
 static bool _GenerateTransitions(IList<FA>closure,CodeStatementCollection dest,CodeStatement adv,CodeExpression cmp,List<FARange>q0ranges,int q,FA fromState,
CodeLabeledStatement state){var trnsgrp=fromState.FillInputTransitionRangesGroupedByState();var attachedlabel=false;foreach(var trn in trnsgrp){if(q==
0){q0ranges.AddRange(trn.Value);}var tmp=new RegexCharsetExpression();foreach(var rng in trn.Value){if(rng.Min==rng.Max){tmp.Entries.Add(new RegexCharsetCharEntry(rng.Min));
}else{tmp.Entries.Add(new RegexCharsetRangeEntry(rng.Min,rng.Max));}}var rngcmt=new CodeCommentStatement(tmp.ToString());if(!attachedlabel){attachedlabel
=true;if(state!=null){state.Statement=rngcmt;}else{dest.Add(rngcmt);}}else{dest.Add(rngcmt);}var iftrans=new CodeConditionStatement(_GenerateRangesExpression(cmp,
trn.Value));dest.Add(iftrans);iftrans.TrueStatements.AddRange(new CodeStatement[]{adv,new CodeGotoStatement("q"+closure.IndexOf(trn.Key).ToString())});
}return attachedlabel;}private static void _GenerateBlockEnds(bool textReader,CodeTypeDeclaration type,IList<FA>blockEnds,FAGeneratorOptions opts){if(blockEnds
==null){return;}CodeStatement adv;CodeExpression tostr=null;if(!textReader){
#if FALIB_SPANS
if(opts.UseSpans){tostr=new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("s"),
"Slice",new CodeArgumentReferenceExpression("position"),new CodeArgumentReferenceExpression("len")),"ToString"));}
#endif
if(tostr==null){tostr=new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("s"),"Substring",new CodeArgumentReferenceExpression("position"),
new CodeArgumentReferenceExpression("len"));}adv=new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(),
"Advance"),new CodeExpression[]{new CodeArgumentReferenceExpression("s"),new CodeDirectionExpression(FieldDirection.Ref,new CodeArgumentReferenceExpression("cp")),
new CodeDirectionExpression(FieldDirection.Ref,new CodeArgumentReferenceExpression("len")),new CodePrimitiveExpression(false)}));}else{tostr=new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"capture"),"ToString"));adv=new CodeExpressionStatement(new
 CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(),"Advance")));}var position=new CodeArgumentReferenceExpression("position");
var line=new CodeArgumentReferenceExpression("line");var column=new CodeArgumentReferenceExpression("column");var lcapturebuffer=new CodePropertyReferenceExpression(new
 CodeThisReferenceExpression(),"capture");var lccbts=new CodeMethodInvokeExpression(lcapturebuffer,"ToString");var crm=new CodeMethodReferenceExpression(new
 CodeTypeReferenceExpression(typeof(FAMatch).Name),"Create");for(int i=0;i<blockEnds.Count;++i){var be=blockEnds[i];if(be!=null){var closure=be.FillClosure();
var ch=new CodeVariableReferenceExpression("ch");var mb=new CodeMemberMethod();mb.Name="_BlockEnd"+i.ToString();mb.Attributes=MemberAttributes.Private;
mb.ReturnType=new CodeTypeReference(typeof(FAMatch).Name);if(!textReader){
#if FALIB_SPANS
if(opts.UseSpans){mb.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference("ReadOnlySpan`1",new CodeTypeReference[]{new CodeTypeReference(typeof(char))
}),"s"));}
#endif
if(mb.Parameters.Count==0){mb.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)),"s"));}mb.Parameters.Add(new
 CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)),"cp"));mb.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(int)),
"len"));}mb.Parameters.AddRange(new CodeParameterDeclarationExpression[]{new CodeParameterDeclarationExpression(typeof(int),"position"),new CodeParameterDeclarationExpression(typeof(int),"line"),
new CodeParameterDeclarationExpression(typeof(int),"column")});type.Members.Add(mb);var dest=mb.Statements;CodeExpression cmp;if(textReader){cmp=new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"current");}else{cmp=new CodeArgumentReferenceExpression("cp");}for(var q=0;q<closure.Count;++q){var fromState=closure[q];
var state=new CodeLabeledStatement("q"+q.ToString());dest.Add(state);var trnsgrp=fromState.FillInputTransitionRangesGroupedByState();var attachedlabel
=false;foreach(var trn in trnsgrp){var tmp=new RegexCharsetExpression();foreach(var rng in trn.Value){if(rng.Min==rng.Max){tmp.Entries.Add(new RegexCharsetCharEntry(rng.Min));
}else{tmp.Entries.Add(new RegexCharsetRangeEntry(rng.Min,rng.Max));}}var rngcmt=new CodeCommentStatement(tmp.ToString());if(!attachedlabel){attachedlabel
=true;if(state!=null){state.Statement=rngcmt;}else{dest.Add(rngcmt);}}else{dest.Add(rngcmt);}var iftrans=new CodeConditionStatement(_GenerateRangesExpression(cmp,
trn.Value));dest.Add(iftrans);iftrans.TrueStatements.AddRange(new CodeStatement[]{adv,new CodeGotoStatement("q"+closure.IndexOf(trn.Key).ToString())});
}if(fromState.IsAccepting){var retmatch=new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(FAMatch).Name),
"Create"),new CodeExpression[]{new CodePrimitiveExpression(fromState.AcceptSymbol),tostr,position,line,column}));if(!attachedlabel){attachedlabel=true;
state.Statement=retmatch;}else{dest.Add(retmatch);}}else{var gerror=new CodeGotoStatement("errorout");if(!attachedlabel){attachedlabel=true;state.Statement
=gerror;}else{dest.Add(gerror);}}}var error=new CodeLabeledStatement("errorout");var ifEnd=new CodeConditionStatement(new CodeBinaryOperatorExpression(cmp,
CodeBinaryOperatorType.ValueEquality,new CodePrimitiveExpression(-1)),new CodeStatement[]{new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new
 CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(FAMatch).Name),"Create"),new CodeExpression[]{new CodePrimitiveExpression(-1),tostr,
position,line,column}))});error.Statement=ifEnd;dest.Add(error);dest.Add(adv);dest.Add(new CodeGotoStatement("q0"));}}}private static CodeTypeDeclaration
 _GenerateRunner(bool textReader,IList<FA>closure,IList<FA>blockEnds,FAGeneratorOptions options){var result=new CodeTypeDeclaration(textReader?options.TextReaderRunnerClassName:options.StringRunnerClassName);
result.TypeAttributes=TypeAttributes.NotPublic|TypeAttributes.Sealed;result.BaseTypes.Add(new CodeTypeReference(textReader?typeof(FATextReaderRunner).Name
:typeof(FAStringRunner).Name));result.IsClass=true;result.IsPartial=true;var nextMatch=new CodeMemberMethod();nextMatch.Name="NextMatch";nextMatch.Attributes
=MemberAttributes.Public|MemberAttributes.Override;nextMatch.ReturnType=new CodeTypeReference(typeof(FAMatch).Name);CodeStatementCollection target;_GenerateBlockEnds(textReader,
result,blockEnds,options);if(!textReader){var nextMatchImpl=new CodeMemberMethod();nextMatchImpl.Name="NextMatchImpl";CodeTypeReference pt=null;
#if FALIB_SPANS
if(options.UseSpans){pt=new CodeTypeReference("ReadOnlySpan`1",new CodeTypeReference[]{new CodeTypeReference(typeof(char))});}
#endif
if(pt==null){pt=new CodeTypeReference(typeof(string));}nextMatchImpl.Parameters.Add(new CodeParameterDeclarationExpression(pt,"s"));nextMatchImpl.ReturnType
=new CodeTypeReference(typeof(FAMatch).Name);result.Members.Add(nextMatchImpl);target=nextMatchImpl.Statements;nextMatch.Statements.Add(new CodeMethodReturnStatement(new
 CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(),nextMatchImpl.Name),new CodeFieldReferenceExpression(new
 CodeThisReferenceExpression(),"string"))));}else{target=nextMatch.Statements;}_GenerateMatchImplBody(textReader,closure,blockEnds,target,options);result.Members.Add(nextMatch);
return result;}private static CodeArrayCreateExpression _CreateDfaArray(int[]dfa){var result=new CodeArrayCreateExpression(new CodeTypeReference(typeof(int)));
for(int i=0;i<dfa.Length;++i){result.Initializers.Add(new CodePrimitiveExpression(dfa[i]));}return result;}private static CodeArrayCreateExpression _CreateBlockEndArray(IList<int[]>
blockEnds){var intArrays=new CodeTypeReference(new CodeTypeReference(new CodeTypeReference(typeof(int)),1),1);var result=new CodeArrayCreateExpression(intArrays);
var nullExp=new CodePrimitiveExpression(null);for(int i=0;i<blockEnds.Count;++i){if(blockEnds[i]==null){result.Initializers.Add(nullExp);}else{result.Initializers.Add(_CreateDfaArray(blockEnds[i]));
}}return result;}private static CodeTypeDeclaration _GenerateTableRunner(bool textReader,IList<FA>closure,IList<FA>blockEnds,FAGeneratorOptions options)
{var result=new CodeTypeDeclaration(textReader?options.TextReaderRunnerClassName:options.StringRunnerClassName);var hasBlockEnds=(blockEnds!=null&&blockEnds.Count
>0);result.TypeAttributes=TypeAttributes.NotPublic|TypeAttributes.Sealed;result.BaseTypes.Add(new CodeTypeReference(textReader?typeof(FATextReaderDfaTableRunner).Name
:typeof(FAStringDfaTableRunner).Name));var dfaField=new CodeMemberField(new CodeTypeReference(new CodeTypeReference(typeof(int)),1),"_dfa");dfaField.Attributes
=MemberAttributes.Private|MemberAttributes.Static;dfaField.InitExpression=_CreateDfaArray(closure[0].ToArray());result.Members.Add(dfaField);if(hasBlockEnds)
{var blockEndsField=new CodeMemberField(new CodeTypeReference(new CodeTypeReference(new CodeTypeReference(typeof(int)),1),1),"_blockEnds");blockEndsField.Attributes
=MemberAttributes.Private|MemberAttributes.Static;var belist=new List<int[]>(blockEnds.Count);for(int i=0;i<blockEnds.Count;++i){if(blockEnds[i]!=null)
{belist.Add(blockEnds[i].ToArray());}else{belist.Add(null);}}blockEndsField.InitExpression=_CreateBlockEndArray(belist);result.Members.Add(blockEndsField);
}var ctor=new CodeConstructor();ctor.Attributes=MemberAttributes.Public;ctor.BaseConstructorArgs.Add(new CodeFieldReferenceExpression(null,"_dfa"));ctor.BaseConstructorArgs.Add(hasBlockEnds
?(CodeExpression)new CodeFieldReferenceExpression(null,"_blockEnds"):(CodeExpression)new CodePrimitiveExpression(null));result.Members.Add(ctor);result.IsClass
=true;result.IsPartial=true;return result;}public static CodeCompileUnit Generate(this FA fa,IList<FA>blockEnds=null,FAGeneratorOptions options=null,IProgress<int>
progress=null){if(options==null){options=new FAGeneratorOptions();}if(!fa.IsDeterministic){fa=fa.ToDfa(progress);}if(blockEnds!=null){for(int i=0;i<blockEnds.Count;
++i){var be=blockEnds[i];if(be!=null){blockEnds[i]=be.ToMinimizedDfa(progress);}}}var closure=fa.FillClosure();var result=new CodeCompileUnit();var ns
=new CodeNamespace();if(options.Namespace!=null){ns.Name=options.Namespace;}switch(options.Dependencies){case FAGeneratorDependencies.None:result.ReferencedAssemblies.Add(typeof(WeakReference<>).Assembly.FullName);
ns.Imports.AddRange(new CodeNamespaceImport[]{new CodeNamespaceImport("System"),new CodeNamespaceImport("System.IO"),new CodeNamespaceImport("System.Text"),
new CodeNamespaceImport("System.Collections.Generic")});break;case FAGeneratorDependencies.UseRuntime:result.ReferencedAssemblies.Add(typeof(WeakReference<>).Assembly.FullName);
result.ReferencedAssemblies.Add(typeof(FA).Assembly.FullName);ns.Imports.AddRange(new CodeNamespaceImport[]{new CodeNamespaceImport("System"),new CodeNamespaceImport("System.IO"),
new CodeNamespaceImport("System.Text"),new CodeNamespaceImport("System.Collections.Generic"),new CodeNamespaceImport("VisualFA")});break;case FAGeneratorDependencies.GenerateSharedCode:
ns.Imports.AddRange(new CodeNamespaceImport[]{new CodeNamespaceImport("System"),new CodeNamespaceImport("System.IO"),new CodeNamespaceImport("System.Text"),
new CodeNamespaceImport("System.Collections.Generic")});
#if FALIB_SPANS
ns.Types.AddRange(Deslanged.GetFAMatch(options.UseSpans).Namespaces[0].Types);ns.Types.AddRange(Deslanged.GetFARunner(options.UseSpans).Namespaces[0].Types);
#else
ns.Types.AddRange(Deslanged.GetFAMatch(false).Namespaces[0].Types);ns.Types.AddRange(Deslanged.GetFARunner(false).Namespaces[0].Types);
#endif
if(options.GenerateTables){
#if FALIB_SPANS
ns.Types.AddRange(Deslanged.GetFADfaTableRunner(options.UseSpans).Namespaces[0].Types);
#else
ns.Types.AddRange(Deslanged.GetFADfaTableRunner(false).Namespaces[0].Types);
#endif
}break;}result.Namespaces.Add(ns);if(options.GenerateStringRunner){CodeTypeDeclaration type;if(options.GenerateTables){type=_GenerateTableRunner(false,
closure,blockEnds,options);}else{type=_GenerateRunner(false,closure,blockEnds,options);}_GenerateSymbols(type,options);ns.Types.Add(type);}if(options.GenerateTextReaderRunner)
{CodeTypeDeclaration type;if(options.GenerateTables){type=_GenerateTableRunner(true,closure,blockEnds,options);}else{type=_GenerateRunner(true,closure,
blockEnds,options);}_GenerateSymbols(type,options);ns.Types.Add(type);}var ver=typeof(FA).Assembly.GetName().Version.ToString();var gendecl=new CodeAttributeDeclaration(new
 CodeTypeReference(typeof(GeneratedCodeAttribute)),new CodeAttributeArgument(new CodePrimitiveExpression("Visual FA")),new CodeAttributeArgument(new CodePrimitiveExpression(ver)));
foreach(CodeTypeDeclaration t in ns.Types){t.CustomAttributes.Add(gendecl);}return result;}}}