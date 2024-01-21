'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text

<System.CodeDom.Compiler.GeneratedCodeAttribute("Visual FA", "1.0.0.0")>  _
Partial Friend Structure FAMatch
    Private _symbolId As Integer
    Private _value As String
    Private _position As Long
    Private _line As Integer
    Private _column As Integer
    ''' <summary>
    ''' The matched symbol - this is the accept id
    ''' </summary>
    Public ReadOnly Property SymbolId() As Integer
        Get
            Return Me._symbolId
        End Get
    End Property
    ''' <summary>
    ''' The matched value
    ''' </summary>
    Public ReadOnly Property Value() As String
        Get
            Return Me._value
        End Get
    End Property
    ''' <summary>
    ''' The position of the match within the input
    ''' </summary>
    Public ReadOnly Property Position() As Long
        Get
            Return Me._position
        End Get
    End Property
    ''' <summary>
    ''' The one based line number
    ''' </summary>
    Public ReadOnly Property Line() As Integer
        Get
            Return Me._line
        End Get
    End Property
    ''' <summary>
    ''' The one based column
    ''' </summary>
    Public ReadOnly Property Column() As Integer
        Get
            Return Me._column
        End Get
    End Property
    ''' <summary>
    ''' Indicates whether the text matched the expression
    ''' </summary>
    ''' <remarks>Non matches are returned with negative accept symbols. You can use this property to determine if the text therein was part of a match.</remarks>
    Public ReadOnly Property IsSuccess() As Boolean
        Get
            Return (Me._symbolId > -1)
        End Get
    End Property
    ''' <summary>
    ''' Provides a string representation of the match
    ''' </summary>
    ''' <returns>A string containing match information</returns>
    Public Overrides Function ToString() As String
        Dim sb As System.Text.StringBuilder = New System.Text.StringBuilder()
        sb.Append("[SymbolId: ")
        sb.Append(Me.SymbolId)
        sb.Append(", Value: ")
        If (Not (Me.Value) Is Nothing) Then
            sb.Append("""")
            sb.Append(Me.Value.Replace(""&Global.Microsoft.VisualBasic.ChrW(13), "\r").Replace(""&Global.Microsoft.VisualBasic.ChrW(9), "\t").Replace(""&Global.Microsoft.VisualBasic.ChrW(10), "\n").Replace("", "\v"))
            sb.Append(""", Position: ")
        Else
            sb.Append("null, Position: ")
        End If
        sb.Append(Me.Position)
        sb.Append(" (")
        sb.Append(Me.Line)
        sb.Append(", ")
        sb.Append(Me.Column)
        sb.Append(")]")
        Return sb.ToString
    End Function
    ''' <summary>
    ''' Constructs a new instance
    ''' </summary>
    ''' <param name="symbolId">The symbol id</param>
    ''' <param name="value">The matched value</param>
    ''' <param name="position">The match position</param>
    ''' <param name="line">The line</param>
    ''' <param name="column">The column</param>
    <System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)>  _
    Public Shared Function Create(ByVal symbolId As Integer, ByVal value As String, ByVal position As Long, ByVal line As Integer, ByVal column As Integer) As FAMatch
        Dim result As FAMatch = CType(Nothing, FAMatch)
        result._symbolId = symbolId
        result._value = value
        result._position = position
        result._line = line
        result._column = column
        Return result
    End Function
End Structure
<System.CodeDom.Compiler.GeneratedCodeAttribute("Visual FA", "1.0.0.0")>  _
Partial Friend MustInherit Class FARunner
    Inherits [Object]
    Implements IEnumerable(Of FAMatch)
    Protected Friend Sub New()
        MyBase.New
        Me.position = -1
        Me.line = 1
        Me.column = 1
        Me._tabWidth = 4
    End Sub
    Public Class Enumerator
        Inherits [Object]
        Implements IEnumerator(Of FAMatch)
        Private _state As Integer
        Private _current As FAMatch
        Private _parent As WeakReference(Of FARunner)
        Public Sub New(ByVal parent As FARunner)
            MyBase.New
            Me._parent = New WeakReference(Of FARunner)(parent)
            Me._state = -2
        End Sub
        Public ReadOnly Property Current() As FAMatch Implements IEnumerator(Of FAMatch).Current
            Get
                If (Me._state = -3) Then
                    Throw New ObjectDisposedException("Enumerator")
                End If
                If (Me._state < 0) Then
                    Throw New InvalidOperationException("The enumerator is not positioned on an element")
                End If
                Return Me._current
            End Get
        End Property
        Function System_Collections_IEnumerator_MoveNext() As Boolean Implements System.Collections.IEnumerator.MoveNext
            Return Me.MoveNext
        End Function
        ReadOnly Property System_Collections_IEnumerator_Current() As Object Implements System.Collections.IEnumerator.Current
            Get
                Return Me.Current
            End Get
        End Property
        Sub System_Collections_IEnumerator_Reset() Implements System.Collections.IEnumerator.Reset
            Me.Reset
        End Sub
        Sub IDisposable_Dispose() Implements IDisposable.Dispose
            Me._state = -3
        End Sub
        Public Function MoveNext() As Boolean
            If (Me._state = -3) Then
                Throw New ObjectDisposedException("Enumerator")
            End If
            If (Me._state = -1) Then
                Return false
            End If
            Me._state = 0
            Dim parent As FARunner
            If (false = Me._parent.TryGetTarget(parent)) Then
                Throw New InvalidOperationException("The parent was destroyed")
            End If
            Me._current = parent.NextMatch
            If (Me._current.SymbolId = -2) Then
                Me._state = -2
                Return false
            End If
            Return true
        End Function
        Public Sub Reset()
            If (Me._state = -3) Then
                Throw New ObjectDisposedException("Enumerator")
            End If
            Dim parent As FARunner
            If (false = Me._parent.TryGetTarget(parent)) Then
                Throw New InvalidOperationException("The parent was destroyed")
            End If
            parent.Reset
            Me._state = -2
        End Sub
    End Class
    ''' <summary>
    ''' Indicates the width of a tab, in columns
    ''' </summary>
    Public Property TabWidth() As Integer
        Get
            Return Me._tabWidth
        End Get
        Set
            If (value < 1) Then
                Throw New ArgumentOutOfRangeException()
            End If
            Me._tabWidth = value
        End Set
    End Property
    Private _tabWidth As Integer
    Protected position As Integer
    Protected line As Integer
    Protected column As Integer
    Protected Shared Sub ThrowUnicode(ByVal pos As Integer)
        Throw New IOException(String.Concat("Unicode error in stream at position ", pos.ToString))
    End Sub
    Public MustOverride Function NextMatch() As FAMatch
    Public MustOverride Sub Reset()
    Public Function GetEnumerator() As IEnumerator(Of FAMatch) Implements IEnumerable(Of FAMatch).GetEnumerator
        Return New Enumerator(Me)
    End Function
    Function System_Collections_IEnumerable_GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return New Enumerator(Me)
    End Function
