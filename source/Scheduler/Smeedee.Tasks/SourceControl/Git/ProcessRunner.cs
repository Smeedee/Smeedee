using System;
using System.Diagnostics;

namespace Smeedee.Tasks.SourceControl.Git
{
    public class ProcessRunner : IRunProcesses
    {
        public void RunProcess(Process process)
        {
            process.Start();
            process.WaitForExit();
            int exitCode = process.ExitCode;
            process.Close();

            if (exitCode != 0)
                throw new InvalidProgramException("Git exited with exit code " + process.ExitCode);
        }
    }
}