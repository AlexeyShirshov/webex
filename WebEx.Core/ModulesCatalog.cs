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
                catch(Exception ex)
                {

                }
            }

            return r.ToArray();
        }  
      
        public static void RegisterModules(HttpApplicationState appState, string viewExtension = "cshtml")
        {
            appState[HtmlExtensions.webexViewExtension] = viewExtension;
            appState[ModuleExtensions._webexInternalModuleTypes] = GetModules();
        }
    }
}