﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebEx.Core
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ModuleActionFilterAttribute : FilterAttribute, IActionFilter
    {
        private string[] _modules;
        public ModuleActionFilterAttribute(params string[] moduleNames)
        {
            _modules = moduleNames;
        }
        public string ExcludeMethods { get; set; }
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //do nothing
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!string.IsNullOrEmpty(ExcludeMethods))
            {
                if (ExcludeMethods.Split(',').Select(it=>it.ToLower()).Contains(filterContext.ActionDescriptor.ActionName.ToLower()))
                {
                    return;
                }
            }

            if (_modules != null && _modules.Any())
            {
                foreach (var moduleName in _modules)
                {
                    filterContext.Controller.LoadModule(moduleName, filterContext.ActionParameters.Values.ToArray());
                }
            }
            else
            {
                filterContext.Controller.LoadModules(filterContext.ActionParameters.Values.ToArray());
            }
        }
    }
}
