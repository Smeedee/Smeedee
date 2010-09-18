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
            var extractPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(xapFilePath));



            UnZipXap( xapFilePath, extractPath );
            DirectoryInfo extractDirInfo = new DirectoryInfo(extractPath);

            return extractDirInfo.GetFiles("*.dll", SearchOption.AllDirectories)
                .Select(af => Assembly.ReflectionOnlyLoadFrom(af.FullName));
        }

        private void UnZipXap(string xapFilePath, string unzipToFolderPath)
        {
            FastZip zip = new FastZip();
            zip.ExtractZip(xapFilePath, unzipToFolderPath, FastZip.Overwrite.Always, null, "", "", true);
        }
    }
}
