using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

        class InlineModuleModel
        {
            public string type;
            public string view;
            public object model;
            public IDictionary<string, object> args;
            public string instanceId;

            public InlineModuleModel(string type, string view, object model)
            {
                this.type = type;
                this.view = view;
                this.model = model;
            }
        }

        private static TraceSwitch _t = new TraceSwitch("webex:html", "Switch WebExHtmlExtensions", "3");
        public const string _webexInternalInlineModuleInstances = "webex:inlinemodules";

        public static bool HasModule(this HtmlHelper helper, string moduleName, bool ignoreCase = false)
        {
            var mt = Type.GetType(moduleName, false, ignoreCase);
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
            return _GetModules(helper.GetStorage()).Select(it => it.Inner);
        }
        public static IEnumerable<IModule> GetModules(IDictionary items)
        {
            return _GetModules(items).Select(it => it.Inner);
        }
        internal static IEnumerable<CachedModule> _GetModules(IDictionary items)
        {
            foreach (DictionaryEntry item in items)
            {
                if (item.Key.ToString().StartsWith(WebExModuleExtensions._webexInternalModuleInstances, StringComparison.Ordinal))
                {
                    var module = item.Value as CachedModule;
                    if (module != null)
                        yield return module;
                }
            }
        }
        public static IEnumerable<Tuple<string, object, IDictionary<string, object>, string>> GetInlineModules(this HtmlHelper helper, string type)
        {
            object res;
            if (!helper.GetStorage().TryGetValue(_webexInternalInlineModuleInstances, out res))
            {
                res = new InlineModuleModel[] { };
            }
            return from k in res as IEnumerable<InlineModuleModel> where k.type == type select new Tuple<string, object, IDictionary<string, object>,string>(k.view, k.model, k.args, k.instanceId);
        }
        public static void RegisterInlineModule(this HtmlHelper helper, string type, string view, object model, IDictionary<string, object> args, string moduleInstanceId)
        {
            object res;
            if (!helper.GetStorage().TryGetValue(_webexInternalInlineModuleInstances, out res))
            {
                res = new List<InlineModuleModel>();
                helper.GetStorage()[_webexInternalInlineModuleInstances] = res;
            }

            var l = res as List<InlineModuleModel>;

            if (!l.Any((it) => it.type == type && it.view == view && object.Equals(it.model, model)))
                l.Add(new InlineModuleModel(type, view, model) { args = args, instanceId = moduleInstanceId });
        }
        public static IModule GetModule(this HtmlHelper helper, string moduleName, bool ignoreCase = false)
        {
            return helper._GetModule(moduleName, ignoreCase)?.Inner;
        }
        internal static CachedModule _GetModule(this HtmlHelper helper, string moduleName, bool ignoreCase = false)
        {
            return WebExModuleExtensions._GetModule(helper.ViewContext.RequestContext.HttpContext.Items, ModulesCatalog.GetModule(helper.ViewContext.HttpContext.Application, moduleName, ignoreCase));
        }
        //public static MvcHtmlString RenderPartialIfExists(this HtmlHelper html, string partialViewName, object model = null)
        //{
        //    if (html.PartialViewExists(partialViewName, model))
        //        return html.Partial(partialViewName, model);

        //    return null;
        //}
        /// <summary>
        /// Check whether view exists. If model was specified, compare it type with view model type (if exists)
        /// </summary>
        /// <param name="html"></param>
        /// <param name="partialViewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool IsPartialViewExists(this HtmlHelper html, string partialViewName, object model)
        {
            var vr = ViewEngines.Engines.FindPartialView(html.ViewContext.Controller.ControllerContext, partialViewName);
            var bm = vr.View as BuildManagerCompiledView;
            if (bm != null && model != null/* && html.ViewData.Model != null && html.ViewData.ModelMetadata != null*/)
            {
                var rv = System.Web.Compilation.BuildManager.GetCompiledType(bm.ViewPath);
                if (typeof(WebViewPage).IsAssignableFrom(rv) && rv.BaseType.IsGenericType)
                {
                    //if (_t.TraceVerbose)
                    //{
                    //    Debug.WriteLine("{0} == {1}", model.GetType().AssemblyQualifiedName, rv.BaseType.GetGenericArguments()[0].AssemblyQualifiedName);
                    //    Debug.WriteLine("{0}", model.GetType() == rv.BaseType.GetGenericArguments()[0]);
                    //    Debug.WriteLine("{0} == {1}", model.GetType().Assembly.Location, rv.BaseType.GetGenericArguments()[0].Assembly.Location);
                    //    Debug.WriteLine("{0}", model.GetType().Assembly == rv.BaseType.GetGenericArguments()[0].Assembly);
                    //}
                    return rv.BaseType.GetGenericArguments()[0].IsAssignableFrom(model.GetType());
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

        public static MvcHtmlString Concat(this MvcHtmlString first, params MvcHtmlString[] strings)
        {
#pragma warning disable RECS0106 // Finds calls to ToString() which would be generated automatically by the compiler
            return MvcHtmlString.Create(first.ToString() + string.Concat(strings.Select(s => s.ToString())));
#pragma warning restore RECS0106 // Finds calls to ToString() which would be generated automatically by the compiler
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