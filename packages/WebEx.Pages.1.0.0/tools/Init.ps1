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
 - If the same package is installed into additional projects in the solution, the script is not run during those installations. 
 - The script also runs every time the solution is opened (Package Manager Console window has to be open at the same for the script to run). 
 For example, if you install a package, close Visual Studio, and then start Visual Studio and open the solution with Package Manager Console window, 
 the Init.ps1 script runs again.

#>

param($installPath, $toolsPath, $package, $project) 

"init in $project"