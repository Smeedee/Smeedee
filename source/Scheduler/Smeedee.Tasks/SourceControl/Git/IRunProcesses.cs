using System.Diagnostics;

namespace Smeedee.Tasks.SourceControl.Git
{
    public interface IRunProcesses
    {
        void RunProcess(Process process);
    }
}