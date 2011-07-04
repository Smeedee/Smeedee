using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Smeedee.Widgets.Tests.WebSnapshot.ViewModel
{
    public class WebSnapshotViewModelSpec
    {

        [TestFixture]
        public class When_created : Shared
        {
            [Test]
            public void Then_assure_input_URL_has_some_default_text()
            {
                webSnapshotViewModel.InputUrl.ShouldBe("Enter URL here");
            }
        }

        public class Shared
        {
            protected WebSnapshotViewModel webSnapshotViewModel;

            [SetUp]
            public void Setup()
            {
                webSnapshotViewModel = new WebSnapshotViewModel();
            }

        }
    }
}
