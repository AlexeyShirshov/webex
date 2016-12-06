
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Mvc
Imports WebEx.Core

Namespace PagesModule
    Public Class Page
        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(value As String)
                m_Name = value
            End Set
        End Property
        Private m_Name As String
        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(value As String)
                m_Title = value
            End Set
        End Property
        Private m_Title As String
    End Class
    <ModuleAlias("Pages")>
    Public Class PagesModule
        Implements IModuleWithModel
        'private string _name;
        Private _view As String
        Private _page As Page
        Public Sub New(name As String, page As String)
            '_name = name;
            _page = CreatePage(name)
            _view = "index"
        End Sub
        Private Shared Function CreatePage(page As String) As Page
            Dim title As String = If(String.IsNullOrEmpty(page), "Главная", "О компании")
            Return New Page() With { _
                .Name = page, _
                .Title = title _
            }
        End Function

        Public ReadOnly Property Model() As Object Implements IModuleWithModel.Model
            Get
                Return _page
            End Get
        End Property

        Public Function GetViewOfType(type As String, html As HtmlHelper) As IModuleView Implements IModule.GetView
            Select Case type
                Case "js"
                    Return New ModuleView("index.js")
                Case Else
                    Return New ModuleAutoView(type)
            End Select
        End Function
    End Class
End Namespace
