using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Smeedee.Integration.WidgetDiscovery;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Integration.Tests.WidgetDiscovery
{
    class XapUnpackerTests
    {
        public class Shared
        {
            protected XapUnpacker unpacker = new XapUnpacker();
        }

        [TestFixture][Category("IntegrationTest")]
        public class When_getting_assemblies_from_stream : Shared
        {
            [Test]
            public void Assure_all_assemblies_are_returned()
            {
                MemoryStream memoryStream = new MemoryStream(TestResources.Smeedee_Widget_SourceControl_SL);
                var assemblies = unpacker.GetAssemblies(memoryStream);
                assemblies.Count().ShouldBe(24);
            }


            [Test]
            public void Assure_corrupt_stream_throws_ArgumentException()
            {
                byte[] corrupt = new byte[] {23, 5, 123, 128, 255};
                MemoryStream memoryStream = new MemoryStream(corrupt);

                this.ShouldThrowException<ArgumentException>(() => unpacker.GetAssemblies(memoryStream));
            }
        }


        [TestFixture][Category("IntegrationTest")]
        public class When_getting_assemblies_from_file : Shared
        {
            [Test]
            public void Assure_all_assemblies_are_returned()
            {
                var testXap = new FileInfo(@"..\..\Resources\Smeedee.Widget.SourceControl.SL.xap");
                testXap.Exists.ShouldBeTrue();

                var assembliesFound = unpacker.GetAssemblies(testXap.FullName);
                
                assembliesFound.Count().ShouldBe(24);

            }

            [Test]
            public void Assure_non_existing_file_throws_fileNotFoundException()
            {
                this.ShouldThrowException<FileNotFoundException>(()=>unpacker.GetAssemblies("hello, i don't exist.png"));
            }

            [Test]
            public void Assure_file_is_closed()
            {
                var testXap = new FileInfo(@"..\..\Resources\Smeedee.Widget.SourceControl.SL.xap");
                testXap.Exists.ShouldBeTrue();

                var assembliesFound = unpacker.GetAssemblies(testXap.FullName);
                var secondRun = unpacker.GetAssemblies(testXap.FullName);
            }
        }
    }
}