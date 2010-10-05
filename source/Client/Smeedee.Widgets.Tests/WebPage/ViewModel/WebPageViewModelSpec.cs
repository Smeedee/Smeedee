using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.Widgets.WebPage.ViewModel;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Widgets.Tests.WebPage.ViewModel
{
    public class WebPageViewModelSpec
    {
        [TestFixture]
        public class When_created : Shared
        {
            [Test]
            public void Then_there_should_be_a_URL()
            {
                webPageViewModel.Url.ShouldBe("");
            }

            [Test]
            public void Then_assure_it_has_a_GoTo_Command()
            {
                webPageViewModel.GoTo.ShouldNotBeNull();
            }

            [Test]
            public void Then_assure_default_refresh_interval_is_set_to_30_seconds()
            {
                webPageViewModel.RefreshInterval.ShouldBe(30);
            }
        }

        [TestFixture]
        public class When_Url_has_not_been_specified : Shared
        {
            [Test]
            public void Then_assure_GoTo_command_is_disabled()
            {
                webPageViewModel.Url = "";
                webPageViewModel.GoTo.CanExecute().ShouldBe(false);
            }
        }

        [TestFixture]
        public class When_Url_is_specified : Shared
        {
            [Test]
            public void Then_assure_GoTo_command_is_specified()
            {
                webPageViewModel.Url = "http://www.smeedee.org";
                webPageViewModel.GoTo.CanExecute().ShouldBeTrue();
            }
        }

        [TestFixture]
        public class When_Url_is_specified_incorrectly : Shared
        {
            [Test]
            public void Then_assure_GoTo_command_is_disabled()
            {
                var truth_table = new List<string>
                {
                    "this % is N0T a valid 33 ## URL",
                    "htp://doesntExist",
                    "http",
                    "http://smeedee.#",
                    "http:///smeedee.org",
                    "http://smee_dee.org",
                    //"http://www.smeedee#org",
                };

                foreach (var url in truth_table)
                {
                    webPageViewModel.Url = url;
                    Assert.IsFalse(webPageViewModel.GoTo.CanExecute(), url);
                }
            }
        }

        [TestFixture]
        public class When_Url_is_specified_correctly : Shared
        {
            [Test]
            public void Then_assure_GoTo_command_is_enabled()
            {
                var truth_table = new List<string>
                {
                    "http://subdomain.smeedee.org",
                    "http://subdomain.smeedee.org/",
                    "http://subdomain.smeedee.com",
                    "http://smeedee.co.uk",
                    "http://www.smeedee.co.uk",
                    "http://www.smeedee.info",
                    "http://www.to.smeedee.co.uk",
                    "http://www.smeedee.org/default.aspx",
                    "http://www.smeedee.org/default.aspx.js",
                    "http://www.smeedee.org/default.htm",
                    "http://www.smeedee.org/downloads",
                    "http://www.smeedee.org/downloads/top.aspx",
                };

                foreach (var url in truth_table)
                {
                    webPageViewModel.Url = url;
                    Assert.IsTrue(webPageViewModel.GoTo.CanExecute(), url);
                }
            }
        }

        public class Shared
        {
            protected WebPageViewModel webPageViewModel;

            [SetUp]
            public void Setup()
            {
                webPageViewModel = new WebPageViewModel();
            }
        }
    }
}
