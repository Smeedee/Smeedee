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
using APD.Framework.Settings.Repository;
using APD.Tests;
using Moq;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;


namespace APD.Admin.Tests.SettingsManagerSpecs
{

    public class Shared
    {
        protected const string TEST_SETTING_NAME = "mysetting";
        protected Settings adminSettings = new Settings();
        protected Settings userSettings = new Settings();
        protected const int ADMIN_SETTING_VALUE = 2000;
        protected const int USER_SETTING_VALUE = 1000;


        protected SettingsManager manager;
        protected Mock<ISettingsRepository> userSettingsRepository;
        protected Mock<ISettingsRepository> adminSettingsRepository;

        protected Mock<Settings> userSettingsMock;
        protected Mock<Settings> adminSettingsMock;


        protected void RaiseUserSettingsReposioryGetCompletedEvent(Settings settings)
        {
            userSettingsRepository.Raise(r => r.GetSettingsComplete += null,
                new SettingsRepositoryEventArgs(settings));
        }

        protected void RaiseAdminSettingsReposioryGetCompletedEvent(Settings settings)
        {
            adminSettingsRepository.Raise(r => r.GetSettingsComplete += null,
                new SettingsRepositoryEventArgs(settings));
        }

        protected void RaiseUserSettingsReposiorySetCompletedEvent()
        {
            userSettingsRepository.Raise(r => r.SetSettingsComplete += null, EventArgs.Empty);
        }

        protected void RaiseAdminSettingsReposiorySetCompletedEvent()
        {
            adminSettingsRepository.Raise(r => r.SetSettingsComplete += null, EventArgs.Empty);
        }
    }

    [TestFixture]
    public class when_spawned : Shared
    {
        [SetUp]
        public void Setup()
        {
            userSettingsRepository = new Mock<ISettingsRepository>();
            this.userSettings = new Settings();
            userSettings.Set(TEST_SETTING_NAME, 9000);

            adminSettingsRepository = new Mock<ISettingsRepository>();

            manager = new SettingsManager(userSettingsRepository.Object, adminSettingsRepository.Object);

        }

        [Test]
        public void should_have_isAdmin_property()
        {
            PropertyTester.TestForExistence<SettingsManager>(sm => sm.IsAdmin);
        }

        [Test]
        public void should_have_isLoading_property()
        {
            PropertyTester.TestForExistence<SettingsManager>(sm => sm.IsLoading);
        }

        [Test]
        public void should_have_isSaving_property()
        {
            PropertyTester.TestForExistence<SettingsManager>(sm => sm.IsSaving);
        }

        [Test]
        public void should_not_be_in_adminMode()
        {
            manager.IsAdmin.ShouldBeFalse();
        }

        [Test]
        public void should_load_userSettings_async()
        {
            userSettingsRepository.Verify(r => r.GetSettingsAsync(), Times.Once());
            RaiseUserSettingsReposioryGetCompletedEvent(userSettings);

            manager.GetSetting(TEST_SETTING_NAME).ShouldBe(9000);
        }

    }

    [TestFixture]
    public class when_changing_mode : Shared
    {
        [SetUp]
        public void Setup()
        {
            userSettingsRepository = new Mock<ISettingsRepository>();
            this.userSettings = new Settings();
            userSettings.Set(TEST_SETTING_NAME, USER_SETTING_VALUE);
            adminSettingsRepository = new Mock<ISettingsRepository>();
            this.adminSettings = new Settings();
            adminSettings.Set(TEST_SETTING_NAME, ADMIN_SETTING_VALUE);

            manager = new SettingsManager(
                userSettingsRepository.Object, adminSettingsRepository.Object);
                
            RaiseUserSettingsReposioryGetCompletedEvent(userSettings);
        }

        [Test]
        public void changeToAdminMode_should_set_isLoading()
        {
            change_mode_and_assert_IsLoading_property(true);
        }

        [Test]
        public void changing_between_modes_should_fire_events()
        {
            change_Mode_And_Assert_Events(true);
            change_Mode_And_Assert_Events(false);
        }

        [Test]
        public void setting_the_same_mode_should_not_fire_events()
        {
            bool loadingSettingsWasCalled = false;
            manager.LoadingSettings += (o, e) => loadingSettingsWasCalled = true;

            manager.ChangeToUserMode();

            loadingSettingsWasCalled.ShouldBeFalse();
        }

