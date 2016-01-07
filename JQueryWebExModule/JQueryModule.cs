using System;
using System.Web;
using System.Web.Mvc;
using WebEx.Core;
/// <summary>
/// Usage @Html.RenderModule("JQuery")
/// </summary>
public class JQueryModule : IModule
{
    public IModuleView GetView(string type, HtmlHelper html)
    {
        var url = new UrlHelper(html.ViewContext.RequestContext);
        if (type == Contracts.JavascriptView || type == Contracts.DefaultView)
            return new ModuleViewString("<script src='{0}'></script>" ,url.Content("~/Scripts/jquery-2.1.4.js"));

        return null;
    }
}