param($installPath, $toolsPath, $package, $project)

Import-Module (Join-Path $toolsPath "MSBuild.psm1")

function Main 
{
    Write-Host ("NuGetPack uninstalled successfully")
}

Main
