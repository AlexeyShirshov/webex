
Imports System.Collections.Generic
Imports System.Dynamic
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Web
Imports System.Web.Mvc
Imports System.Web.Mvc.Html

''' <summary>
''' Summary description for WebExHtmlExtensions
''' </summary>
Public Module WebExHtmlExtensions
    Public Const DefaultViewType As String = "webex:defaulviewtype"
    Public Const webexViewExtension As String = "webex:viewext"
    Public Function GetViewExtension(app As HttpApplicationStateBase) As String
        Dim ext = TryCast(app(webexViewExtension), String)
        If String.IsNullOrEmpty(ext) Then
            ext = "cshtml"
        End If

        Return ext
    End Function
    <System.Runtime.CompilerServices.Extension> _
    Public Function RenderModule(helper As HtmlHelper, [module] As IModule, Optional view As String = "", Optional moduleModel As Object = Nothing) As MvcHtmlString
        If [module] Is Nothing Then
            Dim viewName = String.Format("~/Views/Modules/{0}.{1}", view, GetViewExtension(helper.ViewContext.HttpContext.Application))
            Return helper.[Partial](viewName, New WebExModuleNotFoundModel(Nothing, viewName, moduleModel))
        End If

        Dim v = view
        If String.IsNullOrEmpty(v) Then
            v = [module].GetViewOfType(DefaultViewType)
        End If

        Dim modelModule = TryCast([module], IModuleWithModel)
        If modelModule Is Nothing Then
            Return New MvcHtmlString(v)
        Else
            If String.IsNullOrEmpty(v) OrElse v = DefaultViewType Then
                v = "index"
            End If

            Return helper.RenderModule(GetModuleName([module]), DirectCast(modelModule.Model, Object), v)
        End If
    End Function

    Private Function GetModuleName([module] As IModule) As String
        Dim t = [module].[GetType]()
        Return t.Name.Replace("Module", String.Empty)
    End Function
    <System.Runtime.CompilerServices.Extension> _
    Public Function RenderModule(helper As HtmlHelper, [module] As String, Optional model As Object = Nothing, Optional view As String = "index") As MvcHtmlString
        Dim viewName = String.Format("~/Views/Modules/{0}/{1}.{2}", [module], view, GetViewExtension(helper.ViewContext.HttpContext.Application))
        If helper.PartialViewExists(viewName) Then
            Return helper.[Partial](viewName, model)
        End If

        viewName = String.Format("~/Views/Modules/{0}.{1}", view, GetViewExtension(helper.ViewContext.HttpContext.Application))
        If helper.PartialViewExists(viewName) Then
            Return helper.[Partial](viewName, New WebExModuleNotFoundModel([module], viewName, model))
        Else
            If helper.PartialViewExists(String.Format("~/Views/Modules/index.{0}", GetViewExtension(helper.ViewContext.HttpContext.Application))) Then
                Return helper.[Partial](String.Format("~/Views/Modules/index.{0}", GetViewExtension(helper.ViewContext.HttpContext.Application)), New WebExModuleNotFoundModel([module], viewName, model))
            End If
        End If

        Return Nothing
    End Function
    <System.Runtime.CompilerServices.Extension> _
    Public Function RenderModule(helper As HtmlHelper, model As WebExModel, [module] As String, Optional view As String = "index", Optional moduleModel As Object = Nothing) As MvcHtmlString
        Dim res As Object
        If model.TryGetProperty([module], res) Then
            Dim m As IModule = TryCast(res, IModule)
            If m IsNot Nothing Then
                Return helper.RenderModule(m, view, moduleModel)
            Else
                Return helper.RenderModule([module], res, view)
            End If
        End If
        Return Nothing
    End Function
    <System.Runtime.CompilerServices.Extension> _
    Public Function RenderPartialIfExists(html As HtmlHelper, partialViewName As String) As MvcHtmlString
        If html.PartialViewExists(partialViewName) Then
            Return html.[Partial](partialViewName)
        End If

        Return Nothing
    End Function
    <System.Runtime.CompilerServices.Extension> _
    Public Function PartialViewExists(html As HtmlHelper, partialViewName As String) As Boolean
        Return ViewEngines.Engines.FindPartialView(html.ViewContext.Controller.ControllerContext, partialViewName).View IsNot Nothing
    End Function
    <System.Runtime.CompilerServices.Extension> _
    Public Function ViewExists(html As HtmlHelper, viewName As String, Optional master As String = Nothing) As Boolean
        Return ViewEngines.Engines.FindView(html.ViewContext.Controller.ControllerContext, viewName, master).View IsNot Nothing
    End Function
    <System.Runtime.CompilerServices.Extension> _
    Public Iterator Function RenderModulesViewOfType(helper As HtmlHelper, model As WebExModel, viewType As String, Optional getOrderWeight As Func(Of IModule, Integer) = Nothing) As IEnumerable(Of MvcHtmlString)
        For Each [module] In model.GetMultiViewModules().OrderBy(Function(it) If(getOrderWeight Is Nothing, 0, getOrderWeight(it)))
            Dim view = [module].GetViewOfType(viewType)
            If Not String.IsNullOrEmpty(view) Then
                Yield helper.RenderModule([module], view:=view)
            End If
        Next
    End Function
End Module

