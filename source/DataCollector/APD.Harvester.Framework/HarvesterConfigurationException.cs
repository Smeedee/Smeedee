using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APD.Harvester.Framework
{
    public class HarvesterConfigurationException : Exception
    {
        public HarvesterConfigurationException(string message)
            : base(message)
        {
            
        }
    }
}
