using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebEx.Core;

/// <summary>
/// Summary description for WebExController
/// </summary>
public class DefaultController : Controller
{
    public DefaultController()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public ActionResult index(string page = "", string id = "")
    {
        WebEx.Core.WebExControllerExtensions.LoadModules(this, page, id);
        return View();
    }
}