End Class
<System.CodeDom.Compiler.GeneratedCodeAttribute("Visual FA", "1.0.0.0")>  _
Partial Friend MustInherit Class FAStringRunner
    Inherits FARunner
    Public Shared ReadOnly Property UsingSpans() As Boolean
        Get
            Return false
        End Get
    End Property
    Protected [string] As String
    Public Sub [Set](ByVal [string] As String)
        Me.[string] = [string]
        Me.position = -1
        Me.line = 1
        Me.column = 1
    End Sub
    Public Overrides Sub Reset()
        Me.position = -1
        Me.line = 1
        Me.column = 1
    End Sub
    ' much bigger, but faster code
    <System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)>  _
    Protected Overridable Sub Advance(ByVal str As String, ByRef ch As Integer, ByRef len As Integer, ByVal first As Boolean)
        If (false = first) Then
            If (ch = 10) Then
                Me.line = (Me.line + 1)
                Me.column = 1
            Else
                If (ch = 13) Then
                    Me.column = 1
                Else
                    If (ch = 9) Then
                        Me.column = (((Me.column - 1)  _
                                    / Me.TabWidth)  _
                                    * (Me.TabWidth + 1))
                    Else
                        If (ch > 31) Then
                            Me.column = (Me.column + 1)
                        End If
                    End If
                End If
            End If
            len = (len + 1)
            If (ch > 65535) Then
                len = (len + 1)
            End If
            Me.position = (Me.position + 1)
        End If
        If (Me.position < str.Length) Then
            Dim ch1 As Char = str(Me.position)
            If Char.IsHighSurrogate(ch1) Then
                Me.position = (Me.position + 1)
                If (Me.position >= str.Length) Then
                    FAStringRunner.ThrowUnicode(Me.position)
                End If
                Dim ch2 As Char = str(Me.position)
                ch = Char.ConvertToUtf32(ch1, ch2)
            Else
                ch = System.Convert.ToInt32(ch1)
            End If
        Else
            ch = -1
        End If
    End Sub
