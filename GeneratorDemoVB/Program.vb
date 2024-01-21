Imports System
Imports System.IO
Imports System.CodeDom
Imports System.CodeDom.Compiler
Imports VisualFA
Module Program
    Sub Main(args As String())
        Dim exp As String = "/* foo *//*baz*/ &%^ the quick /*bar */#(@*$//brown fox /* tricky */ jumped over the -10 $#(%*& lazy dog ^%$@@"
        Dim commentStart As FA = FA.Parse("\/\*", 0, False)
        Dim commentEnd = FA.Parse("\*\/", 0, False)
        Dim commentLine = FA.Parse("\/\/[^\n]*", 1, False)
        Dim lexer As FA = FA.ToLexer(New FA() {commentStart, commentLine}, True)
        Dim gopts As FAGeneratorOptions = New FAGeneratorOptions()
        gopts.GenerateTables = False
        gopts.GenerateTextReaderRunner = False
        gopts.ClassName = "CommentRunner"
        gopts.UseSpans = False ' not supported by VB
        ' must generate our own shared code if we're not using spans
        ' and Visual FA's string runners are
        gopts.Dependencies = FAGeneratorDependencies.GenerateSharedCode
        Dim ccu = lexer.Generate(New FA() {commentEnd}, gopts)
        Dim vb As VBCodeProvider = New VBCodeProvider()
        Dim cgopts As CodeGeneratorOptions = New CodeGeneratorOptions()
        cgopts.IndentString = "    "
        cgopts.BlankLinesBetweenMembers = False
        cgopts.VerbatimOrder = True
        Dim sw As StreamWriter = New StreamWriter("..\..\..\CommentRunner.vb", False)
        vb.GenerateCodeFromCompileUnit(ccu, sw, cgopts)
        sw.Close()

        Console.WriteLine("Hello World!")
        ' there are some conflicts due to shared code generation so we have to fully qualify here
        Dim stringRunner As VisualFA.FARunner = lexer.Run(exp, New FA() {commentEnd})
        For Each m As VisualFA.FAMatch In stringRunner
            Console.WriteLine(m)
        Next
        Console.WriteLine("---------------------")
        Dim genRunner As New CommentRunner()
        genRunner.Set(exp)
        For Each m As FAMatch In genRunner
            Console.WriteLine(m)
        Next
    End Sub
End Module
