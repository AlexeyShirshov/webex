using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebEx.Core;

namespace PagesModule
{
    public class Page
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }
    [ModuleAlias("Pages")]
    public class PagesModule : IModuleWithModel
    {
        //private string _name;
        private string _view;
        private Page _page;
        public PagesModule(string name, string page)
        {
            _page = CreatePage(name);
            _view = "index";
        }
        private static Page CreatePage(string page)
        {
            string title = string.IsNullOrEmpty(page) ? "Главная" : "О компании";
            return new Page { Name = page, Title = title };
        }

        public object Model
        {
            get
            {
                return _page;
            }
        }

        public IModuleView GetView(string type, HtmlHelper html)
        {
            switch(type)
            {
                case Contracts.JavascriptView:
                    return new ModuleView("index.js");  
                default:
                    return new ModuleAutoView(type);
            }
        }
    }
}