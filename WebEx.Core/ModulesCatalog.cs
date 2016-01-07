using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WebEx.Core
{
    /// <summary>
    /// Catalog of modules
    /// </summary>
    public class ModulesCatalog
    {
        public const string _webexInternalModuleAliases = "webex:modulealiases";
        public const string _webexInternalModuleTypes = "webex:modules";

        public static Type[] GetModules(string assemblyPattern = "App_Code")
        {
            List<Type> r = new List<Type>();

            foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (ass.FullName.IndexOf("webexmodule", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                    ass.FullName.IndexOf(assemblyPattern, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    foreach (var type in ass.GetTypes())
                    {
                        if (typeof(IModule).IsAssignableFrom(type))
                            r.Add(type);
                    }
                }
            }

            foreach (var file in Directory.GetFiles(HttpContext.Current.Server.MapPath("~/Bin"), "*webexmodule*.dll"))
            {
                try
                {
                    var ass = Assembly.LoadFile(file);
                    if (ass != null)
                        foreach (var type in ass.GetTypes())
                        {
                            if (typeof(IModule).IsAssignableFrom(type))
                                r.Add(type);
                        }
                }
                catch (Exception ex)
                {

                }
            }

            return r.ToArray();
        }

        public static void RegisterModules(HttpApplicationState appState, string viewExtension = "cshtml")
        {
            appState[WebExHtmlExtensions.webexViewExtension] = viewExtension;
            var modules = GetModules();
            appState[_webexInternalModuleTypes] = modules;
            foreach (var type in modules)
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
                appState[MakeAliasViewDataKey(alias)] = type.AssemblyQualifiedName;
            }
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
    }
}