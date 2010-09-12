using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;

namespace Smeedee.Integration.WidgetDiscovery
{
    public class XapUnpacker : IGetAssembliesFromXAP
    {
        public IEnumerable<Assembly> GetAssemblies(string xapFilePath)
        {
            using (var fileStream = File.Open(xapFilePath, FileMode.Open)) 
            {
                return GetAssemblies(fileStream);
            }
        }

        public IEnumerable<Assembly> GetAssemblies(Stream xapFile)
        {
            try
            {
                ZipFile zipFile = new ZipFile(xapFile);
                return zipFile.Cast<ZipEntry>()
                    .Where(f => f.Name.EndsWith(".dll"))
                    .Select(entry => CreateAssembly(zipFile, entry))
                    .ToList();
            }
            catch (Exception e)
            {
                throw new ArgumentException("Unable to get assemblies", e);
            }
        }

        private Assembly CreateAssembly(ZipFile zipFile, ZipEntry assemblyFile)
        {
            byte[] buffer = new byte[assemblyFile.Size];
            var unZipStream = zipFile.GetInputStream(assemblyFile);
            unZipStream.Read(buffer, 0, buffer.Length);
            return Assembly.Load(buffer);
        }
    }
}
