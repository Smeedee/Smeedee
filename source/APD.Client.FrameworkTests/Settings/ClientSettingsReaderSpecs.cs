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

using System.Threading;
using APD.Client.Framework.Settings;
using APD.Framework.Settings.Repository;
using Moq;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;


namespace APD.Client.FrameworkTests.Settings.ClientSettingsReaderSpecs
{
    public class Shared
    {
        protected Mock<ISettingsRepository> localSettingsRepositoryMock = new Mock<ISettingsRepository>();
        protected Mock<ISettingsRepository> defaultSettingsRepositoryMock = new Mock<ISettingsRepository>();

        protected Mock<APD.Framework.Settings.Settings> localSettingsMock;
        protected Mock<APD.Framework.Settings.Settings> defaultSettingsMock;

        protected ClientSettingsReader reader;

        protected const string SETTING_NAME = "mySetting";
    }

    [TestFixture]
    public class when_spawned : Shared
    {
        [SetUp]
        public void Setup()
        {
            reader = new ClientSettingsReader(
                localSettingsRepositoryMock.Object, defaultSettingsRepositoryMock.Object);
        }

        [Test]
        public void when_not_loaded_settingExists_should_return_false()
        {
            reader.IsLoaded.ShouldBeFalse();
            bool aSettingExists = reader.SettingExists(SETTING_NAME);
            aSettingExists.ShouldBeFalse();
        }

        [Test]
        public void when_not_loaded_readSetting_should_return_null()
        {
            reader.IsLoaded.ShouldBeFalse();
            object settingValue = reader.ReadSetting(SETTING_NAME);
            settingValue.ShouldBeNull();
        }

        [Test]
        public void should_load_settings_async()
        {

            reader.IsLoaded.ShouldBeFalse();

            localSettingsRepositoryMock.Raise(
                s => s.GetSettingsComplete += null, new SettingsRepositoryEventArgs(null));

            reader.IsLoaded.ShouldBeFalse();

            defaultSettingsRepositoryMock.Raise(
                s => s.GetSettingsComplete += null, new SettingsRepositoryEventArgs(null));

            reader.IsLoaded.ShouldBeTrue();        
        }
    }

    [TestFixture]
    public class when_both_settingsrepositories_have_loaded : Shared
    {
        [SetUp]
        public void Setup()
        {
            reader = new ClientSettingsReader(
                localSettingsRepositoryMock.Object, defaultSettingsRepositoryMock.Object);
        }

        [Test]
        public void should_fire_settingsLoaded_event()
        {
            bool settingsLoadedWasCalled = false;
            reader.SettingsLoaded += (sender, args) =>
            {
                settingsLoadedWasCalled = true;
            };

            localSettingsRepositoryMock.Raise(
                s => s.GetSettingsComplete += null, new SettingsRepositoryEventArgs(null));

            defaultSettingsRepositoryMock.Raise(
                s => s.GetSettingsComplete += null, new SettingsRepositoryEventArgs(null));

            Thread.Sleep(500);

            settingsLoadedWasCalled.ShouldBeTrue();
        }
    }

    [TestFixture]
    public class when_localSetting_exists : Shared
    {
        [SetUp]
        public void Setup()
        {
            localSettingsMock = new Mock<APD.Framework.Settings.Settings>();
            defaultSettingsMock = new Mock<APD.Framework.Settings.Settings>();

            reader = new ClientSettingsReader(
                localSettingsRepositoryMock.Object, defaultSettingsRepositoryMock.Object );

            localSettingsMock.Setup(s => s.SettingExists(SETTING_NAME)).Returns(true);
            localSettingsMock.Setup(s => s.Get(SETTING_NAME)).Returns(1000);

            localSettingsRepositoryMock.Raise(
                s => s.GetSettingsComplete += null, new SettingsRepositoryEventArgs(localSettingsMock.Object));

            defaultSettingsRepositoryMock.Raise(
                s => s.GetSettingsComplete += null, new SettingsRepositoryEventArgs(defaultSettingsMock.Object));
        }

