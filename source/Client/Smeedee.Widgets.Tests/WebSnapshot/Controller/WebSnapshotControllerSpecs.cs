using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;

namespace Smeedee.Widgets.Tests.WebSnapshot.Controller
{
    public class Shared : ScenarioClass
    { 

        [SetUp]
        public void SetUp()
        {
            Scenario("");
        }
        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
        
    }
    [TestFixture]
    public class When_Spawned : Shared
    {

        [Test]
        public void assure_we_get_data_from_repo()
        {
            
        }
    }
}
