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
using System.Collections.Generic;
using APD.Framework.Settings;
using APD.Framework.Settings.Repository;


namespace APD.Admin
{
    public class SettingsManager
    {
        private readonly ISettingsRepository userSettingsRepository;
        private readonly ISettingsRepository adminSettingsRepository;
        private ISettingsRepository currentSettingsRepository;
        private Settings currentSettings = new Settings();

        public event EventHandler SaveSettingsCompleted;
        public event EventHandler LoadSettingsCompleted;
        public event EventHandler SavingSettings;
        public event EventHandler LoadingSettings;

        public bool IsAdmin
        {
            get; private set;
        }

        public bool IsLoading { get; private set; }
        public bool IsSaving { get; private set; }  


        public SettingsManager(ISettingsRepository localSettingsRepository, 
            ISettingsRepository defaultSettingsRepository)
        {
            this.userSettingsRepository = localSettingsRepository;
            this.adminSettingsRepository = defaultSettingsRepository;

            SetupRepositoryEventHandlers();
            SetCurrentSettingsRepository();
            ReloadSettingsAsync();
        }

        private void SetupRepositoryEventHandlers()
        {
            userSettingsRepository.GetSettingsComplete += new EventHandler<SettingsRepositoryEventArgs>(GetSettingsCompleted);
            adminSettingsRepository.GetSettingsComplete += new EventHandler<SettingsRepositoryEventArgs>(GetSettingsCompleted);

            userSettingsRepository.SetSettingsComplete += new EventHandler(SetSettingsCompleted);
            adminSettingsRepository.SetSettingsComplete += new EventHandler(SetSettingsCompleted);

        }

        private void SetCurrentSettingsRepository()
        {
            currentSettingsRepository = IsAdmin ?
                                                adminSettingsRepository
                                            :
                                                userSettingsRepository;
        }

        private void GetSettingsCompleted(object sender, SettingsRepositoryEventArgs e)
        {
            IsLoading = false;
            currentSettings = e.Settings;
            if (LoadSettingsCompleted != null)
                LoadSettingsCompleted(this, EventArgs.Empty);

        }

        private void SetSettingsCompleted(object sender, EventArgs e)
        {
            IsSaving = false;
            if (SaveSettingsCompleted != null)
                SaveSettingsCompleted(this, EventArgs.Empty);
        }

        public void ChangeToAdminMode()
        {
            ChangeMode(true);
        }

        public void ChangeToUserMode()
        {
            ChangeMode(false);
        }

        private void ChangeMode(bool value)
        {
            if (value != IsAdmin)
            {
                ThrowExceptionIfLoadingOrSaving();
                IsAdmin = value;
                SetCurrentSettingsRepository();
                ReloadSettingsAsync();
            }
        }

        public void ReloadSettingsAsync()
        {
            ThrowExceptionIfLoadingOrSaving();

            IsLoading = true;

            if (LoadingSettings != null)
                LoadingSettings(this, EventArgs.Empty);

            currentSettingsRepository.GetSettingsAsync();
        }

        public void SaveCurrentSettingsAsync()
        {
            ThrowExceptionIfLoadingOrSaving();

            IsSaving = true;
            if (SavingSettings != null)
                SavingSettings(this, EventArgs.Empty);

            currentSettingsRepository.SetSettingsAsync(currentSettings);
        }

        public void SetSetting(string settingName, object value)
        {
            ThrowExceptionIfLoadingOrSaving();
            currentSettings[settingName] = value;
        }

        public object GetSetting(string settingName)
        {
            ThrowExceptionIfLoadingOrSaving();
            return currentSettings.Get(settingName);
        }

        public bool SettingExists(string settingName)
        {
            ThrowExceptionIfLoadingOrSaving();
            return currentSettings.SettingExists(settingName);
        }

        public void RemoveSetting(string settingName)
        {
            ThrowExceptionIfLoadingOrSaving();
            currentSettings.Remove(settingName);
        }

        public Dictionary<string, object>.Enumerator GetSettingsEnumerator()
        {
            ThrowExceptionIfLoadingOrSaving();
            return currentSettings.GetEnumerator();
        }

        private void ThrowExceptionIfLoadingOrSaving()
        {
            if (IsLoading || IsSaving)
                throw new SettingsException(
                "The settingsmanager is currently saving or loading settings. You must wait until it is finished." +
                "You can subscribe to the events of this class to get notified when the operation is finished.", null);
        }
    }
}
