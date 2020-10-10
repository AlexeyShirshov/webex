using System;
using System.Collections;
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
        private static readonly object _null = new object();
        public static object CreateInstance(this Type type, Func<Type, Type, object> mapTypes, params object[] args)
        {
            return type.CallMethod(null, () => type.GetConstructors(), mapTypes, args);
        }
        public static object CallMethod(this Type type, object target, Func<IEnumerable<MethodBase>> methods, Func<Type, Type, object> mapTypes, params object[] args)
        {
            if (type == null)
                return null;

            var dic = new List<Tuple<MethodBase, object[], int[]>>();
            foreach (var ctor in methods())
            {
                var methodParams = ctor.GetParameters();
                if (!methodParams.Any() && (args == null || args.Length == 0))
                {
                    return Activator.CreateInstance(type);
                }
                else
                {
                    var params2Call = new object[methodParams.Count()];
                    var argsIdx = new List<int>();
                    for (int i = 0; i < params2Call.Length; i++)
                        params2Call[i] = _null;

                    if (args != null && args.Length > 0)
                    {
                        for (int i = 0, j = 0; i < params2Call.Length && j < args.Length; i++)
                        {
                            var p = methodParams[i];
                            var mtype = p.ParameterType;

                            object arg = args[j];

                            if (arg == null)
                            {
                                //var hasVal = false;
                                //if (mapTypes != null)
                                //{
                                //    var o = mapTypes(mtype, null);
                                //    if (o != null)
                                //    {
                                //        hasVal = true;
                                //        params2Call[i] = o;
                                //    }
                                //}

                                //if (!hasVal)
                                //{
                                //    params2Call[i] = p.DefaultValue;
                                //}

                                argsIdx.Add(j);
                                params2Call[i] = null;// Convert.ChangeType(null, mtype);
                                j++;
                            }
                            else
                            {
                                if (mtype == arg.GetType())
                                {
                                    argsIdx.Add(j);
                                    params2Call[i] = arg;
                                    j++;
                                }
                                //else if (mapTypes != null)
                                //{
                                //    params2Call[i] = mapTypes(mtype, arg.GetType());
                                //}
                                //else
                                //{
                                //    try
                                //    {
                                //        params2Call[i] = Convert.ChangeType(arg, mtype);
                                //        j++;
                                //    }
                                //    catch (Exception ex)
                                //    {
                                //        //eat an exception
                                //    }
                                //}
                            }
                        }
                    }
                    dic.Add(new Tuple<MethodBase, object[], int[]>(ctor, params2Call, (from k in Enumerable.Range(0, args.Length)
                                                                                       where !argsIdx.Contains(k)
                                                                                       select k).ToArray()));
                }
            }
            if (dic.Count == 0)
                return Activator.CreateInstance(type);

            var bestParams = dic.First();
            IEnumerable<Tuple<MethodBase, object[], int[]>> sorted = null;
            //if (dic.Count > 1)
            {
                sorted = dic.OrderByDescending(it => it.Item2.Count(it2 => it2 != _null)).ToArray();
                bestParams = sorted.First();
            }

            //if (bestParams.Item2.Count(it => it != _null) == bestParams.Item1.GetParameters().Length && bestParams.Item3.Length == 0)
            //{

            //}
            //else if (dic.Count == 1)
            //{
            //    for (int i = 0; i < bestParams.Item2.Length; i++)
            //    {
            //        if (bestParams.Item2[i] == _null)
            //        {
            //            bestParams.Item2[i] = null;
            //        }
            //    }
            //}
            //else
            {
                sorted = sorted.OrderByDescending(it => it.Item2.Count(it2 => it2 != _null)).ThenBy(it => it.Item1.GetParameters().Length).ToArray();
                foreach (var candidate in sorted)
                {
                    if (candidate.Item3.Length == 0)
                    {
                        for (int i = 0; i < candidate.Item2.Length; i++)
                        {
                            if (candidate.Item2[i] == _null)
                            {
                                var p = candidate.Item1.GetParameters()[i];
                                var mtype = p.ParameterType;
                                if (mapTypes != null)
                                {
                                    var o = mapTypes(mtype, null);
                                    if (o != null)
                                    {
                                        candidate.Item2[i] = o;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0, j = 0; i < candidate.Item2.Length && j < candidate.Item3.Length; i++)
                        {
                            if (candidate.Item2[i] == _null)
                            {
                                var p = candidate.Item1.GetParameters()[i];
                                var mtype = p.ParameterType;
                                var arg = args[candidate.Item3[j]];
                                if (arg != null)
                                {
                                    var atype = arg.GetType();
                                    if (mtype.IsAssignableFrom(atype))
                                    {
                                        candidate.Item2[i] = arg;
                                        j++;
                                        continue;
                                    }
                                    else if (mapTypes != null)
                                    {
                                        var o = mapTypes(mtype, atype);
                                        if (o != null)
                                        {
                                            candidate.Item2[i] = o;
                                            j++;
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                    }
                                }
                                else if (candidate.Item2.Length == candidate.Item3.Length)
                                {
                                    j++;
                                }
                            }
                        }
                    }
                }

                sorted = sorted.OrderByDescending(it => it.Item2.Count(it2 => it2 != _null)).ThenBy(it => it.Item1.GetParameters().Length).ToArray();
                foreach (var candidate in sorted)
                {
                    for (int i = 0; i < candidate.Item2.Length; i++)
                    {
                        if (candidate.Item2[i] == _null)
                        {
                            candidate.Item2[i] = null;
                        }
                    }
                    bestParams = candidate;
                    break;
                }
            }

            var ci = bestParams.Item1 as ConstructorInfo;
            if (ci != null)
                return ci.Invoke(bestParams.Item2);
            else
                return bestParams.Item1.Invoke(target, bestParams.Item2);
        }

        #region LoadModule

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

            var r = type.CreateInstance((mtype, atype) =>
                {
                    if (typeof(ControllerBase).IsAssignableFrom(mtype) && !typeof(ControllerBase).IsAssignableFrom(atype))
                        return ctrl;

                    return null;
                }, args) as IModule;

            if (r != null)
            {
                WebExModuleExtensions.RegisterModule(ctrl.ControllerContext.RequestContext.HttpContext.Items, r);
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

        #endregion

        #region LoadModuleAsync

        public async static Task<IModule> LoadModuleAsync(this ControllerBase ctrl, string moduleName, params object[] args)
        {
            return await LoadModuleAsync(ctrl, moduleName, false, args);
        }
        public async static Task<IModule> LoadModuleAsync(this ControllerBase ctrl, string moduleName, bool ignoreCase, params object[] args)
        {
            return await LoadModuleAsync(ctrl, ModulesCatalog.GetModule(ctrl.ControllerContext.HttpContext.Application, moduleName, ignoreCase), args);
        }
        public async static Task<IModule> LoadModuleAsync(this ControllerBase ctrl, Type type, params object[] args)
        {
            if (type == null)
                return null;

            var r = Activator.CreateInstance(type) as IModule;

            Task t = type.CallMethod(r, () => type.GetMethods().Where(it => it.Name == "Init" && it.ReturnType == typeof(Task)), (mtype, atype) =>
            {
                if (typeof(ControllerBase).IsAssignableFrom(mtype) && !typeof(ControllerBase).IsAssignableFrom(atype))
                    return ctrl;

                return null;
            }, args) as Task;

            if (t != null)
                await t;

            if (r != null)
            {
                WebExModuleExtensions.RegisterModule(ctrl.ControllerContext.RequestContext.HttpContext.Items, r);
            }

            return r;
        }
        public async static Task<IModule> LoadModuleAsync(this ControllerBase ctrl, string type, Func<AssemblyName, Assembly> assemblyResolver,
            Func<Assembly, string, bool, Type> typeResolver, params object[] args)
        {
            return await LoadModuleAsync(ctrl, type, assemblyResolver, typeResolver, false, args);
        }
        public async static Task<IModule> LoadModuleAsync(this ControllerBase ctrl, string type, Func<AssemblyName, Assembly> assemblyResolver,
            Func<Assembly, string, bool, Type> typeResolver, bool ignoreCase, params object[] args)
        {
            return await LoadModuleAsync(ctrl, Type.GetType(type, assemblyResolver, typeResolver, false, ignoreCase), args);
        }

        #endregion

        public static void LoadModules(this ControllerBase ctrl, params object[] args)
        {
            var modules = ctrl.ControllerContext.HttpContext.Application[ModulesCatalog._webexInternalModuleTypes] as IEnumerable<Type>;
            if (modules != null)
                foreach (var module in modules)
                {
                    LoadModule(ctrl, module, args);
                }
        }
        public static IEnumerable<T> LoadModules<T>(this ControllerBase ctrl, params object[] args)
        {
            var modules = ctrl.ControllerContext.HttpContext.Application[ModulesCatalog._webexInternalModuleTypes] as IEnumerable<Type>;
            var l = new List<T>();
            if (modules != null)
                foreach (var module in modules)
                {
                    if (typeof(T).IsAssignableFrom(module))
                        l.Add((T)LoadModule(ctrl, module, args));
                }

            return l;
        }
        internal static CachedModule _GetModule(this ControllerBase ctrl, string moduleName, bool ignoreCase = false)
        {
            return WebExModuleExtensions._GetModule(ctrl.ControllerContext.RequestContext.HttpContext.Items, ModulesCatalog.GetModule(ctrl.ControllerContext.HttpContext.Application, moduleName, ignoreCase));
        }
        public static IModule GetModule(this ControllerBase ctrl, string moduleName, bool ignoreCase = false)
        {
            return ctrl._GetModule(moduleName, ignoreCase)?.Inner;
        }
    }
}
