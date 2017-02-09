using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebEx.Core
{
    public class ModuleInstance
    {
        public string InstanceId { get; set; }
        public IDictionary<string, object> Params;
        public string Folder { get; set; }
    }
}
