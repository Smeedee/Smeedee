using System;
using System.Windows.Media.Imaging;
using NUnit.Framework;
using Smeedee.Widgets.WebSnapshot.ViewModel;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Widgets.Tests.WebSnapshot.ViewModel
{
    public class Shared : ScenarioClass
    {
        protected static WebSnapshotViewModel viewmodel;
        protected Context viewmodel_is_created = () => viewmodel = new WebSnapshotViewModel();

        protected When picture_is_loaded = () =>
                                               {
                                                   viewmodel.Snapshot = new WriteableBitmap(new BitmapImage(new Uri("http://my.pic/somePicture.jpg")));
                                               };

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
    public class When_spawned : Shared
    {
        [Test]
        public void assure_viewmodel_is_instansiated()
        {
            Given(viewmodel_is_created);
            When("");
            Then("viewmodel is not null", () => viewmodel.ShouldNotBeNull());
        }

        [Test]
        public void assure_picture_is_loaded()
        {
            Given(viewmodel_is_created);
            When(picture_is_loaded);
            Then("picture should be gg", () => viewmodel.Snapshot.ShouldNotBeNull());
        }

    }
}
