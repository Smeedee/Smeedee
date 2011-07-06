using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.Widgets.WebSnapshot.ViewModel;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Widgets.Tests.WebSnapshot.ViewModel
{
    public class WebSnapshotViewModelSpec
    {

        [TestFixture]
        public class When_created : Shared
        {
            [Test]
            public void Then_assure_input_URL_has_some_default_text()
            {
                webSnapshotViewModel.InputUrl.ShouldBe(INPUT_MESSAGE);
            }

            [Test]
            public void Then_there_should_be_a_validated_URL()
            {
                webSnapshotViewModel.ValidatedUrl.ShouldBe("");
            }

            [Test]
            public void Then_assure_it_has_a_save_command()
            {
                webSnapshotViewModel.Save.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_URL_is_not_specified : Shared
        {
            [Test]
            public void Then_assure_save_command_is_disabled()
            {
                webSnapshotViewModel.InputUrl = "";
                webSnapshotViewModel.Save.CanExecute().ShouldBe(false);
            }

            [Test]
            public void Then_assure_it_has_error_message()
            {
                webSnapshotViewModel.InputUrl = "";
                webSnapshotViewModel.ErrorMessage.ShouldBeSameAs(ERROR_MESSAGE);
            }
        }

        [TestFixture]
        public class When_URL_is_specified : Shared
        {
            [Test]
            public void Then_assure_save_command_is_enabled()
            {
                var truth_table = new List<string>
                {
                    "http://smeedee.org",
                    "http://smeedee.org/images/code.png"
                };

                foreach (string url in truth_table)
                {
                    webSnapshotViewModel.InputUrl = url;
                    webSnapshotViewModel.Save.CanExecute().ShouldBeTrue();                                    
                }
            }
        }

        [TestFixture]
        public class When_URL_is_specified_incorrectly : Shared
        {
            [Test]
            public void Then_assure_save_command_is_disabled()
            {
                var thruth_table = new List<string>
                                       {
                                           "http",
                                           "http:///smeedee.org",
                                           "this is n0t a valid url!123"
                                       };
                foreach (string url in thruth_table)
                {
                    webSnapshotViewModel.InputUrl = url;
                    webSnapshotViewModel.Save.CanExecute().ShouldBeFalse();
                }
            }

            [Test]
            public void Then_assure_error_message_is_set()
            {
                webSnapshotViewModel.InputUrl = "invalid UR1!";
                webSnapshotViewModel.ErrorMessage.ShouldBe(ERROR_MESSAGE);
            }

            [Test]
            public void Then_assure_the_validated_URL_is_not_set()
            {
                webSnapshotViewModel.InputUrl = "http://smeedee.org/";
                webSnapshotViewModel.ValidatedUrl.ShouldBe("http://smeedee.org/");

                webSnapshotViewModel.InputUrl = "does not work";
                webSnapshotViewModel.ValidatedUrl.ShouldBe("");
            }
        }

        [TestFixture]
        public class When_URL_is_specified_correctly : Shared
        {
            [Test]
            public void Then_assure_save_command_is_enabled()
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
                                          "http://www.smeedee.org/images/code.png"
                                      };
                foreach (string url in truth_table)
                {
                    webSnapshotViewModel.InputUrl = url;
                    webSnapshotViewModel.Save.CanExecute().ShouldBeTrue();
                }
            }
       
            [Test]
            public void Then_assure_ErrorMessage_is_removed_after_URL_is_fixed()
            {
                webSnapshotViewModel.InputUrl = "invalid url";
                webSnapshotViewModel.ErrorMessage.ShouldBe(ERROR_MESSAGE);
                webSnapshotViewModel.InputUrl = "http://www.smeedee.org/";
                webSnapshotViewModel.ErrorMessage.ShouldBe("");
            }

            [Test]
            public void Then_assure_validated_URL_is_set()
            {
                webSnapshotViewModel.InputUrl = "http://www.smeedee.org/";
                webSnapshotViewModel.ValidatedUrl.ShouldBe("http://www.smeedee.org/");
            }
        
        }

        [TestFixture]
        public class When_URL_is_link_to_picture : Shared
        {
            [Test]
            public void Then_assure_URL_is_identified_as_picture_after_having_been_set_as_webpage()
            {
                webSnapshotViewModel.InputUrl = "http://smeedee.org/";
                webSnapshotViewModel.IsPictureUrl().ShouldBeFalse();
                webSnapshotViewModel.IsPictureUrl().ShouldNotBeNull();
                webSnapshotViewModel.InputUrl = "http://smeedee.org/images/code.png";
                webSnapshotViewModel.IsPictureUrl().ShouldBeTrue();
            }

            [Test]
            public void Then_assure_file_types_are_valid()
            {
                var thruth_table = new[]
                                       {
                                           "http://smeedee.org/images/code.png",
                                           "http://smeedee.org/images/code.gif",
                                           "http://smeedee.org/images/code.jpg",
                                           "http://smeedee.org/images/code.jpeg",
                                           "http://10.0.0.1/images/code.bmp",
                                           "http://10.0.0.1/images/code.tiff"
                                       };
                foreach (string url in thruth_table)
                {
                    webSnapshotViewModel.InputUrl = url;
                    webSnapshotViewModel.IsPictureUrl().ShouldBeTrue();
                }
            }

            [Test]
            public void Then_assure_it_should_be_fetched_as_image()
            {
                webSnapshotViewModel.InputUrl = "http://smeedee.org/images/code.png";
                webSnapshotViewModel.FetchImage.CanExecute().ShouldBeTrue();
            }
        }



        public class Shared
        {
            protected WebSnapshotViewModel webSnapshotViewModel;
            protected const string ERROR_MESSAGE = "Invalid URL!";
            protected const string INPUT_MESSAGE = "Enter URL here";

            [SetUp]
            public void Setup()
            {
                webSnapshotViewModel = new WebSnapshotViewModel();
            }

        }
    }
}
