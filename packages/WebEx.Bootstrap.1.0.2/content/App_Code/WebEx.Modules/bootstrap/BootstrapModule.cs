using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebEx.Core;

[Dependency("JQuery")]
public class BootstrapModule : IModule    
{    
    public IModuleView GetView(string type, HtmlHelper html)
    {
        var url = new UrlHelper(html.ViewContext.RequestContext);
        switch(type)
        {
            case "css":
                return new ModuleViewString("<link href='{0}' rel='stylesheet'/>", url.Content("~/Content/bootstrap/bootstrap.css"));
            case "js":
                return new ModuleViewString("<script src='{0}'></script>", url.Content("~/Scripts/bootstrap.js"));
        }

        return null;
    }
}