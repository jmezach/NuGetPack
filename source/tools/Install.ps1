param($installPath, $toolsPath, $package, $project)

Import-Module (Join-Path $toolsPath "MSBuild.psm1")

function Delete-Temporary-File 
{
    $project.ProjectItems | Where-Object { $_.Name -eq 'NuGetPack-Readme.txt' } | Foreach-Object {
        Remove-Item ( $_.FileNames(0) )
        $_.Remove() 
    }
}

function Get-RelativePath ( $folder, $filePath ) 
{
    Write-Verbose "Resolving paths relative to '$Folder'"
    $from = $Folder = split-path $Folder -NoQualifier -Resolve:$Resolve
    $to = $filePath = split-path $filePath -NoQualifier -Resolve:$Resolve

    while($from -and $to -and ($from -ne $to)) {
        if($from.Length -gt $to.Length) {
            $from = split-path $from
        } else {
            $to = split-path $to
        }
    }

    $filepath = $filepath -replace "^"+[regex]::Escape($to)+"\\"
    $from = $Folder
    while($from -and $to -and $from -gt $to ) {
        $from = split-path $from
        $filepath = join-path ".." $filepath
    }
    Write-Output $filepath
}

function Install-Targets ( $project, $importFile )
{
    $buildProject = Get-MSBuildProject $project.Name

    $buildProject.Xml.Imports | Where-Object { $_.Project -match "NuGetPack" } | foreach-object {     
        Write-Host ("Removing old import:      " + $_.Project)
        $buildProject.Xml.RemoveChild($_) 
    }

    $projectItem = Get-ChildItem $project.FullName
    Write-Host ("Adding MSBuild targets import: " + $importFile)

    $target = $buildProject.Xml.AddImport( $importFile )

    $project.Save()
}

function Get-NuGetPackTargetsPath ($project) {
    $projectItem = Get-ChildItem $project.FullName
    $importFile = Join-Path $toolsPath "..\targets\NuGetPack.targets"
    $importFile = Resolve-Path $importFile
    $importFile = Get-RelativePath $projectItem.Directory $importFile 
    return $importFile
}

function Copy-NuGetPackTargetsToSolutionRoot($project) {
    $solutionDir = Get-SolutionDir
    $nupackFolder = (Join-Path $solutionDir .nugetpack)

    # Get the target file's path
    $targetsFolder = Join-Path $toolsPath "..\targets" | Resolve-Path
    
    if(!(Test-Path $nugetpackFolder)) {
        mkdir $nugetpackFolder | Out-Null
    }

    $nugetpackFolder = resolve-path $nugetpackFolder

    Write-Host "Copying NuGetPack MSBuild targets to: $nugetpackFolder"

    Copy-Item "$targetsFolder\*.*" $nugetpackFolder -Force | Out-Null

    Write-Host "IMPORTANT: You must commit/check in the .nugetpack folder to your source control system"

    $projectItem = Get-ChildItem $project.FullName
    return '$(SolutionDir)\.nugetpack\NuGetPack.targets'
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

    #$addToSolution = (Get-MSBuildProperty RestorePackages $project.Name).EvaluatedValue

    #$importFile = ''

    #if($addToSolution){
    #    Write-Host "NuGet package restore is enabled. Adding NuGetPack to the solution directory."
    #    $importFile = Copy-NuGetPackTargetsToSolutionRoot $project
    #} else {
    #    Write-Host "NuGet package restore is not enabled. Adding NuGetPack from the package directory."
    #    $importFile = Get-NuGetPackTargetsPath $project
    #}

    #Install-Targets $project $importFile

    Generate-NuSpecFile $project $toolsPath

    Write-Host ("NuGetPack installed successfully")
}

Main
