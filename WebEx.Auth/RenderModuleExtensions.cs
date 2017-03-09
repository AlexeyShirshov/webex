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
        public static MvcHtmlString RenderModule(this HtmlHelper helper, Type module, AuthorizeAttribute auth, IDictionary < string, object> args = null,
            string view = null,
            string moduleInstanceId = null)
        {
            return helper.RenderModule(module, args, view, moduleInstanceId, new[] { auth }, null);
        }

        public static MvcHtmlString RenderModule(this HtmlHelper helper, Type module, IEnumerable<string> AllowUsers,
            IEnumerable<string> AllowRoles = null,
            IEnumerable<string> DenyUsers = null,
            IEnumerable<string> DenyRoles = null,
            IDictionary<string, object> args = null,
            string view = null,
            string moduleInstanceId = null)
        {
            return helper.RenderModule(module, args, view, moduleInstanceId, new[] { new AuthorizeAttribute { AllowUsers = AllowUsers, AllowRoles = AllowRoles, DenyUsers = DenyUsers, DenyRoles = DenyRoles} }, null);
        }

        public static MvcHtmlString RenderModules(this HtmlHelper helper, string viewType, AuthorizeAttribute auth, 
            Func<IModule, int> getOrderWeight = null)
        {
            return helper.RenderModules(viewType, getOrderWeight, new[] { auth }, null);
        }

        public static MvcHtmlString RenderModules(this HtmlHelper helper, string viewType, IEnumerable<string> AllowUsers,
            IEnumerable<string> AllowRoles = null,
            IEnumerable<string> DenyUsers = null,
            IEnumerable<string> DenyRoles = null,
            Func<IModule, int> getOrderWeight = null)
        {
            return helper.RenderModules(viewType, getOrderWeight, new[] { new AuthorizeAttribute { AllowUsers = AllowUsers, AllowRoles = AllowRoles, DenyUsers = DenyUsers, DenyRoles = DenyRoles } }, null);
        }

        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName, IDictionary<string, object> args, AuthorizeAttribute auth, 
            string view = null, object moduleModel = null,
            bool ignoreCase = false, string moduleInstanceId = null)
        {
            return helper.RenderModule(moduleName, args, view, moduleModel, ignoreCase, moduleInstanceId, new[] { auth }, null);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName, IDictionary<string, object> args,
            IEnumerable<string> AllowUsers,
            IEnumerable<string> AllowRoles = null,
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
            IEnumerable<string> AllowUsers,
            IEnumerable<string> AllowRoles = null,
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
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleFolder, IModuleView view, IEnumerable<string> AllowUsers,
            IEnumerable<string> AllowRoles = null,
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
            IEnumerable<string> AllowUsers,
            IEnumerable<string> AllowRoles = null,
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
            IEnumerable<string> AllowUsers,
            IEnumerable<string> AllowRoles = null,
            IEnumerable<string> DenyUsers = null,
            IEnumerable<string> DenyRoles = null)
        {
            return helper.RenderModule(moduleName, (IDictionary<string, object>)null, AllowUsers, AllowRoles, DenyUsers, DenyRoles, null, null, false, null);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName, string view,
            AuthorizeAttribute auth,
            object moduleModel = null, string moduleInstanceId = null)
        {
            return helper.RenderModule(moduleName, (IDictionary<string, object>)null, auth, view, moduleModel, false, moduleInstanceId);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName, string view,
            IEnumerable<string> AllowUsers,
            IEnumerable<string> AllowRoles = null,
            IEnumerable<string> DenyUsers = null,
            IEnumerable<string> DenyRoles = null,
            object moduleModel = null, string moduleInstanceId = null)
        {
            return helper.RenderModule(moduleName, (IDictionary<string, object>)null, AllowUsers, AllowRoles, DenyUsers, DenyRoles, view, moduleModel, false, moduleInstanceId);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName, object moduleModel, AuthorizeAttribute auth, string moduleInstanceId = null)
        {
            return helper.RenderModule(moduleName, (IDictionary<string, object>)null, auth, null, moduleModel, false, moduleInstanceId);
        }
        public static MvcHtmlString RenderModule(this HtmlHelper helper, string moduleName, object moduleModel,
            IEnumerable<string> AllowUsers,
            IEnumerable<string> AllowRoles = null,
            IEnumerable<string> DenyUsers = null,
            IEnumerable<string> DenyRoles = null,
            string moduleInstanceId = null)
        {
            return helper.RenderModule(moduleName, (IDictionary<string, object>)null, AllowUsers, AllowRoles, DenyUsers, DenyRoles, null, moduleModel, false, moduleInstanceId);
        }
    }
}
