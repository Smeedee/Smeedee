using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.Widget.Admin.ViewModels.Configuration;
using APD.DomainModel.Framework.Services;

using Moq;

using TinyBDD.Dsl.GivenWhenThen;
using APD.Client.Widget.Admin.Controllers;
using NUnit.Framework;

using TinyBDD.Specification.NUnit;


namespace APD.Client.Widget.AdminTests.Controllers.URLCheckerControllerSpecs
{
    public class SharedForUrlChecker : SharedForUrlAndAuthChecker
    {
        protected static URLCheckerController controller;

        protected Context Controller_is_created = () =>
        {
            controller = new URLCheckerController(viewModel, 
                urlCheckerMock.Object,
                backgroundWorkerMock.Object);
        };
    }

    [TestFixture]
    public class When_spawning : SharedForUrlChecker
    {
        [SetUp]
        public void Setup()
        {
            Scenario("Spawning controller");
            Given("Controller is not created");
        }

        [Test]
        public void Assure_ViewModel_argument_is_validated()
        {
            Given("ViewModel is NullObj");
            When("spawning");
            Then("assure ArgumentNullException is thrown", () =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new URLCheckerController(null, null, null), ex => { }));
        }

        [Test]
        public void Assure_URLChecker_is_validated()
        {
            Given(ViewModel_is_created).
                And("UrlChecker is NullObj");
            When("spawning");
            Then("assure ArgumentNullException is thrown", () =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new URLCheckerController(viewModel, null, null), ex => { }));
        }

        [Test]
        public void Assure_BackgroundWorker_is_validated()
        {
            Given(ViewModel_is_created).
                And(URLChecker_is_created).
                And("BackgroundWorker is NullObj");
            When("spawning");
            Then("assure ArgumentNullException is thrown", () =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new URLCheckerController(viewModel, urlCheckerMock.Object, null), ex => { }));
        }
    }

    [TestFixture]
    public class When_validating_URL : SharedForUrlChecker
    {
        [SetUp]
        public void Setup()
        {
            Scenario("validating VCS configuration");
            Given(Dependencies_are_created()).
                And(Controller_is_created);

            When("URL is entered in the ViewModel", () =>
                viewModel.URL = "http://valid.com");

        }

        [Test]
        public void Assure_URL_is_checked_against_URLChecker()
        {
            Then("assure URL is checked against the URLChecker", () =>
                urlCheckerMock.Verify(s => s.Check(It.IsAny<string>()), Times.Once()));
        }

        [Test]
        public void Assure_work_is_done_in_background()
        {
            Then("assure work is done in background", () =>
                backgroundWorkerMock.Verify(w => w.RunAsyncVoid(It.IsAny<Action>()), Times.Once()));
        }

        [Test]
        public void Assure_Status_is_set_on_ViewModel_when_validation_is_starting()
        {
            Then("assure Status on ViewModel is set to 'Checking URL...' when URL checking is starting", () =>
                vmRecorder.Changes["Status"].First().ShouldBe("Checking URL..."));
        }
    }

    [TestFixture]
    public class When_URL_is_invalid : SharedForUrlChecker
    {
        [SetUp]
        public void Setup()
        {
            Scenario("Invalid URL is entered in ViewModel");
            Given(Dependencies_are_created()).
                And(Controller_is_created);

            When(invalid_URL_is_entered);
        }

        [Test]
        public void Assure_Status_is_set_on_ViewModel()
        {
            Then("assure Status is set on ViewModel", () =>
                viewModel.Status.ShouldBe("Invalid URL"));
        }
    }

    [TestFixture]
    public class When_URL_is_changed_from_invalid_to_valid : SharedForUrlChecker
    {
        [SetUp]
        public void Setup()
        {
            Scenario("URL is changed from invalid to valid in ViewModel");
            Given(Dependencies_are_created()).
                And(Controller_is_created).
                And(invalid_URL_is_entered_in_ViewModel);

            When(valid_URL_is_entered);
        }

        [Test]
        public void Assure_Status_is_set_on_ViewModel()
        {
            Then("assure Status is set on ViewModel", () =>
                viewModel.Status.ShouldBe("URL is ok"));
        }
    }
}
