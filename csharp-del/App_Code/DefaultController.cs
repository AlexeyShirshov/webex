using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        WebExModel model = new WebExModel();
        model.Load(this, page, id);
        return View(model);
    }
}