        [Test]
        public void changeToAdminMode_should_load_the_admin_settings()
        {
            manager.ChangeToAdminMode();
            
            adminSettingsRepository.Raise(
                r=>r.GetSettingsComplete += null, new SettingsRepositoryEventArgs(adminSettings));

            manager.GetSetting(TEST_SETTING_NAME).ShouldBe(ADMIN_SETTING_VALUE);
        }


        private void change_Mode_And_Assert_Events(bool adminMode)
        {
            bool loadingSettingsWasCalled = false;
            manager.LoadingSettings += ( (sender, e) => loadingSettingsWasCalled = true );

            bool loadSettingsCompletedWasCalled = false;
            manager.LoadSettingsCompleted += ( (sender, args) => loadSettingsCompletedWasCalled = true );

            if( adminMode )
                manager.ChangeToAdminMode();
            else
                manager.ChangeToUserMode();

            loadingSettingsWasCalled.ShouldBeTrue();

            RaiseAdminSettingsReposioryGetCompletedEvent(adminSettings);

            loadSettingsCompletedWasCalled.ShouldBeTrue();
        }

        private void change_mode_and_assert_IsLoading_property(bool adminMode)
        {
            manager.IsLoading.ShouldBeFalse();

            if(adminMode)
                manager.ChangeToAdminMode();
            else
                manager.ChangeToUserMode();

            manager.IsLoading.ShouldBeTrue();

            RaiseAdminSettingsReposioryGetCompletedEvent(adminSettings);

            manager.IsLoading.ShouldBeFalse();
        }
    }

    [TestFixture]
    public class when_loading_settings : Shared
    {
        [SetUp]
        public void Setup()
        {
            userSettingsRepository = new Mock<ISettingsRepository>();
            this.userSettings = new Settings();
            userSettings.Set(TEST_SETTING_NAME, USER_SETTING_VALUE);
            adminSettingsRepository = new Mock<ISettingsRepository>();
            this.adminSettings = new Settings();
            adminSettings.Set(TEST_SETTING_NAME, ADMIN_SETTING_VALUE);

            manager = new SettingsManager(
                userSettingsRepository.Object, adminSettingsRepository.Object);
        }

        [Test]
        public void IsLoading_should_be_true()
        {
            manager.IsLoading.ShouldBeTrue();
        }

        [Test]
        [ExpectedException(typeof(SettingsException))]
        public void settingExists_should_throw_exception()
        {
            manager.SettingExists("whatever setting");
        }

        [Test]
        [ExpectedException(typeof(SettingsException))]
        public void getSetting_should_throw_exception()
        {
            manager.GetSetting("Whatever setting");
        }


        [Test]
        [ExpectedException(typeof(SettingsException))]
        public void setSetting_should_throw_exception()
        {
            manager.SetSetting("Whatever setting", "whatever value");
        }

        [Test]
        [ExpectedException(typeof(SettingsException))]
        public void RemoveSetting_should_throw_exception()
        {
            manager.RemoveSetting("Whatever setting");
        }

        [Test]
        [ExpectedException(typeof(SettingsException))]
        public void reloadSettingsAsync_should_throw_exception()
        {
            manager.ReloadSettingsAsync();
        }

        [Test]
        [ExpectedException(typeof(SettingsException))]
        public void saveCurrentSettingsAsync_should_throw_exception()
        {
            manager.SaveCurrentSettingsAsync();
        }

        [Test]
        [ExpectedException(typeof(SettingsException))]
        public void changeToAdminMode_should_throw_exception()
        {
            manager.ChangeToAdminMode();
        }
                [Test]
        [ExpectedException(typeof(SettingsException))]
        public void GetSettingsEnumerator_should_throw_exception()
        {
            manager.GetSettingsEnumerator();
        }
        
        [Test]
        public void isLoading_should_be_false_when_repository_returns()
        {
            RaiseUserSettingsReposioryGetCompletedEvent(userSettings);
            manager.IsLoading.ShouldBeFalse();
        }

        [Test]
        public void loadCompleted_event_should_fire_when_done_loading()
        {
            bool eventFired = false;
            manager.LoadSettingsCompleted += (o, w) => eventFired = true;
            RaiseUserSettingsReposioryGetCompletedEvent(new Settings());
            eventFired.ShouldBeTrue();
        }

    }

