public partial class AppConfig
{
    public static Plugins PluginsRegistry = new Plugins();

    public static void RegisterPlugins(System.Web.HttpApplication app)
    {
        app.Application[WebExModel.webexPluginsRegistry] = PluginsRegistry;
    }
}