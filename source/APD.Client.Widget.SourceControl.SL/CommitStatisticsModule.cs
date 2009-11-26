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
using APD.Client.Framework.ViewModels;
using APD.Client.Widget.SourceControl.ViewModels;
using APD.Client.Widget.SourceControl.SL.Views;
using APD.Client.Widget.SourceControl.Controllers;
using APD.DomainModel.SourceControl;
using Microsoft.Practices.Unity;


namespace APD.Client.Widget.SourceControl.SL
{
    public class CommitStatisticsModule
        : VisibleModule
              <BindableViewModel<CommitStatisticsForDate>, CommitStatisticsView, CommitStatisticsController>
    {
        private const long REFRESH_INTERVAL = 1000 * 60 * 10; // ms * sec * min

        public CommitStatisticsModule(IUnityContainer globalIocContainer)
            : base(globalIocContainer) {}

        protected override void ConfigureIocContainer()
        {
            localIocContainer.RegisterInstance<INotifyWhenToRefresh>(
                new TimerNotifyRefresh(REFRESH_INTERVAL, true));
            localIocContainer.RegisterType<IInvokeBackgroundWorker<IEnumerable<Changeset>>, AsyncClient<IEnumerable<Changeset>>>();
        }
    }
}