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
        public const string _webexInternalInlineModuleInstances = "webex:inlinemodules";

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
            return helper._GetModules().Select(it => it.Inner);
        }
        internal static IEnumerable<CachedModule> _GetModules(this HtmlHelper helper)
        {
            foreach (DictionaryEntry item in helper.ViewContext.RequestContext.HttpContext.Items)
            {
                if (item.Key.ToString().StartsWith(WebExModuleExtensions._webexInternalModuleInstances, StringComparison.Ordinal))
                {
                    var module = item.Value as CachedModule;
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