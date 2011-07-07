using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Widgets.WebSnapshot.Controllers;
using Smeedee.Widgets.WebSnapshot.ViewModel;

namespace Smeedee.Widgets.Tests.WebSnapshot.Controllers
{
    public class WebSnapshotControllerSpec
    {

        public class Shared
        {
            protected Mock<ITimer> timerFake;
            protected WebSnapshotViewModel webSnapshotViewModel;
            protected WebSnapshotController webSnapshotController;
            
            [SetUp]
            public void Setup()
            {
                timerFake = new Mock<ITimer>();
                webSnapshotViewModel = new WebSnapshotViewModel();
                webSnapshotController = new WebSnapshotController(webSnapshotViewModel, WebSnapshotController.GetDefaultConfiguration(), timerFake.Object);
            }

        }

    }
}