        [Test]
        public void should_not_query_defaultSettings_for_setting()
        {
            object result = reader.ReadSetting(SETTING_NAME);
            defaultSettingsMock.Verify( s=> s.Get(SETTING_NAME), Times.Never() );
        }

        [Test]
        public void settingExist_should_return_true_if_queried_setting_exists()
        {
            bool exists = reader.SettingExists(SETTING_NAME);
            exists.ShouldBeTrue();
        }

        [Test]
        public void should_query_localSettings_for_setting()
        {
            object result = reader.ReadSetting(SETTING_NAME);
            localSettingsMock.Verify(s => s.Get(SETTING_NAME), Times.Once());
        }

    }

    [TestFixture]
    public class when_localSetting_dont_exists_but_default_settings_exists : Shared
    {
        [SetUp]
        public void Setup()
        {
            localSettingsMock = new Mock<APD.Framework.Settings.Settings>();
            defaultSettingsMock = new Mock<APD.Framework.Settings.Settings>();

            reader = new ClientSettingsReader(
                localSettingsRepositoryMock.Object, defaultSettingsRepositoryMock.Object);

            localSettingsMock.Setup(s => s.SettingExists(SETTING_NAME)).Returns(false);

            defaultSettingsMock.Setup(s => s.Get(SETTING_NAME)).Returns(1000);
            defaultSettingsMock.Setup(s => s.SettingExists(SETTING_NAME)).Returns(true);

            localSettingsRepositoryMock.Raise(
                s => s.GetSettingsComplete += null, new SettingsRepositoryEventArgs(localSettingsMock.Object));

            defaultSettingsRepositoryMock.Raise(
                s => s.GetSettingsComplete += null, new SettingsRepositoryEventArgs(defaultSettingsMock.Object));
        }

        [Test]
        public void should_query_defaultSettings_for_setting()
        {
            object result = reader.ReadSetting(SETTING_NAME);

            defaultSettingsMock.Verify(s => s.SettingExists(SETTING_NAME), Times.Once());
            defaultSettingsMock.Verify(s => s.Get(SETTING_NAME), Times.Once());
        }

        [Test]
        public void should_ask_localSettings_if_setting_exists()
        {
            object result = reader.ReadSetting(SETTING_NAME);

            localSettingsMock.Verify(s => s.SettingExists(SETTING_NAME), Times.Once());
            localSettingsMock.Verify(s => s.Get(SETTING_NAME), Times.Never());
        }

    }

    [TestFixture]
    public class when_reader_is_loaded_but_setting_doesnt_exists : Shared
    {
        [SetUp]
        public void Setup()
        {
            localSettingsMock = new Mock<APD.Framework.Settings.Settings>();
            defaultSettingsMock = new Mock<APD.Framework.Settings.Settings>();

            reader = new ClientSettingsReader(
                localSettingsRepositoryMock.Object, defaultSettingsRepositoryMock.Object);

            localSettingsMock.Setup(s => s.SettingExists(It.IsAny<string>())).Returns(false);
            defaultSettingsMock.Setup(s => s.SettingExists(It.IsAny<string>())).Returns(false);

            localSettingsRepositoryMock.Raise(
                s => s.GetSettingsComplete += null, new SettingsRepositoryEventArgs(localSettingsMock.Object));

            defaultSettingsRepositoryMock.Raise(
                s => s.GetSettingsComplete += null, new SettingsRepositoryEventArgs(defaultSettingsMock.Object));
        }

        [Test]
        public void querying_for_setting_should_return_null()
        {
            reader.ReadSetting(SETTING_NAME).ShouldBeNull();
        }

        [Test]
        public void settingExist_should_return_false()
        {
            reader.SettingExists(SETTING_NAME).ShouldBeFalse();
        }

    }

}
