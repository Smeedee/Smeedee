using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APD.Client.Framework.ViewModels;
using APD.Tests;

using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace APD.Client.Framework.ViewModels.AbstractViewModelSpecs
{
    public class TestViewModel : AbstractViewModel
    {
        // This class is for testing the abstract viewmodel.
        public TestViewModel(IInvokeUI uiInvoker)
            : base(uiInvoker)
        {
        }
    }

    [TestFixture]
    public class When_properties_change
    {
        AbstractViewModel viewModel = new TestViewModel(new NoUIInvocation());

        [Test]
        public void propertyhanged_event_should_be_invoked_by_given_invokeui()
        {
            Scenario.New(this, scenario =>
            {
                UiInvokerMock UiInvoker = new UiInvokerMock();
                AbstractViewModel viewModel = new TestViewModel(UiInvoker);

                scenario.Given("The object is created with a IInvokeUI implementation", () =>
                    viewModel = new TestViewModel(UiInvoker)
                ).And("someone has subscribed to the propertychanged event", () =>
                    viewModel.PropertyChanged += (o,e) => { }
                );
                 
                scenario.When("a value for a property is set", () =>
                    viewModel.IsLoading = true);

                System.Threading.Thread.Sleep(500);

                scenario.Then("the observers should be notified about the change via the given invoker", () =>
                    UiInvoker.WasInvoked.ShouldBeTrue());
            }).Execute();
        }

        [Test]
        public void propertychanged_event_should_be_invoked_for_hasConnectionProblem()
        {
            PropertyTester.TestChange<AbstractViewModel>(viewModel, vm=>vm.HasConnectionProblems);
            Assert.IsTrue(PropertyTester.WasNotified);
        }
    }
}
