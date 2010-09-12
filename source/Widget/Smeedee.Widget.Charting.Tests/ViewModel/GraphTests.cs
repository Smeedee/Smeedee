using System;
using NUnit.Framework;
using Moq;
using TinyBDD.Specification.NUnit;
using Tskjortebutikken.Widgets.Tests.ViewModel.TestScenarios;
using Tskjortebutikken.Widgets.ViewModel;
using System;
using System.Collections.ObjectModel;

namespace Tskjortebutikken.Widgets.Tests.ViewModel
{
	public class GraphTests
	{
	    [TestFixture]
	    public class When_spawned : GraphTestScenario<When_spawned>
	    {
            protected override void Before()
            {
                When.Graph_is_spawned();
            }

	        [Test]
	        public void Then_assure_it_has_Data()
	        {
	            viewModel.Data.ShouldNotBeNull();
	        }
	    }
	}
}

