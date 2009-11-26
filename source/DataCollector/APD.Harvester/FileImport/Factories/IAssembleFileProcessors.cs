using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APD.Harvester.FileImport.Services;

namespace APD.Harvester.FileImport.Factories
{
    public interface IAssembleFileProcessors
    {
        IProcessFiles Assemble(string fileExtension);
    }
}
