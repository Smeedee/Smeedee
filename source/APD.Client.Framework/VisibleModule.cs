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

using System;
using System.Windows.Controls;
using APD.Client.Framework.Commands;
using APD.Client.Framework.Controllers;
using APD.Client.Framework.ViewModels;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.Regions;
using Microsoft.Practices.Unity;


namespace APD.Client.Framework
{
    /// <typeparam name="VM">A ViewModel type.</typeparam>
    /// <typeparam name="V">A View type.</typeparam>
    /// <typeparam name="C">A ControllerBase type.</typeparam>
    public abstract class VisibleModule<VM, V, C> : IVisibleModule, IModule
        where VM : AbstractViewModel
        where V : UserControl
        where C : ControllerBase<VM>
    {
        protected readonly IUnityContainer localIocContainer;
        protected readonly IRegionManager regionManager;

        protected DisplayLoadingAnimationCommandPublisher displayLoadingCommandPublisher;
        protected HideLoadingAnimationCommandPublisher hideLoadingCommandPublisher;

        public virtual UserControl View { get; protected set; }
        protected ControllerBase<VM> Controller { get; set; }

        protected VisibleModule(IUnityContainer globalIocContainer)
        {
            localIocContainer = globalIocContainer.CreateChildContainer();
            regionManager = globalIocContainer.Resolve<IRegionManager>();

            var eventAggregator = globalIocContainer.Resolve<IEventAggregator>();

            displayLoadingCommandPublisher = new DisplayLoadingAnimationCommandPublisher(eventAggregator);
            hideLoadingCommandPublisher = new HideLoadingAnimationCommandPublisher(eventAggregator);
        }

        protected abstract void ConfigureIocContainer();

        public virtual void Initialize()
        {
            ConfigureIocContainer();
            SetupViewAndDataContext();
            AfterInitialize();
        }

        protected virtual void AfterInitialize() {}

        protected virtual void SetupViewAndDataContext()
        {
            // Gets an instance of the controller from the IOC-Container
            Controller = (C) localIocContainer.Resolve(typeof (C));

            Controller.ViewModel.PropertyChanged += LoadingAnimationHandler;

            // Dynamicly invoke the View's constructor 
            View = (UserControl) Activator.CreateInstance(typeof (V));
            View.Width = float.NaN; //Width=Auto
            View.Height = float.NaN; //Height=Auto
            View.DataContext = Controller.ViewModel;
        }

        private void LoadingAnimationHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsLoading"))
            {
                if (Controller.ViewModel.IsLoading)
                {
                    displayLoadingCommandPublisher.Notify();
                }
                else
                {
                    hideLoadingCommandPublisher.Notify();
                }
            }
        }
    }
}