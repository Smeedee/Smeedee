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
using System.ComponentModel.Composition;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.Widget.BurndownChart.Controllers;
using Smeedee.Widget.BurndownChart.SL.Views;
using Smeedee.Widget.BurndownChart.ViewModel;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.BurndownChart.SL
{
    [Export(typeof(Slide))]
    public class BurndownChartModule : Slide
    {
        private BurndownChartController _controller;

        public BurndownChartModule()
        {
            Title = "Burndown Chart";

            var viewModel = new BurndownChartViewModel();
            View = new BurndownChartView();
            View.DataContext = viewModel;

            var serviceLocator = ServiceLocator.Instance;
            var projectRepo = serviceLocator.GetInstance<IRepository<ProjectInfoServer>>();
            var timer = serviceLocator.GetInstance<ITimer>();
            var uiInvoker = serviceLocator.GetInstance<IUIInvoker>();
            var asyncClient = serviceLocator.GetInstance<IInvokeBackgroundWorker<IEnumerable<ProjectInfoServer>>>();

            _controller = new BurndownChartController(viewModel, projectRepo, timer, uiInvoker,asyncClient);
        }
    }
}