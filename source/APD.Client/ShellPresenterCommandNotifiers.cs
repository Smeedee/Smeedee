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

using System.Windows.Controls;
using APD.Client.Framework.Commands;
using Microsoft.Practices.Composite.Events;


namespace APD.Client
{
    public class FreezeViewCommandNotifier : NoPayloadCommandNotifierBase<FreezeViewCommand>
    {
        public FreezeViewCommandNotifier(IEventAggregator eventAggregator)
            : base(eventAggregator) { }
    }

    public class UnFreezeViewCommandNotifier : NoPayloadCommandNotifierBase<UnFreezeViewCommand>
    {
        public UnFreezeViewCommandNotifier(IEventAggregator eventAggregator)
            : base(eventAggregator) { }
    }


    public class TogglePauseSlideShowCommandNotifier : NoPayloadCommandNotifierBase<TogglePauseSlideShowCommand>
    {
        public TogglePauseSlideShowCommandNotifier(IEventAggregator eventAggregator)
            : base(eventAggregator) {}
    }

    public class ToggleAdminModeCommandNotifier : NoPayloadCommandNotifierBase<ToggleAdminModeCommand>
    {
        public ToggleAdminModeCommandNotifier(IEventAggregator eventAggregator)
            : base(eventAggregator) { }
    }

    public class NextSlideCommandNotifier : NoPayloadCommandNotifierBase<NextSlideCommand>
    {
        public NextSlideCommandNotifier(IEventAggregator eventAggregator)
            : base(eventAggregator) {}
    }

    public class PreviousSlideCommandNotifier : NoPayloadCommandNotifierBase<PreviousSlideCommand>
    {
        public PreviousSlideCommandNotifier(IEventAggregator eventAggregator)
            : base(eventAggregator) {}
    }



}
