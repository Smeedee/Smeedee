using System;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;

namespace Smeedee.Tests
{
    public class SmeedeeScenarioTestClass : ScenarioClass
    {
        [SetUp]
        public void SetUp()
        {
            Scenario("");
            Before();
        }

        public virtual void Before()
        {
            
        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
            After();
        }

        public virtual void After()
        {
            
        }
    }
}