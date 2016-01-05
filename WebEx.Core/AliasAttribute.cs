using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebEx.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public sealed class ModuleAliasAttribute : Attribute
    {
        private string _alias;
        public ModuleAliasAttribute(string alias)
        {
            _alias = alias;
        }
        public string Alias
        {
            get
            {
                return _alias;
            }
        }
    }
}
