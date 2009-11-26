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
using System.Collections;
using System.IO.IsolatedStorage;
using System.Threading;
using APD.Framework.Settings.Repository;


namespace APD.Client.Framework.Settings.Repository
{
    public class IsolatedStorageSettingsRepository : ISettingsRepository
    {
        #region Implementation of ISettingsRepository

        public event EventHandler<SettingsRepositoryEventArgs> GetSettingsComplete;
        public event EventHandler SetSettingsComplete;

        public void GetSettingsAsync()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                var settings = BuildSettingsFromDictionary();
                FireGetCompletedEvent(settings);
            });
        }

        private APD.Framework.Settings.Settings BuildSettingsFromDictionary()
        {
            var settings = new APD.Framework.Settings.Settings();
            
            try
            {
                ICollection keys = IsolatedStorageSettings.SiteSettings.Keys;
                foreach (var k in keys)
                {
                    string key = k.ToString();
                    object value = IsolatedStorageSettings.SiteSettings[key];
                    settings.Set(key, value);
                }
            }
            catch (IsolatedStorageException)
            {
                // TODO: Report that isolated storage is not available?
            }
            
            return settings;
        }

        private void FireGetCompletedEvent(APD.Framework.Settings.Settings settings)
        {
            if (GetSettingsComplete != null)
                GetSettingsComplete(this, new SettingsRepositoryEventArgs(settings) );
        }


        public void SetSettingsAsync(APD.Framework.Settings.Settings settings)
        {            
            ThreadPool.QueueUserWorkItem((o) =>
            {
                ReplaceCurrentSettings(settings);
                FireSaveCompletedEvent();
            });
        }

        private void ReplaceCurrentSettings(APD.Framework.Settings.Settings settings)
        {
            IsolatedStorageSettings.SiteSettings.Clear();
            foreach (var setting in settings)
            {
                IsolatedStorageSettings.SiteSettings.Add(setting.Key, setting.Value);
            }
            IsolatedStorageSettings.SiteSettings.Save();
        }

        private void FireSaveCompletedEvent()
        {
            if (SetSettingsComplete != null)
                SetSettingsComplete(this, EventArgs.Empty);
        }

        #endregion
    }
}