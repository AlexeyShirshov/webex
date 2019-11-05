using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;

namespace WebEx.Core
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple =false)]
    public class ModuleContainerAttribute : Attribute
    {

    }
    /// <summary>
    /// Catalog of modules
    /// </summary>
    public static class ModulesCatalog
    {
        internal static TraceSwitch _ts = new TraceSwitch("webex:mcatalog", "Switch ModulesCatalog", "3");
        public const string _webexInternalModuleAliases = "webex:modulealiases";
        public const string _webexInternalModuleTypes = "webex:modules";
        private const string DefaultWebExHandlerName = "webex-handler";

        public static Type[] GetModulesByAttribute()
        {
            return GetModules("*.dll", true);
        }
        public static Type[] GetModules(string assemblyPattern = "*webexmodule*.dll", bool checkAttribute = false)
        {
            var r = new List<Type>();

            if (BuildManager.CodeAssemblies != null)
            foreach (var ass in BuildManager.CodeAssemblies.OfType<Assembly>())
            {
                foreach (var type in ass.GetTypes())
                {
                    if (typeof(IModule).IsAssignableFrom(type) && !type.IsAbstract)
                        r.Add(type);
                }
            }

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var file in Directory.GetFiles(GetAssemblyDir(), assemblyPattern))
            {
                try
                {
                    var ass = Assembly.Load(File.ReadAllBytes(file));
                    if (ass != null)
                    {
                        var lass = loadedAssemblies.FirstOrDefault(la => la.FullName == ass.FullName);
                        if (lass != null) ass = lass;

                        if (checkAttribute)
                        {
                            if (ass.GetCustomAttribute<ModuleContainerAttribute>() == null)
                                continue;
                        }

                        foreach (var type in ass.GetTypes())
                        {
                            if (typeof(IModule).IsAssignableFrom(type) && !type.IsAbstract)
                                r.Add(type);
                        }
                    }
                }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
#pragma warning disable CS0168 // Variable is declared but never used
                catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
                {

                }
            }

            return r.ToArray();
        }
        private static string GetAssemblyDir()
        {
            return HostingEnvironment.IsHosted
                ? HttpRuntime.BinDirectory
                : Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
        }

        public static void RegisterModules(HttpApplicationState appState, string viewExtension = null)
        {
            RegisterModules(appState, GetModules(), viewExtension);

        }
        public static void RegisterModules(HttpApplicationState appState, Type[] modules, string viewExtension = null)
        {
            appState[WebExHtmlRenderModuleExtensions.webexViewExtension] = viewExtension;
            appState[_webexInternalModuleTypes] = modules;
            foreach (var type in modules)
            {
                string alias = GetModuleName(type);
                appState[MakeAliasViewDataKey(alias)] = type.AssemblyQualifiedName;
            }
        }
        public static IEnumerable<Type> GetRegisteredModules(HttpApplicationState appState)
        {
            return appState[ModulesCatalog._webexInternalModuleTypes] as IEnumerable<Type> ?? new Type [] { };
        }
        public static string GetModuleName(Type type)
        {
            var attr = type.GetCustomAttribute<ModuleAliasAttribute>();
            string alias = null;
            if (attr != null)
                alias = attr.Alias;

            if (string.IsNullOrEmpty(alias))
            {
                alias = type.Name;
                if (alias.EndsWith("module", StringComparison.InvariantCultureIgnoreCase))
                    alias = alias.Substring(0, alias.Length - 6);
            }

            return alias;
        }

        public static void SetModuleAlias(HttpApplicationState appState, Type module, string alias)
        {
            if (module == null || string.IsNullOrEmpty(alias))
                return;

            appState[MakeAliasViewDataKey(alias)] = module.AssemblyQualifiedName;

        }
        public static string MakeAliasViewDataKey(string alias)
        {
            return string.Format("{0}:{1}", _webexInternalModuleAliases, alias);
        }

        public static Type GetModule(HttpApplicationStateBase appState, string moduleName, bool ignoreCase = false)
        {
            if (string.IsNullOrEmpty(moduleName))
                return null;

            object qname = null;
            if (appState != null)
            {
                if (appState.AllKeys.Contains(MakeAliasViewDataKey(moduleName)))
                {
                    qname = appState[MakeAliasViewDataKey(moduleName)];
                }
            }
            else
            {
                qname = moduleName;
            }

            if (qname != null)
                return Type.GetType(qname.ToString(), false, ignoreCase);

            return null;
        }
        public static void LoadModules(HttpApplicationStateBase appState, IDictionary storage, params object[] args)
        {
            var modules = appState[ModulesCatalog._webexInternalModuleTypes] as IEnumerable<Type>;
            if (modules != null)
                foreach (var module in modules)
                {
                    LoadModule(storage, module, args);
                }
        }
        public static IEnumerable<T> LoadModules<T>(HttpApplicationStateBase appState, IDictionary storage, params object[] args)
        {
            var modules = appState[ModulesCatalog._webexInternalModuleTypes] as IEnumerable<Type>;
            var l = new List<T>();
            if (modules != null)
                foreach (var module in modules)
                {
                    if (typeof(T).IsAssignableFrom(module))
                        l.Add((T)LoadModule(storage, module, args));
                }

            return l;
        }
        public static IModule LoadModule(IDictionary storage, Type type, params object[] args)
        {
            if (type == null)
                return null;

            var r = type.CreateInstance(null, args) as IModule;

            if (r != null)
            {
                WebExModuleExtensions.RegisterModule(storage, r);
            }

            return r;
        }
        public static void RegisterHandler(RouteCollection routes, string handlerUrl=DefaultWebExHandlerName + "/{module}/{method}/{id}")
        {
            routes.Add(new Route
            (
                handlerUrl,
                new RouteValueDictionary(new { id = UrlParameter.Optional}),
                new RouteHandler()
            ));
        }
        public static string GetModuleHandlerUrl(string module, string method, string webexHandler = DefaultWebExHandlerName)
        {
            return $"{webexHandler}/{module}/{method}";
        }
    }

    public class RouteHandler : IRouteHandler
    {
        class EmptyController : Controller
        {
            public void Initialize(RequestContext requestContext, ControllerContext cc)
            {
                ControllerContext = cc;
                TempData = new TempDataDictionary();
                ViewData = new ViewDataDictionary();
                base.Initialize(requestContext);
            }
        }
        class ViewDataContainer : System.Web.Mvc.IViewDataContainer
        {
            public System.Web.Mvc.ViewDataDictionary ViewData { get; set; }

            public ViewDataContainer(System.Web.Mvc.ViewDataDictionary viewData)
            {
                ViewData = viewData;
            }
        }
        class View : IView
        {
            public void Render(ViewContext viewContext, TextWriter writer)
            {
                //do nothing
            }
        }
        class HttpHandler : IHttpHandler
        {
            private RequestContext _rc;
            public HttpHandler(RequestContext rc) { _rc = rc; }
            public bool IsReusable => false;

            public void ProcessRequest(HttpContext context)
            {
                var module = _rc.RouteData.Values["module"].ToString();
                var method = _rc.RouteData.Values["method"].ToString();
                object id;
                _rc.RouteData.Values.TryGetValue("id", out id);
                //var urlHelper = new UrlHelper(_rc);

                var type = ModulesCatalog.GetModule(new HttpApplicationStateWrapper(context.Application), module, false);
                if (type == null)
                {
                    if (ModulesCatalog._ts.TraceInfo)
                        Debug.WriteLine("{0}: Cannot create type from module {1}", ModulesCatalog._ts.DisplayName, module);

                    return;
                }

                var r = type.CreateInstance((mtype, atype) =>
                {
                    if (typeof(UrlHelper).IsAssignableFrom(mtype) && !typeof(UrlHelper).IsAssignableFrom(atype))
                        return new UrlHelper(_rc);

                    return null;
                }) as IModuleHandler;

                if (r == null)
                {
                    if (ModulesCatalog._ts.TraceInfo)
                        Debug.WriteLine("{0}: Module {1} does not implement IModuleHandler", ModulesCatalog._ts.DisplayName, module);

                    return;
                }

                string view;
                if (!r.Call(method, id, _rc, out view))
                    return;

                _rc.RouteData.Values["controller"] = "Empty";
                using (var ctrl = new EmptyController())
                {
                    var cc = new ControllerContext(_rc, ctrl);
                    ctrl.Initialize(_rc, cc);
                    var ct = new ViewContext(cc, new View(), cc.Controller.ViewData, cc.Controller.TempData, context.Response.Output);
                    var h = new HtmlHelper(ct, new ViewDataContainer(cc.Controller.ViewData));

                    context.Response.Output.Write(h.RenderModule(r, view).ToHtmlString());
                }
            }
        }
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new HttpHandler(requestContext);
        }
    }
}