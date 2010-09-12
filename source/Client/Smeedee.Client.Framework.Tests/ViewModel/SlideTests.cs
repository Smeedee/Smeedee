using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using NUnit.Framework;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Tests.ViewModel;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Client.Framework.Tests.ViewModel
{
    public class SlideTests
    {
        [TestFixture]
        public class when_spawned : SlideTestContext
        {
            public override void Context()
            {
                Given_Slide_is_created();
            }

            [Test]
            public void Assure_SecondsOnScreen_is_initialized()
            {
                viewModel.SecondsOnScreen.ShouldBe(Slide.DEFAULT_SECONDSONSCREEN);
            }
        }



        [TestFixture]
        public class When_Widget_is_set : SlideTestContext
        {
            public override void Context()
            {
                Given_Slide_is_created();
                And_Widget_is_set(new Widget() { View = new Grid() });
            }

            [Test]
            public void Then_assure_Thumbnail_can_be_generated()
            {
                viewModel.Thumbnail.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_Widget_is_set_but_has_no_View : SlideTestContext
        {
            public override void Context()
            {
                Given_Slide_is_created();
                And_Widget_is_set(new Widget() { View = null });
            }

            [Test]
            public void Then_assure_Thumbnail_can_not_be_generated()
            {
                viewModel.Thumbnail.ShouldBeNull();
            }
        }

        [TestFixture]
        public class When_Widget_is_not_set : SlideTestContext
        {
            public override void Context()
            {
                Given_Slide_is_created();
                And_Widget_is_not_set();
            }

            private void And_Widget_is_not_set()
            {
                viewModel.Widget = null;
            }

            [Test]
            public void Then_assure_Thumbnail_can_not_be_generated()
            {
                viewModel.Thumbnail.ShouldBeNull();
            }
        }
    }
}
