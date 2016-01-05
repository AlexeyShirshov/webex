using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebEx.Core
{
    public static class WebExModuleExtensions
    {
        public const string _webexInternalModuleInstances = "webex:moduleinstances";
        public const string _webexInternalModuleAliases = "webex:modulealiases";
        public const string _webexInternalModuleTypes = "webex:modules";
        public static string MakeViewDataKey(Type module)
        {
            return string.Format("{0}:{1}", _webexInternalModuleInstances, module.AssemblyQualifiedName);
        }
        public static string MakeAliasViewDataKey(string alias)
        {
            return string.Format("{0}:{1}", _webexInternalModuleAliases, alias);
        }
        public static IEnumerable<Type> GetModuleDependencies(this IModule module, ViewDataDictionary viewData = null)
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
                var t = item.GetDependencyType(viewData);
                if (t != null && !dep.Contains(t))
                    dep.Add(t);
            }

            return dep;
        }

        public static void RegisterModule(this ViewDataDictionary ViewData, Type type, IModule r, string alias)
        {
            ViewData[MakeViewDataKey(type)] = r;

            if (string.IsNullOrEmpty(alias))
            {
                alias = type.Name;
                if (alias.EndsWith("module", StringComparison.InvariantCultureIgnoreCase))
                    alias = alias.Substring(0, alias.Length - 6);
            }
            ViewData[MakeAliasViewDataKey(alias)] = type.AssemblyQualifiedName;
        }

        public static Type GetModule(this ViewDataDictionary viewData, string moduleName, bool ignoreCase = false)
        {
            object qname;
            if (viewData != null)
            {
                if (!viewData.TryGetValue(MakeAliasViewDataKey(moduleName), out qname))
                {
                    qname = moduleName;
                }                
            }
            else
            {
                qname = moduleName;
            }

            return Type.GetType(qname.ToString(), false, ignoreCase);
        }
    }
}
