using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace WebEx.Core
{

    /// <summary>
    /// Summary description for WebExHtmlExtensions
    /// </summary>
    public static class WebExHtmlExtensions
    {
        public const string ModulesFolder = "webex.modules";
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
                return helper.RenderModuleCustom(null, null, view);
            }

            var v = view;
            if (string.IsNullOrEmpty(v))
                v = module.GetViewOfType(Contracts.DefaultView, helper);

            var modelModule = module as IModuleWithModel;
            if (modelModule == null)
            {
                return new MvcHtmlString(v);
            }
            else
            {
                return helper.RenderModuleCustom(GetModuleFolder(module), (object)modelModule.Model, v);
            }
        }

        private static string GetModuleFolder(IModule module)
        {
            return GetModuleFolder(module.GetType().Name);
        }
        private static string GetModuleFolder(string moduleName)
        {            
            if (moduleName.EndsWith("module", StringComparison.InvariantCultureIgnoreCase))
                moduleName = moduleName.Substring(0, moduleName.Length - 6);
            return moduleName;
        }
        private static string GetModuleFolder(Type module)
        {
            return GetModuleFolder(module.Name);
        }
        public static MvcHtmlString RenderModuleCustom(this HtmlHelper helper, string moduleFolder, object model = null, string view = "index")
        {
            var realViewName = view;
            if (view == Contracts.DefaultView)
                realViewName = "index";

            if (!string.IsNullOrEmpty(moduleFolder))
            {
                var viewNameInner = string.Format("~/Views/{3}/{0}/{1}.{2}", moduleFolder, realViewName,
                    GetViewExtension(helper.ViewContext.HttpContext.Application), ModulesFolder);

                if (helper.PartialViewExists(viewNameInner))
                {
                    return helper.Partial(viewNameInner, model);
                }
                else if (view == Contracts.DefaultView)
                {
                    realViewName = "default";
                    viewNameInner = string.Format("~/Views/{3}/{0}/{1}.{2}", moduleFolder, realViewName,
                        GetViewExtension(helper.ViewContext.HttpContext.Application), ModulesFolder);
                    if (helper.PartialViewExists(viewNameInner))
                    {
                        return helper.Partial(viewNameInner, model);
                    }
                }
            }

            var viewName = string.Format("~/Views/{2}/{0}.{1}", realViewName, 
                GetViewExtension(helper.ViewContext.HttpContext.Application), ModulesFolder);
            if (helper.PartialViewExists(viewName))
            {
                return helper.Partial(viewName, new WebExModuleNotFoundModel(moduleFolder, view, model));
            }
            else if (view == Contracts.DefaultView)
            {
                realViewName = "default";
                viewName = string.Format("~/Views/{2}/{0}.{1}", realViewName,
                    GetViewExtension(helper.ViewContext.HttpContext.Application), ModulesFolder);
                if (helper.PartialViewExists(viewName))
                {
                    return helper.Partial(viewName, new WebExModuleNotFoundModel(moduleFolder, view, model));
                }
            }

            return null;
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName, string view = "index", object moduleModel = null, bool ignoreCase = false)
        {
            Type mt = WebExModuleExtensions.GetModule(helper.ViewData, moduleName, ignoreCase);
            if (mt != null)
            {
                return RenderModule(helper, mt, view, moduleModel);
            }

            return helper.RenderModuleCustom(moduleName, moduleModel, view); ;
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, Type module, string view = "index", object moduleModel = null)
        {
            object res;
            if (module != null && helper.ViewData.TryGetValue(WebExModuleExtensions.MakeViewDataKey(module), out res))
            {
                IModule m = res as IModule;
                if (m != null)
                    return helper.RenderModule(m, view, moduleModel);
                else
                    return helper.RenderModuleCustom(GetModuleFolder(module), res, view);
            }

            return null;
        }
        public static bool HasModule(this HtmlHelper helper, string moduleName, bool ignoreCase = false)
        {
            Type mt = Type.GetType(moduleName, false, ignoreCase);
            if (mt != null)
            {
                return helper.ViewData.ContainsKey(WebExModuleExtensions.MakeViewDataKey(mt));
            }

            return false;
        }

        public static bool HasModule(this HtmlHelper helper, Type module)
        {
            return helper.ViewData.ContainsKey(WebExModuleExtensions.MakeViewDataKey(module));
        }
        public static IEnumerable<IModule> GetModules(this HtmlHelper helper)
        {
            foreach(var item in helper.ViewData)
            {
                if (item.Key.StartsWith(WebExModuleExtensions._webexInternalModuleInstances))
                {
                    var module = item.Value as IModule;
                    if (module != null)
                        yield return module;
                }
            }
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
        public static IEnumerable<MvcHtmlString> RenderModulesViewOfType(this HtmlHelper helper, string viewType,
            Func<IModule, int> getOrderWeight = null)
        {
            foreach (var module in GetModules(helper).OrderBy(it => getOrderWeight == null ? 0 : getOrderWeight(it)).
                ThenBy(it=>it, new DependencyComparer(helper.ViewData)))
            {

                var view = module.GetViewOfType(viewType, helper);
                if (!string.IsNullOrEmpty(view))
                    yield return helper.RenderModule(module, view: view);
            }
        }
    }
}