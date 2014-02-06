using Microsoft.Build.Framework;

namespace NuGetPack.Tasks
{
    public abstract class AbstractTask : ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }
        
        public abstract bool Execute();

        protected void LogMessage(string message, MessageImportance importance = MessageImportance.High)
        {
            BuildEngine.LogMessageEvent(new BuildMessageEventArgs("NuGetPack: " + message, "NuGetPack", "NuGetPack", importance));
        }

        protected void LogWarning(string code, string message)
        {
            BuildEngine.LogWarningEvent(new BuildWarningEventArgs("NuGetPack", code, null, 0, 0, 0, 0, message, "NuGetPack", "NuGetPack"));
        }

        protected void LogError(string code, string message)
        {
            BuildEngine.LogErrorEvent(new BuildErrorEventArgs("NuGetPack", code, null, 0, 0, 0, 0, message, "NuGetPack", "NuGetPack"));
        }
    }
}