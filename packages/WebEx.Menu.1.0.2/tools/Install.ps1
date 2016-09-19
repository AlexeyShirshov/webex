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
 - If the same package is installed in multiple projects in a solution, the script runs each time the package is installed. 
 - The package must have files in the content or lib folder for Install.ps1 to run. Just having something in the tools folder will not kick this off.
 - If your package also has an init.ps1, install.ps1 runs after init.ps1.


#>

param($installPath, $toolsPath, $package, $project) 

#Write-Host $project.FullName
#Write-Host $project.FileName
$dir = ""
$project.Properties | % {
    $propName = $_.Name
    $val = $_.Value
    #Write-Host "$propName=$val"
    if ($propName -eq "FullPath"){
        $dir = $val
    }

    if ($propName -eq "CurrentWebsiteLanguage") {
        $projectType = $val
    }
}

$ext2del = ""
if($projectType -eq "Visual C#")
{
    $ext2del = "vb"
}
if($projectType -eq "Visual Basic")
{
    $ext2del = "cs"
}

if ($dir -ne "") {
    $ext2delhtml = $ext2del + "html"
    rm $dir\Views\WebEx.Modules\Menu\*.$ext2delhtml -Verbose
    rm $dir\App_Code\WebEx.Modules\menu\*.$ext2del -Verbose
}