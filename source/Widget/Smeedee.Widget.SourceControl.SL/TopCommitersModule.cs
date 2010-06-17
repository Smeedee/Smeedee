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

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Smeedee.Client.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.Users;
using Smeedee.Widget.SourceControl.Controllers;
using Smeedee.Widget.SourceControl.SL.Views;
using Smeedee.Widget.SourceControl.ViewModels;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;


namespace Smeedee.Widget.SourceControl.SL
{
    [Export(typeof(Slide))]
    public class TopCommitersModule : Slide
    {
        private TopCommitersController _controller;
        private const long REFRESH_INTERVAL = 1000 * 60 * 2; // ms * sec * min

        public TopCommitersModule()
        {
            Title = "Top commiters";

            var viewModel = new BindableViewModel<CodeCommiterViewModel>();
            View = new TopCommiters()
                       {
                           DataContext = viewModel
                       };

            var serviceLocator = ServiceLocator.Instance;
            var changesetRepository = serviceLocator.GetInstance<IRepository<Changeset>>();
            var timer = serviceLocator.GetInstance<ITimer>();
            var uiInvoker = serviceLocator.GetInstance<IUIInvoker>();
            var userRepository = serviceLocator.GetInstance<IRepository<User>>();
            var asyncClient = serviceLocator.GetInstance<IInvokeBackgroundWorker<IEnumerable<Changeset>>>();
            var configRepostory = serviceLocator.GetInstance<IRepository<Configuration>>();
            var configPersister = serviceLocator.GetInstance<IPersistDomainModels<Configuration>>();
            var logger = serviceLocator.GetInstance<ILog>();

            _controller = new TopCommitersController(viewModel, changesetRepository, asyncClient, timer, uiInvoker, configRepostory, configPersister, userRepository, logger);
        }
    }
}