using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using TinyMVVM.Framework.Services;

namespace Smeedee.Client.Tests.ViewModel
{
    class ErrorInfoTests
    {
        [TestFixture]
        public class When_state_is_updated : ErrorInfoTestContext
        {
            private Mock<IUIInvoker> uiInvokerFake;

            public override void Context()
            {
                uiInvokerFake = GetFakeFor<IUIInvoker>();

                Given_ErrorInfo_is_created();

                When_HasError_is_set(true);
            }

            [Test]
            public void assure_the_state_update_is_performed_on_the_UIThread()
            {
                uiInvokerFake.Verify(s => s.Invoke(It.IsAny<Action>()), Times.Once());
            }
        }
    }
}
