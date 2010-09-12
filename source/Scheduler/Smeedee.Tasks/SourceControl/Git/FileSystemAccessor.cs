using System;
using System.IO;

namespace Smeedee.Tasks.SourceControl.Git
{
    public class FileSystemAccessor : IAccessFileSystems
    {
        public string AppDataDirectory
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); }
        }

        public string ProgramFilesDirectory
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles); }
        }

        public void WriteFile(string path, string fileName, string contents)
        {
            Directory.CreateDirectory(path);
            string fullPath = Path.Combine(path, fileName);
            File.WriteAllText(fullPath, contents);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void SetCurrentDirectory(string path)
        {
            Directory.SetCurrentDirectory(path);
        }
    }
}