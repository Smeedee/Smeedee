using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.DomainModel.Framework;


namespace APD.Harvester.FileImport.Services
{
    public interface IProcessFiles
    {
        void Process(string filePath);
    }
}
