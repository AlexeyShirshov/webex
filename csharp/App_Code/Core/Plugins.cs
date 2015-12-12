using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Plugins
/// </summary>
public class Plugins
{
	public Plugins()
	{
		
	}
    [ImportMany(typeof(IExternalModule))]
    public IEnumerable<IExternalModule> ExternalModules { get; set; }
}