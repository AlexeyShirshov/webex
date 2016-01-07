using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebEx.Core;
//using System.Web.WebPages.Administration.PackageManager;
/// <summary>
/// Summary description for WebExController
/// </summary>
//[ModuleActionFilter]
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
        //WebEx.Core.WebExControllerExtensions.LoadModules(this, page, id);
        return View();
    }

    //public ActionResult modules()
    //{
    //    var remoteRepo = PackageRepositoryFactory.Default.CreateRepository("http://owa:8001/nuget/");
    //    var localRepo = PackageRepositoryFactory.Default.CreateRepository(Server.MapPath("~/App_Data/Packages"));
    //    return View(localRepo.GetPackages());
    //}

    //private WebProjectManager GetProjectManager()
    //{
    //    string remoteSource = ConfigurationManager.AppSettings["PackageSource"];
    //    return new WebProjectManager(remoteSource, Request.MapPath("~/"));
    //}
}