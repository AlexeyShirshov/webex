﻿
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
    <ModuleAlias("Pages"), Dependency("JQuery")>
    Public Class PagesModule
        Implements IModuleWithModel, IPreRenderFilter ', IPostRenderFilter

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
            Return New Page() With {
                .Name = page,
                .Title = title
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

        Public Function Exec(helper As HtmlHelper, moduleInstanceId As String, args As IDictionary(Of String, Object), renderedViewName As String, view As IModuleView) As PreRenderFilterResult Implements IPreRenderFilter.Exec
            If view.IsAuto Then
                Return New PreRenderFilterResult With {.View = New ModuleViewString(Function(model, id, args2) String.Format("<div id='{0}'>", id)), .PostRenderView = New ModuleViewString("</div>")}
            End If
        End Function

        'Public Function Exec(helper As HtmlHelper, moduleInstanceId As String, args As IDictionary(Of String, Object), renderedViewName As String, view As IModuleView, mainViewResult As MvcHtmlString) As RenderFilterResult Implements IPostRenderFilter.Exec
        '    If view.IsAuto Then

        '    End If
        'End Function
    End Class
End Namespace
