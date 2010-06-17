using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Smeedee.DomainModel.Framework.Logging
{
    public interface ILog
    {
        void WriteEntry(LogEntry entry);

        int VerbosityLevel { get; set; }
    }
}