## This is the NuGetPack build script, written in PSAKE. Run with 
## 
##   Import-Module .\tools\psake\psake.psm1
##   Invoke-psake .\build.ps1
##

Framework "4.0"

properties {
	$build_number = "1.0.2"
    $configuration = "Release"
    $nuget_path = "tools\nuget.exe"
}

task default -depends VerifyProperties, Test, Package

task Clean {
	write-host "Clean"

    $directories = ".\build"

    $directories | ForEach-Object { 
        Write-Output "Clean directory $_"
        Remove-Item $_ -Force -Recurse -ErrorAction Ignore
        New-Item -Path $_ -ItemType Directory -Force
    }
}

task Versions {
	write-host "Apply version stamp"
	
	Generate-Assembly-Info `
        -file "source\NuGetPack.Tasks\Properties\AssemblyInfo.cs" `
        -title "NuGetPack Tasks $build_number" `
        -description "MSBuild tasks for NuGetPack" `
        -company "" `
        -product "NuGetPack $build_number" `
        -clsCompliant false `
        -version $build_number `
        -copyright ""	
}

task Build -depends Clean, Versions {
	write-host "Build"
    
    exec {
        msbuild .\source\NuGetPack.sln /p:Configuration=$configuration /t:Build
    }
}

task Test -depends Build {
	write-host "Run unit tests"
}

task Package -depends Build {
	write-host "Package"

    mkdir .\build\content
    mkdir .\build\build
    mkdir .\build\tools
    dir -recurse .\source\NuGetPack.Tasks\bin\$configuration | copy -destination build\build
    dir -recurse .\source\build | copy -destination build\build
    dir -recurse .\source\tools | copy -destination build\tools
    dir -recurse .\source\content | copy -destination build\content
    Copy-Item .\source\NuGetPack.nuspec .\build 
    Copy-Item .\source\tools\NuGet.exe .\build\build

    $base = (resolve-path "build")
    write-host $base
	exec {
        & $nuget_path pack build\NuGetPack.nuspec -basepath $base -outputdirectory $base -version $build_number -NoPackageAnalysis
    }	
}

## Helpers

task VerifyProperties {
	Assert (![string]::IsNullOrEmpty($build_number)) 'Property build_number was null or empty'

    Write-Output "Build number: $build_number"
}

function Generate-Assembly-Info
{
    param(
	    [string]$clsCompliant = "true",
	    [string]$title, 
	    [string]$description, 
	    [string]$company, 
	    [string]$product, 
	    [string]$copyright, 
	    [string]$version,
	    [string]$file = $(throw "file is a required parameter.")
    )

    $asmInfo = "using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: CLSCompliantAttribute($clsCompliant )]
[assembly: ComVisibleAttribute(false)]
[assembly: AssemblyTitleAttribute(""$title"")]
[assembly: AssemblyDescriptionAttribute(""$description"")]
[assembly: AssemblyCompanyAttribute(""$company"")]
[assembly: AssemblyProductAttribute(""$product"")]
[assembly: AssemblyCopyrightAttribute(""$copyright"")]
[assembly: AssemblyVersionAttribute(""$version"")]
[assembly: AssemblyInformationalVersionAttribute(""$version"")]
[assembly: AssemblyFileVersionAttribute(""$version"")]
[assembly: AssemblyDelaySignAttribute(false)]
"

	$dir = [System.IO.Path]::GetDirectoryName($file)
	if ([System.IO.Directory]::Exists($dir) -eq $false)
	{
		Write-Host "Creating directory $dir"
		[System.IO.Directory]::CreateDirectory($dir)
	}
	Write-Host "Generating assembly info file: $file"
	Write-Output $asmInfo > $file
}

Import-Module .\tools\psake\teamcity.psm1

TaskSetup {
	if ($env:TEAMCITY_VERSION) {
	    TeamCity-ReportBuildProgress "Running task $($psake.context.Peek().currentTaskName)"	
	}
}
