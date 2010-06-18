using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.ViewModel;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework.Services;

namespace Smeedee.Client.Framework.Tests
{
    class TestViewmodel : AbstractViewModel {}

    class TestController : ControllerBase<AbstractViewModel>
    {
        public bool WasNotified { get; set; }

        public TestController(TestViewmodel viewModel, ITimer timer, IUIInvoker uiInvoker) 
            : base(viewModel, timer, uiInvoker)
        {
            WasNotified = false;
        }

        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            WasNotified = true;
        }
    }

    [TestFixture]
    public class ControllerBaseSpecs
    {
        private TestController _controller;
        private Mock<ITimer> _timermock;

        [SetUp]
        public void Setup()
        {
            _timermock = new Mock<ITimer>();
            _controller = new TestController(new TestViewmodel(), _timermock.Object, new NoUIInvokation());
        }

        [Test]
        public void should_subcribe_to_timer_elapsed()
        {
            _timermock.Raise(t=>t.Elapsed += null, EventArgs.Empty);
            _controller.WasNotified.ShouldBeTrue();
        }

        [Test]
        public void should_start_timer_when_started()
        {
            _controller.Start(1000);
            _timermock.Verify(t=>t.Start(1000));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void should_throw_argument_exception_when_timer_is_null()
        {
            _controller = new TestController(new TestViewmodel(), null, new NoUIInvokation());     
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void should_throw_argument_exception_when_uiinvoker_is_null()
        {
            _controller = new TestController(new TestViewmodel(), _timermock.Object, null);
        }

        [Test]
        public void should_not_call_start_on_the_timer_when_spawned()
        {
            _timermock.Verify(t=>t.Start(It.IsAny<int>()), Times.Never());
        }

        [Test]
        public void Should_call_stop_on_the_timer_when_stopped()
        {
            _controller.Stop();
            _timermock.Verify(t=>t.Stop(),Times.Once());
        }
    }
}
