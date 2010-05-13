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
using System.Linq;
using APD.DataCollector;
using APD.DomainModel.Framework.Logging;

using Moq;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.Harvester.Framework;


namespace APD.DataCollectorTests.DirectoryHarvesterLoaderSpecs
{
    public class MyHarvesterForTEsting : AbstractHarvester
    {
        public override void DispatchDataHarvesting()
        {
            throw new NotImplementedException();
        }
    }

    public class Shared
    {
        protected static Mock<ILog> loggerMock;
        protected static string existingDirectoryPath = Environment.CurrentDirectory; 
        protected static string nonexistingDirectoryPath = "C:\\WindowsHas\\no\\folder\\here";
        protected static DirectoryHarvesterCatalogLoader harvesterCatalogLoader;

        protected Context a_valid_harvesterCatalog_is_created = () =>
        {
            loggerMock = new Mock<ILog>();
            harvesterCatalogLoader = new DirectoryHarvesterCatalogLoader(existingDirectoryPath, new ConsoleLogger());
        };

        protected static void CreateInvalidHarvesterCataloger()
        {
            loggerMock = new Mock<ILog>();
            harvesterCatalogLoader = new DirectoryHarvesterCatalogLoader(nonexistingDirectoryPath,loggerMock.Object);
        }

        protected Context a_harvester_registeres = () =>
        {

        };
    }


    [TestFixture]
    public class when_spawned  : Shared
    {
        [Test]
        public void should_throw_if_directory_does_not_exist()
        {
            Scenario.StartNew(this, scenario =>
                {
                    scenario.Given("The given directory path is non-existing");

                    scenario.When("The directoryloader is creted");

                    scenario.Then("It should throw an exception", () =>
                       {
                           this.ShouldThrowException<ArgumentException>(() =>
                                     CreateInvalidHarvesterCataloger(), ex => 
                                         ex.ShouldBeInstanceOfType<ArgumentException>());
                       });
                });
        }
    }

    [TestFixture]
    public class when_loading_harvesters : Shared
    {
        [Test]
        public void should_return_a_list_of_harvesters()
        {
            loggerMock = new Mock<ILog>();
            DirectoryHarvesterCatalogLoader catalogLoader = new DirectoryHarvesterCatalogLoader(existingDirectoryPath, loggerMock.Object);
            IEnumerable<Type> harvesters = catalogLoader.GetCatalog();
            harvesters.ShouldNotBeNull();
        }

        [Test]
        public void should_use_the_given_logger()
        {
            loggerMock = new Mock<ILog>();
            DirectoryHarvesterCatalogLoader catalogLoader = new DirectoryHarvesterCatalogLoader(existingDirectoryPath, loggerMock.Object);
            IEnumerable<Type> harvesters = catalogLoader.GetCatalog();
            loggerMock.Verify(l=>l.WriteEntry(It.IsAny<InfoLogEntry>()), Times.AtLeastOnce());
        }

        [Test]
        public void should_only_return_types_that_inherit_from_abstractHarvester()
        {
            Scenario.StartNew(this, scenario =>
                {
                    IEnumerable<Type> harvesters = null;

                    scenario.Given(a_valid_harvesterCatalog_is_created);
                    scenario.When("harvester catalog has been created", ()=>
                    {
                        harvesters = harvesterCatalogLoader.GetCatalog();
                    });
                    scenario.Then("all types in the catalog should inherit from abstractHarvester", () =>
                    {
                        bool hasHarvesters = harvesters.Count() > 0;
                        hasHarvesters.ShouldBeTrue();
                        Type baseType = typeof (AbstractHarvester);
                        foreach (var type in harvesters)
                        {
                            bool typeInheritsBaseType =  type.IsSubclassOf(baseType);
                            typeInheritsBaseType.ShouldBeTrue();
                        }
                    });
                });
        }

        [Test]
        public void should_ignore_invalid_dll_files()
        {
            
            Scenario.StartNew(this, scenario =>
                {
                    IEnumerable<Type> harvesters = null;

                    scenario.Given("an invalid dll exists in the directory", ()=>
                    {
                        loggerMock = new Mock<ILog>();
                        harvesterCatalogLoader = new DirectoryHarvesterCatalogLoader(Path.Combine(Environment.CurrentDirectory,"Testdata"), loggerMock.Object);
                    });
                    scenario.When("harvester catalog has been created", ()=>
                    {
                        harvesters = harvesterCatalogLoader.GetCatalog();
                    });
                    scenario.Then("assure the faulty dll load was logged as a warning", () =>
                    {
                        loggerMock.Verify(l=>l.WriteEntry(It.IsAny<WarningLogEntry>()), Times.Once());
                    });
                });
        }
    }
}
