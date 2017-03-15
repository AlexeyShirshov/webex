using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebEx.Core;

namespace WebEx.Auth
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class AuthorizeAttribute : Attribute, IPreRenderFilter
    {
        public IEnumerable<string> AllowUsers { get; set; }
        public IEnumerable<string> AllowRoles { get; set; }
        public IEnumerable<string> DenyUsers { get; set; }
        public IEnumerable<string> DenyRoles { get; set; }
        public bool AllowAnonym { get; set; } = true;
        public virtual PreRenderFilterResult Exec(HtmlHelper helper, string moduleInstanceId, IDictionary<string, object> args, string renderedViewName, IModuleView view)
        {
            if (!IsAuthenticated(helper, moduleInstanceId, args, renderedViewName, view))
                return PreRenderFilterResult.DontRender;

            return null;
        }

        protected virtual bool IsAuthenticated(HtmlHelper helper, string moduleInstanceId, IDictionary<string, object> args, string renderedViewName, IModuleView view)
        {
            var curUser = helper.ViewContext.HttpContext.User;

            bool res = true;

            if (curUser == null && !AllowAnonym)
                return false;

            if (DenyUsers != null)
            {
                if (curUser != null)
                foreach (var user in DenyUsers)
                {
                    if (user == curUser.Identity.Name)
                        return false;
                }
            }

            if (DenyRoles != null)
            {
                if (curUser != null)
                foreach (var role in DenyRoles)
                {
                    if (curUser.IsInRole(role))
                        return false;
                }
            }

            if (AllowRoles != null)
            {                
                foreach (var role in AllowRoles)
                {
                    res = false;

                    if (curUser != null && curUser.IsInRole(role))
                        return true;
                }
            }

            if (AllowUsers != null)
            {
                foreach (var user in AllowUsers)
                {
                    res = false;

                    if (curUser != null && user == curUser.Identity.Name)
                        return true;
                }
            }

            return res;
        }
    }
}