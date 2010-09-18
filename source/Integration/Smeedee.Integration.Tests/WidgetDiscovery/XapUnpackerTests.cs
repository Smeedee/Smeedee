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
        public class When_getting_assemblies_from_file : Shared
        {
            private FileInfo validTestXap;

            [SetUp]
            public void Setup()
            {
                validTestXap = new FileInfo(@"..\..\Resources\Smeedee.Widget.SourceControl.SL.xap");
            }

            [Test]
            public void Assure_all_assemblies_are_returned()
            {
                validTestXap.Exists.ShouldBeTrue();
                var assembliesFound = unpacker.GetAssemblies(validTestXap.FullName);
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
                validTestXap.Exists.ShouldBeTrue();

                var assembliesFound = unpacker.GetAssemblies(validTestXap.FullName);
                var secondRun = unpacker.GetAssemblies(validTestXap.FullName);
            }


            [Test]
            [Ignore]
            public void Assure_types_can_be_loaded_from_assemblies()
            {
                var assembliesInXap = unpacker.GetAssemblies(validTestXap.FullName);
                foreach (var assembly in assembliesInXap)
                {
                    var types = assembly.GetTypes();
                    (types.Length > 0).ShouldBeTrue();
                }
            }
        }
    }
}