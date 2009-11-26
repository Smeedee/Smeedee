using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APD.Harvester.FileImport.Services
{
    public interface IKnowTheFileImportQueue
    {
        string GetDirectoryPath();
        string GetCompletedDirPath();
        string GetErrorDirPath();
        string GetUnrecognizedDirPath();
    }
}
