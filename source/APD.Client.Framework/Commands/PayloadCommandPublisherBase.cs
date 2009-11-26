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

using Microsoft.Practices.Composite.Events;


namespace APD.Client.Framework.Commands
{
    public abstract class CommandPublisher
    {
        protected readonly IEventAggregator eventAggregator;

        public CommandPublisher(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }
    }


    public abstract class PayloadCommandPublisherBase<TC, TP> : CommandPublisher
        where TC : PayloadCommandBase<TP>
    {
        private TC command;

        public PayloadCommandPublisherBase(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
            command = eventAggregator.GetEvent<TC>();
        }

        public virtual void Notify(TP payload)
        {
            if (command != null)
                command.Publish(payload);
        }
    }
}
