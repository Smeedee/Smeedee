using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APD.Harvester.Framework.Services
{
    public interface IManageFileSystems
    {
        IEnumerable<string> GetFiles(string directory);
        void Move(string fromFile, string toFile);
    }
}
