using System;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.ViewModel;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Testing.Services;
using TinyMVVM.IoC;

namespace Smeedee.Client.Framework.Tests
{
    class TestViewmodel : AbstractViewModel {}

    class TestController : ControllerBase<AbstractViewModel>
    {
        public bool WasNotified { get; set; }

        public TestController(TestViewmodel viewModel, ITimer timer, IUIInvoker uiInvoker, IProgressbar loadingNotifier) 
            : base(viewModel, timer, uiInvoker, loadingNotifier)
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
        private Mock<IProgressbar> _iProgressbarMock = new Mock<IProgressbar>();

        [SetUp]
        public void Setup()
        {
            ViewModelBootstrapperForTests.Initialize();

            _timermock = new Mock<ITimer>();
            _controller = new TestController(new TestViewmodel(), _timermock.Object, new NoUIInvokation(), _iProgressbarMock.Object);
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
            _controller.Start();
            _timermock.Verify(t=>t.Start(It.IsAny<int>()));
        }

        [Test]
        public void should_throw_argument_exception_when_timer_is_null()
        {
            bool expectedExceptionWasThrown = false;
            try
            {
                _controller = new TestController(new TestViewmodel(), null, new NoUIInvokation(), _iProgressbarMock.Object);  
            }
            catch (ArgumentException)
            {
                expectedExceptionWasThrown = true;
            }

            expectedExceptionWasThrown.ShouldBe(true);
        }

        [Test]
        public void should_throw_argument_exception_when_uiinvoker_is_null()
        {
            bool expectedExceptionWasThrown = false;
            try
            {
                _controller = new TestController(new TestViewmodel(), _timermock.Object, null, _iProgressbarMock.Object);
            }
            catch (ArgumentException)
            {
                expectedExceptionWasThrown = true;
            }

            expectedExceptionWasThrown.ShouldBe(true);
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
        
        [Test]
        public void should_stop_refreshing_when_in_settings_mode()
        {
            var slide = new Client.Framework.ViewModel.Widget();
            slide.IsInSettingsMode = false;
            slide.PropertyChanged += _controller.ToggleRefreshInSettingsMode;
            slide.IsInSettingsMode = true;

            _timermock.Verify(t => t.Start(It.IsAny<int>()), Times.Never());
            _timermock.Verify(t => t.Stop(), Times.Once());
        }


        [Test]
        public void should_be_refreshing_when_not_settings_mode()
        {
            var slide = new Client.Framework.ViewModel.Widget();
            slide.IsInSettingsMode = true;
            slide.PropertyChanged += _controller.ToggleRefreshInSettingsMode;
            slide.IsInSettingsMode = false;

            _timermock.Verify(t => t.Stop(), Times.Never());
            _timermock.Verify(t => t.Start(It.IsAny<int>()), Times.Once());
        }
    }
}
