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
using System.ComponentModel;
using APD.Client.Framework.SL;
//using APD.Client.Framework.SL.SettingsService;
using APD.Client.Framework.SL.SettingsService;
//using APD.Client.Silverlight.SettingsService;
using APD.Framework.Settings.Repository;
using SettingsCollection = APD.Client.Framework.Settings;
using System.Threading;
using System.Windows;


namespace APD.Client.Framework.Settings.Repository
{
    public class ServerSettingsRepository : ISettingsRepository
    {


        public event EventHandler<SettingsRepositoryEventArgs> GetSettingsComplete;
        public event EventHandler SetSettingsComplete;

        //private SettingsServiceClient client;

        public ServerSettingsRepository()
        {
            //client = new SettingsServiceClient();
            //client.Endpoint.Address =
            //    WebServiceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            //client.GetSettingsCompleted += GetClientResultAndFireGetCompletedEvent;
            //client.SaveSettingsCompleted += FireSaveSettingsCompletedEvent;
        }

        void GetClientResultAndFireGetCompletedEvent(object sender, GetSettingsCompletedEventArgs e)
        {
            //if( GetSettingsComplete != null  )
            //    GetSettingsComplete(this, new SettingsRepositoryEventArgs(e.Result));
        }

        void FireSaveSettingsCompletedEvent(object sender, AsyncCompletedEventArgs e)
        {
            //if (SetSettingsComplete != null)
            //    SetSettingsComplete(this, EventArgs.Empty);
        }

        public void GetSettingsAsync()
        {
            //client.GetSettingsAsync();
            if (GetSettingsComplete != null)
                GetSettingsComplete(this, new SettingsRepositoryEventArgs(new APD.Framework.Settings.Settings()));
        }

        public void SetSettingsAsync(APD.Framework.Settings.Settings settings)
        {
            if (SetSettingsComplete != null)
                SetSettingsComplete(this, new EventArgs());
        }
    }
}