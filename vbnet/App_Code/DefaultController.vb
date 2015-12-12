
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Mvc

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
        Dim model As New WebExModel()
        model.Load(Me, page, id)
        Return View(model)
    End Function
End Class
