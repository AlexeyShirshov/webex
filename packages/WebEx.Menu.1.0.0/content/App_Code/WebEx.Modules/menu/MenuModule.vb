
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Mvc
Imports WebEx.Core

Public Class MenuModule
    Implements IModuleWithModel

    Private _menu As IEnumerable(Of MenuItem)
    Public Sub New(ctrl As Controller)
        _menu = LoadMenu(ctrl)
    End Sub
    Public Shared Function LoadMenu(ctrl As Controller) As IEnumerable(Of MenuItem)
        Return {New MenuItem() With { _
            .Name = "Main", _
            .Url = ctrl.Url.Action("index", New With { _
                .page = "" _
            }) _
        }, New MenuItem() With { _
            .Name = "About", _
            .Url = ctrl.Url.Action("index", New With { _
                 .page = "about" _
            }) _
        }}
    End Function

    Public ReadOnly Property Menu() As IEnumerable(Of MenuItem)
        Get
            Return _menu
        End Get
    End Property

    Public Function GetViewOfType(type As String, helper As HtmlHelper) As String Implements IModule.GetViewOfType

    End Function

    Public ReadOnly Property Model As Object Implements IModuleWithModel.Model
        Get
            Return Menu
        End Get
    End Property
End Class

Public Class MenuItem
    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(value As String)
            m_Name = value
        End Set
    End Property
    Private m_Name As String
    Public Property Url() As String
        Get
            Return m_Url
        End Get
        Set(value As String)
            m_Url = value
        End Set
    End Property
    Private m_Url As String
End Class
