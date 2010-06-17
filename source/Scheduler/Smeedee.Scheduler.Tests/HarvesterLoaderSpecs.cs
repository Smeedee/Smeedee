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
using Moq;
using NUnit.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.Scheduler;
using Smeedee.Tasks.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace Smeedee.SchedulerTests.taskLoaderSpecs
{
    public class Shared
    {
        protected static Mock<IGetTaskCatalog> catalogMock;
        protected static Mock<ILog> logMock;
        protected static Mock<IScheduler> schedulerMock;
        protected static Mock<IAssembleTasks> taskFactory;
        protected static TaskLoader instance;

        protected static List<TaskBase> registeredTasks;

        protected static List<Type> typesToCreate;

        private static void SetupMocks()
        {
            logMock = new Mock<ILog>();
            catalogMock = new Mock<IGetTaskCatalog>();
            schedulerMock = new Mock<IScheduler>();
            taskFactory = new Mock<IAssembleTasks>();

            registeredTasks = new List<TaskBase>();
            typesToCreate = new List<Type>
                            {
                                typeof(DummyTask), 
                                typeof(YADummyTask), 
                                typeof(DummyTask)
                            };

            schedulerMock.Setup(s => s.RegisterTasks(It.IsAny<IEnumerable<TaskBase>>())).Callback(
                (IEnumerable<TaskBase> tasks) => 
                    registeredTasks.AddRange(tasks));

            taskFactory.Setup(f => f.Assemble(It.IsAny<Type>())).Returns(
                (Type taskType) =>
                    Activator.CreateInstance(taskType) as TaskBase
                );

            catalogMock.Setup(c => c.GetCatalog()).Returns(typesToCreate);
        }

        protected Context an_instance_is_created = () =>
        {
            SetupMocks();
            instance = new TaskLoader(schedulerMock.Object,catalogMock.Object, logMock.Object, taskFactory.Object);
        };

    }


    public class DummyTask : TaskBase
    {
        public override void Execute()
        {throw new NotImplementedException();}
    }
    public class YADummyTask : TaskBase
    {
        public override void Execute()
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
        public void assure_registers_correct_number_of_task_instances()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(an_instance_is_created);
                scenario.When("when the tasks have been instanced");
                scenario.Then("all specified tasks were instanced", () =>
                {
                    registeredTasks.Count().ShouldBe(typesToCreate.Count);
                    for (int i = 0; i < registeredTasks.Count(); i++)
                    {
                        //Console.WriteLine("found a " + registeredTasks[i].GetType());
                        (registeredTasks[i].GetType() == typesToCreate[i]).ShouldBeTrue();
                    }
                });
            });
        }
    }
}
