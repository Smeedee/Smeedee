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
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using APD.Framework.Settings;


namespace APD.Client.Web.Services
{
    [ServiceContract(Namespace = "http://agileprojectdashboard.org")]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SettingsService
    {
        private string settingsFileName = "ServerSettings.xml";

        [OperationContract]
        public void SaveSettings(Settings settings)
        {
            string path = GetServerSettingsFilePath();

            string xmlData = settings.Serialize();
            using (var writer = new StreamWriter(path, false))
            {
                writer.Write(xmlData);
            }
        }

        private string GetServerSettingsFilePath()
        {
            string appDirPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string settingsFilePath = System.IO.Path.Combine(appDirPath, settingsFileName);
            return settingsFilePath;
        }

        [OperationContract]
        public Settings GetSettings()
        {
            string settingsFilePath = GetServerSettingsFilePath();

            if (!File.Exists(settingsFilePath))
                return new Settings();

            string xmlData = string.Empty;
            try
            {
                using (var reader = new StreamReader(settingsFilePath))
                {
                    xmlData = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return new Settings();
            }

            return Settings.Deserialize(xmlData);
        }
    }
}