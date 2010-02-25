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

using System.Collections.Generic;
using APD.Client.Framework;
using APD.Client.Widget.BurndownChart.SL.Repositories;
using APD.Client.Widget.ProjectInfo.Controllers;
using APD.Client.Widget.ProjectInfo.ViewModels;
using APD.DomainModel.Framework;
using APD.DomainModel.ProjectInfo;
using Microsoft.Practices.Unity;
using APD.DomainModel.Config;


namespace APD.Client.Widget.ProjectInfo.SL
{
    public class WorkingDaysLeftModule
        : VisibleModule<WorkingDaysLeftViewModel, WorkingDaysLeft, WorkingDaysLeftController>
    {
        private const long REFRESH_INTERVAL = 1000 * 60 * 20; // ms * sec * min

        public WorkingDaysLeftModule(IUnityContainer iocContainer)
            : base(iocContainer) {}

        protected override void ConfigureIocContainer()
        {
            localIocContainer.RegisterType<IRepository<ProjectInfoServer>, ProjectInfoRepository>();
            localIocContainer.RegisterType<IInvokeBackgroundWorker<IEnumerable<ProjectInfoServer>>, AsyncClient<IEnumerable<ProjectInfoServer>>>();
            localIocContainer.RegisterInstance<INotifyWhenToRefresh>(new TimerNotifyRefresh(REFRESH_INTERVAL, false));
            localIocContainer.RegisterInstance<IVisibleModule>(this);
        }
    }
}