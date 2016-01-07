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
            //var ext = app[webexViewExtension] as string;
            //if (string.IsNullOrEmpty(ext))
            //    ext = "cshtml";

            return app[webexViewExtension] as string;
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, IModule module, IModuleView view, object moduleModel = null)
        {
            //string.IsNullOrEmpty(view)?(IModuleView)new DefaultView():new ModuleView(view)
            if (module == null)
            {
                return helper.RenderModuleManual(null, view, moduleModel);
            }

            if (view != null)
            {
                var modelModule = module as IModuleWithModel;
                var mstr = view as ModuleViewString;
                if (mstr != null)
                {
                    return new MvcHtmlString(mstr.Value);
                }
                else if (modelModule != null)
                {
                    return helper.RenderModuleManual(GetModuleFolder(module), view, (object)modelModule.Model);
                }
            }
            return null;
        }

        private static string GetModuleFolder(IModule module)
        {
            return GetModuleFolder(module.GetType());
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
        private static bool TryGetProp(object model, IModuleView view, ref string val)
        {
            if (model == null)
                return false;

            if (view == null || string.IsNullOrEmpty(view.Value) || view.Value == Contracts.DefaultView)
                return false;

            var props = from k in model.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty)
                        where string.Equals(view.Value, k.Name, StringComparison.InvariantCultureIgnoreCase) && k.PropertyType == typeof(string)
                        select k;

            var prop = props.FirstOrDefault();

            if (prop != null)
            {
                val = prop.GetValue(model) as string;
                return true;
            }

            return false;
        }
        public static MvcHtmlString RenderModuleManual(this HtmlHelper helper, string moduleFolder, IModuleView view, object model = null)
        {
            var exts = new string[] { };
            var ext = GetViewExtension(helper.ViewContext.HttpContext.Application);
            if (string.IsNullOrEmpty(ext))
                exts = new [] {"cshtml", "vbhtml"};
            else
                exts = new [] {ext.Trim('.')};

            foreach (var extension in exts)
            {
                MvcHtmlString res;
                if (RenderModuleInternal(helper, moduleFolder, view, model, extension, out res))
                    return res;
            }

            return null;
        }
        public static bool RenderModuleInternal(this HtmlHelper helper, string moduleFolder, IModuleView view, object model, string ext, out MvcHtmlString res)
        {
            string realViewName = null;
            if (view.IsDefault())
                realViewName = "index";
            else if (view != null)
                realViewName = view.Value;

            if (string.IsNullOrEmpty(realViewName) || realViewName == Contracts.DefaultView)
                realViewName = "index";

            if (!string.IsNullOrEmpty(moduleFolder))
            {
                string val = null;
                
                var viewNameInner = string.Format("~/Views/{3}/{0}/{1}.{2}", moduleFolder, realViewName, ext, ModulesFolder);

                if (helper.PartialViewExists(viewNameInner, model))
                {
                    res = helper.Partial(viewNameInner, model);
                    return true;
                }
                else if (view.IsDefault() || (view.IsAuto() && view != null && (string.IsNullOrEmpty(view.Value) || view.Value == Contracts.DefaultView)))
                {
                    realViewName = "default";
                    viewNameInner = string.Format("~/Views/{3}/{0}/{1}.{2}", moduleFolder, realViewName, ext, ModulesFolder);
                    if (helper.PartialViewExists(viewNameInner, model))
                    {
                        res =  helper.Partial(viewNameInner, model);
                        return true;
                    }
                    //else if (TryGetProp(model, view, ref val) && !string.IsNullOrEmpty(val))
                    //{
                    //    res = new MvcHtmlString(val);
                    //    return true;
                    //}
                    //else if (view.IsAuto())
                    //{
                    //    res = null;
                    //    return false;
                    //}
                }
                
                if (TryGetProp(model, view, ref val) && !string.IsNullOrEmpty(val))
                {
                    res = new MvcHtmlString(val);
                    return true;
                }
                else if (view.IsAuto())
                {
                    res = null;
                    return false;
                }
            }

            var viewName = string.Format("~/Views/{2}/{0}.{1}", realViewName, ext, ModulesFolder);
            if (helper.PartialViewExists(viewName, model))
            {
                res = helper.Partial(viewName, new WebExModuleNotFoundModel(moduleFolder, view, model));
                return true;
            }
            else if (view.IsDefault())
            {
                realViewName = "default";
                viewName = string.Format("~/Views/{2}/{0}.{1}", realViewName, ext, ModulesFolder);
                if (helper.PartialViewExists(viewName, model))
                {
                    res = helper.Partial(viewName, new WebExModuleNotFoundModel(moduleFolder, view, model));
                    return true;
                }
            }

            res = null;
            return false;
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName, string view = null, object moduleModel = null, 
            bool ignoreCase = false)
        {
            Type mt = ModulesCatalog.GetModule(helper.ViewContext.HttpContext.Application, moduleName, ignoreCase);
            if (mt != null)
            {
                object res;
                if (mt != null && helper.ViewData.TryGetValue(WebExModuleExtensions.MakeViewDataKey(mt), out res))
                {
                    IModule m = res as IModule;
                    if (m != null)
                    {
                        if (string.IsNullOrEmpty(view))
                            view = Contracts.DefaultView;

                        return helper.RenderModule(m, m.GetView(view, helper), moduleModel);
                    }
                }
            }

            return helper.RenderModuleManual(moduleName, new ModuleAutoView(view), moduleModel);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, Type module, string view = null, object moduleModel = null)
        {
            object res;
            if (module != null && helper.ViewData.TryGetValue(WebExModuleExtensions.MakeViewDataKey(module), out res))
            {
                IModule m = res as IModule;
                if (m != null)
                {
                    if (string.IsNullOrEmpty(view))
                        view = Contracts.DefaultView;

                    return helper.RenderModule(m, m.GetView(view, helper), moduleModel);
                }
                //else
                //    return helper.RenderModuleManual(GetModuleFolder(module), res, view);
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
            foreach (var item in helper.ViewData)
            {
                if (item.Key.StartsWith(WebExModuleExtensions._webexInternalModuleInstances))
                {
                    var module = item.Value as IModule;
                    if (module != null)
                        yield return module;
                }
            }
        }
        public static IModule GetModule(this HtmlHelper helper, string moduleName, bool ignoreCase = false)
        {
            return WebExModuleExtensions.GetModule(helper.ViewData, ModulesCatalog.GetModule(helper.ViewContext.HttpContext.Application, moduleName, ignoreCase));
        }
        //public static MvcHtmlString RenderPartialIfExists(this HtmlHelper html, string partialViewName, object model = null)
        //{
        //    if (html.PartialViewExists(partialViewName, model))
        //        return html.Partial(partialViewName, model);

        //    return null;
        //}
        public static bool PartialViewExists(this HtmlHelper html, string partialViewName, object model)
        {
            var vr = ViewEngines.Engines.FindPartialView(html.ViewContext.Controller.ControllerContext, partialViewName);
            var bm = vr.View as BuildManagerCompiledView;
            if (bm != null && model == null && html.ViewData.Model != null && html.ViewData.ModelMetadata != null)
            {
                var rv = System.Web.Compilation.BuildManager.GetCompiledType(bm.ViewPath);
                if (typeof(WebViewPage).IsAssignableFrom(rv) && rv.BaseType.IsGenericType)
                {
                    return html.ViewData.ModelMetadata.ModelType == rv.BaseType.GetGenericArguments()[0];
                }                
            }
            return vr.View != null;
        }        
        //public static bool ViewExists(this HtmlHelper html, string viewName, string master = null)
        //{
        //    return ViewEngines.Engines.FindView(html.ViewContext.Controller.ControllerContext, viewName, master).View != null;
        //}
        public static IEnumerable<MvcHtmlString> RenderModules(this HtmlHelper helper, string viewType,
            Func<IModule, int> getOrderWeight = null)
        {
            foreach (var module in GetModules(helper).OrderBy(it => getOrderWeight == null ? 0 : getOrderWeight(it)).
                ThenBy(it => it, new DependencyComparer(helper.ViewContext.HttpContext.Application)))
            {

                var view = module.GetView(viewType, helper);
                if (view != null)
                    yield return helper.RenderModule(module, view);
            }
        }
    }
}