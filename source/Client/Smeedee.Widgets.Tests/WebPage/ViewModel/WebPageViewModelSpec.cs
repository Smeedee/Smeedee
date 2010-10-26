using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            public void Then_assure_input_URL_has_some_default_text()
            {
                webPageViewModel.InputUrl.ShouldBe("Enter URL here");
            }

            [Test]
            public void Then_there_should_be_a_validated_URL()
            {
                webPageViewModel.ValidatedUrl.ShouldBe("");
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
        public class When_Url_is_not_specified : Shared
        {
            [Test]
            public void Then_assure_GoTo_command_is_disabled()
            {
                webPageViewModel.InputUrl = "";
                webPageViewModel.GoTo.CanExecute().ShouldBe(false);
            }
        }

        [TestFixture]
        public class When_Url_is_specified : Shared
        {
            [Test]
            public void Then_assure_GoTo_command_is_enabled()
            {
                webPageViewModel.InputUrl = "http://smeedee.org";
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
                    "http",
                    "http:///smeedee.org",
                };

                foreach (var url in truth_table)
                {
                    webPageViewModel.InputUrl = url;
                    Assert.IsFalse(webPageViewModel.GoTo.CanExecute(), url);
                }
            }

            [Test]
            public void Then_assure_error_message_is_set()
            {
                webPageViewModel.InputUrl = "an invalid URL % ||";
                webPageViewModel.ErrorMessage.ShouldBe("Invalid URL!");
            }

            [Test]
            public void Then_assure_the_validated_URL_is_not_set()
            {
                webPageViewModel.InputUrl = "http://www.smeedee.org/";
                webPageViewModel.ValidatedUrl.ShouldBe("http://www.smeedee.org/");

                webPageViewModel.InputUrl = "foo bar";
                webPageViewModel.ValidatedUrl.ShouldBe("");
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
                    "http://smeedee.org",
                    "http://www.smeedee.co.uk",
                    "http://www.smeedee.info",
                    "http://www.to.smeedee.co.uk",
                    "http://www.smeedee.org/default.aspx",
                    "http://www.smeedee.org/default.aspx.js",
                    "http://www.smeedee.org/default.htm",
                    "http://www.smeedee.org/downloads",
                    "http://www.smeedee.org/downloads/",
                    "http://www.smeedee.org/downloads/top.aspx",
                    "http://www.smeedee.org/downloads/top.aspx/",
                    "http://www.smeedee.org/downloads/top.aspx?size=100",
                    "http://buildserver/buildstatus",
                };

                foreach (var url in truth_table)
                {
                    webPageViewModel.InputUrl = url;
                    Assert.IsTrue(webPageViewModel.GoTo.CanExecute(), url);
                }
            }

            [Test]
            public void Then_assure_ErrorMessage_is_removed_after_url_is_fixed()
            {
                webPageViewModel.InputUrl = "invalid";
                webPageViewModel.ErrorMessage.ShouldNotBeNull();
                webPageViewModel.ErrorMessage.ShouldNotBe(string.Empty);

                webPageViewModel.InputUrl = "http://smeedee.org";
                webPageViewModel.ErrorMessage.ShouldBe(string.Empty);
            }

            [Test]
            public void Then_assure_Validated_Url_is_set()
            {
                webPageViewModel.InputUrl = "http://www.smeedee.org/";
                webPageViewModel.ValidatedUrl.ShouldBe("http://www.smeedee.org/");
            }
        }

    	[TestFixture]
    	public class When_refresh_after_an_interval : Shared
    	{
    		[Test]
    		public void Then_assure_ValidatedUrl_is_updated()
    		{
				webPageViewModel.InputUrl = "http://smeedee.org:8111";
    			webPageViewModel.PropertyChangeRecorder.Start();

				webPageViewModel.OnRefresh();

				webPageViewModel.PropertyChangeRecorder.Data.Count(r => r.PropertyName == "ValidatedUrl").ShouldBe(1);
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
