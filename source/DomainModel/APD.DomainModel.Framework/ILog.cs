using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APD.DomainModel.Framework
{
    public interface ILog
    {
        void WriteEntry(LogEntry entry);

        int VerbosityLevel { get; set; }
    }
}
