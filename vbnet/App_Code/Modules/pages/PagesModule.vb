
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Mvc

Partial Public Class WebExModel
    Private _page As IModuleWithModel
    <LoadModel> _
    Public Sub LoadPage(ctrl As Controller, page As String)
        _page = New PagesModule.PagesModule(page, CreatePage(page))
    End Sub

    Private Function CreatePage(page As String) As PagesModule.Page
        Dim title As String = If(String.IsNullOrEmpty(page), "Главная", "О компании")
        Return New PagesModule.Page() With { _
            .Name = page, _
            .Title = title _
        }
    End Function

    Public ReadOnly Property Page() As IModuleWithModel
        Get
            Return _page
        End Get
    End Property
End Class

Namespace PagesModule
    Public Class Page
        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(value As String)
                m_Name = Value
            End Set
        End Property
        Private m_Name As String
        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(value As String)
                m_Title = Value
            End Set
        End Property
        Private m_Title As String
    End Class

    Public Class PagesModule
        Implements IModuleWithModel
        'private string _name;
        Private _view As String
        Private _page As Page
        Public Sub New(name As String, page As Page)
            '_name = name;
            _page = page
            _view = "index"
        End Sub
        'public string Name
        '{
        '    get
        '    {
        '        return _name;
        '    }
        '}

        'public string View
        '{
        '    get
        '    {
        '        return _view;
        '    }
        '}

        Public ReadOnly Property Model() As Object Implements IModuleWithModel.Model
            Get
                Return _page
            End Get
        End Property

        Public Function GetViewOfType(type As String) As String Implements IModule.GetViewOfType
            Select Case type
                Case WebExHtmlExtensions.DefaultViewType
                    Return WebExHtmlExtensions.DefaultViewType
                Case "js"
                    Return "index.js"
            End Select

            Return Nothing
        End Function
    End Class
End Namespace
