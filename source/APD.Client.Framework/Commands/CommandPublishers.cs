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
using Microsoft.Practices.Composite.Events;


namespace APD.Client.Framework.Commands
{
    public class DisplayLoadingAnimationCommandPublisher : NoPayloadCommandPublisherBase<DisplayLoadingAnimationCommand>
    {
        public DisplayLoadingAnimationCommandPublisher(IEventAggregator eventAggregator)
            : base(eventAggregator) {}
    }

    public class HideLoadingAnimationCommandPublisher : NoPayloadCommandPublisherBase<HideLoadingAnimationCommand>
    {
        public HideLoadingAnimationCommandPublisher(IEventAggregator eventAggregator)
            : base(eventAggregator) {}
    }


    public class FreezeViewCommandPublisher : NoPayloadCommandPublisherBase<FreezeViewCommand>
    {
        public FreezeViewCommandPublisher(IEventAggregator eventAggregator)
            : base(eventAggregator) {}
    }

    public class UnFreezeViewCommandPublisher : NoPayloadCommandPublisherBase<UnFreezeViewCommand>
    {
        public UnFreezeViewCommandPublisher(IEventAggregator eventAggregator)
            : base(eventAggregator) {}
    }

    public class TogglePauseSlideShowCommandPublisher : NoPayloadCommandPublisherBase<TogglePauseSlideShowCommand>
    {
        public TogglePauseSlideShowCommandPublisher(IEventAggregator eventAggregator)
            : base(eventAggregator) {}
    }

    public class NextSlideCommandPublisher : NoPayloadCommandPublisherBase<NextSlideCommand>
    {
        public NextSlideCommandPublisher(IEventAggregator eventAggregator)
            : base(eventAggregator) {}
    }

    public class PreviousSlideCommandPublisher : NoPayloadCommandPublisherBase<PreviousSlideCommand>
    {
        public PreviousSlideCommandPublisher(IEventAggregator eventAggregator)
            : base(eventAggregator) {}
    }

    public class ToggleAdminModeCommandPublisher : NoPayloadCommandPublisherBase<ToggleAdminModeCommand>
    {
        public ToggleAdminModeCommandPublisher(IEventAggregator eventAggregator)
            : base(eventAggregator) { }
    }

}
