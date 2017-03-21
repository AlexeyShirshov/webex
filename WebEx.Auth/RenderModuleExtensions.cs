using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebEx.Core;

namespace WebEx.Auth
{
    public static class WebExHtmlRenderModuleExtensions
    {
        public static MvcHtmlString RenderModule(this HtmlHelper helper, Type module, AuthorizeAttribute auth, IDictionary<string, object> args = null,
            string view = null,
            string moduleInstanceId = null)
        {
            return helper.RenderModule(module, args, view, moduleInstanceId, new[] { auth }, null);
        }

        public static MvcHtmlString RenderModule(this HtmlHelper helper, Type module, IEnumerable<string> AllowRoles,
            IEnumerable<string> AllowUsers = null,
            IEnumerable<string> DenyUsers = null,
            IEnumerable<string> DenyRoles = null,
            IDictionary<string, object> args = null,
            string view = null,
            string moduleInstanceId = null)
        {
            return helper.RenderModule(module, args, view, moduleInstanceId, new[] { new AuthorizeAttribute { AllowUsers = AllowUsers, AllowRoles = AllowRoles, DenyUsers = DenyUsers, DenyRoles = DenyRoles } }, null);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, Type module, bool allowAnonym, IDictionary<string, object> args = null,
            string view = null,
            string moduleInstanceId = null)
        {
            return helper.RenderModule(module, args, view, moduleInstanceId, new[] { new AuthorizeAttribute { AllowAnonym = allowAnonym } }, null);
        }
        public static MvcHtmlString RenderModules(this HtmlHelper helper, string viewType, Func<IModule, int> getOrderWeight, AuthorizeAttribute auth)
        {
            return helper.RenderModules(viewType, getOrderWeight, new[] { auth }, null);
        }
        public static MvcHtmlString RenderModules(this HtmlHelper helper, string viewType, Func<IModule, int> getOrderWeight, bool allowAnonym)
        {
            return helper.RenderModules(viewType, getOrderWeight, new[] { new AuthorizeAttribute { AllowAnonym = allowAnonym } }, null);
        }
        public static MvcHtmlString RenderModules(this HtmlHelper helper, string viewType, Func<IModule, int> getOrderWeight,
            IEnumerable<string> AllowRoles,
            IEnumerable<string> AllowUsers = null,
            IEnumerable<string> DenyUsers = null,
            IEnumerable<string> DenyRoles = null)
        {
            return helper.RenderModules(viewType, getOrderWeight, new[] { new AuthorizeAttribute { AllowUsers = AllowUsers, AllowRoles = AllowRoles, DenyUsers = DenyUsers, DenyRoles = DenyRoles } }, null);
        }
        public static MvcHtmlString RenderModules(this HtmlHelper helper, string viewType,
            IDictionary<IModule, int> moduleOrderWeight,
            AuthorizeAttribute auth)
        {
            return helper.RenderModules(viewType, moduleOrderWeight, new[] { auth }, null);
        }
        public static MvcHtmlString RenderModules(this HtmlHelper helper, string viewType,
            IDictionary<IModule, int> moduleOrderWeight,
            bool allowAnonym)
        {
            return helper.RenderModules(viewType, moduleOrderWeight, new[] { new AuthorizeAttribute { AllowAnonym = allowAnonym } }, null);
        }
        public static MvcHtmlString RenderModules(this HtmlHelper helper, string viewType,
            IDictionary<IModule, int> moduleOrderWeight,
            IEnumerable<string> AllowRoles,
            IEnumerable<string> AllowUsers = null,
            IEnumerable<string> DenyUsers = null,
            IEnumerable<string> DenyRoles = null)
        {
            return helper.RenderModules(viewType, moduleOrderWeight, new[] { new AuthorizeAttribute { AllowUsers = AllowUsers, AllowRoles = AllowRoles, DenyUsers = DenyUsers, DenyRoles = DenyRoles } }, null);
        }
        public static MvcHtmlString RenderModules(this HtmlHelper helper, string viewType, AuthorizeAttribute auth)
        {
            return helper.RenderModules(viewType, new[] { auth }, null);
        }
        public static MvcHtmlString RenderModules(this HtmlHelper helper, string viewType, bool allowAnonym)
        {
            return helper.RenderModules(viewType, new[] { new AuthorizeAttribute { AllowAnonym = allowAnonym } }, null);
        }
        public static MvcHtmlString RenderModules(this HtmlHelper helper, string viewType, IEnumerable<string> AllowRoles,
            IEnumerable<string> AllowUsers = null,
            IEnumerable<string> DenyUsers = null,
            IEnumerable<string> DenyRoles = null)
        {
            return helper.RenderModules(viewType, new[] { new AuthorizeAttribute { AllowUsers = AllowUsers, AllowRoles = AllowRoles, DenyUsers = DenyUsers, DenyRoles = DenyRoles } }, null);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName, IDictionary<string, object> args, AuthorizeAttribute auth,
            string view = null, object moduleModel = null,
            bool ignoreCase = false, string moduleInstanceId = null)
        {
            return helper.RenderModule(moduleName, args, view, moduleModel, ignoreCase, moduleInstanceId, new[] { auth }, null);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName, IDictionary<string, object> args, bool allowAnonym,
            string view = null, object moduleModel = null,
            bool ignoreCase = false, string moduleInstanceId = null)
        {
            return helper.RenderModule(moduleName, args, view, moduleModel, ignoreCase, moduleInstanceId, new[] { new AuthorizeAttribute { AllowAnonym = allowAnonym } }, null);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName, IDictionary<string, object> args,
            IEnumerable<string> AllowRoles,
            IEnumerable<string> AllowUsers = null,
            IEnumerable<string> DenyUsers = null,
            IEnumerable<string> DenyRoles = null,
            string view = null, object moduleModel = null,
            bool ignoreCase = false, string moduleInstanceId = null)
        {
            return helper.RenderModule(moduleName, args, view, moduleModel, ignoreCase, moduleInstanceId, new[] { new AuthorizeAttribute { AllowUsers = AllowUsers, AllowRoles = AllowRoles, DenyUsers = DenyUsers, DenyRoles = DenyRoles } }, null);
        }
        public static MvcHtmlString RenderModuleArgs(this HtmlHelper helper, string moduleName, object args,
            AuthorizeAttribute auth, string view = null, object moduleModel = null,
            bool ignoreCase = false, string moduleInstanceId = null)
        {
            return helper.RenderModuleArgs(moduleName, args, view, moduleModel, ignoreCase, moduleInstanceId, new[] { auth }, null);
        }
        public static MvcHtmlString RenderModuleArgs(this HtmlHelper helper, string moduleName, object args,
            bool allowAnonym, string view = null, object moduleModel = null,
            bool ignoreCase = false, string moduleInstanceId = null)
        {
            return helper.RenderModuleArgs(moduleName, args, view, moduleModel, ignoreCase, moduleInstanceId, new[] { new AuthorizeAttribute { AllowAnonym = allowAnonym } }, null);
        }
        public static MvcHtmlString RenderModuleArgs(this HtmlHelper helper, string moduleName, object args,
            IEnumerable<string> AllowRoles,
            IEnumerable<string> AllowUsers = null,
            IEnumerable<string> DenyUsers = null,
            IEnumerable<string> DenyRoles = null,
            string view = null, object moduleModel = null,
            bool ignoreCase = false, string moduleInstanceId = null)
        {
            return helper.RenderModuleArgs(moduleName, args, view, moduleModel, ignoreCase, moduleInstanceId, new[] { new AuthorizeAttribute { AllowUsers = AllowUsers, AllowRoles = AllowRoles, DenyUsers = DenyUsers, DenyRoles = DenyRoles } }, null);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleFolder, IModuleView view, AuthorizeAttribute auth, object model = null,
            string moduleInstanceId = null, IDictionary<string, object> args = null)
        {
            return helper.RenderModule(moduleFolder, view, model, moduleInstanceId, args, new[] { auth }, null);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleFolder, IModuleView view, bool allowAnonym, object model = null,
            string moduleInstanceId = null, IDictionary<string, object> args = null)
        {
            return helper.RenderModule(moduleFolder, view, model, moduleInstanceId, args, new[] { new AuthorizeAttribute { AllowAnonym = allowAnonym } }, null);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleFolder, IModuleView view, IEnumerable<string> AllowRoles,
            IEnumerable<string> AllowUsers = null,
            IEnumerable<string> DenyUsers = null,
            IEnumerable<string> DenyRoles = null,
            object model = null,
            string moduleInstanceId = null, IDictionary<string, object> args = null)
        {
            return helper.RenderModule(moduleFolder, view, model, moduleInstanceId, args, new[] { new AuthorizeAttribute { AllowUsers = AllowUsers, AllowRoles = AllowRoles, DenyUsers = DenyUsers, DenyRoles = DenyRoles } }, null);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, IModule module, IDictionary<string, object> args,
            IModuleView view, object model, string moduleInstanceId,
            AuthorizeAttribute auth)
        {
            return helper.RenderModule(module, args, view, model, moduleInstanceId, new[] { auth }, null);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, IModule module, IDictionary<string, object> args,
            IModuleView view, object model, string moduleInstanceId,
            bool allowAnonym)
        {
            return helper.RenderModule(module, args, view, model, moduleInstanceId, new[] { new AuthorizeAttribute { AllowAnonym = allowAnonym } }, null);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, IModule module, IDictionary<string, object> args,
            IModuleView view, object model, string moduleInstanceId,
            IEnumerable<string> AllowRoles,
            IEnumerable<string> AllowUsers = null,
            IEnumerable<string> DenyUsers = null,
            IEnumerable<string> DenyRoles = null)
        {
            return helper.RenderModule(module, args, view, model, moduleInstanceId, new[] { new AuthorizeAttribute { AllowUsers = AllowUsers, AllowRoles = AllowRoles, DenyUsers = DenyUsers, DenyRoles = DenyRoles } }, null);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName,
            AuthorizeAttribute auth)
        {
            return helper.RenderModule(moduleName, (IDictionary<string, object>)null, auth, null, null, false, null);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName,
            bool allowAnonym)
        {
            return helper.RenderModule(moduleName, (IDictionary<string, object>)null, new AuthorizeAttribute { AllowAnonym = allowAnonym }, null, null, false, null);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName,
            IEnumerable<string> AllowRoles,
            IEnumerable<string> AllowUsers = null,
            IEnumerable<string> DenyUsers = null,
            IEnumerable<string> DenyRoles = null)
        {
            return helper.RenderModule(moduleName, (IDictionary<string, object>)null, AllowRoles, AllowUsers, DenyUsers, DenyRoles, null, null, false, null);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName, string view,
            AuthorizeAttribute auth,
            object moduleModel = null, string moduleInstanceId = null)
        {
            return helper.RenderModule(moduleName, (IDictionary<string, object>)null, auth, view, moduleModel, false, moduleInstanceId);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName, string view,
            bool allowAnonym,
            object moduleModel = null, string moduleInstanceId = null)
        {
            return helper.RenderModule(moduleName, (IDictionary<string, object>)null, new AuthorizeAttribute { AllowAnonym = allowAnonym }, view, moduleModel, false, moduleInstanceId);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName, string view,
            IEnumerable<string> AllowRoles,
            IEnumerable<string> AllowUsers = null,
            IEnumerable<string> DenyUsers = null,
            IEnumerable<string> DenyRoles = null,
            object moduleModel = null, string moduleInstanceId = null)
        {
            return helper.RenderModule(moduleName, (IDictionary<string, object>)null, AllowRoles, AllowUsers, DenyUsers, DenyRoles, view, moduleModel, false, moduleInstanceId);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName, object moduleModel, AuthorizeAttribute auth, string moduleInstanceId = null)
        {
            return helper.RenderModule(moduleName, (IDictionary<string, object>)null, auth, null, moduleModel, false, moduleInstanceId);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName, object moduleModel, bool allowAnonym, string moduleInstanceId = null)
        {
            return helper.RenderModule(moduleName, (IDictionary<string, object>)null, new AuthorizeAttribute { AllowAnonym = allowAnonym }, null, moduleModel, false, moduleInstanceId);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName, object moduleModel,
            IEnumerable<string> AllowRoles,
            IEnumerable<string> AllowUsers = null,
            IEnumerable<string> DenyUsers = null,
            IEnumerable<string> DenyRoles = null,
            string moduleInstanceId = null)
        {
            return helper.RenderModule(moduleName, (IDictionary<string, object>)null, AllowRoles, AllowUsers, DenyUsers, DenyRoles, null, moduleModel, false, moduleInstanceId);
        }
        public static MvcHtmlString RenderModules(this HtmlHelper helper, IEnumerable<IModule> modules, AuthorizeAttribute auth,
            IDictionary<string, object> args = null,
            string view = null)
        {
            return helper.RenderModules(modules, args, view, new[] { auth }, null);
        }
        public static MvcHtmlString RenderModules(this HtmlHelper helper, IEnumerable<IModule> modules, bool allowAnonym,
            IDictionary<string, object> args = null,
            string view = null)
        {
            return helper.RenderModules(modules, args, view, new[] { new AuthorizeAttribute { AllowAnonym = allowAnonym } }, null);
        }
        public static MvcHtmlString RenderModules(this HtmlHelper helper, IEnumerable<IModule> modules, IEnumerable<string> AllowRoles,
            IEnumerable<string> AllowUsers = null,
            IEnumerable<string> DenyUsers = null,
            IEnumerable<string> DenyRoles = null,
            IDictionary<string, object> args = null,
            string view = null)
        {
            return helper.RenderModules(modules, args, view, new[] { new AuthorizeAttribute { AllowUsers = AllowUsers, AllowRoles = AllowRoles, DenyUsers = DenyUsers, DenyRoles = DenyRoles } }, null);
        }
        public static MvcHtmlString RenderModules(this HtmlHelper helper, IEnumerable<IModule> modules,
            string view, AuthorizeAttribute auth,
            IDictionary<string, object> args = null)
        {
            return helper.RenderModules(modules, auth, args, view);
        }
        public static MvcHtmlString RenderModules(this HtmlHelper helper, IEnumerable<IModule> modules,
            string view, bool allowAnonym,
            IDictionary<string, object> args = null)
        {
            return helper.RenderModules(modules, allowAnonym, args, view);
        }
        public static MvcHtmlString RenderModules(this HtmlHelper helper, IEnumerable<IModule> modules,
            string view, IEnumerable<string> AllowRoles,
            IEnumerable<string> AllowUsers = null,
            IEnumerable<string> DenyUsers = null,
            IEnumerable<string> DenyRoles = null,
            IDictionary<string, object> args = null)
        {
            return helper.RenderModules(modules, AllowRoles, AllowUsers, DenyUsers, DenyRoles, args, view);
        }
        public static MvcHtmlString RenderModulesFolder(this HtmlHelper helper, string modulesFolder, AuthorizeAttribute auth,
            IDictionary<string, object> args = null,
            string view = null,
            string pluginView = null)
        {
            return helper.RenderModulesFolder(modulesFolder, args, view, pluginView, new[] { auth }, null);
        }
        public static MvcHtmlString RenderModulesFolder(this HtmlHelper helper, string modulesFolder, bool allowAnonym,
            IDictionary<string, object> args = null,
            string view = null,
            string pluginView = null)
        {
            return helper.RenderModulesFolder(modulesFolder, args, view, pluginView, new[] { new AuthorizeAttribute { AllowAnonym = allowAnonym } }, null);
        }
        public static MvcHtmlString RenderModulesFolder(this HtmlHelper helper, string modulesFolder, IEnumerable<string> AllowRoles,
            IEnumerable<string> AllowUsers = null,
            IEnumerable<string> DenyUsers = null,
            IEnumerable<string> DenyRoles = null,
            IDictionary<string, object> args = null,
            string view = null,
            string pluginView = null)
        {
            return helper.RenderModulesFolder(modulesFolder, args, view, pluginView, new[] { new AuthorizeAttribute { AllowUsers = AllowUsers, AllowRoles = AllowRoles, DenyUsers = DenyUsers, DenyRoles = DenyRoles } }, null);
        }
    }
}
