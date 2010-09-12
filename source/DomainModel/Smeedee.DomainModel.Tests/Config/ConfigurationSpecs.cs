using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Smeedee.DomainModel.Config;

using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.DomainModel.Tests.Config.ConfigurationSpecs
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

        [Test]
        public void Assure_name_is_not_null()
        {
            configuration.Name.ShouldNotBeNull();
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

        [Test]
        public void Assure_new_Id_is_created()
        {
            var config = new Configuration();
            config.Id.ShouldNotBe(Guid.Empty);
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
            configuration.ContainsSetting("does not exists").ShouldBeFalse();
        }
        
        [Test]
        public void Assure_its_possible_to_find_out_if_an_setting_exist()
        {
            configuration.ContainsSetting("provider").ShouldBeTrue();   
        }

        [Test]
        public void Assure_its_possible_to_check_if_setting_exist_when_there_are_no_settings()
        {
            configuration = new Configuration("new config");
            configuration.ContainsSetting("does not exist").ShouldBeFalse();
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


    [TestFixture]
    public class when_changing_a_setting
    {
        [Test]
        public void Assure_value_is_changed()
        {
            var configuration = new Configuration();
            configuration.NewSetting("hello", "world");
            configuration.ChangeSetting("hello", "moon");

            configuration.GetSetting("hello").Value.ShouldBe("moon");
        }
    }

    [TestFixture]
    public class When_clone_config_without_settings
    {
        private Configuration configuration;
        private Configuration copy;

        [SetUp]
        public void Setup()
        {
            configuration = new Configuration("source control");
            configuration.Id = Guid.NewGuid();

            copy = configuration.Clone();
        }

        [Test]
        public void Then_assure_no_settings_are_cloned()
        {
            copy.Settings.Count().ShouldBe(0);
        }
    }

    [TestFixture]
    public class When_clone
    {
        private Configuration configuration;
        private Configuration copy;

        [SetUp]
        public void Setup()
        {
            configuration = new Configuration("source control");
            configuration.Id = Guid.NewGuid();
            configuration.NewSetting("supported providers", "svn", "tfs", "clear case");
            configuration.NewSetting("selected provider", "svn");
            configuration.IsConfigured = true;

            copy = configuration.Clone();
        }

        [Test]
        public void Then_assure_Name_is_cloned()
        {
            copy.Name.ShouldBe(configuration.Name);
        }

        [Test]
        public void Then_assure_Id_is_cloned()
        {
            copy.Id.ShouldBe(configuration.Id);
        }

        [Test]
        public void Then_assure_IsConfigured_flag_is_cloned()
        {
            copy.IsConfigured.ShouldBe(configuration.IsConfigured);
        }

        [Test]
        public void Then_assure_Settings_are_cloned()
        {
            copy.Settings.Count().ShouldBe(configuration.Settings.Count());
        }

        [Test]
        public void Then_assure_Setting_Name_is_cloned()
        {
            copy.ContainsSetting("selected provider").ShouldBeTrue();
            copy.ContainsSetting("supported providers").ShouldBeTrue();
        }

        [Test]
        public void Then_assure_Setting_Values_are_cloned()
        {
            var selectedProviderSetting = copy.GetSetting("selected provider");
            selectedProviderSetting.Name.ShouldBe("selected provider");
            selectedProviderSetting.Value.ShouldBe("svn");
            selectedProviderSetting.HasMultipleValues.ShouldBeFalse();

            var supportedProvidersSetting = copy.GetSetting("supported providers");
            supportedProvidersSetting.Name.ShouldBe("supported providers");
            supportedProvidersSetting.Vals.ShouldContain("svn");
            supportedProvidersSetting.Vals.ShouldContain("clear case");
            supportedProvidersSetting.Vals.ShouldContain("tfs");
            supportedProvidersSetting.Vals.Count().ShouldBe(3);
        }
    }

}
