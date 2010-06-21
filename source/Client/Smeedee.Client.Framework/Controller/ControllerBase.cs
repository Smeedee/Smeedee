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

using System;
using System.ComponentModel;
using System.Threading;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.ViewModel;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Services.Impl;


namespace Smeedee.Client.Framework.Controller
{
    public abstract class ControllerBase<T>
        where T : AbstractViewModel
    {
        public T ViewModel { get; private set; }
        protected ITimer refreshNotifier;
        protected IUIInvoker uiInvoker;

        public ControllerBase(T viewModel, ITimer timer, IUIInvoker uiInvoker)
        {
            ThrowIfNull(timer, "timer");
            ThrowIfNull(uiInvoker, "uiInvoker");
            
            this.refreshNotifier = timer;
            this.uiInvoker = uiInvoker;

            ViewModel = viewModel;
            refreshNotifier.Elapsed += OnNotifiedToRefresh;
        }

        private void ThrowIfNull(object obj, string parameterName)
        {
            if (obj == null)
            {
                throw new ArgumentException("The" + parameterName + " argument cannot be null");
            }
        }

        protected abstract void OnNotifiedToRefresh(object sender, EventArgs e);


        public void Start(int interval)
        {
            refreshNotifier.Start(interval);
        }

        public void Stop()
        {
            refreshNotifier.Stop();
        }
    }
}