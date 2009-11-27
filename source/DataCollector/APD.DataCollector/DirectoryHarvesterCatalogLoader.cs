#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using APD.DomainModel.Framework;
using APD.Harvester.Framework;


namespace APD.DataCollector
{
    public class DirectoryHarvesterCatalogLoader : IGetHarvesterCatalog
    {
        private readonly ILog logger;
        private DirectoryInfo harvesterDirectory;

        public DirectoryHarvesterCatalogLoader(string directoryPath, ILog logger)
        {
            this.logger = logger;
            if( Directory.Exists(directoryPath) == false )
                throw new ArgumentException("The directory " + directoryPath + " does not exist");
            else
                harvesterDirectory = new DirectoryInfo(directoryPath);
        }


        public IEnumerable<Type> GetCatalog()
        {
            List<Type> harvestersInDirectory = new List<Type>();

            LogInfoFormat("Starting discovery of harvesters");

            LogInfoFormat("Getting all DLLs (*.dll) in folder '{0}'", harvesterDirectory.FullName);
            FileInfo[] dllFilesInFolder = harvesterDirectory.GetFiles("*.dll", SearchOption.AllDirectories);

            LogInfoFormat("Found {0} dlls in folder", dllFilesInFolder.Length);

            foreach (var file in dllFilesInFolder)
            {
                Assembly assembly = LoadAssemblyFromFile(file);
                if( assembly != null )
                {
                    List<Type> harvestersInAssembly = GetHarvestersInAssembly(assembly);
                    harvestersInDirectory.AddRange(harvestersInAssembly);
                }
            }

            LogInfoFormat("Ended discovery of harvesters. (Found {0} of them)", harvestersInDirectory.Count);
            return harvestersInDirectory;
        }

        private Assembly LoadAssemblyFromFile(FileInfo file)
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.LoadFile(file.FullName);
                LogInfoFormat("Loaded the assembly '{0}'", file.Name);
            }
            catch (Exception ex)
            {
                LogWarningFormat("Could not load the file '{0}'. Message: {1}", file.FullName, ex.Message);
            }
            return assembly;
        }

        private List<Type> GetHarvestersInAssembly(Assembly assembly)
        {
            List<Type> harvestersInAssembly =  new List<Type>();
            Type[] typesInAssembly = assembly.GetTypes();
            LogInfoFormat("Found {0} types in '{1}'", typesInAssembly.Length, assembly.FullName);
            foreach (var type in typesInAssembly)
            {
                if( type.IsSubclassOf(typeof(AbstractHarvester)))
                {
                    harvestersInAssembly.Add(type);
                    LogInfoFormat("Found harvester '{0}'", type.Name);
                }
            }

            return harvestersInAssembly;
        }


        private void LogInfoFormat(string message, params object[] parameters)
        {
            logger.WriteEntry(new InfoLogEntry("DirectoryHarvesterCatalogLoader", string.Format(message, parameters)));
        }

        private void LogWarningFormat(string message, params object[] parameters)
        {
            logger.WriteEntry(new WarningLogEntry("DirectoryHarvesterCatalogLoader", string.Format(message, parameters)));
        }
    }
}
