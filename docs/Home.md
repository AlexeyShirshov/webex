**Project Description**
ASP.NET MVC based small library for extensible web applications.
# Intro
The key concept of the project is a **Module**.
[Module](Module) is a logic to pick up the view and optionally create model for that view. Logic is class that implements [IModule](IModule) interface.

Website does know nothing about modules. Modules doesn't know about website and another modules. So we get very loosely coupled application where different parts maybe developed and maintained separately.

How does it work? In short 
# controller [creates module](LoadModule) and puts it in ViewData
# view [render module](RenderModule) passing to Html.Partial method module model
Very simple idea and very simple [implementation](https://webex.codeplex.com/SourceControl/latest#WebEx.Core/).
# Quick start
## 1. Add WebEx.Core to you project [via nugget](https://www.nuget.org/packages/WebEx.Core/1.0.14)
## 2. Load modules in your controller. 
There are no modules in your application yet, but it's ok. We'll [add them](Create-module) later. There are multiple alternatives
* Load module explicitly
{code:c#}
public ActionResult index()
{
        LoadModules(); // you can pass parameters to this method
        return View();
}
{code:c#}
* Load module by attribute
{code:c#}
[ModuleActionFilter](ModuleActionFilter)
public ActionResult index()
{
        return View();
}
{code:c#}
The attribute [ModuleActionFilter](ModuleActionFilter) maybe applied to controller class. In that case all actions will load modules automatically.
* Implicitly load module for all actions in all controllers
{code:c#}
//in Global.asax file
void Application_Start(object sender, EventArgs e) 
{
       GlobalFilters.Filters.Add(new ModuleActionFilterAttribute());
}
{code:c#}
## 3. Render module in view
{code:aspx c#}
Hello from WebEx!

@Html.RenderModule("Pages") @**renders Pages module**@
{code:aspx c#}
## 4. [Register modules](Register-modules) in application
{code:c#}
//in Global.asax file
void Application_Start(object sender, EventArgs e) 
{
        ModulesCatalog.RegisterModules(Application);
}
{code:c#}
This is most costly operation in terms of performance, because it uses reflection to find all types that implements IModule interface. Fortunately it runs only once.
## 5. Run the application. 
You will see only "Hello from WebEx!" greeting and this is good. At least there are no exceptions about non-existent views! :)
Designers may use the [RenderModule](RenderModule) extension to layout pages while developers write the modules.
# What next?
Now we are ready to create module. Again there are multiple variants
* [Module without view](Module-without-view)
* [Module without logic](Module-without-logic) (only view)
* [Module without model](Module-without-model) (logic and view)
* [Full-fledged module](Full-fledged-module) contains logic, model and view
# Module examples
## JQuery module
This is a module without model and without view. It renders string.
{code:c#}
using System;
using System.Web;
using System.Web.Mvc;
using WebEx.Core;
/// <summary>
/// Usage @Html.RenderModule("JQuery")
/// </summary>
public class JQueryModule : IModule
{
    public IModuleView GetView(string type, HtmlHelper html)
    {
        var url = new UrlHelper(html.ViewContext.RequestContext);
        if (type == Contracts.JavascriptView || type == Contracts.DefaultView)
            return new ModuleViewString("<script src='{0}'></script>" ,url.Content("~/Scripts/jquery-2.1.4.js"));

        return null;
    }
}
{code:c#}
## Bootstrap module
The example demonstrates
* [Module dependency](Module-dependency)
* [Multiple views](Multiple-views)
{code:c#}
using System;
using System.Web;
using System.Web.Mvc;
using WebEx.Core;

[Dependency("JQuery")](Dependency(_JQuery_))
public class BootstrapModule : IModule    
{    
    public IModuleView GetView(string type, HtmlHelper html)
    {
        var url = new UrlHelper(html.ViewContext.RequestContext);
        switch(type)
        {
            case Contracts.CSSView:
                return new ModuleViewString("<link href='{0}' rel='stylesheet'/>", url.Content("~/Content/bootstrap/bootstrap.css"));
            case Contracts.JavascriptView:
                return new ModuleViewString("<script src='{0}'></script>", url.Content("~/Scripts/bootstrap.js"));
        }

        return null;
    }
}
{code:c#}
## Pages module
The example demonstrates
* Constructor with parameters
* [ModuleAutoView](ModuleAutoView)
* [ModuleAlias](ModuleAlias)
* [Module model](Module-model)
See [Pages module](https://webex.codeplex.com/wikipage?title=Pages&referringTitle=Documentation)
# Q & A
## What is the difference between module and partial view?
* Module is more than partial view. 
* It may have multiple views
* It doesn't throw exception if not exists
* It contains logic to create model
* It may have css and js rendered in layout page
* You can pass parameters to module view
{code:c# as.net}
@Html.RenderModule("Pages", args: new {id="pageId"}) /**Get param in view via @Html.GetViewParam("pageId") **/
{code:c# asp.net}
## Is the module costly?
No, it is very fast. As fast as asp.net. There is little reflection when mapping arguments between [LoadModule](LoadModule) method and constructor, but it's all.
## Can I use WebEx with regular partial view?
Absolutely!
{code:c# as.net}
@Html.RenderModule("~/Views/Shared/partial")
{code:c# asp.net}
Note - you may leave view extension, you may pass params to view as with module.
# Repository
There is nuget repository - [http://webex.shirshov.net/](http://webex.shirshov.net/) that you can [add to you project](Add-repository). You can contact me to publish your module in it.
Happy coding!