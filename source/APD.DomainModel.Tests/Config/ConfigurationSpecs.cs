using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.DomainModel.Config;

using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace APD.DomainModel.Tests.Config.ConfigurationSpecs
{
    [TestFixture]
    public class Configuration_specification
    {
        private Configuration configuration;

        [SetUp]
        public void Setup()
        {
            configuration = new Configuration();
        }

        [Test]
        public void Assure_it_has_a_constructor_without_args()
        {
            //This is for Serialization purposes
            var newConfig = new Configuration();
        }

        [Test]
        public void Assure_it_has_Settings()
        {
            configuration.Settings.ShouldNotBeNull();
        }

        [Test]
        public void Assure_it_has_IsConfigured()
        {
            configuration.IsConfigured.ShouldBeFalse();
        }
    }

    [TestFixture]
    public class When_spawning_new_Configuration
    {
        [Test]
        public void Assure_Name_parameter_is_validated()
        {
            this.ShouldThrowException<ArgumentException>(() =>
                new Configuration(null), exception =>
                    exception.Message.ShouldBe("Name must be specified"));
        }      
  
        [Test]
        public void Assure_Name_parameter_is_set_as_corresponding_Value()
        {
            var config = new Configuration("source control");
            config.Name.ShouldBe("source control");
        }
    }

    [TestFixture]
    public class When_spawning_default_CI_Configuration
    {
        private Configuration configuration;

        [SetUp]
        public void Setup()
        {
            configuration = Configuration.DefaultCIConfiguration();
        }

        [Test]
        public void Assure_it_has_Supported_Providers()
        {
            configuration.GetSetting("supported-providers").ShouldNotBeInstanceOfType<EntryNotFound>();
        }

        [Test]
        public void Assure_it_has_Provider()
        {
            configuration.GetSetting("provider").ShouldNotBeInstanceOfType<EntryNotFound>();
        }

        [Test]
        public void Assure_it_has_Username()
        {
            configuration.GetSetting("username").ShouldNotBeInstanceOfType<EntryNotFound>();
        }

        [Test]
        public void Assure_it_has_Password()
        {
            configuration.GetSetting("password").ShouldNotBeInstanceOfType<EntryNotFound>();
        }

        [Test]
        public void Assure_it_has_URL()
        {
            configuration.GetSetting("url").ShouldNotBeInstanceOfType<EntryNotFound>();
        }

        [Test]
        public void Assure_it_has_IsExpanded()
        {
            configuration.GetSetting("is-expanded").ShouldNotBeInstanceOfType<EntryNotFound>();
        }

        [Test]
        public void Assure_it_has_Project()
        {
            configuration.GetSetting("project").ShouldNotBeInstanceOfType<EntryNotFound>();
        }

    }

    [TestFixture]
    public class When_spawning_Provider_Configuration
    {
        private Configuration configuration;

        [SetUp]
        public void Setup()
        {
            configuration = Configuration.ProviderConfiguration(
                "vcs",
                "goeran",
                "hansen",
                "http://smeedee.org/ccnet",
                "svn",
                string.Empty,
                "svn", "tfs");
        }

        [Test]
        public void Assure_Configuration_Name_is_set()
        {
            configuration.Name.ShouldBe("vcs");
        }

        [Test]
        public void Assure_SupportedProviders_is_set()
        {
            configuration.GetSetting("supported-providers").Vals.ShouldHave("svn");
            configuration.GetSetting("supported-providers").Vals.ShouldHave("tfs");
        }

        [Test]
        public void Assure_Provider_is_set()
        {
            configuration.GetSetting("provider").Value.ShouldBe("svn");
        }

        [Test]
        public void Assure_Username_is_set()
        {
            configuration.GetSetting("username").Value.ShouldBe("goeran");
        }

        [Test]
        public void Assure_Password_is_set()
        {
            configuration.GetSetting("password").Value.ShouldBe("hansen");
        }

        [Test]
        public void Assure_URL_is_set()
        {
            configuration.GetSetting("url").Value.ShouldBe("http://smeedee.org/ccnet");
        }

        [Test]
        public void Assure_it_has_Project()
        {
            configuration.GetSetting("project").ShouldNotBeInstanceOfType<EntryNotFound>();
        }

    }

    [TestFixture]
    public class When_spawning_default_Dashboard_Configuration
    {
        private Configuration configuration;
        
        [SetUp]
        public void Setup()
        {
            configuration = Configuration.DefaultDashboardConfiguration();
        }

        [Test]
        public void Assure_it_has_SlideWidgets()
        {
            var slideWidgets = configuration.GetSetting("slide-widgets").Vals;
            slideWidgets.ShouldHave("latest-commits");
            slideWidgets.ShouldHave("ci");
            slideWidgets.ShouldHave("commit-heroes");
            slideWidgets.ShouldHave("commit-stats");
            slideWidgets.ShouldHave("working-days-left");
        }

        [Test]
        public void Assure_it_has_SelectedSlideWidgets()
        {
            var slideWidgets = configuration.GetSetting("selected-slide-widgets").Vals;
            slideWidgets.ShouldHave("latest-commits");
            slideWidgets.ShouldHave("ci");
            slideWidgets.ShouldHave("commit-heroes");
            slideWidgets.ShouldHave("commit-stats");
            slideWidgets.ShouldHave("working-days-left");
        }

        [Test]
        public void Assure_it_has_TrayWidgets()
        {
            var trayWidgets = configuration.GetSetting("tray-widgets").Vals;
            trayWidgets.ShouldHave("working-days-left");
            trayWidgets.ShouldHave("ci");
        }

        [Test]
        public void Assure_it_has_SelectedTrayWidgets()
        {
            var trayWidgets = configuration.GetSetting("selected-tray-widgets").Vals;
            trayWidgets.ShouldHave("working-days-left");
            trayWidgets.ShouldHave("ci");
        }

        [Test]
        public void Assure_it_has_IsExpanded()
        {
            configuration.GetSetting("is-expanded").ShouldNotBeInstanceOfType<EntryNotFound>();
        }

    }

    [TestFixture]
    public class When_spawning_default_VCS_Configuration
    {
        private Configuration configuration;

        [SetUp]
        public void Setup()
        {
            configuration = Configuration.DefaultVCSConfiguration();
        }
        
        [Test]
        public void Assure_it_has_Supported_Providers()
        {
            configuration.GetSetting("supported-providers").ShouldNotBeInstanceOfType<EntryNotFound>();
        }

        [Test]
        public void Assure_it_has_Provider()
        {
            configuration.GetSetting("provider").ShouldNotBeInstanceOfType<EntryNotFound>();
        }

        [Test]
        public void Assure_it_has_Username()
        {
            configuration.GetSetting("username").ShouldNotBeInstanceOfType<EntryNotFound>();
        }

        [Test]
        public void Assure_it_has_Password()
        {
            configuration.GetSetting("password").ShouldNotBeInstanceOfType<EntryNotFound>();
        }

        [Test]
        public void Assure_it_has_URL()
        {
            configuration.GetSetting("url").ShouldNotBeInstanceOfType<EntryNotFound>();
        }

        [Test]
        public void Assure_it_has_IsExpanded()
        {
            configuration.GetSetting("is-expanded").ShouldNotBeInstanceOfType<EntryNotFound>();
        }

        [Test]
        public void Assure_it_has_Project()
        {
            configuration.GetSetting("project").ShouldNotBeInstanceOfType<EntryNotFound>();
        }

    }

    [TestFixture]
    public class When_adding_new_Setting
    {
        private Configuration configuration;

        [SetUp]
        public void Setup()
        {
            configuration = new Configuration("source control");
        }

        [Test]
        public void Assure_Name_parameter_is_validated()
        {
            this.ShouldThrowException<ArgumentException>(() =>
                configuration.NewSetting(null, "a value"), exception =>
                    exception.Message.ShouldBe("Name must be specified"));
        }

        [Test]
        public void Assure_Value_parameter_is_validated()
        {
            this.ShouldThrowException<ArgumentException>(() =>
                configuration.NewSetting("name", null), exception =>
                    exception.Message.ShouldBe("Value must be specified"));
        }

        [Test]
        public void Assure_its_possible_to_add_a_value()
        {
            configuration.NewSetting("username", "goeran");   
        }

        [Test]
        public void Assure_its_possible_to_add_multiple_values()
        {
            configuration.NewSetting("supported providers", "svn", "tfs", "clear case");
        }

        [Test]
        public void Assure_Configuration_is_set_on_new_settings_entry()
        {
            configuration.NewSetting("username", "goeran");

            configuration.Settings.Where(e => e.Name == "username").FirstOrDefault().Configuration.ShouldBe(configuration);
        }
    }

    [TestFixture]
    public class When_setting_is_added
    {
        private Configuration configuration;
        private SettingsEntry entry;

        [SetUp]
        public void Setup()
        {
            configuration = new Configuration("source control");
            configuration.NewSetting("supported providers", "svn", "tfs", "clear case");
            configuration.NewSetting("selected provider", "svn");
        }

        [Test]
        public void Assure_new_entries_extis_in_Settings()
        {            
            (configuration.Settings.Count() > 0).ShouldBeTrue();
        }

        [Test]
        public void Assure_single_value_is_stored_in_entry()
        {
            configuration.GetSetting("supported providers").Value.ShouldBe("svn");
        }

        [Test]
        public void Assure_multiple_values_is_stored_in_entry()
        {
            var entry = configuration.GetSetting("supported providers");
            entry.Value.ShouldBe("svn");
            entry.Vals.ShouldHave("svn");
            entry.Vals.ShouldHave("tfs");
            entry.Vals.ShouldHave("clear case");
        }

        [Test]
        public void Assure_its_possible_to_determine_if_entry_have_multiple_values()
        {
            var entry = configuration.GetSetting("supported providers");
            entry.HasMultipleValues.ShouldBeTrue();

            configuration.GetSetting("selected provider").HasMultipleValues.ShouldBeFalse();
        }
    }

    [TestFixture]
    public class When_querying_to_check_if_Setting_exist
    {
        private Configuration configuration;

        [SetUp]
        public void Setup()
        {
            configuration = new Configuration("source control");
            configuration.NewSetting("provider", "svn");
        }

        [Test]
        public void Assure_its_possible_to_find_out_if_an_setting_does_not_exist()
        {
            configuration.ContainSetting("does not exists").ShouldBeFalse();
        }
        
        [Test]
        public void Assure_its_possible_to_find_out_if_an_setting_exist()
        {
            configuration.ContainSetting("provider").ShouldBeTrue();   
        }

        [Test]
        public void Assure_its_possible_to_check_if_setting_exist_when_there_are_no_settings()
        {
            configuration = new Configuration("new config");
            configuration.ContainSetting("does not exist").ShouldBeFalse();
        }
    }

    [TestFixture]
    public class When_query_settings
    {
        private Configuration configuration;

        [SetUp]
        public void Setup()
        {
            configuration = new Configuration("source control");
            configuration.NewSetting("username", "goeran");
        }

        [Test]
        public void Assure_EntryNotFound_is_returned_if_entry_does_not_exists()
        {
            var entry = configuration.GetSetting("does not exist");
            entry.ShouldBeInstanceOfType<EntryNotFound>();
        }

        [Test]
        public void Assure_SettingsEntry_is_returned_when_entry_exist()
        {
            var entry = configuration.GetSetting("username");
            entry.ShouldBeInstanceOfType<SettingsEntry>();
        }
    }
}
