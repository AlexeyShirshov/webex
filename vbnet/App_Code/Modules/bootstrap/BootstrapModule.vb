
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Mvc

Partial Public Class WebExModel
    Private _bs As IModule
    <LoadModel> _
    Public Sub LoadBootstrap(ctrl As Controller)
        _bs = New BootstrapModule(ctrl)
    End Sub

    Public ReadOnly Property BootstrapModule() As IModule
        Get
            Return _bs
        End Get
    End Property
End Class
Public Class BootstrapModule
    Implements IModule
    Private _ctrl As Controller
    Public Sub New(ctrl As Controller)
        _ctrl = ctrl
    End Sub
    Public Function GetViewOfType(type As String) As String Implements IModule.GetViewOfType
        Select Case type
            Case "css"
                Return String.Format("<link href='{0}' rel='stylesheet'/>", _ctrl.Url.Content("~/Content/bootstrap/bootstrap.css"))
            Case "corejs"
                Return String.Format("<script src='{0}'></script>", _ctrl.Url.Content("~/Scripts/bootstrap.js"))
        End Select

        Return Nothing
    End Function
End Class
