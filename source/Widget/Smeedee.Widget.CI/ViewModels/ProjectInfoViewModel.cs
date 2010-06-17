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
// The project webpage is located at http://www.smeedee.org
// which contains all the neccessary information.
// </contactinfo>

#endregion

using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Widget.CI.ViewModels
{
    public class ProjectInfoViewModel : AbstractViewModel
    {
        public ProjectInfoViewModel()
        {
            this.LatestBuild = new BuildViewModel();
        }

        private string projectName;
        public string ProjectName
        {
            get { return projectName; }
            set
            {
                if (value != projectName)
                {
                    projectName = value;
                    TriggerPropertyChanged<ProjectInfoViewModel>(vm => vm.ProjectName);
                }
            }
        }

        private bool isSoundEnabled;
        public bool IsSoundEnabled
        {
            get { return isSoundEnabled; }
            set
            {
                if( value != isSoundEnabled )
                {
                    isSoundEnabled = value;
                    TriggerPropertyChanged<ProjectInfoViewModel>(vm => vm.IsSoundEnabled);
                }
            }
        }

        private BuildViewModel latestBuild;
        public BuildViewModel LatestBuild
        {
            get { return latestBuild; }
            set
            {
                if (value != latestBuild)
                {
                    latestBuild = value;
                    TriggerPropertyChanged<ProjectInfoViewModel>(vm => vm.LatestBuild);
                }
            }
        }
    }
}