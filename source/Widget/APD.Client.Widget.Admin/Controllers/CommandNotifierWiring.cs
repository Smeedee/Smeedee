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

#endregion

using System;
using APD.Client.Framework.Controllers;
using APD.Client.Widget.Admin.ViewModels;
using APD.Client.Framework;


namespace APD.Client.Widget.Admin.Controllers
{
    public class CommandNotifierAdapter : INotifyWhenToRefresh
    {
        private CommandNotifierWiring<EventArgs> commandWiring;

        public CommandNotifierAdapter(CommandNotifierWiring<EventArgs> commandWiring)
        {
            this.commandWiring = commandWiring;

            this.commandWiring.NewNotification += new EventHandler<EventArgs>(commandWiring_NewNotification);
        }

        void commandWiring_NewNotification(object sender, EventArgs e)
        {
            if (Refresh != null)
                Refresh(this, new RefreshEventArgs());
        }

        #region INotifyWhenToRefresh Members

        public event EventHandler<RefreshEventArgs> Refresh;

        #endregion
    }

    public class CommandNotifierWiring<TNotifyEventArgsType> : ITriggerCommand, INotify<TNotifyEventArgsType> where TNotifyEventArgsType : EventArgs
    {
        public CommandNotifierWiring()
        {

        }

        #region ITriggerCommand Members

        public void Trigger()
        {
            TriggerNewNotification();
        }

        #endregion

        #region INotify<EventArgs> Members

        public event EventHandler<TNotifyEventArgsType> NewNotification;

        #endregion

        private void TriggerNewNotification()
        {
            if (NewNotification != null)
                NewNotification(this, default( TNotifyEventArgsType ));
        }
    }
}