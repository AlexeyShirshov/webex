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
        private readonly string _str;
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
    public sealed class FallBackModuleView : IModuleView
    {
        private string _str;
        public FallBackModuleView(string viewName)
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
        public IModuleView FallBackView { get; set; } = new ModuleDefaultView();
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
        private readonly string _str;
        private readonly Func<object, string, IDictionary<string, object>, string> _getValue;
        private bool _strSet;

        public ModuleViewString(string str)
        {
            _str = str;
            _strSet = true;
        }
        public ModuleViewString(string format, params object[] args)
        {
            _str = string.Format(format, args);
            _strSet = true;
        }
        public ModuleViewString(Func<object, string, IDictionary<string, object>, string> getString)
        {
            _getValue = getString;
        }
        public string Value
        {
            get
            {
                if (!_strSet)
                    throw new NotSupportedException("Use GetValue method");
                else
                    return _str;
            }
        }
        public string GetValue(object model, string moduleInstanceId, IDictionary<string, object> args)
        {
            if (_strSet)
                return _str;
            else if (_getValue != null)
                return _getValue(model, moduleInstanceId, args);

            return null;
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
