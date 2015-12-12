Partial Public Class AppConfig
    Public Shared PluginsRegistry As New Plugins()

    Public Shared Sub RegisterPlugins(app As System.Web.HttpApplication)
        'app.Application(WebExModel.webexPluginsRegistry) = PluginsRegistry
    End Sub
End Class