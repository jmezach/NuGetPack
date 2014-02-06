using System;

namespace NuGetPack.Tasks.Util
{
    public interface IFileInfo
    {
        string Extension { get; }
        DateTime LastAccessTimeUtc { get; }
        DateTime LastWriteTimeUtc { get; }
    }
}