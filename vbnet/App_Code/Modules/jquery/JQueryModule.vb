
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Mvc

Public Class JQueryExternalModule
    Implements IExternalModule
    Implements IModule
    Private _ctrl As Controller
    Public Sub Load(model As Object, ParamArray args As Object()) Implements IExternalModule.Load
        model.JQuery = Me
        If args IsNot Nothing Then
            _ctrl = args.OfType(Of Controller)().FirstOrDefault()
        End If
    End Sub

    Public Function GetViewOfType(type As String) As String Implements IModule.GetViewOfType
        If type = "corejs" Then
            Return String.Format("<script src='{0}'></script>", _ctrl.Url.Content("/Scripts/jquery-2.1.4.js"))
        End If

        Return Nothing
    End Function
End Class
