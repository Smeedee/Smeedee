using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Smeedee.Tasks.Script.Services.Impl
{
    public class ProcessService : IProcessService
    {
        public void Start(string workingDirectory, string fileName, string args)
        {
            Console.WriteLine("Start: " + fileName);
            Process.Start(new ProcessStartInfo(fileName, args)
            {
                WorkingDirectory = workingDirectory                   
            });
            Console.WriteLine(fileName + " finished");
        }
    }
}
