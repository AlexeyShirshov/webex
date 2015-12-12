using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

/// <summary>
/// Summary description for WebExHtmlExtensions
/// </summary>
public static class WebExHtmlExtensions
{
    public const string DefaultViewType = "webex:defaulviewtype";
    public const string webexViewExtension = "webex:viewext";
    public static string GetViewExtension(HttpApplicationStateBase app)
    {
        var ext = app[webexViewExtension] as string;
        if (string.IsNullOrEmpty(ext))
            ext = "cshtml";

        return ext;
    }
    public static MvcHtmlString RenderModule(this HtmlHelper helper, IModule module, string view = "", object moduleModel = null)
    {        
        if (module == null)
        {
            var viewName = string.Format("~/Views/Modules/{0}.{1}", view, GetViewExtension(helper.ViewContext.HttpContext.Application));
            return helper.Partial(viewName, new WebExModuleNotFoundModel(null, viewName, moduleModel));
        }

        var v = view;
        if (string.IsNullOrEmpty(v))
            v = module.GetViewOfType(DefaultViewType);

        var modelModule = module as IModuleWithModel;
        if (modelModule == null)
        {
            return new MvcHtmlString(v);
        }
        else
        {
            if (string.IsNullOrEmpty(v) || v == DefaultViewType)
                v = "index";

            return helper.RenderModule(GetModuleName(module), (object)modelModule.Model, v);
        }
    }

    private static string GetModuleName(IModule module)
    {
        var t = module.GetType();
        return t.Name.Replace("Module", string.Empty);
    }
    public static MvcHtmlString RenderModule(this HtmlHelper helper, string module, object model = null, string view = "index")
    {
        var viewName = string.Format("~/Views/Modules/{0}/{1}.{2}", module, view, GetViewExtension(helper.ViewContext.HttpContext.Application));
        if (helper.PartialViewExists(viewName))
        {
            return helper.Partial(viewName, model);
        }

        viewName = string.Format("~/Views/Modules/{0}.{1}", view, GetViewExtension(helper.ViewContext.HttpContext.Application));
        if (helper.PartialViewExists(viewName))
        {
            return helper.Partial(viewName, new WebExModuleNotFoundModel(module, viewName, model));
        }
        else
        {
            if (helper.PartialViewExists(string.Format("~/Views/Modules/index.{0}", GetViewExtension(helper.ViewContext.HttpContext.Application))))
                return helper.Partial(string.Format("~/Views/Modules/index.{0}", GetViewExtension(helper.ViewContext.HttpContext.Application)), 
                    new WebExModuleNotFoundModel(module, viewName, model));
        }

        return null;
    }
    public static MvcHtmlString RenderModule(this HtmlHelper helper, WebExModel model, string module, string view = "index", object moduleModel = null)
    {
        object res;
        if (model.TryGetProperty(module, out res))
        {
            IModule m = res as IModule;
            if (m != null)
                return helper.RenderModule(m, view, moduleModel);
            else
                return helper.RenderModule(module, res, view);
        }
        return null;
    }
    public static MvcHtmlString RenderPartialIfExists(this HtmlHelper html, string partialViewName)
    {
        if (html.PartialViewExists(partialViewName))
            return html.Partial(partialViewName);

        return null;
    }
    public static bool PartialViewExists(this HtmlHelper html, string partialViewName)
    {
        return ViewEngines.Engines.FindPartialView(html.ViewContext.Controller.ControllerContext, partialViewName).View != null;
    }
    public static bool ViewExists(this HtmlHelper html, string viewName, string master = null)
    {
        return ViewEngines.Engines.FindView(html.ViewContext.Controller.ControllerContext, viewName, master).View != null;
    }
    public static IEnumerable<MvcHtmlString> RenderModulesViewOfType(this HtmlHelper helper, WebExModel model, string viewType,
        Func<IModule, int> getOrderWeight = null)
    {
        foreach (var module in model.GetMultiViewModules().OrderBy(it=>getOrderWeight == null?0:getOrderWeight(it)))
        {
            var view = module.GetViewOfType(viewType);
            if (!string.IsNullOrEmpty(view))
                yield return helper.RenderModule(module, view: view);
        }
    }
}