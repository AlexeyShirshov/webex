using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebEx.Core
{
    public class DependencyComparer : IComparer<IModule>
    {
        private HttpApplicationStateBase _vd;
        public DependencyComparer()
        {

        }
        public DependencyComparer(HttpApplicationStateBase viewData)
        {
            _vd = viewData;
        }
        public int Compare(IModule x, IModule y)
        {
            if (Object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (y == null)
                    return 1;
                else
                {
                    var dep = x.GetModuleDependencies(_vd);
                    if (dep.Contains(y.GetType()))
                        return 1;

                    dep = y.GetModuleDependencies(_vd);
                    if (dep.Contains(x.GetType()))
                        return -1;
                    
                    return 0;
                }
            }
        }
    }
}
