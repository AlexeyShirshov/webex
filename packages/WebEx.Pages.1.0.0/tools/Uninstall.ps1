<#

.PARAMETER installPath
    the path to the folder where the package is installed
.PARAMETER toolsPath
    the path to the tools directory in the folder where the package is installed
.PARAMETER package
    a reference to the package object
.PARAMETER project
    a reference to the EnvDTE project object and represents the project the package is installed into

.NOTES
runs every time a package is uninstalled

#>

param($installPath, $toolsPath, $package, $project) 

"uninstall from $project"