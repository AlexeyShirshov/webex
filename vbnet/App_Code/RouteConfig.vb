
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Routing
Imports System.Web.Mvc

''' <summary>
''' Summary description for RouteConfig
''' </summary>
Partial Public Class AppConfig
    Public Shared Sub RegisterRoutes(routes As RouteCollection)
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}")

        routes.MapRoute("Default", "{page}/{id}", New With { _
            .controller = "Default", _
            .action = "index", _
            .page = UrlParameter.[Optional], _
            .id = UrlParameter.[Optional] _
        })
    End Sub
End Class
