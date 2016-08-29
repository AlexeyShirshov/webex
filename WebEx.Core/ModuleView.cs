using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebEx.Core
{
    public interface IModuleView
    {
        string Value { get; }
    }

    public sealed class ModuleView : IModuleView
    {
        private string _str;
        public ModuleView(string viewName)
        {
            _str = viewName;
        }
        public string Value
        {
            get
            {
                return _str;
            }
        }
    }
    public sealed class ModuleAutoView : IModuleView
    {
        private string _str;
        private string _ext = string.Empty;
        public ModuleAutoView() { }
        public ModuleAutoView(string viewName)
        {
            _str = viewName;
        }
        public string Value
        {
            get
            {
                return _str;
            }
        }
        internal string Ext
        {
            get
            {
                return _ext;
            }
            set
            {
                _ext = value;
            }
        }
    }
    public sealed class ModuleViewString : IModuleView
    {
        private string _str;
        public ModuleViewString(string str)
        {
            _str = str;
        }
        public ModuleViewString(string format, params object[] args)
        {
            _str = string.Format(format, args);
        }
        public string Value
        {
            get
            {
                return _str;
            }
        }
    }
    public sealed class ModuleDefaultView : IModuleView
    {
        public string Value
        {
            get
            {
                throw new NotSupportedException("This is a default view");
            }
        }
    }
}
