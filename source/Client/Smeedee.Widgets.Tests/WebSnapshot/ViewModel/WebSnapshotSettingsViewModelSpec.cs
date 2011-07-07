using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.Widgets.WebSnapshot.ViewModel;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Widgets.Tests.WebSnapshot.ViewModel
{
    public class WebSnapshotSettingsViewModelSpec
    {

        [TestFixture]
        public class When_created : Shared
        {
            [Test]
            public void Then_assure_input_URL_has_some_default_text()
            {
                webSnapshotSettingsViewModel.InputUrl.ShouldBe(INPUT_MESSAGE);
            }

            [Test]
            public void Then_there_should_be_a_validated_URL()
            {
                webSnapshotSettingsViewModel.ValidatedUrl.ShouldBe("");
            }

            [Test]
            public void Then_assure_it_has_a_save_command()
            {
                webSnapshotSettingsViewModel.Save.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_URL_is_not_specified : Shared
        {
            [Test]
            public void Then_assure_save_command_is_disabled()
            {
                webSnapshotSettingsViewModel.InputUrl = "";
                webSnapshotSettingsViewModel.Save.CanExecute().ShouldBe(false);
            }

            [Test]
            public void Then_assure_it_has_error_message()
            {
                webSnapshotSettingsViewModel.InputUrl = "";
                webSnapshotSettingsViewModel.ErrorMessage.ShouldBeSameAs(ERROR_MESSAGE);
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
                    webSnapshotSettingsViewModel.InputUrl = url;
                    webSnapshotSettingsViewModel.Save.CanExecute().ShouldBeTrue();                                    
                }
            }

            [Test]
            public void Then_assure_it_can_be_fetched_as_snapshot()
            {
                webSnapshotSettingsViewModel.InputUrl = "http://smeedee.org/";
                webSnapshotSettingsViewModel.FetchMethod.ShouldBeSameAs(
                    webSnapshotSettingsViewModel.FetchAsSnapshot);
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
                    webSnapshotSettingsViewModel.InputUrl = url;
                    webSnapshotSettingsViewModel.Save.CanExecute().ShouldBeFalse();
                }
            }

            [Test]
            public void Then_assure_error_message_is_set()
            {
                webSnapshotSettingsViewModel.InputUrl = "invalid UR1!";
                webSnapshotSettingsViewModel.ErrorMessage.ShouldBe(ERROR_MESSAGE);
            }

            [Test]
            public void Then_assure_the_validated_URL_is_not_set()
            {
                webSnapshotSettingsViewModel.InputUrl = "http://smeedee.org/";
                webSnapshotSettingsViewModel.ValidatedUrl.ShouldBe("http://smeedee.org/");

                webSnapshotSettingsViewModel.InputUrl = "does not work";
                webSnapshotSettingsViewModel.ValidatedUrl.ShouldBe("");
            }

            [Test]
            public void Then_assure_it_cannot_be_fetched()
            {
                webSnapshotSettingsViewModel.InputUrl = "http://10.0.0.1/img/bilde.png";
                webSnapshotSettingsViewModel.FetchMethod.CanExecute().ShouldBeTrue();

                webSnapshotSettingsViewModel.InputUrl = "captain kirk is climbing a mountain";
                webSnapshotSettingsViewModel.FetchAsImage.CanExecute().ShouldBeFalse();
                webSnapshotSettingsViewModel.FetchAsSnapshot.CanExecute().ShouldBeFalse();
                webSnapshotSettingsViewModel.FetchMethod.CanExecute().ShouldBeFalse();
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
                    webSnapshotSettingsViewModel.InputUrl = url;
                    webSnapshotSettingsViewModel.Save.CanExecute().ShouldBeTrue();
                }
            }
       
            [Test]
            public void Then_assure_ErrorMessage_is_removed_after_URL_is_fixed()
            {
                webSnapshotSettingsViewModel.InputUrl = "invalid url";
                webSnapshotSettingsViewModel.ErrorMessage.ShouldBe(ERROR_MESSAGE);
                webSnapshotSettingsViewModel.InputUrl = "http://www.smeedee.org/";
                webSnapshotSettingsViewModel.ErrorMessage.ShouldBe("");
            }

            [Test]
            public void Then_assure_validated_URL_is_set()
            {
                webSnapshotSettingsViewModel.InputUrl = "http://www.smeedee.org/";
                webSnapshotSettingsViewModel.ValidatedUrl.ShouldBe("http://www.smeedee.org/");
            }
        
        }

        [TestFixture]
        public class When_URL_is_link_to_picture : Shared
        {
            [Test]
            public void Then_assure_URL_is_identified_as_picture_after_having_been_set_as_webpage()
            {
                webSnapshotSettingsViewModel.InputUrl = "http://smeedee.org/";
                webSnapshotSettingsViewModel.IsPictureUrl().ShouldBeFalse();
                webSnapshotSettingsViewModel.IsPictureUrl().ShouldNotBeNull();
                webSnapshotSettingsViewModel.InputUrl = "http://smeedee.org/images/code.png";
                webSnapshotSettingsViewModel.IsPictureUrl().ShouldBeTrue();
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
                    webSnapshotSettingsViewModel.InputUrl = url;
                    webSnapshotSettingsViewModel.IsPictureUrl().ShouldBeTrue();
                }
            }

            [Test]
            public void Then_assure_it_should_be_fetched_as_image()
            {
                webSnapshotSettingsViewModel.InputUrl = "http://smeedee.org/images/code.png";
                webSnapshotSettingsViewModel.FetchAsImage.CanExecute().ShouldBeTrue();
                webSnapshotSettingsViewModel.FetchAsSnapshot.CanExecute().ShouldBeFalse();
            }

            [Test]
            public void Then_assure_it_is_should_not_be_fetched_as_image_after_changing_URL()
            {
                webSnapshotSettingsViewModel.InputUrl = "http://smeedee.org/images/code.png";
                webSnapshotSettingsViewModel.FetchAsImage.CanExecute().ShouldBeTrue();
                webSnapshotSettingsViewModel.InputUrl = "http://smeedee.org/";
                webSnapshotSettingsViewModel.FetchAsImage.CanExecute().ShouldBeFalse();
            }
        }



        public class Shared
        {
            protected WebSnapshotSettingsViewModel webSnapshotSettingsViewModel;
            protected const string ERROR_MESSAGE = "Invalid URL!";
            protected const string INPUT_MESSAGE = "Enter URL here";

            [SetUp]
            public void Setup()
            {
                webSnapshotSettingsViewModel = new WebSnapshotSettingsViewModel();
            }

        }
    }
}
