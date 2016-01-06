
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Mvc
Imports WebEx.Core

Public Class JQueryModule
    Implements IModule

    Public Function GetViewOfType(type As String, html As HtmlHelper) As IModuleView Implements IModule.GetView
        Dim url As New UrlHelper(html.ViewContext.RequestContext)
        If type = "js" Then
            Return New ModuleViewString(String.Format("<script src='{0}'></script>", url.Content("~/Scripts/jquery-2.1.4.js")))
        End If

        Return Nothing
    End Function
End Class
