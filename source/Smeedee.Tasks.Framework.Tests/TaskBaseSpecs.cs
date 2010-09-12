using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.Tests;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Tasks.Framework.Tests
{
    [TestFixture]
    public class TaskBaseSpecs : SmeedeeScenarioTestClass
    {
        private TaskBaseTestImplementation task;

        [Test]
        public void default_timespan_should_be_1_minute()
        {
            Given("We create an instance that does not override taskbase interval implementation", () =>
                task = new TaskBaseTestImplementation());

            When("We the property is left unchanged");

            Then("It should return 1 minute as interval", () =>
                task.Interval.ShouldBe(new TimeSpan(0, 10, 0)));
        }
    }

    internal class TaskBaseTestImplementation : TaskBase
    {
        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
