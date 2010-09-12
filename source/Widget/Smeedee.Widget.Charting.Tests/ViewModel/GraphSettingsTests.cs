using System;
using NUnit.Framework;
using Moq;
using Tskjortebutikken.Widgets.Tests.ViewModel.TestScenarios;
using Tskjortebutikken.Widgets.ViewModel;
using System;
using System.Collections.ObjectModel;
using TinyBDD.Specification.NUnit;

namespace Tskjortebutikken.Widgets.Tests.ViewModel
{
	public class GraphSettingsTests
	{
	    [TestFixture]
	    public class When_spawned : GraphSettingsTestScenario<When_spawned>
	    {
            protected override void Before()
            {
                When.GraphSettings_is_spawned();
            }

	        [Test]
	        public void Then_assure_it_has_Databases()
	        {
	            viewModel.Databases.ShouldNotBeNull();
	        }
	    }
	}
}

