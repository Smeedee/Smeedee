using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Smeedee.Tasks.Framework.Services.Impl
{
    public class FileSystemService : IManageFileSystems
    {
        public IEnumerable<string> GetFiles(string directory)
        {
            return Directory.GetFiles(directory);
        }

        public void Move(string fromFile, string toFile)
        {
            File.Copy(fromFile, toFile);
        }

        public string Read(string filePath)
        {
            string fileContent = null;

            using (var fs = File.OpenText(filePath))
            {
                fileContent = fs.ReadToEnd();
            }

            return fileContent;
        }
    }
}
