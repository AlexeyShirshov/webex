﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebEx.Core;

public class JQueryModule : IModule
{
    public string GetViewOfType(string type, HtmlHelper html)
    {
        var url = new UrlHelper(html.ViewContext.RequestContext);
        if (type == "js")
            return string.Format("<script src='{0}'></script>" ,url.Content("~/Scripts/jquery-2.1.4.js"));

        return null;
    }
}