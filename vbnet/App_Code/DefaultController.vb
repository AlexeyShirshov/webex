
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Mvc
Imports WebEx.Core

''' <summary>
''' Summary description for WebExController
''' </summary>
Public Class DefaultController
    Inherits Controller
    '
    ' TODO: Add constructor logic here
    '
    Public Sub New()
    End Sub

    Public Function index(Optional page As String = "", Optional id As String = "") As ActionResult
        LoadModules(page, id)
        Return View()
    End Function
End Class
