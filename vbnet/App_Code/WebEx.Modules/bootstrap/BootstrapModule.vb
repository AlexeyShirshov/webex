
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Mvc
Imports WebEx.Core

<Dependency("JQuery")>
Public Class BootstrapModule
    Implements IModule

    Public Function GetViewOfType(type As String, html As HtmlHelper) As String Implements IModule.GetViewOfType
        Dim url As New UrlHelper(html.ViewContext.RequestContext)
        Select Case type
            Case "css"
                Return String.Format("<link href='{0}' rel='stylesheet'/>", url.Content("~/Content/bootstrap/bootstrap.css"))
            Case "js"
                Return String.Format("<script src='{0}'></script>", url.Content("~/Scripts/bootstrap.js"))
        End Select

        Return Nothing
    End Function
End Class
