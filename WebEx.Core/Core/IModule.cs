using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for IModule
/// </summary>
public interface IModule
{
    string GetViewOfType(string type);
}

public interface IModuleWithModel : IModule
{
    dynamic Model { get; }
}