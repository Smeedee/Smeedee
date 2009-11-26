using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.Framework.Factories;
using APD.Client.Widget.Admin.Controllers;
using APD.Client.Widget.Admin.ViewModels.Configuration;
using APD.DomainModel.Config;
using APD.DomainModel.Framework.Services;

using Moq;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using APD.Client.Framework;
using APD.Tests;


namespace APD.Client.Widget.AdminTests.Controllers
{
    public class SharedForUrlAndAuthChecker : ScenarioClass
    {
        protected const int ZERO_TIMES = 0;
        protected const int ONCE = 1;
        protected const int TWICE = 2;
        protected const bool VALID = true;
        protected const bool INVALID = false;

        protected static ProviderConfigItemViewModel viewModel;
        protected static Mock<ICheckIfResourceExists> urlCheckerMock;
        protected static Mock<ICheckIfCredentialsIsValid> authCheckerMock;
        protected static Mock<IInvokeBackgroundWorker<bool>> backgroundWorkerMock;
        protected static PropertyChangedRecorder vmRecorder;

        protected Context ViewModel_is_created = () =>
        {
            viewModel = new ProviderConfigItemViewModel(UIInvokerFactory.Assemlble(), Configuration.DefaultVCSConfiguration());
            vmRecorder = new PropertyChangedRecorder(viewModel);
        };

        protected Context BackgroundWorker_is_created = () =>
        {
            backgroundWorkerMock = new Mock<IInvokeBackgroundWorker<bool>>();

            backgroundWorkerMock.Setup(w => w.RunAsyncVoid(It.IsAny<Action>())).Callback<Action>(a =>
                a.Invoke());
        };

        protected Context URLChecker_is_created = () =>
        {
            urlCheckerMock = new Mock<ICheckIfResourceExists>();
        };

        protected Context AuthChecker_is_created = () =>
        {
            authCheckerMock = new Mock<ICheckIfCredentialsIsValid>();
        };

        protected GivenSemantics Dependencies_are_created()
        {
            return Given(ViewModel_is_created).
                And(URLChecker_is_created).
                And(AuthChecker_is_created).
                And(BackgroundWorker_is_created);
        }

        protected Context URL_is_entered_in_ViewModel = () =>
        {
            viewModel.URL = "http://valid.com";
        };

        protected Context Username_is_entered_in_ViewModel = () =>
        {
            viewModel.Username = "goeran";
        };

        protected static Context Password_is_entered_in_ViewModel = () =>
        {
            viewModel.Password = "hansen";
        };

        protected Context invalid_Credentials_are_entered_in_ViewModel = () =>
        {
            CredentialsEnteredAre(INVALID);
        };

        protected Context invalid_URL_is_entered_in_ViewModel = () =>
        {
            InvalidUrlIsEntered();
        };

        protected When valid_Credentials_are_entered_in_ViewModel = () =>
        {
            CredentialsEnteredAre(VALID);
        };

        protected When Password_is_entered = () =>
        {
            PasswordIsEntered();
        };

        private static void PasswordIsEntered()
        {
            viewModel.Password = "hansen";
        }

        protected When Username_is_entered = () =>
        {
            UsernameIsEntered();
        };

        private static void UsernameIsEntered()
        {
            viewModel.Username = "goeran";
        }

        protected When invalid_URL_is_entered = () =>
        {
            InvalidUrlIsEntered();
        };

        private static void InvalidUrlIsEntered()
        {
            urlCheckerMock.Setup(s => s.Check(It.IsAny<string>())).Returns(false);
            viewModel.URL = "http://invalid.com";
        }

        protected When valid_URL_is_entered = () =>
        {
            ValidUrlIsEntered();
        };

        private static void ValidUrlIsEntered()
        {
            urlCheckerMock.Setup(s => s.Check(It.IsAny<string>())).Returns(true);
            viewModel.URL = "http://goeran.no";
        }

        protected When invalid_Credentials_are_entered = () =>
        {
            CredentialsEnteredAre(INVALID);
        };

        private static void CredentialsEnteredAre(bool valid)
        {
            authCheckerMock.Setup(s => s.Check(It.IsAny<string>(),
                                   It.IsAny<string>(),
                                   It.IsAny<string>(),
                                   It.IsAny<string>())).Returns(valid);
            UsernameIsEntered();
            PasswordIsEntered();
        }

        protected void VerifyAuthCheckerIsCalled(int times)
        {
            authCheckerMock.Verify(s => s.Check(It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                    Times.Exactly(times));
        }

        [TearDown]
        public void Teardown()
        {
            StartScenario();
        }
    }

}
