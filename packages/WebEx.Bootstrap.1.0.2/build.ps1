﻿nuget pack "F:\projects\nuget\custom packages\WebEx.Bootstrap\WebEx.Bootstrap.1.0.2.nuspec" -OutputDirectory "F:\projects\nuget\custom packages"
nuget.exe push 'F:\projects\nuget\custom packages\WebEx.Bootstrap.1.0.2.nupkg' -s http://owa:8001/ webex