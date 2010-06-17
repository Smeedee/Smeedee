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
// The project webpage is located at http://www.smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.Holidays;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.Widget.ProjectInfo.Controllers;
using Smeedee.Widget.ProjectInfo.SL.Views;
using Smeedee.Widget.ProjectInfo.ViewModels;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.ProjectInfo.SL
{
    [Export(typeof(Slide))]
    public class WorkingDaysLeftModule : Slide

    {
        private const long REFRESH_INTERVAL = 1000 * 60 * 20; // ms * sec * min
        private WorkingDaysLeftController _controller;

        public WorkingDaysLeftModule()
        {
            Title = "Working days left";

            var viewModel = new WorkingDaysLeftViewModel();
            View = new WorkingDaysLeft();
            View.DataContext = viewModel;

            var serviceLocator = ServiceLocator.Instance;
            var projectInfoRepo = serviceLocator.GetInstance<IRepository<ProjectInfoServer>>();
            var configRepo = serviceLocator.GetInstance<IRepository<Configuration>>();
            var configPersister = serviceLocator.GetInstance<IPersistDomainModels<Configuration>>();
            var holidayRepo = serviceLocator.GetInstance<IRepository<Holiday>>();
            var timer = serviceLocator.GetInstance<ITimer>();
            var uiInvoker = serviceLocator.GetInstance<IUIInvoker>();
            var asyncClient = serviceLocator.GetInstance<IInvokeBackgroundWorker<IEnumerable<ProjectInfoServer>>>();
            var logger = serviceLocator.GetInstance<ILog>();

            _controller = new WorkingDaysLeftController(viewModel, projectInfoRepo, configRepo, holidayRepo,configPersister, timer, uiInvoker, asyncClient, logger);
        }

    }
}