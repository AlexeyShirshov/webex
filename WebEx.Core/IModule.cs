using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebEx.Core
{
    /// <summary>
    /// Module interface
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">Type of rendered view. Maybe constant within <see cref="Contracts"/> or custom value</param>
        IEnumerable<IModuleView> GetViews(string type, string viewName, HtmlHelper helper);
    }
    /// <summary>
    /// Module with Model. <see cref="IModule.GetView"/> should return name of the view in Webex.Modules\ModuleName folder
    /// </summary>
    public interface IModuleWithModel : IModule
    {
        dynamic Model { get; }
    }
    public interface IModuleFolder : IModule
    {
        string Folder { get; }
    }
    public interface IModuleHandler : IModule
    {
        bool Call(string method, object id, RequestContext rc, out string view);
    }
    public interface IModuleDependency
    {
        IEnumerable<string> GetDependencyByName();
        IEnumerable<Type> GetDependency();
    }

    internal class CachedModule
    {
        private readonly IModule _inner;
        private readonly List<Tuple<string,string>> _views = new List<Tuple<string, string>>();

        internal CachedModule(IModule inner)
        {
            _inner = inner;
        }

        public IModule Inner
        {
            get
            {
                return _inner;
            }
        }

        public IEnumerable<IModuleView> GetViews(string type, string viewName, HtmlHelper helper)
        {
            return _inner.GetViews(type, viewName, helper);
        }

        public string Folder { get; set; }

        internal void AddView(string type, string value)
        {
            if (!_views.Any(it => it.Item1 == type && it.Item2 == value))
                _views.Add(new Tuple<string, string>(type, value));
        }
        internal IEnumerable<string> GetViews(string type)
        {
            return from k in _views
                   where k.Item1 == type
                   select k.Item2;
        }
    }
}