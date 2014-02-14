param($installPath, $toolsPath, $package, $project)

Import-Module (Join-Path $toolsPath "MSBuild.psm1")

function Delete-Temporary-File 
{
    $project.ProjectItems | Where-Object { $_.Name -eq 'NuGetPack-Readme.txt' } | Foreach-Object {
        Remove-Item ( $_.FileNames(0) )
        $_.Remove() 
    }
}

function Generate-NuSpecFile($project, $toolsPath) {
    # Get the path to NuGet and to the project
    $nugetPath = Join-Path $toolsPath "\NuGet.exe" | Resolve-Path
    $projectPath = Split-Path $project.FullName -parent

    Write-Host "Generating NuSpec file for: $($project.Name)"

    cd $projectPath
    $output = & "$nugetPath" spec $($project.Name) -NonInteractive
    Write-Host $output
    cd ..

    # Get the path to the generated NuSpec file
    $nugetSpecPath = Join-Path $projectPath ($project.Name + ".nuspec")

    $project.ProjectItems.AddFromFile($nugetSpecPath)
}

function Main 
{
    Delete-Temporary-File

    Generate-NuSpecFile $project $toolsPath

    Write-Host ("NuGetPack installed successfully")
}

Main
