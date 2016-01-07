using System;
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
            case Contracts.CSSView:
                return new ModuleViewString("<link href='{0}' rel='stylesheet'/>", url.Content("~/Content/bootstrap/bootstrap.css"));
            case Contracts.JavascriptView:
                return new ModuleViewString("<script src='{0}'></script>", url.Content("~/Scripts/bootstrap.js"));
        }

        return null;
    }
}