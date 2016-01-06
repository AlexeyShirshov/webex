using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebEx.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public sealed class DependencyAttribute : Attribute
    {
        private Type _type;
        private string _moduleName;
        private bool _ignoreCase;
        public DependencyAttribute(string moduleName, bool ignoreCase = false)
        {
            _type = Type.GetType(moduleName, false, ignoreCase);
            if (_type == null)
            {
                _moduleName = moduleName;
                _ignoreCase = ignoreCase;
            }
        }
        public DependencyAttribute(Type type)
        {
            _type = type;
        }
        public DependencyAttribute(string type, Func<AssemblyName, Assembly> assemblyResolver,
            Func<Assembly, string, bool, Type> typeResolver, bool ignoreCase = false)
        {
            _type = Type.GetType(type, assemblyResolver, typeResolver, false, ignoreCase);
        }
        public Type GetDependencyType(HttpApplicationStateBase appState = null)
        {
            if (_type != null)
                return _type;

            return ModulesCatalog.GetModule(appState, _moduleName, _ignoreCase);
        }
    }
}
