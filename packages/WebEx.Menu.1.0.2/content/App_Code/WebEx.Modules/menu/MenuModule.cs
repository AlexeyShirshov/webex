using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebEx.Core;

public class MenuModule : IModuleWithModel
{
    private IEnumerable<MenuItem> _menu;
    public MenuModule(Controller ctrl)
    {
        _menu = LoadMenu(ctrl);
    }
    public static IEnumerable<MenuItem> LoadMenu(Controller ctrl)
    {
        return new[] { 
            new MenuItem {Name = "Main", Url=ctrl.Url.Action("index", new {page=""})},
            new MenuItem {Name = "About", Url=ctrl.Url.Action("index", new {page="about"})}
        };
    }

    public IEnumerable<MenuItem> Menu
    {
        get
        {
            return _menu;
        }
    }
    public object Model
    {
        get
        {
            return Menu;
        }
    }

    public IModuleView GetView(string type, HtmlHelper html)
    {
        return new ModuleAutoView(type);
    }
}

public class MenuItem
{
    public string Name { get; set; }
    public string Url { get; set; }
}