    [TestFixture]
    public class when_saving_settings : Shared
    {
        [SetUp]
        public void Setup()
        {
            userSettingsRepository = new Mock<ISettingsRepository>();
            this.userSettings = new Settings();
            userSettings.Set(TEST_SETTING_NAME, USER_SETTING_VALUE);
            adminSettingsRepository = new Mock<ISettingsRepository>();
            this.adminSettings = new Settings();
            adminSettings.Set(TEST_SETTING_NAME, ADMIN_SETTING_VALUE);

            manager = new SettingsManager(
                userSettingsRepository.Object, adminSettingsRepository.Object);

            RaiseUserSettingsReposioryGetCompletedEvent(userSettings);

            manager.SaveCurrentSettingsAsync();
        }

        [Test]
        public void IsSaving_should_be_true()
        {
            manager.IsSaving.ShouldBeTrue();
        }

        [Test]
        [ExpectedException(typeof(SettingsException))]
        public void settingExists_should_throw_exception()
        {
            manager.SettingExists("whatever setting");
        }

        [Test]
        [ExpectedException(typeof(SettingsException))]
        public void getSetting_should_throw_exception()
        {
            manager.GetSetting("Whatever setting");
        }

        [Test]
        [ExpectedException(typeof(SettingsException))]
        public void setSetting_should_throw_exception()
        {
            manager.SetSetting("Whatever setting", "whatever value");
        }

        [Test]
        [ExpectedException(typeof(SettingsException))]
        public void RemoveSetting_should_throw_exception()
        {
            manager.RemoveSetting("Whatever setting");
        }

        [Test]
        [ExpectedException(typeof(SettingsException))]
        public void reloadSettingsAsync_should_throw_exception()
        {
            manager.ReloadSettingsAsync();
        }

        [Test]
        [ExpectedException(typeof(SettingsException))]
        public void saveCurrentSettingsAsync_should_throw_exception()
        {
            manager.SaveCurrentSettingsAsync();
        }

        [Test]
        [ExpectedException(typeof(SettingsException))]
        public void changeToAdminMode_should_throw_exception()
        {
            manager.ChangeToAdminMode();
        }
        [Test]
        [ExpectedException(typeof(SettingsException))]
        public void GetSettingsEnumerator_should_throw_exception()
        {
            manager.GetSettingsEnumerator();
        }

        [Test]
        public void isSaving_should_be_false_when_repository_returns()
        {
            RaiseUserSettingsReposiorySetCompletedEvent();
            manager.IsSaving.ShouldBeFalse();
        }

        [Test]
        public void saveCompleted_event_should_fire_when_done_saving()
        {
            bool eventFired = false;
            manager.SaveSettingsCompleted += (o, w) => eventFired = true;
            RaiseUserSettingsReposiorySetCompletedEvent();
            eventFired.ShouldBeTrue();
        }

        [Test]
        public void saving_event_should_fire_when_saving()
        {
            RaiseUserSettingsReposiorySetCompletedEvent();
            bool eventFired = false;
            manager.SavingSettings += (o, w) => eventFired = true;
            manager.SaveCurrentSettingsAsync();
            eventFired.ShouldBeTrue();
        }
    }

    [TestFixture]
    public class when_manipulating_settings : Shared
    {
        [SetUp]
        public void Setup()
        {
            userSettingsRepository = new Mock<ISettingsRepository>();
            userSettingsMock = new Mock<Settings>();

            adminSettingsRepository = new Mock<ISettingsRepository>();

            manager = new SettingsManager(
                userSettingsRepository.Object, adminSettingsRepository.Object);

            RaiseUserSettingsReposioryGetCompletedEvent(userSettingsMock.Object);
        }

        [Test]
        public void setSetting_should_set_setting()
        {
            manager.SetSetting("a", "s");
            userSettingsMock.Verify(s => s.Set(It.IsAny<string>(), It.IsAny<object>()), Times.Once());
        }

        [Test]
        public void getSetting_should_set_setting()
        {
            manager.GetSetting("s");
            userSettingsMock.Verify(s => s.Get(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void removeSetting_should_remove_setting()
        {
            manager.RemoveSetting(It.IsAny<string>());
            userSettingsMock.Verify(s => s.Remove(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void settingExists_should_call_settingExists_on_the_current_setting()
        {
            manager.SettingExists(It.IsAny<string>());
            userSettingsMock.Verify(s => s.SettingExists(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void getSettingsEnumerator_should_get_the_settingsEnumerator()
        {
            var enumerator = manager.GetSettingsEnumerator();
            userSettingsMock.Verify(s=>s.GetEnumerator(), Times.Once());
        }
    }
}
