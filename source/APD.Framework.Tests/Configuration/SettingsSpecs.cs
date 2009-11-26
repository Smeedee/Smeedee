#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using APD.Framework.Settings;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;


namespace APD.FrameworkTests.SettingsSpecs
{
    [TestFixture]
    public class serializing_or_deserializing
    {
        private Settings settings;

        [SetUp]
        public void SetUp()
        {
            settings = new Framework.Settings.Settings();
            settings["mySetting"] = "blah";
            settings["oleAndre"] = 1337;
            settings["torstein"] = DateTime.Now;
            settings["anotherone"] = 0.43f;
            settings["isThereAGod"] = false;
        }

        [Test]
        public void should_be_serializable()
        {
            string serializedSettings = settings.Serialize();
            serializedSettings.ShouldNotBeNull();
        }

        [Test]
        public void should_be_able_to_deserialize_serialized_settingscollction()
        {
            string serializedSettings = settings.Serialize();
            Framework.Settings.Settings newSettingsColletion = Framework.Settings.Settings.Deserialize(serializedSettings);
            newSettingsColletion.ShouldNotBeNull();
        }

        [Test]
        [ExpectedException(typeof(SettingsException))]
        public void should_throw_exception_on_deserialization_failure()
        {
            string notASerializedSettings = "i'm a bug";
            Framework.Settings.Settings newSettings = Framework.Settings.Settings.Deserialize(notASerializedSettings);
        }

        [Test]
        [ExpectedException(typeof(SettingsException))]
        public void should_throw_exception_on_serialization_failure()
        {
            settings["this cannot be serialized"] = new TimeSpan(100);
            string x = settings.Serialize();
        }

    }

    [TestFixture]
    public class when_spawned
    {
        private Framework.Settings.Settings settings;

        [SetUp]
        public void SetUp()
        {
            settings = new Framework.Settings.Settings();
        }

        [Test]
        public void should_have_count_of_zero_settings()
        {
            settings.Count.ShouldBe(0);
        }

        [Test]
        public void should_be_able_to_add_setting()
        {
            settings["ci.play_sound"] = true;
        }

        [Test]
        public void should_have_settingExists_method()
        {
            bool exists = settings.SettingExists("any setting name");
            exists.ShouldBe(false);
        }

        [Test]
        public void should_have_getEnumerator_method()
        {
            settings = new Framework.Settings.Settings();
            settings.Set("test setting", 1000);
            var enumerator = settings.GetEnumerator();
            enumerator.ShouldNotBeNull();
            enumerator.MoveNext();
            enumerator.Current.ShouldNotBeNull();
        }
    }


    [TestFixture]
    public class when_settingsCollection_contains_settings
    {
        private const string TEST_SETTING_NAME = "this is a setting";
        private Framework.Settings.Settings settings;

        [SetUp]
        public void SetUp()
        {
            settings = new Framework.Settings.Settings();
            settings[TEST_SETTING_NAME] = 1000;
        }

        [Test]
        public void should_be_able_to_read_setting()
        {
            object settingValue = settings[TEST_SETTING_NAME];
            settingValue.ShouldBe(1000);
        }

        [Test]
        public void should_return_null_when_attempting_to_read_name_null_or_nonexistent_name()
        {
            object name = settings.Get(null);
            name.ShouldBe(null);
            name = settings.Get("per");
            name.ShouldBe(null);
        }

        [Test]
        public void should_have_a_count_equal_number_of_settings()
        {
            settings.Count.ShouldBe(1);
        }

        [Test]
        public void settingExist_should_be_true_when_key_exist()
        {
            settings.SettingExists(TEST_SETTING_NAME).ShouldBe(true);
        }

        [Test]
        public void should_be_able_to_delete_setting()
        {
            bool existedBeforeDelete = settings.SettingExists(TEST_SETTING_NAME);
            settings.Remove(TEST_SETTING_NAME);
            bool existsAfterDelete = settings.SettingExists(TEST_SETTING_NAME);
            
            
            existedBeforeDelete.ShouldBeTrue();
            existsAfterDelete.ShouldBeFalse();
        }

        [Test]
        public void should_update_when_setting_exists()
        {
            var oldValue = settings[TEST_SETTING_NAME];
            settings[TEST_SETTING_NAME] = "bla";
            var updatedValue = settings[TEST_SETTING_NAME];

            oldValue.ShouldNotBe(updatedValue);
        }
    }
}
