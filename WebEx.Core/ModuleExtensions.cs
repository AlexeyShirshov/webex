using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebEx.Core
{
    public static class WebExModuleExtensions
    {
        public const string _webexInternalModuleInstances = "webex:moduleinstances";
        public static string MakeViewDataKey(Type module)
        {
            return string.Format("{0}:{1}", _webexInternalModuleInstances, module.AssemblyQualifiedName);
        }
        public static IEnumerable<Type> GetModuleDependencies(this IModule module, HttpApplicationStateBase appState = null)
        {
            if (module == null)
                return new Type[] { };

            var md = module as IModuleDependency;
            var dep = new List<Type>();
            if (md != null)
            {
                var depStrings = md.GetDependencyByName();
                if (depStrings != null)
                foreach (var item in depStrings)
                {
                    var t = Type.GetType(item, false, false);
                    if (t != null && !dep.Contains(t))
                        dep.Add(t);
                }

                var depTypes = md.GetDependency();
                if (depTypes != null)
                foreach (var t in depTypes)
                {
                    if (t != null && !dep.Contains(t))
                        dep.Add(t);
                }
            }

            foreach (DependencyAttribute item in module.GetType().GetCustomAttributes(typeof(DependencyAttribute), false))
            {
                var t = item.GetDependencyType(appState);
                if (t != null && !dep.Contains(t))
                    dep.Add(t);
            }

            return dep;
        }
              
        public static bool IsDefault(this IModuleView view)
        {
            return view != null && view is ModuleDefaultView;
        }
        public static bool IsAuto(this IModuleView view)
        {
            return view != null && view is ModuleAutoView;
        }
        public static bool IsEmpty(this IModuleView view)
        {
            return view == null || string.IsNullOrEmpty(view.Value);
        }
        public static IModuleView GetFallBack(this IModuleView view)
        {
            return view != null && view is FallBackModuleView ? (view as FallBackModuleView).FallBackView : null;
        }
        public static IModule GetModule(IDictionary storage, Type module)
        {
            return _GetModule(storage, module)?.Inner;
        }
        internal static CachedModule _GetModule(IDictionary storage, Type module)
        {
            object res;
            if (module != null && storage.TryGetValue(MakeViewDataKey(module), out res))
                return res as CachedModule;

            return null;
        }
        public static string GetModuleName(this IModule module)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            return ModulesCatalog.GetModuleName(module.GetType());
        }
        public static void RegisterModule(IDictionary storage, IModule r)
        {
            if (r == null)
                throw new ArgumentNullException(nameof(r));
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));
            storage[WebExModuleExtensions.MakeViewDataKey(r.GetType())] = new CachedModule(r);
        }
        public static string GetExt(this IModuleView view)
        {
            var v = view as ModuleAutoView;
            if (v != null)
                if (v.Type == Contracts.CSSView || v.Type == Contracts.JavascriptView)
                    return "." + v.Type;
                else if (v.Type == Contracts.OnceView)
                    return ".-once";

            return null;
        }
    }
}
