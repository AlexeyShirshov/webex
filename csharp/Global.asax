<%@ Application Language="C#" %>
<%@ Import namespace="System.Web.Mvc" %>
<%@ Import namespace="System.Web.Routing" %>
<%@ Import namespace="System.Reflection" %>
<%@ Import namespace="System.IO" %>
<%@ Import namespace="WebEx.Core" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        AppConfig.RegisterRoutes(RouteTable.Routes);
        ModulesCatalog.RegisterModules(Application, "cshtml");
        GlobalFilters.Filters.Add(new ModuleActionFilterAttribute());
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
       
</script>
