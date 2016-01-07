using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebEx.Core
{
    public static class WebExControllerExtensions
    {
        public static IModule LoadModule(this ControllerBase ctrl, string moduleName, params object[] args)
        {
            return LoadModule(ctrl, moduleName, false, args);
        }
        public static IModule LoadModule(this ControllerBase ctrl, string moduleName, bool ignoreCase, params object[] args)
        {
            return LoadModule(ctrl, ModulesCatalog.GetModule(ctrl.ControllerContext.HttpContext.Application, moduleName, ignoreCase), args);
        }
        public static IModule LoadModule(this ControllerBase ctrl, Type type, params object[] args)
        {
            if (type == null)
                return null;

            IModule r = null;

            foreach (var method in type.GetConstructors())
            {
                var methodParams = method.GetParameters();
                if (methodParams.Count() == 0)
                {
                    r = Activator.CreateInstance(type) as IModule;
                }
                else 
                {
                    var params2Call = new object[methodParams.Count()];
                    int j = 0;
                    for (int i = 0; i < params2Call.Length; i++)
                    {
                        var mtype = methodParams[i].ParameterType;
                        object arg = null;
                        if (args != null && args.Length > j)
                            arg = args[j];

                        if (arg == null)
                        {
                            if (typeof(Controller).IsAssignableFrom(mtype))
                                params2Call[i] = ctrl;

                            if (args != null && args.Length > j)
                                j++;
                        }
                        else
                        {
                            if (mtype == arg.GetType())
                            {
                                params2Call[i] = arg;
                                j++;
                            }
                            else
                            {
                                try
                                {
                                    params2Call[i] = Convert.ChangeType(arg, mtype);
                                    j++;
                                }
                                catch(Exception ex)
                                {
                                    if (typeof(Controller).IsAssignableFrom(mtype))
                                    {
                                        params2Call[i] = ctrl;
                                    }
                                }
                            }
                        }
                    }
                    r = Activator.CreateInstance(type, params2Call) as IModule;
                }
            }

            if (r != null)
            {
                RegisterModule(ctrl.ViewData, type, r);
            }

            return r;
        }
        public static IModule LoadModule(this ControllerBase ctrl, string type, Func<AssemblyName, Assembly> assemblyResolver,
            Func<Assembly, string, bool, Type> typeResolver, params object[] args)
        {
            return LoadModule(ctrl, type, assemblyResolver, typeResolver, false, args);
        }
        public static IModule LoadModule(this ControllerBase ctrl, string type, Func<AssemblyName, Assembly> assemblyResolver,
            Func<Assembly, string, bool, Type> typeResolver, bool ignoreCase, params object[] args)
        {
            return LoadModule(ctrl, Type.GetType(type, assemblyResolver, typeResolver, false, ignoreCase), args);
        }
        public static void LoadModules(this ControllerBase ctrl, params object[] args)
        {
            var modules = ctrl.ControllerContext.HttpContext.Application[ModulesCatalog._webexInternalModuleTypes] as IEnumerable<Type>;
            if (modules != null)
                foreach (var module in modules)
                {
                    LoadModule(ctrl, module, args);
                }
        }
        public static void RegisterModule(this ViewDataDictionary ViewData, Type type, IModule r)
        {
            ViewData[WebExModuleExtensions.MakeViewDataKey(type)] = r;
        }
        public static IModule GetModule(this ControllerBase ctrl, string moduleName, bool ignoreCase = false)
        {
            return WebExModuleExtensions.GetModule(ctrl.ViewData, ModulesCatalog.GetModule(ctrl.ControllerContext.HttpContext.Application, moduleName, ignoreCase));
        }
    }
}
