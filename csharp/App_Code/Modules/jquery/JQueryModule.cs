using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public class JQueryExternalModule : IExternalModule, IModule
{
    private Controller _ctrl;
    public void Load(dynamic model, params object[] args)
    {
        model.JQuery = this;
        if (args != null)
            _ctrl = args.OfType<Controller>().FirstOrDefault();
    }

    //public string Name
    //{
    //    get { return string.Empty; }
    //}

    //public string View
    //{
    //    get {return "raw"; }
    //}

    //public object Model
    //{
    //    get { return @"<script src='/Scripts/jquery-2.1.4.js'></script>"; }
    //}

    public string GetViewOfType(string type)
    {
        if (type == "corejs")
            return string.Format("<script src='{0}'></script>" ,_ctrl.Url.Content("/Scripts/jquery-2.1.4.js"));

        return null;
    }
}