using System;

namespace Smeedee.Tasks.SourceControl.Git
{
    public interface IAccessFileSystems
    {
        string AppDataDirectory { get; }

        string ProgramFilesDirectory { get; }

        void WriteFile(string path, string fileName, string contents);

        bool DirectoryExists(string path);

        void CreateDirectory(string path);

        void SetCurrentDirectory(string path);
    }
}