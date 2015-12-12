using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;

[InheritedExport]
/// <summary>
/// Summary description for IExternalModule
/// </summary>
public interface IExternalModule
{
	void Load(dynamic model, params object[] args);
}