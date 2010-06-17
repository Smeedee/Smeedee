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
// The project webpage is located at http://www.smeedee.org
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.Users;
using Smeedee.Widget.CI.Controllers;
using Smeedee.Widget.CI.SL.Views;
using Smeedee.Widget.CI.ViewModels;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.CI.SL
{
    [Export(typeof(Slide))]
    public class ContinuousIntegrationModule : Slide
    {
        private CIController _controller;

        public ContinuousIntegrationModule()
        {
            Title = "Build Status";

            var viewModel = new CIViewModel();
            View = new CIView();
            View.DataContext = viewModel;

            var serviceLocator = ServiceLocator.Instance;
            var ciRepository = serviceLocator.GetInstance<IRepository<CIServer>>();
            var timer = serviceLocator.GetInstance<ITimer>();
            var uiInvoker = serviceLocator.GetInstance<IUIInvoker>();
            var userRepository = serviceLocator.GetInstance<IRepository<User>>();
            var asyncClient = serviceLocator.GetInstance<IInvokeBackgroundWorker<IEnumerable<CIProject>>>();
            var logger = serviceLocator.GetInstance<ILog>();

            _controller = new CIController(viewModel, ciRepository, userRepository,asyncClient, timer, uiInvoker, logger );
        }
    }
}