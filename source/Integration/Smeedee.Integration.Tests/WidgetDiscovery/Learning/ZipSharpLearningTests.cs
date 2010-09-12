using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Integration.Tests.WidgetDiscovery.Learning
{
    [TestFixture][Category("IntegrationTest")]
    public class when_unzipping_xap_files
    {
        private MemoryStream memoryStream;
        private ZipFile zipFile;

        [SetUp]
        public void Setup()
        {
            memoryStream = new MemoryStream(TestResources.Smeedee_Widget_SourceControl_SL);
            zipFile = new ZipFile(memoryStream);
        }

        [Test]
        public void How_to_read_xap_files()
        {

            zipFile.ShouldNotBeNull();
        }

        [Test]
        public void How_to_find_files_in_xap()
        {
            int count = 0;
            foreach (var entry in zipFile.Cast<ZipEntry>())
            {
                entry.IsFile.ShouldBeTrue();
                count++;
            }

            count.ShouldBe(25);
        }

        [Test]
        public void How_to_decompress_a_file_in_the_xap_into_memory()
        {
            foreach (var file in zipFile.Cast<ZipEntry>())
            {
                byte[] buffer = new byte[file.Size];
                var unZipStream = zipFile.GetInputStream(file);
                unZipStream.Read(buffer, 0, buffer.Length);

                buffer.Any(b => b != 0).ShouldBeTrue();
            }
        }

        [Test]
        public void How_to_load_assemblies_form_xap_file()
        {
            foreach (var file in zipFile.Cast<ZipEntry>())
            {
                byte[] buffer = new byte[file.Size];
                var unZipStream = zipFile.GetInputStream(file);
                unZipStream.Read(buffer, 0, buffer.Length);

                if (file.Name.EndsWith(".dll"))
                {
                    var assembly = Assembly.Load(buffer);
                    assembly.ShouldNotBeNull();
                }
            }
        }

    }
}
