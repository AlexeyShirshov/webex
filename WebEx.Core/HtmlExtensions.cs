using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace WebEx.Core
{

    /// <summary>
    /// Summary description for WebExHtmlExtensions
    /// </summary>
    public static class WebExHtmlExtensions
    {
        public const string ModulesFolder = "webex.modules";
        public const string webexViewExtension = "webex:viewext";
        public const string webexModuleInstance = "webex:moduleInstance";
        public const string _webexInternalInlineModuleInstances = "webex:inlinemodules";
        public static string GetViewExtension(HttpApplicationStateBase app)
        {
            //var ext = app[webexViewExtension] as string;
            //if (string.IsNullOrEmpty(ext))
            //    ext = "cshtml";

            return app[webexViewExtension] as string;
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

        public static bool HasModule(this HtmlHelper helper, string moduleName, bool ignoreCase = false)
        {
            Type mt = Type.GetType(moduleName, false, ignoreCase);
            if (mt != null)
            {
                return helper.GetStorage().ContainsKey(WebExModuleExtensions.MakeViewDataKey(mt));
            }

            return false;
        }

        public static bool HasModule(this HtmlHelper helper, Type module)
        {
            return helper.GetStorage().ContainsKey(WebExModuleExtensions.MakeViewDataKey(module));
        }
        public static IEnumerable<IModule> GetModules(this HtmlHelper helper)
        {
            foreach (DictionaryEntry item in helper.ViewContext.RequestContext.HttpContext.Items)
            {
                if (item.Key.ToString().StartsWith(WebExModuleExtensions._webexInternalModuleInstances))
                {
                    var module = item.Value as IModule;
                    if (module != null)
                        yield return module;
                }
            }
        }
        public static IEnumerable<Tuple<string, object>> GetInlineModules(this HtmlHelper helper, string type)
        {
            object res;
            if (!helper.GetStorage().TryGetValue(_webexInternalInlineModuleInstances, out res))
            {
                res = new Tuple<string, string, object>[] { };
            }
            return from k in res as IEnumerable<Tuple<string, string, object>> where k.Item1 == type select new Tuple<string, object>(k.Item2, k.Item3);
        }
        public static void RegisterInlineModule(this HtmlHelper helper, string type, string view, object model)
        {
            object res;
            if (!helper.GetStorage().TryGetValue(_webexInternalInlineModuleInstances, out res))
            {
                res = new List<Tuple<string, string, object>>();
                helper.GetStorage()[_webexInternalInlineModuleInstances] = res;
            }

            var l = res as List<Tuple<string, string, object>>;

            if (!l.Any((it) => it.Item1 == type && it.Item2 == view && object.Equals(it.Item3, model)))
                l.Add(new Tuple<string, string, object>(type, view, model));
        }
        public static IModule GetModule(this HtmlHelper helper, string moduleName, bool ignoreCase = false)
        {
            return WebExModuleExtensions.GetModule(helper.ViewContext.RequestContext.HttpContext.Items, ModulesCatalog.GetModule(helper.ViewContext.HttpContext.Application, moduleName, ignoreCase));
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
        public static string GetViewPath(this HtmlHelper html)
        {
            if (html == null)
                return null;

            var wpb = html.ViewDataContainer as System.Web.WebPages.WebPageBase;
            if (wpb == null)
                return null;

            return wpb.VirtualPath;
        }
        #region RenderModule
        public static MvcHtmlString RenderModule(this HtmlHelper helper, IModule module, IDictionary<string, object> args, 
            IModuleView view, object model, string moduleInstanceId)
        {
            //string.IsNullOrEmpty(view)?(IModuleView)new DefaultView():new ModuleView(view)
            //if (module == null)
            //{
            //    return helper.RenderModuleManual(null, view, moduleModel);
            //}

            if (module != null && view != null)
            {
                var modelModule = module as IModuleWithModel;
                var mstr = view as ModuleViewString;
                if (mstr != null)
                {
                    return new MvcHtmlString(mstr.Value);
                }
                else 
                {
                    if (modelModule != null && model == null)
                        model = modelModule.Model;

                    var moduleFolder = GetModuleFolder(module);
                    var r = helper.RenderModuleManual(moduleFolder, view, model, moduleInstanceId, args);
                    if (r == null)
                    {
                        var curViewPath = helper.GetViewPath();
                        if (!string.IsNullOrEmpty(curViewPath))
                        {
                            var dir = System.IO.Path.GetDirectoryName(curViewPath);
                            moduleFolder = System.IO.Path.Combine(dir, moduleFolder);
                        }
                        r = helper.RenderModuleManual(moduleFolder, view, model, moduleInstanceId, args);
                    }

                    return r;
                }
            }
            return null;
        }

        public static MvcHtmlString RenderModuleManual(this HtmlHelper helper, string moduleFolder, IModuleView view, object model = null,
            string moduleInstanceId = null, IDictionary<string, object> args = null)
        {
            var exts = new string[] { };
            var ext = GetViewExtension(helper.ViewContext.HttpContext.Application);
            if (string.IsNullOrEmpty(ext))
                exts = new[] { "cshtml", "vbhtml" };
            else
                exts = new[] { ext.Trim('.') };

            foreach (var extension in exts)
            {
                MvcHtmlString res;
                string viewName;
                if (RenderModuleInternal(helper, moduleFolder, view, model, extension, out res, moduleInstanceId, args, out viewName))
                {
                    var cssViewName = viewName.Replace(extension, "css." + extension);
                    if (helper.PartialViewExists(cssViewName, model))
                        helper.RegisterInlineModule("css", cssViewName, model);

                    var jsViewName = viewName.Replace(extension, "js." + extension);
                    if (helper.PartialViewExists(jsViewName, model))
                        helper.RegisterInlineModule("js", jsViewName, model);

                    return res;
                }
            }

            //support for regular partial views
            var hasExt = !string.IsNullOrEmpty(System.IO.Path.GetExtension(moduleFolder));
            if (hasExt) exts = new[] { System.IO.Path.GetExtension(moduleFolder).Trim('.') };
            var defViews = new[] { "index", "default" };
            foreach (var defView in defViews)
            {
                foreach (var extension in exts)
                {
                    var viewPath = moduleFolder.TrimEnd('/');
                    if (view.IsDefault())
                    {
                        viewPath += "/" + defView;
                    }
                    else if (!view.IsEmpty())
                    {
                        viewPath += "/" + view.Value;
                    }

                    if (!hasExt)
                        viewPath += "." + extension;

                    if (helper.PartialViewExists(viewPath, model))
                    {
                        if (string.IsNullOrEmpty(moduleInstanceId))
                            moduleInstanceId = Guid.NewGuid().ToString();

                        MvcHtmlString res;
                        using (new AutoCleanup(() => helper.PrepareRender(moduleInstanceId, args), () => helper.CleanupRender(moduleInstanceId)))
                        {
                            res = helper.Partial(viewPath, model);
                        }

                        if (model == null)
                            model = helper.ViewData.Model;

                        var cssViewName = viewPath.Replace(extension, "css." + extension);
                        if (helper.PartialViewExists(cssViewName, model))
                            helper.RegisterInlineModule("css", cssViewName, model);

                        var jsViewName = viewPath.Replace(extension, "js." + extension);
                        if (helper.PartialViewExists(jsViewName, model))
                            helper.RegisterInlineModule("js", jsViewName, model);

                        return res;
                    }
                }
            }

            return null;
        }
        public static bool RenderModuleInternal(this HtmlHelper helper, string moduleFolder, IModuleView view, object model, string ext,
            out MvcHtmlString res, string moduleInstanceId, IDictionary<string, object> args, out string renderedViewName)
        {
            renderedViewName = null;
            if (string.IsNullOrEmpty(moduleInstanceId))
                moduleInstanceId = Guid.NewGuid().ToString();

            string realViewName = null;
            string extView = string.Empty;
            if (view.IsDefault())
                realViewName = "index";
            else if (view != null)
            {
                realViewName = view.Value;
                if (view.IsAuto())
                {
                    extView = (view as ModuleAutoView).Ext;
                }
            }
            
            if (string.IsNullOrEmpty(realViewName) || realViewName == Contracts.DefaultView)
                realViewName = "index";

            if (!string.IsNullOrEmpty(moduleFolder))
            {
                string val = null;

                var viewNameInner = string.Format("~/Views/{3}/{0}/{1}{4}.{2}", moduleFolder, realViewName, ext, ModulesFolder, extView);

                if (helper.PartialViewExists(viewNameInner, model))
                {
                    using (new AutoCleanup(() => helper.PrepareRender(moduleInstanceId, args), () => helper.CleanupRender(moduleInstanceId)))
                    {
                        res = helper.Partial(viewNameInner, model);
                    }
                    renderedViewName = viewNameInner;

                    return true;
                }
                else if (view.IsDefault() || (view.IsAuto() && view != null && (string.IsNullOrEmpty(view.Value) || view.Value == Contracts.DefaultView)))
                {
                    realViewName = "default";
                    viewNameInner = string.Format("~/Views/{3}/{0}/{1}{4}.{2}", moduleFolder, realViewName, ext, ModulesFolder, extView);
                    if (helper.PartialViewExists(viewNameInner, model))
                    {
                        using (new AutoCleanup(() => helper.PrepareRender(moduleInstanceId, args), () => helper.CleanupRender(moduleInstanceId)))
                        {
                            res = helper.Partial(viewNameInner, model);
                        }
                        renderedViewName = viewNameInner;
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

            var viewName = string.Format("~/Views/{2}/{0}{3}.{1}", realViewName, ext, ModulesFolder, extView);
            if (helper.PartialViewExists(viewName, model))
            {
                using (new AutoCleanup(() => helper.PrepareRender(moduleInstanceId, args), () => helper.CleanupRender(moduleInstanceId)))
                {
                    res = helper.Partial(viewName, new WebExModuleNotFoundModel(moduleFolder, view, model));
                }
                renderedViewName = viewName;
                return true;
            }
            else if (view.IsDefault())
            {
                realViewName = "default";
                viewName = string.Format("~/Views/{2}/{0}{3}.{1}", realViewName, ext, ModulesFolder, extView);
                if (helper.PartialViewExists(viewName, model))
                {
                    using (new AutoCleanup(() => helper.PrepareRender(moduleInstanceId, args), () => helper.CleanupRender(moduleInstanceId)))
                    {
                        res = helper.Partial(viewName, new WebExModuleNotFoundModel(moduleFolder, view, model));
                    }
                    renderedViewName = viewName;
                    return true;
                }
            }

            res = null;
            return false;
        }

        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName)
        {
            return helper.RenderModule(moduleName, (IDictionary<string, object>)null, null, null, false, null);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName, string view, object moduleModel = null, string moduleInstanceId = null)
        {
            return helper.RenderModule(moduleName, (IDictionary<string, object>)null, view, moduleModel, false, moduleInstanceId);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName, object moduleModel, string moduleInstanceId = null)
        {
            return helper.RenderModule(moduleName, (IDictionary<string, object>)null, null, moduleModel, false, moduleInstanceId);
        }
        public static MvcHtmlString RenderModuleArgs(this HtmlHelper helper, string moduleName, object args,
            string view = null, object moduleModel = null,
            bool ignoreCase = false, string moduleInstanceId = null)
        {
            return helper.RenderModule(moduleName, args == null ? (IDictionary<string, object>)null : new RouteValueDictionary(args),
                view, moduleModel, ignoreCase, moduleInstanceId);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName, IDictionary<string, object> args,
            string view = null, object moduleModel = null,
            bool ignoreCase = false, string moduleInstanceId = null)
        {
            Type mt = ModulesCatalog.GetModule(helper.ViewContext.HttpContext.Application, moduleName, ignoreCase);
            if (mt != null)
            {
                object res;
                if (mt != null && helper.GetStorage().TryGetValue(WebExModuleExtensions.MakeViewDataKey(mt), out res))
                {
                    IModule m = res as IModule;
                    if (m != null)
                    {
                        if (string.IsNullOrEmpty(view))
                            view = Contracts.DefaultView;

                        return helper.RenderModule(m, args, m.GetView(view, helper), moduleModel, moduleInstanceId);
                    }
                }
            }
            
            var r = helper.RenderModuleManual(moduleName, new ModuleAutoView(view), moduleModel, moduleInstanceId, args);
            if (r == null && !moduleName.StartsWith("~"))
            {
                {
                    var curViewPath = helper.GetViewPath();
                    if (!string.IsNullOrEmpty(curViewPath))
                    {
                        var dir = System.IO.Path.GetDirectoryName(curViewPath);
                        moduleName = System.IO.Path.Combine(dir, moduleName);
                    }
                }
                r = helper.RenderModuleManual(moduleName, new ModuleAutoView(view), moduleModel, moduleInstanceId, args);
            }

            return r;
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, Type module, IDictionary<string, object> args = null,
            string view = null,
            string moduleInstanceId = null)
        {
            object res;
            if (module != null && helper.GetStorage().TryGetValue(WebExModuleExtensions.MakeViewDataKey(module), out res))
            {
                IModule m = res as IModule;
                if (m != null)
                {
                    if (string.IsNullOrEmpty(view))
                        view = Contracts.DefaultView;

                    return helper.RenderModule(m, args, m.GetView(view, helper), null, moduleInstanceId);
                }
                //else
                //    return helper.RenderModuleManual(GetModuleFolder(module), res, view);
            }

            return null;
        }
        public static MvcHtmlString RenderModules(this HtmlHelper helper, string viewType,
            Func<IModule, int> getOrderWeight = null)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in GetModules(helper).Select(it => new { module = it, view = it.GetView(viewType, helper) }).
                Where(it => it.view != null).
                OrderBy(it => getOrderWeight == null ? 0 : getOrderWeight(it.module)).
                ThenBy(it => it.module, new DependencyComparer(helper.ViewContext.HttpContext.Application)))
            {
                if (item.view.IsAuto())
                {
                    (item.view as ModuleAutoView).Ext = "." + viewType;
                }
                var r = helper.RenderModule(item.module, null, item.view, null, null);
                if (r != null)
                    sb.Append(r.ToString());
            }

            foreach (var item in helper.GetInlineModules(viewType))
            {
                var r = helper.Partial(item.Item1, item.Item2);
                if (r != null)
                    sb.Append(r.ToString());
            }
            return MvcHtmlString.Create(sb.ToString());
        }
        #endregion

        #region ModuleInstance
        private static void PrepareRender(this HtmlHelper helper, string moduleInstanceId, IDictionary<string, object> args)
        {
            var mi = helper.GetModuleInstance(moduleInstanceId);
            mi.Params = args;
        }

        private static ModuleInstance GetModuleInstance(this HtmlHelper helper, string moduleInstanceId)
        {
            Stack<ModuleInstance> miList = null; object o = null;
            if (!helper.GetStorage().TryGetValue(webexModuleInstance, out o))
            {
                miList = new Stack<ModuleInstance>();
                helper.GetStorage()[webexModuleInstance] = miList;
            }
            else
                miList = o as Stack<ModuleInstance>;

            var mi = miList.FirstOrDefault((it) => it.InstanceId == moduleInstanceId);
            if (mi == null)
            {
                mi = new ModuleInstance { InstanceId = moduleInstanceId };
                miList.Push(mi);
            }

            return mi;
        }
        private static void CleanupRender(this HtmlHelper helper, string moduleInstanceId)
        {
            var mi = helper.GetCurrentModuleInstance();
            if (mi != null)
            {
                var instances = helper.GetStorage()[webexModuleInstance] as Stack<ModuleInstance>;
                do
                {
                    mi = instances.Pop();
                } while (mi.InstanceId != moduleInstanceId && instances.Any());
            }
        }
        private static string GetModuleId(this HtmlHelper helper)
        {
            var mi = helper.GetCurrentModuleInstance();
            if (mi != null)
                return mi.InstanceId;

            return null;
        }
        public static ModuleInstance GetCurrentModuleInstance(this HtmlHelper helper)
        {
            Stack<ModuleInstance> miList = null; object o = null;
            if (helper.GetStorage().TryGetValue(webexModuleInstance, out o))
            {
                miList = o as Stack<ModuleInstance>;
                if (miList != null && miList.Any())
                    return miList.Peek();
            }

            return null;
        }
        public static object GetViewParam(this HtmlHelper helper, string paramName, object defaultValue = null)
        {
            var mi = helper.GetCurrentModuleInstance();
            if (mi != null && mi.Params != null)
            {
                mi.Params.TryGetValue(paramName, out defaultValue);
            }
            return defaultValue;
        }
        public static T GetViewParam<T>(this HtmlHelper helper, string paramName, T defaultValue)
        {
            T v;
            if (!helper.TryGetViewParam(paramName, out v))
            {
                v = defaultValue;
            }
            return v;
        }
        public static bool TryGetViewParam<T>(this HtmlHelper helper, string paramName, out T value)
        {
            value = default(T);
            var mi = helper.GetCurrentModuleInstance();
            if (mi != null && mi.Params != null)
            {
                object v;
                var r = mi.Params.TryGetValue(paramName, out v);
                if (r)
                {
                    value = (T)v;
                }
                return r;
            }

            return false;
        }
        #endregion

        public static MvcHtmlString Concat(this MvcHtmlString first, params MvcHtmlString[] strings)
        {
            return MvcHtmlString.Create(first.ToString() + string.Concat(strings.Select(s => s.ToString())));
        }
        public static IDictionary GetStorage(this HtmlHelper helper)
        {
            return helper.ViewContext.RequestContext.HttpContext.Items;
        }
        public static bool TryGetValue(this IDictionary dic, string key, out object val)
        {
            val = null;
            if (dic.Contains(key))
            {
                val = dic[key];
                return true;
            }
            return false;

        }
        public static bool ContainsKey(this IDictionary dic, string key)
        {
            return dic.Contains(key);
        }
    }
}