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
using System.Linq;
using APD.DataCollector;
using APD.DomainModel.Framework.Logging;

using Moq;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.Harvester.Framework;


namespace APD.DataCollectorTests.HarvesterLoaderSpecs
{
    public class Shared
    {
        protected static Mock<IGetHarvesterCatalog> catalogMock;
        protected static Mock<ILog> logMock;
        protected static Mock<IScheduler> schedulerMock;
        protected static Mock<IAssembleHarvesters> harvesterFactory;
        protected static HarvesterLoader instance;

        protected static List<AbstractHarvester> registeredHarvesters;

        protected static List<Type> typesToCreate;

        private static void SetupMocks()
        {
            logMock = new Mock<ILog>();
            catalogMock = new Mock<IGetHarvesterCatalog>();
            schedulerMock = new Mock<IScheduler>();
            harvesterFactory = new Mock<IAssembleHarvesters>();

            registeredHarvesters = new List<AbstractHarvester>();
            typesToCreate = new List<Type>
                            {
                                typeof(DummyHarvester), 
                                typeof(YADummyHarvester), 
                                typeof(DummyHarvester)
                            };

            schedulerMock.Setup(s => s.RegisterHarvesters(It.IsAny<IEnumerable<AbstractHarvester>>())).Callback(
                (IEnumerable<AbstractHarvester> harvesters) => 
                    registeredHarvesters.AddRange(harvesters));

            harvesterFactory.Setup(f => f.Assemble(It.IsAny<Type>())).Returns(
                (Type harvesterType) =>
                    Activator.CreateInstance(harvesterType) as AbstractHarvester
                );

            catalogMock.Setup(c => c.GetCatalog()).Returns(typesToCreate);
        }

        protected Context an_instance_is_created = () =>
        {
            SetupMocks();
            instance = new HarvesterLoader(schedulerMock.Object,catalogMock.Object, logMock.Object, harvesterFactory.Object);
        };

    }


    public class DummyHarvester : AbstractHarvester
    {
        public override void DispatchDataHarvesting()
        {throw new NotImplementedException();}
    }
    public class YADummyHarvester : AbstractHarvester
    {
        public override void DispatchDataHarvesting()
        {throw new NotImplementedException();}
    }

    [TestFixture]
    public class when_spawned : Shared
    {
        [Test]
        public void should_be_instanced()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(an_instance_is_created);
                scenario.When("the instance is accessed");
                scenario.Then("it was created", () => instance.ShouldNotBeNull());
            });
        }

        [Test]
        public void assure_registers_correct_number_of_harvester_instances()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(an_instance_is_created);
                scenario.When("when the harvesters have been instanced");
                scenario.Then("all specified harvesters were instanced", () =>
                {
                    registeredHarvesters.Count().ShouldBe(typesToCreate.Count);
                    for (int i = 0; i < registeredHarvesters.Count(); i++)
                    {
                        //Console.WriteLine("found a " + registeredHarvesters[i].GetType());
                        (registeredHarvesters[i].GetType() == typesToCreate[i]).ShouldBeTrue();
                    }
                });
            });
        }
    }
}
