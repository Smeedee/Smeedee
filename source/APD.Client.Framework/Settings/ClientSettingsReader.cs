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
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Windows;

using APD.Framework.Settings.Repository;


namespace APD.Client.Framework.Settings
{
    public class ClientSettingsReader : IClientSettingsReader
    {
        private APD.Framework.Settings.Settings localSettings = new APD.Framework.Settings.Settings();
        private APD.Framework.Settings.Settings defaultSettings = new APD.Framework.Settings.Settings();

        private readonly ISettingsRepository localSettingsRepository;
        private readonly ISettingsRepository defaultSettingsRepository;

        private bool localSettingsLoaded = false;
        private bool defaultSettingsLoaded = false;

        public event EventHandler SettingsLoaded;

        public bool IsLoaded
        {
            get { return localSettingsLoaded && defaultSettingsLoaded; }
        }

        public ClientSettingsReader(
                ISettingsRepository localSettingsRepository,
                ISettingsRepository defaultSettingsRepository)
            : this(localSettingsRepository, defaultSettingsRepository, null) {}
        public ClientSettingsReader(
                ISettingsRepository localSettingsRepository,
                ISettingsRepository defaultSettingsRepository, 
                EventHandler settingsLoadedEventHandler)
        {
            this.localSettingsRepository = localSettingsRepository;
            this.defaultSettingsRepository = defaultSettingsRepository;
            if(settingsLoadedEventHandler != null)
                SettingsLoaded += settingsLoadedEventHandler;
                
            AddLocalSettingsRepositoryEventHandler();
            AddDefaultSettingsRepositoryEventHandler();

            this.localSettingsRepository.GetSettingsAsync();
            this.defaultSettingsRepository.GetSettingsAsync();
        }

        private void AddDefaultSettingsRepositoryEventHandler()
        {
            this.defaultSettingsRepository.GetSettingsComplete += (o, e) =>
            {
                this.defaultSettings = e.Settings;
                defaultSettingsLoaded = true;
                FireSettingsLoadedIfBothSettingsAreDone();
            };
        }

        private void AddLocalSettingsRepositoryEventHandler()
        {
            this.localSettingsRepository.GetSettingsComplete += (o, e) =>
            {
                this.localSettings = e.Settings;
                localSettingsLoaded = true;
                FireSettingsLoadedIfBothSettingsAreDone();
            };
        }

        private void FireSettingsLoadedIfBothSettingsAreDone()
        {
            if (localSettingsLoaded && defaultSettingsLoaded && SettingsLoaded != null)
                SettingsLoaded(this, EventArgs.Empty);
        }



        public bool SettingExists(string settingName)
        {
            bool exists =
                localSettings.SettingExists(settingName) ||
                defaultSettings.SettingExists(settingName);

            return exists;
        }

        public object ReadSetting(string settingName)
        {
            if (localSettings.SettingExists(settingName))
            {
                return localSettings.Get(settingName);
            }
            else if(defaultSettings.SettingExists(settingName))
            {
                return defaultSettings.Get(settingName);
            }
            else
            {
                return null;
            }
        }
    }
}