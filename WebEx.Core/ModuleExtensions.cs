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
            return view != null && view.GetType() == typeof(ModuleDefaultView);
        }
        public static bool IsAuto(this IModuleView view)
        {
            return view != null && view.GetType() == typeof(ModuleAutoView);
        }
        public static bool IsEmpty(this IModuleView view)
        {
            return view == null || string.IsNullOrEmpty(view.Value);
        }
        public static IModule GetModule(IDictionary storage, Type module)
        {
            object res;
            if (module != null && storage.TryGetValue(MakeViewDataKey(module), out res))
                return res as IModule;

            return null;
        }        
    }
}
