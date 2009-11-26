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

using System.Collections.Specialized;
using System.ComponentModel;
using APD.Client.Framework.ViewModels;
using APD.Client.Framework;


namespace APD.Client.Widget.CI.ViewModels
{
    public class CINotifierViewModel : AbstractViewModel
    {
        CIViewModel ciViewModel;
        private BuildStatus status;

        public CINotifierViewModel(IInvokeUI uiInvoker) :
            base(uiInvoker){}

        public CINotifierViewModel(CIViewModel ciViewModel, IInvokeUI uiInvoker) :
            base(uiInvoker)
        {
            this.ciViewModel = ciViewModel;

            FindOverallStatus();
            AddStatusListeners();
        }

        public BuildStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                if (value != status)
                {
                    status = value;
                    TriggerPropertyChanged<BuildViewModel>(vm => vm.Status);
                }
            }
        }

        public void ProjectAmountListener(object sender, NotifyCollectionChangedEventArgs e)
        {
            AddStatusListeners();
            FindOverallStatus();
        }

        public void BuildStatusListeners(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Status"))
            {
                FindOverallStatus();
            }
        }

        private void AddStatusListeners()
        {
            ciViewModel.Data.CollectionChanged += new NotifyCollectionChangedEventHandler(ProjectAmountListener);
            
            foreach (var projectInfoViewModel in ciViewModel.Data)
            {
                projectInfoViewModel.PropertyChanged -= new PropertyChangedEventHandler(ProjectInfoListener);
                projectInfoViewModel.PropertyChanged += new PropertyChangedEventHandler(ProjectInfoListener);

                projectInfoViewModel.LatestBuild.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(BuildStatusListeners);
                projectInfoViewModel.LatestBuild.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(BuildStatusListeners);
            }
        }

        public void ProjectInfoListener(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Equals("LatestBuild"))
            {
                FindOverallStatus();
            }
        }

        private void FindOverallStatus() {
            
            bool failed = false;
            bool building = false;
            bool successful = false;

            foreach (var projectInfoViewModel in ciViewModel.Data)
            {
                if(projectInfoViewModel.LatestBuild.Status == BuildStatus.Failed)
                {
                    failed = true;
                }
                if (projectInfoViewModel.LatestBuild.Status == BuildStatus.Building)
                {
                    building = true;
                }
                if (projectInfoViewModel.LatestBuild.Status == BuildStatus.Successful)
                {
                    successful = true;
                }
            }

            SetStatus(failed, building, successful);
        }

        private void SetStatus(bool failed, bool building, bool successful)
        {
            if (failed)
            {
                Status = BuildStatus.Failed;
            }
            else if(building)
            {
                Status = BuildStatus.Building;
            }
            else if(successful)
            {
                Status = BuildStatus.Successful;
            }
            else
            {
                Status = BuildStatus.Unknown;
            }
        }
    }
}
