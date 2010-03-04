using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.Widget.Admin.ViewModels;

using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.Client.Framework;
using Moq;
using APD.Client.Framework.Controllers;
using APD.Client.Framework.ViewModels;


namespace APD.Client.Widget.AdminTests.ViewModels.AdminViewModelSpecs
{
    public class Shared
    {
        protected static AdminViewModel viewModel;
        protected static Mock<ITriggerCommand> saveUserdbNotifierMock;
        protected static Mock<ITriggerCommand> reloadUserdbNotifierMock;
        protected static Mock<ITriggerEvent<EventArgs>> editUserdbNotifierMock;
        protected static Mock<ITriggerCommand> saveConfigNotifierMock;
        protected static Mock<ITriggerCommand> refreshConfigNotifierMock;

        protected static Mock<ITriggerCommand> saveHolidaysNotifierMock;
        protected static Mock<ITriggerCommand> reloadHolidaysMock;

        protected static Context viewModel_is_created = () =>
        {
            saveUserdbNotifierMock = new Mock<ITriggerCommand>();
            reloadUserdbNotifierMock = new Mock<ITriggerCommand>();
            editUserdbNotifierMock = new Mock<ITriggerEvent<EventArgs>>();
            saveConfigNotifierMock = new Mock<ITriggerCommand>();
            refreshConfigNotifierMock = new Mock<ITriggerCommand>();
            saveHolidaysNotifierMock = new Mock<ITriggerCommand>();
            reloadHolidaysMock = new Mock<ITriggerCommand>();

            viewModel = new AdminViewModel(new NoUIInvocation(), 
                                                   saveUserdbNotifierMock.Object,
                                                   reloadUserdbNotifierMock.Object, 
                                                   editUserdbNotifierMock.Object,
                                                   saveConfigNotifierMock.Object,
                                                   refreshConfigNotifierMock.Object,
                                                   saveHolidaysNotifierMock.Object,
                                                   reloadHolidaysMock.Object);
        };
    }

    [TestFixture] 
    public class When_spawned : Shared
    {
        [Test]
        public void Assure_isa_AbstractViewModel()
        {
            viewModel_is_created();

            (viewModel is AbstractViewModel).ShouldBeTrue();
        }

        [Test]
        public void Assure_it_has_a_UserdbViewModel()
        {
            viewModel_is_created();

            viewModel.Userdb.ShouldNotBeNull();
        }

        [Test]
        public void Assure_it_has_configuration()
        {
            viewModel_is_created();

            viewModel.Configuration.ShouldNotBeNull();
        }
    }
}
