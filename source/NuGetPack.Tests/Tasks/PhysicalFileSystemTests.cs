﻿using NUnit.Framework;
using NuGetPack.Tasks.Util;

namespace NuGetPack.Tests.Tasks
{
    [TestFixture]
    public class PhysicalFileSystemTests
    {
        [Test]
        public void GetRelativePathTo_ShouldResolveCorrectPathForRegularContentFiles()
        {
            var uut = new PhysicalFileSystem();
            const string itemPath = @"C:\BuildAgent\work\3ecb795b3ad31160\Root\Web.config";
            const string projectFolder = @"C:\BuildAgent\work\3ecb795b3ad31160\Root\";

            var result = uut.GetPathRelativeTo(itemPath, projectFolder);

            Assert.That(result.ToLower(), Is.EqualTo("web.config".ToLower()));
        }

        [Test]
        public void GetRelativePathTo_ShouldResolveCorrectPathForLinkedContentFiles()
        {
            var uut = new PhysicalFileSystem();
            const string itemPath = @"C:\BuildAgent\work\3ecb795b3ad31160\Root\..\..\Configs\Global\GlobalAppSettings.config";
            const string projectFolder = @"C:\BuildAgent\work\3ecb795b3ad31160\Root\";

            var result = uut.GetPathRelativeTo(itemPath, projectFolder);

            Assert.That(result.ToLower(), Is.EqualTo(@"Configs\Global\GlobalAppSettings.config".ToLower()));
        }

        [Test]
        public void RemovePathTraversal_ShouldRemoveASingleSetOfPathTraversalCharacters()
        {
            var uut = new PhysicalFileSystem();
            const string path = @"..\UserControlServer\Enrollment\EnrollmentControl.ascx";

            var result = uut.RemovePathTraversal(path);

            Assert.That(result.ToLower(), Is.EqualTo(@"UserControlServer\Enrollment\EnrollmentControl.ascx".ToLower()));
        }

        [Test]
        public void RemovePathTraversal_ShouldRemoveMultipleSetsOfPathTraversalCharacters()
        {
            var uut = new PhysicalFileSystem();
            const string path = @"..\..\..\UserControlServer\Enrollment\EnrollmentControl.ascx";

            var result = uut.RemovePathTraversal(path);

            Assert.That(result.ToLower(), Is.EqualTo(@"UserControlServer\Enrollment\EnrollmentControl.ascx".ToLower()));
        }

        [Test]
        public void RemovePathTraversal_ShouldDoNothingIfThereAreNoPathTraversalCharacters()
        {
            var uut = new PhysicalFileSystem();
            const string path = @"UserControlServer\Enrollment\EnrollmentControl.ascx";

            var result = uut.RemovePathTraversal(path);

            Assert.That(result.ToLower(), Is.EqualTo(@"UserControlServer\Enrollment\EnrollmentControl.ascx".ToLower()));
        }
    }
}
