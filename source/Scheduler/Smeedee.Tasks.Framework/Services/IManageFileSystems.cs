using System.Collections.Generic;

namespace Smeedee.Tasks.Framework.Services
{
    public interface IManageFileSystems
    {
        IEnumerable<string> GetFiles(string directory);
        void Move(string fromFile, string toFile);
        string Read(string filePath);
    }
}
