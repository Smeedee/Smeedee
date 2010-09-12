using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.DomainModel.Config.SlideConfig;
using TinyBDD.Specification.NUnit;

namespace Smeedee.DomainModel.Tests.Config.SlideConfig.SlideConfigurationSpecs
{
    [TestFixture]
    public class when_spawned
    {
        private SlideConfiguration _slideConfig;

        [SetUp]
        private void Setup()
        {
            _slideConfig = new SlideConfiguration();
        }

        [Test]
        public void Assure_slide_title_is_initialized_to_default()
        {
            _slideConfig.Title.ShouldBe(SlideConfiguration.DEFAULT_SLIDE_TITLE);
        }

        [Test]
        public void Assure_duration_is_initialized_to_default()
        {
            _slideConfig.Duration = SlideConfiguration.DEFAULT_DURATION;
        }
    }
}