End Class
<System.CodeDom.Compiler.GeneratedCodeAttribute("Visual FA", "1.0.0.0")>  _
Partial Friend MustInherit Class FATextReaderRunner
    Inherits FARunner
    Protected reader As TextReader
    Protected capture As StringBuilder
    Protected current As Integer
    Protected Sub New()
        MyBase.New
        Me.capture = New StringBuilder()
    End Sub
    Public Sub [Set](ByVal reader As TextReader)
        Me.reader = reader
        Me.current = -2
        Me.position = -1
        Me.line = 1
        Me.column = 1
    End Sub
    Public Overrides Sub Reset()
        Throw New NotSupportedException()
    End Sub
    Protected Overridable Sub Advance()
        If (Me.current = 10) Then
            Me.line = (Me.line + 1)
            Me.column = 1
        Else
            If (Me.current = 13) Then
                Me.column = 1
            Else
                If (Me.current = 9) Then
                    Me.column = (((Me.column - 1)  _
                                / Me.TabWidth)  _
                                * (Me.TabWidth + 1))
                Else
                    If (Me.current > 31) Then
                        Me.column = (Me.column + 1)
                    End If
                End If
            End If
        End If
        If (Me.current > -1) Then
            Me.capture.Append(Char.ConvertFromUtf32(Me.current))
        End If
        Me.current = Me.reader.Read
        If (Me.current = -1) Then
            Return
        End If
        Me.position = (Me.position + 1)
        Dim ch1 As Char = System.Convert.ToChar(Me.current)
        If Char.IsHighSurrogate(ch1) Then
            Me.current = Me.reader.Read
            If (Me.current = -1) Then
                FATextReaderRunner.ThrowUnicode(Me.position)
            End If
            Dim ch2 As Char = System.Convert.ToChar(Me.current)
            Me.current = Char.ConvertToUtf32(ch1, ch2)
            Me.position = (Me.position + 1)
        End If
    End Sub
End Class
<System.CodeDom.Compiler.GeneratedCodeAttribute("Visual FA", "1.0.0.0")>  _
Partial Friend NotInheritable Class CommentRunner
    Inherits FAStringRunner
    Private Function _BlockEnd0(ByVal s As String, ByVal cp As Integer, ByVal len As Integer, ByVal position As Integer, ByVal line As Integer, ByVal column As Integer) As FAMatch
    q0:
        '[\*]
        If (cp = 42) Then
            Me.Advance(s, cp, len, false)
            goto q1
        End If
        goto errorout
    q1:
        '[\/]
        If (cp = 47) Then
            Me.Advance(s, cp, len, false)
            goto q2
        End If
        goto errorout
    q2:
        Return FAMatch.Create(0, s.Substring(position, len), position, line, column)
    errorout:
        If (cp = -1) Then
            Return FAMatch.Create(-1, s.Substring(position, len), position, line, column)
        End If
        Me.Advance(s, cp, len, false)
        goto q0
    End Function
    Private Function NextMatchImpl(ByVal s As String) As FAMatch
        Dim ch As Integer
        Dim len As Integer
        Dim p As Integer
        Dim l As Integer
        Dim c As Integer
        ch = -1
        len = 0
        If (Me.position = -1) Then
            Me.position = 0
        End If
        p = Me.position
        l = Me.line
        c = Me.column
        Me.Advance(s, ch, len, true)
        'q0:
        '[\/]
        If (ch = 47) Then
            Me.Advance(s, ch, len, false)
            goto q1
        End If
        goto errorout
    q1:
        '[\*]
        If (ch = 42) Then
            Me.Advance(s, ch, len, false)
            goto q2
        End If
        '[\/]
        If (ch = 47) Then
            Me.Advance(s, ch, len, false)
            goto q3
        End If
        goto errorout
    q2:
        Return _BlockEnd0(s, ch, len, p, l, c)
    q3:
        '[\0-\t\v-\x10ffff]
        If (((ch >= 0)  _
                    AndAlso (ch <= 9))  _
                    OrElse ((ch >= 11)  _
                    AndAlso (ch <= 1114111))) Then
            Me.Advance(s, ch, len, false)
            goto q3
        End If
        Return FAMatch.Create(1, s.Substring(p, len), p, l, c)
    errorout:
        If ((ch = -1)  _
                    OrElse (ch = 47)) Then
            If (len = 0) Then
                Return FAMatch.Create(-2, Nothing, 0, 0, 0)
            End If
            Return FAMatch.Create(-1, s.Substring(p, len), p, l, c)
        End If
        Me.Advance(s, ch, len, false)
        goto errorout
    End Function
    Public Overrides Function NextMatch() As FAMatch
        Return Me.NextMatchImpl(Me.[string])
    End Function
End Class
