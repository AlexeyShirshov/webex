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
        class ViewParams
        {
            public string Type;
            public string Value;
            public IDictionary<string, object> Args;
            public int RunAfter;

            public ViewParams(string type, string value, IDictionary<string, object> args, int runAfter)
            {
                Type = type;
                Value = value;
                Args = args;
                RunAfter = runAfter;
            }
        }
        private readonly IModule _inner;
        private readonly List<ViewParams> _views = new List<ViewParams>();

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
        internal bool RenderedOnce { get; set; }

        internal void AddView(string type, string value, IDictionary<string, object> args, int runAfter = -1)
        {
            if (!_views.Any(it => it.Type == type && it.Value == value))
                _views.Add(new ViewParams(type, value, args, runAfter));
        }
        internal IEnumerable<Tuple<string, IDictionary<string, object>>> GetViews(string type, int runAfter = -1)
        {
            return from k in _views
                   where k.Type == type && k.RunAfter == runAfter
                   select new Tuple<string, IDictionary<string, object>>(k.Value, k.Args);        
        }
    }
}