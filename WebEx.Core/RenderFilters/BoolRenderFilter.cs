using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebEx.Core
{
    public class BoolRenderFilter : IPreRenderFilter
    {
        private readonly bool _condition;

        public BoolRenderFilter(bool condition)
        {
            _condition = condition;
        }
        public PreRenderFilterResult Exec(HtmlHelper helper, string moduleInstanceId, IDictionary<string, object> args, string renderedViewName, IModuleView view)
        {
            if (!_condition)
                return PreRenderFilterResult.DontRender;

            return null;
        }
    }
}
