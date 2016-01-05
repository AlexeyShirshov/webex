using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebEx.Core
{
    public static class ControllerExtensions
    {
        public static IModule LoadModule(this Controller ctrl, string type, params object[] args)
        {
            return LoadModule(ctrl, type, false, args);
        }
        public static IModule LoadModule(this Controller ctrl, string type, bool ignoreCase, params object[] args)
        {
            return LoadModule(ctrl, Type.GetType(type, false, ignoreCase), args);
        }
        public static IModule LoadModule(this Controller ctrl, Type type, params object[] args)
        {
            return LoadModule(ctrl, type, null, args);
        }
        public static IModule LoadModule(this Controller ctrl, Type type, string alias, params object[] args)
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
                else if (args != null && args.Count() > 0)
                {
                    var params2Call = new object[methodParams.Count()];
                    int j = 0;
                    for (int i = 0; i < params2Call.Length; i++)
                    {
                        var mtype = methodParams[i].ParameterType;
                        var arg = args[j];

                        if (arg == null)
                        {
                            if (typeof(Controller).IsAssignableFrom(mtype))
                                params2Call[i] = ctrl;

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
                ModuleExtensions.RegisterModule(ctrl.ViewData, type, r, alias);
            }

            return r;
        }
        public static IModule LoadModule(this Controller ctrl, string type, Func<AssemblyName, Assembly> assemblyResolver,
            Func<Assembly, string, bool, Type> typeResolver, params object[] args)
        {
            return LoadModule(ctrl, type, assemblyResolver, typeResolver, false, args);
        }
        public static IModule LoadModule(this Controller ctrl, string type, Func<AssemblyName, Assembly> assemblyResolver,
            Func<Assembly, string, bool, Type> typeResolver, bool ignoreCase, params object[] args)
        {
            return LoadModule(ctrl, Type.GetType(type, assemblyResolver, typeResolver, false, ignoreCase), args);
        }
        public static void LoadModules(this Controller ctrl, params object[] args)
        {
            var modules = ctrl.HttpContext.Application[ModuleExtensions._webexInternalModuleTypes] as IEnumerable<Type>;
            if (modules != null)
                foreach (var module in modules)
                {
                    LoadModule(ctrl, module, args);
                }
        }
    }
}
