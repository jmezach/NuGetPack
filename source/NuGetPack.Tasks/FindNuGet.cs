using Microsoft.Build.Framework;
using NuGetPack.Tasks.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NuGetPack.Tasks
{
    /// <summary>
    /// An MSBuild Task that finds the location of NuGet.exe.
    /// </summary>
    public class FindNuGet : AbstractTask
    {
        /// <summary>
        /// An <see cref="IFileSystem"/> implementation to use.
        /// </summary>
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// Default constructor for this class.
        /// </summary>
        public FindNuGet()
            : this(new PhysicalFileSystem())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FindNuGet"/> class using the specified <paramref name="fileSystem"/> implementation.
        /// </summary>
        /// <param name="fileSystem">An <see cref="IFileSystem"/> implementation to use.</param>
        public FindNuGet(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        /// <summary>
        /// The path to NuGet.exe.
        /// </summary>
        [Output]
        public string NuGetExePath
        {
            get;
            set;
        }

        /// <summary>
        /// Called when the task executes.
        /// </summary>
        /// <returns>Returns <c>true</c> if NuGet.exe was found, otherwise <c>false</c>.</returns>
        public override bool Execute()
        {
            if (string.IsNullOrWhiteSpace(NuGetExePath) || !_fileSystem.FileExists(NuGetExePath))
            {
                var nuGetPath = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.FullLocalPath()), "NuGet.exe");
                NuGetExePath = nuGetPath;
            }

            return _fileSystem.FileExists(NuGetExePath);
        }
    }
}
