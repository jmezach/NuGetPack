**NuGetPack is a NuGet package for creating [NuGet](www.nuget.org) packages.**

That's a lot of NuGet in one sentence, so let me explain that. Using NuGet packages from Visual Studio is a fairly straight forward process. Just open the NuGet package manager and off you go. However, creating NuGet packages isn't so straight forward, nor is publishing them to some NuGet feed. With **NuGetPack** it now is. Just install NuGetPack into your project, modify the .nuspec file, that is added to the project automatically, to your liking and build your solution with the RunNuGetPack property set to true and you're good to go.

Some of this might be familiar since [OctoPack](https://github.com/OctopusDeploy/OctoPack) does something similar. However, OctoPack is specifically targeted at creating Octopus Deploy packages, while NuGetPack creates general purpose NuGet packages.

## Installing NuGetPack

Given any class library project, creating a NuGet package from it is easy.

1. Ensure you have installed NuGet into your Visual Studio
2. From the View menu, open Other Windows -> Package Manager Console
3. In the Default Project drop down, choose the project that you would like to package

Install the NuGetPack package by typing:

    Install-Package NuGetPack 

By default NuGetPack is installed as a so called development dependency. This means that NuGetPack will not show up as a dependency for your package. This however requires at least NuGet 2.8.

## Modifying the .nuspec file

Upon installation of **NuGetPack** into a project a .nuspec file is automatically created from the project and added to the project as a file. After installation you can modify the .nuspec file however you want. 
 
## Building packages

To have NuGetPack create a NuGet package from your build, set the `RunNuGetPack` MSBuild property to `true`. For example, if you are compiling from the command line, you might use:

    msbuild MySolution.sln /t:Build /p:RunNuGetPack=true

After the build completes, in the output directory you will find a NuGet package. This package is ready to be pushed to any NuGet repository you might use (a file share, NuGet.org or maybe a MyGet feed).

## Version numbers

NuGet packages have version numbers. 
When you use NuGetPack, the NuGet package version number will come from (in order of priority):

 1. The command line, if you pass `/p:NuGetPackPackageVersion=<version>` as an MSBuild parameter when building your project
 2. The `[assembly: AssemblyVersion]` attribute in your `AssemblyInfo.cs` file

## Publishing

To publish your package to a NuGet feed, you can optionally use some extra MSBuild properties:

 - `/p:NuGetPackPublishPackageToFileShare=C:\MyPackages` - copies the package to the path given
 - `/p:NuGetPackPublishPackageToHttp=http://my-nuget-server/api/v2/package` - pushes the package to the NuGet server
 - `/p:NuGetPackPublishApiKey=ABCDEFGMYAPIKEY` - API key to use when publishing
