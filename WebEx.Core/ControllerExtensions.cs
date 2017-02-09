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
            if (type == null)
                return null;

            List<Tuple<ConstructorInfo, object[], int[]>> dic = new List<Tuple<ConstructorInfo, object[], int[]>>();
            foreach (var ctor in type.GetConstructors())
            {
                var methodParams = ctor.GetParameters();
                if (methodParams.Count() == 0 && (args == null || args.Length == 0))
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
                    dic.Add(new Tuple<ConstructorInfo, object[], int[]>(ctor, params2Call, (from k in Enumerable.Range(0, args.Length)
                                                                                            where !argsIdx.Contains(k)
                                                                                            select k).ToArray()));
                }
            }

            var bestParams = dic.First();
            IEnumerable<Tuple<ConstructorInfo, object[], int[]>> sorted = null;
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

            return bestParams.Item1.Invoke(bestParams.Item2);
        }
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

            var r = type.CreateInstance((mtype, atype)=>
            {
                if (typeof(ControllerBase).IsAssignableFrom(mtype) && !typeof(ControllerBase).IsAssignableFrom(atype))
                    return ctrl;

                return null;
            }, args) as IModule;

            //foreach (var method in type.GetConstructors())
            //{
            //    var methodParams = method.GetParameters();
            //    if (methodParams.Count() == 0)
            //    {
            //        r = Activator.CreateInstance(type) as IModule;
            //    }
            //    else 
            //    {
            //        var params2Call = new object[methodParams.Count()];
            //        int j = 0;
            //        for (int i = 0; i < params2Call.Length; i++)
            //        {
            //            var mtype = methodParams[i].ParameterType;
            //            object arg = null;
            //            if (args != null && args.Length > j)
            //                arg = args[j];

            //            if (arg == null)
            //            {
            //                if (typeof(ControllerBase).IsAssignableFrom(mtype))
            //                    params2Call[i] = ctrl;

            //                if (args != null && args.Length > j)
            //                    j++;
            //            }
            //            else
            //            {
            //                if (mtype == arg.GetType())
            //                {
            //                    params2Call[i] = arg;
            //                    j++;
            //                }
            //                else if (typeof(ControllerBase).IsAssignableFrom(mtype) && !typeof(ControllerBase).IsAssignableFrom(arg.GetType()))
            //                {
            //                    params2Call[i] = ctrl;
            //                }
            //                else
            //                {
            //                    try
            //                    {
            //                        params2Call[i] = Convert.ChangeType(arg, mtype);
            //                        j++;
            //                    }
            //                    catch(Exception ex)
            //                    {
            //                        //eat an exception
            //                    }
            //                }
            //            }
            //        }
            //        r = Activator.CreateInstance(type, params2Call) as IModule;
            //    }
            //}

            if (r != null)
            {
                RegisterModule(ctrl.ControllerContext.RequestContext.HttpContext.Items, type, r);
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
        public static void RegisterModule(IDictionary storage, Type type, IModule r)
        {
            storage[WebExModuleExtensions.MakeViewDataKey(type)] = new CachedModule(r);
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
