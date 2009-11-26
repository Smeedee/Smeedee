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
using Microsoft.Practices.Composite.Events;


namespace APD.Client.Framework.Commands
{
    public class CommandPublishedEventArgs<TP> : EventArgs
    {
        public TP Payload { get; set; }
        public CommandPublishedEventArgs(TP payload)
        {
            Payload = payload;
        }
    }

    public abstract class PayloadCommandNotifierBase<TC, TP>
        where TC : PayloadCommandBase<TP>
    {
        private readonly IEventAggregator eventAggregator;
        public event EventHandler<CommandPublishedEventArgs<TP>> CommandPublished;

        protected PayloadCommandNotifierBase(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            var payloadCommand = eventAggregator.GetEvent<TC>();

            if (payloadCommand != null)
                payloadCommand.Subscribe(new Action<TP>(TriggerCommandPublished), true);
        }

        public void TriggerCommandPublished(TP payLoad)
        {
            if (CommandPublished != null)
            {
                CommandPublished(this, new CommandPublishedEventArgs<TP>(payLoad));
            }
        }
    }


}
