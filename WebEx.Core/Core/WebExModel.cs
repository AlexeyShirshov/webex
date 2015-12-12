using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

public partial class WebExModel : Westwind.Utilities.Dynamic.Expando
{
    public const string webexPluginsRegistry = "webex:PluginsRegistry";

    [AttributeUsage(AttributeTargets.Method)]
    public class LoadModelAttribute : Attribute
    {

    }
    [AttributeUsage(AttributeTargets.Property)]
    public class ModuleAttribute : Attribute
    {

    }
    public void Load(params object[] args)
    {
        var methods2call = from method in this.GetType().GetMethods()
                           where IsLoad(method)
                           select method;

        foreach (var method in methods2call)
        {
            var methodParams = method.GetParameters();
            if (methodParams.Count() == 0)
            {
                method.Invoke(this, null);
            }
            else if (args != null && args.Count() > 0)
            {
                if (methodParams.Count() <= args.Count())
                {
                    var params2Call = new object[methodParams.Count()];
                    Array.Copy(args, params2Call, methodParams.Count());
                    method.Invoke(this, params2Call);
                }
            }
        }

        var plugins = System.Web.HttpContext.Current.Application[webexPluginsRegistry] as Plugins;
        if (plugins != null & plugins.ExternalModules != null)
        {
            foreach (var module in plugins.ExternalModules)
            {
                module.Load(this, args);
            }
        }
    }

    private bool IsLoad(System.Reflection.MethodInfo method)
    {
        return method.GetCustomAttributes(typeof(LoadModelAttribute), false).Any();
    }

    public bool TryGetProperty(string name, out object result)
    {
        if (!Properties.TryGetValue(name, out result))
            return GetProperty(this, name, out result);

        return true;
    }

    public IEnumerable<IModule> GetMultiViewModules()
    {
        foreach (var module in (from p in GetType().GetProperties(System.Reflection.BindingFlags.Public | 
                                 System.Reflection.BindingFlags.GetProperty |
                                 System.Reflection.BindingFlags.Instance)
                               where typeof(IModule).IsAssignableFrom(p.GetMethod.ReturnType)
                                let m = p.GetMethod.Invoke(this, null) as IModule
                               where m != null
                               select m).Union(from p in Properties
                                               let m = p.Value as IModule
                                               where m != null
                                               select m))
        {
            yield return module;
        }
    }

}
