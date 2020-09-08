using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebEx.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited=true)]
    public abstract class DynamicLoadAttribute : Attribute
    {
        // Properties
        public abstract bool ShouldLoad { get; }
    }
}
