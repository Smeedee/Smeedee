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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System.Collections.ObjectModel;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Widget.SourceControl.ViewModels
{
    public class CheckInNotificationViewModel : AbstractViewModel
    {
        private bool noChangesets;
        public bool NoChangesets
        {
            get { return noChangesets; }
            set
            {
                if (value != noChangesets)
                {
                    noChangesets = value;
                    TriggerPropertyChanged<CheckInNotificationViewModel>(vm => vm.NoChangesets);
                }
            }
        }

        public ObservableCollection<ChangesetViewModel> Changesets { get; private set; }

        public CheckInNotificationViewModel()
        {
            Changesets = new ObservableCollection<ChangesetViewModel>();
        }
    }
}