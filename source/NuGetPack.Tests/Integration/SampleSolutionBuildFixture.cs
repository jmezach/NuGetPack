using System.IO;
using NUnit.Framework;

namespace NuGetPack.Tests.Integration
{
    [TestFixture]
    public class SampleSolutionBuildFixture : BuildFixture
    {
        [Test]
        public void ShouldBuildAtSolutionLevel()
        {
            MsBuild("Samples.sln /p:RunNuGetPack=true /p:NuGetPackPackageVersion=1.0.9 /p:Configuration=Release /v:m");

            AssertPackage(@"Sample.ConsoleApp\pkg\Sample.ConsoleApp.1.0.9.nupkg", 
                pkg => pkg.AssertContents(
                    "lib\\net40\\Sample.ConsoleApp.exe"));

            AssertPackage(@"Sample.ClassLibrary\pkg\Sample.ClassLibrary.1.0.9.nupkg",
                pkg => pkg.AssertContents(
                    "lib\\net45\\Sample.ClassLibrary.dll"
                ));
        }

        [Test]
        public void ShouldBuildAtProjectLevel()
        {
            MsBuild("Sample.ConsoleApp\\Sample.ConsoleApp.csproj /p:RunNuGetPack=true /p:NuGetPackPackageVersion=1.0.10 /p:Configuration=Release /v:m");

            AssertPackage(@"Sample.ConsoleApp\pkg\Sample.ConsoleApp.1.0.10.nupkg",
                pkg => pkg.AssertContents(
                    "lib\\net40\\Sample.ConsoleApp.exe"));
        }

        [Test]
        public void ShouldBuildWithSpec()
        {
            MsBuild("Sample.WebAppWithSpec\\Sample.WebAppWithSpec.csproj /p:RunNuGetPack=true /p:NuGetPackPackageVersion=1.0.10 /p:Configuration=Release /v:m");

            AssertPackage(@"Sample.WebAppWithSpec\pkg\Sample.WebAppWithSpec.1.0.10.nupkg",
                pkg => Assert.That(pkg.Title, Is.EqualTo("Sample application")));
        }

        [Test]
        public void ShouldCopyToFileShare()
        {
            File.WriteAllText("Notes.txt", "Hello world!");

            MsBuild("Samples.sln /p:RunNuGetPack=true /p:NuGetPackPackageVersion=1.1.5 /p:NuGetPackPublishPackageToFileShare=..\\Packages /p:Configuration=Release /v:m");

            AssertPackage(@"Packages\Sample.ConsoleApp.1.1.5.nupkg", pkg => Assert.That(pkg.Version.ToString(), Is.EqualTo("1.1.5")));
            AssertPackage(@"Packages\Sample.WebApp.1.1.5.nupkg", pkg => Assert.That(pkg.Version.ToString(), Is.EqualTo("1.1.5")));
            AssertPackage(@"Packages\Sample.WebAppWithSpec.1.1.5.nupkg", pkg => Assert.That(pkg.Version.ToString(), Is.EqualTo("1.1.5")));
        }

        [Test]
        public void ShouldPackageClassLibrary()
        {
            MsBuild("Sample.ClassLibrary\\Sample.ClassLibrary.csproj /p:RunNuGetPack=true /p:NuGetPackPackageVersion=1.0.9 /p:Configuration=Release");

            AssertPackage(@"Sample.ClassLibrary\pkg\Sample.ClassLibrary.1.0.9.nupkg",
                pkg => pkg.AssertContents(
                    "lib\\net45\\Sample.ClassLibrary.dll"
                ));
        }

        [Test]
        public void ShouldPackageClassLibraryWithSymbols()
        {
            MsBuild("Sample.ClassLibrary\\Sample.ClassLibrary.csproj /p:RunNuGetPack=true /p:NuGetPackPackageVersion=1.0.9 /p:NuGetPackSymbolsPackage=true /p:Configuration=Release");

            AssertPackage(@"Sample.ClassLibrary\pkg\Sample.ClassLibrary.1.0.9.nupkg",
                pkg => pkg.AssertContents(
                    "lib\\net45\\Sample.ClassLibrary.dll"
                ));

            AssertPackage(@"Sample.ClassLibrary\pkg\Sample.ClassLibrary.1.0.9.symbols.nupkg",
                pkg => pkg.AssertContents(
                    "lib\\net45\\Sample.ClassLibrary.dll",
                    "lib\\net45\\Sample.ClassLibrary.pdb",
                    "src\\Class1.cs",
                    "src\\Properties\\AssemblyInfo.cs"
                ));
        }

        [Test]
        public void ShouldAllowCustomFilesSection()
        {
            MsBuild("Sample.WebAppWithSpecAndCustomContent\\Sample.WebAppWithSpecAndCustomContent.csproj /p:RunNuGetPack=true /p:NuGetPackPackageVersion=1.0.11 /p:Configuration=Release");

            AssertPackage(@"Sample.WebAppWithSpecAndCustomContent\pkg\Sample.WebAppWithSpecAndCustomContent.1.0.11.nupkg",
                pkg => pkg.AssertContents(
                    "bin\\Sample.WebAppWithSpecAndCustomContent.dll",
                    "SomeFiles\\Foo.css",
                    "lib\\net40\\Sample.WebAppWithSpecAndCustomContent.dll",
                    "content\\Web.config"));
        }
    }
}