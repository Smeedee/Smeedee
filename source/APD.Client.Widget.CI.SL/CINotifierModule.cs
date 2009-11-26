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
using APD.Client.Widget.CI.Controllers;
using APD.Client.Widget.CI.SL.Repository;
using APD.Client.Widget.CI.SL.Views;
using APD.Client.Widget.CI.ViewModels;
using APD.DomainModel.CI;

using Microsoft.Practices.Unity;
using APD.DomainModel.Framework;


namespace APD.Client.Widget.CI.SL
{
    public class CINotifierModule : VisibleModule<CIViewModel, CIView, CIController>
    {
        private const long REFRESH_INTERVAL = 1000 * 10; // ms * sec

        public CINotifierModule(IUnityContainer iocContainer)
            : base(iocContainer) {}

        protected override void ConfigureIocContainer()
        {
            localIocContainer.RegisterInstance<INotifyWhenToRefresh>(new TimerNotifyRefresh(REFRESH_INTERVAL, false));
            localIocContainer.RegisterType<IRepository<CIServer>, CIProjectRepository>();
            localIocContainer.RegisterType
                <IInvokeBackgroundWorker<IEnumerable<CIProject>>, AsyncClient<IEnumerable<CIProject>>>();
            localIocContainer.RegisterInstance<IVisibleModule>(this);
        }

        protected override void AfterInitialize()
        {
            var view = new CINotifierView();
            var viewModel = new CINotifierViewModel(Controller.ViewModel, new NoUIInvocation());
            view.DataContext = viewModel;
            View = view;
        }
    }
}