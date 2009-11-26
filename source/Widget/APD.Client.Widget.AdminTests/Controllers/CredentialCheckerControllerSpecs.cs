using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyBDD.Dsl.GivenWhenThen;
using NUnit.Framework;
using APD.Client.Widget.Admin.Controllers;
using TinyBDD.Specification.NUnit;
using APD.Client.Widget.Admin.ViewModels.Configuration;
using APD.Client.Framework.Factories;
using APD.DomainModel.Config;
using Moq;
using APD.DomainModel.Framework.Services;

namespace APD.Client.Widget.AdminTests.Controllers.AuthCheckerControllerSpecs
{
    public class SharedForAuthChecker : SharedForUrlAndAuthChecker
    {
        protected static CredentialsCheckerController controller;

        protected Context Controller_is_created = () =>
        {
            controller = new CredentialsCheckerController(viewModel, 
                authCheckerMock.Object,
                backgroundWorkerMock.Object);
        };
    }

    [TestFixture]
    public class When_spawning : SharedForAuthChecker
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
                    new CredentialsCheckerController(null, null, null), ex => { }));
        }

        [Test]
        public void Assure_AuthChecker_is_validated()
        {
            Given(ViewModel_is_created).
                And("AuthChecker is NullObj");
            When("spawning");
            Then("assure ArgumentNullException is thrown", () =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new CredentialsCheckerController(viewModel, null, null), ex => { }));
        }

        [Test]
        public void Assure_BackgroundWorker_is_validated()
        {
            Given(ViewModel_is_created).
                And(AuthChecker_is_created).
                And("BackgroundWorker is NullObj");
            When("spawning");
            Then("assure ArgumentNullException is thrown", () =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new CredentialsCheckerController(viewModel, authCheckerMock.Object, null), ex => { }));
        }
    }

    [TestFixture]
    public class When_validating_Credentials : SharedForAuthChecker
    {
        [SetUp]
        public void Setup()
        {
            Scenario("validating Credentials");
            Given(Dependencies_are_created()).
                And(Controller_is_created).
                And("svn is set as SelectedProvider in ViewModel", () =>
                    viewModel.SelectedProvider = "svn").
                And(URL_is_entered_in_ViewModel);

            When(Username_is_entered);

        }

        [Test]
        public void Assure_work_is_done_in_background()
        {
            Then("assure work is done in background", () =>
                backgroundWorkerMock.Verify(w => w.RunAsyncVoid(It.IsAny<Action>()), Times.Once()));
        }

        [Test]
        public void Assure_Status_is_set_on_ViewModel_when_Authentication_starts()
        {
            Then("assure Status on ViewModel is set to 'Authenticating...' when Authentication starts", () =>
                vmRecorder.Changes["Status"].First().ShouldBe("Authenticating..."));
        }

        [Test]
        public void Assure_correct_ViewModel_data_is_sent_to_AuthChecker_service()
        {
            Then("assure correct ViewModel data is sent to AuthChecker service", () =>
            {
                authCheckerMock.Verify(s => s.Check(viewModel.SelectedProvider,
                    viewModel.URL,
                    viewModel.Username,
                    viewModel.Password), Times.Once());
            });
        }

        [Test]
        [Ignore]
        public void Assure_Credentials_are_not_validated_without_Password()
        {
            Scenario("validating Credentials");
            Given(Dependencies_are_created()).
                And(Controller_is_created).
                And(URL_is_entered_in_ViewModel);

            When("Username is entered in ViewModel", () =>
                viewModel.Username = "goeran");

            Then("assure Credentials are not validated when Password is missing", () =>
                VerifyAuthCheckerIsCalled(ZERO_TIMES));
        }

        [Test]
        public void Assure_Credentials_are_not_validated_without_URL()
        {
            Scenario("validating Credentials");
            Given(Dependencies_are_created()).
                And(Controller_is_created).
                And(Username_is_entered_in_ViewModel);

            When(Password_is_entered);
            Then("assure Credentials are not validated when URL is missing", () =>
                VerifyAuthCheckerIsCalled(ZERO_TIMES));
        }

        [Test]
        public void Assure_Credentials_are_not_validated_without_Username()
        {
            Scenario("validating Credentials");
            Given(Dependencies_are_created()).
                And(Controller_is_created).
                And(URL_is_entered_in_ViewModel);

            When(Password_is_entered);
            Then("assure Credentials are not validated when Username is missing", () =>
                VerifyAuthCheckerIsCalled(ZERO_TIMES));
        }

        [Test]
        public void Assure_Credentials_are_validated_when_Password_Username_And_URL_is_entered()
        {
            Scenario("validating Credentials");
            Given(Dependencies_are_created()).
                And(Controller_is_created).
                And(URL_is_entered_in_ViewModel).
                And(Username_is_entered_in_ViewModel);

            When("Password is entered in the ViewModel", () =>
                viewModel.Password = "hansen");

            Then("assure credentials are validated", () =>
                VerifyAuthCheckerIsCalled(TWICE));
        }

    }

    [TestFixture]
    public class When_Credentials_are_invalid : SharedForAuthChecker
    {
        [SetUp]
        public void Setup()
        {
            Scenario("Invalid Credentials is entered in ViewModel");
            Given(Dependencies_are_created()).
                And(Controller_is_created).
                And(URL_is_entered_in_ViewModel);

            When(invalid_Credentials_are_entered);
        }

        [Test]
        public void Assure_Status_is_set_on_ViewModel()
        {
            Then("assure Status is set on ViewModel", () =>
                viewModel.Status.ShouldBe("Authentication failed"));
        }
    }

    [TestFixture]
    public class When_Credentials_are_changed_from_invalid_to_valid : SharedForAuthChecker
    {
        [SetUp]
        public void Setup()
        {
            Scenario("Credentials are changed from invalid to valid");
            Given(Dependencies_are_created()).
                And(Controller_is_created).
                And(URL_is_entered_in_ViewModel).
                And(invalid_Credentials_are_entered_in_ViewModel);

            When(valid_Credentials_are_entered_in_ViewModel);
        }

        [Test]
        public void Assure_Status_is_set_on_ViewModel()
        {
            Then("assure Status is set on ViewModel", () =>
                viewModel.Status.ShouldBe("Authentication successful"));
        }
    }
}
