using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebEx.Core
{
    public interface IRenderFilterProvider
    {
        IEnumerable<IPreRenderFilter> GetPreRenderFilters();
        IEnumerable<IPostRenderFilter> GetPostRenderFilters();
    }
    public interface IPreRenderFilter
    {
        PreRenderFilterResult Exec(HtmlHelper helper, string moduleInstanceId, IDictionary<string, object> args, string renderedViewName, IModuleView view);
    }
    public class RenderFilterResult
    {
        public IModuleView view { get; set; }
        public object model { get; set; }
        public bool Add2RenderIfMainViewRendered { get; set; }
    }
    //public enum PreRenderFilterResultModeEnum
    //{
    //    None,
    //    Add2Render,
    //    Add2RenderIfMainViewRendered,
    //    ReplaceRender,
    //    DontRenderMainView
    //}
    public class PreRenderFilterResult : RenderFilterResult
    {
        public static PreRenderFilterResult DontRender = new PreRenderFilterResult() { DontRenderMainView = true };
        //public PreRenderFilterResultModeEnum FilterResultMode { get; set; }
        public bool DontRenderMainView { get; set; }
    }

    public interface IPostRenderFilter
    {
        RenderFilterResult Exec(HtmlHelper helper, string moduleInstanceId, IDictionary<string, object> args, string renderedViewName, IModuleView view, MvcHtmlString mainViewResult);
    }
}
