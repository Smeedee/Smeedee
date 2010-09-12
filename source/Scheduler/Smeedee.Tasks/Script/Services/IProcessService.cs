using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smeedee.Tasks.Script.Services
{
    public interface IProcessService
    {
        void Start(string workingDirectory, string fileName, string args);
    }
}
