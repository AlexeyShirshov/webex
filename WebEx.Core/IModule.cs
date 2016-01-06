using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        IModuleView GetView(string type, HtmlHelper helper);
    }
    /// <summary>
    /// Module with Model. <see cref="IModule.GetViewOfType"/> should return name of the view in Webex.Modules\<ModuleName> folder
    /// </summary>
    public interface IModuleWithModel : IModule
    {
        dynamic Model { get; }
    }
    public interface IModuleDependency
    {
        IEnumerable<string> GetDependencyByName();
        IEnumerable<Type> GetDependency();
    }    
}