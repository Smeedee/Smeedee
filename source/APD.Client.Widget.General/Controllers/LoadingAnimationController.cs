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
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using APD.Client.Widget.General.ViewModels;


namespace APD.Client.Widget.General.Controllers
{
    public class LoadingAnimationController
    {
        public LoadingAnimationViewModel ViewModel { get; private set; }

        public LoadingAnimationController(  LoadingAnimationViewModel viewModel,
                                            DisplayLoadingAnimationNotifier showLoadingAnimationNotifier,
                                            HideLoadingAnimationNotifier hideLoadingAnimationNotifier)
        {
            ThrowArgumentExceptionIfArgIsNull(viewModel, "viewModel");
            ThrowArgumentExceptionIfArgIsNull(showLoadingAnimationNotifier, "showLoadingAnimationNotifier");
            ThrowArgumentExceptionIfArgIsNull(hideLoadingAnimationNotifier, "hideLoadingAnimationNotifier");

            this.ViewModel = viewModel;

            showLoadingAnimationNotifier.CommandPublished += new EventHandler(displayCmdNotifier_Display);
            hideLoadingAnimationNotifier.CommandPublished += new EventHandler(displayCmdNotifier_Hide);
        }

        private void ThrowArgumentExceptionIfArgIsNull(Object obj, string argName)
        {
            if (obj == null)
                throw new ArgumentException("Value can't be null", argName);
        }

        void displayCmdNotifier_Hide(object sender, EventArgs e)
        {
            ViewModel.Display = false;
        }

        void displayCmdNotifier_Display(object sender, EventArgs e)
        {
            ViewModel.Display = true;
        }
    }
}