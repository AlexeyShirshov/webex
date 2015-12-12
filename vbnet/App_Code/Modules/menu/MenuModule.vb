
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Mvc

Partial Public Class WebExModel
    Private _menu As IEnumerable(Of MenuItem)
    <LoadModel> _
    Public Sub LoadMenu(ctrl As Controller)
        _menu = {New MenuItem() With { _
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
    End Sub
    <[Module]> _
    Public ReadOnly Property Menu() As IEnumerable(Of MenuItem)
        Get
            Return _menu
        End Get
    End Property
End Class

Public Class MenuItem
    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(value As String)
            m_Name = Value
        End Set
    End Property
    Private m_Name As String
    Public Property Url() As String
        Get
            Return m_Url
        End Get
        Set(value As String)
            m_Url = Value
        End Set
    End Property
    Private m_Url As String
End Class
