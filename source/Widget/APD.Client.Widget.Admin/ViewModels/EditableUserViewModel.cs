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
using APD.Client.Framework.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;


namespace APD.Client.Widget.Admin.ViewModels
{
    public class EditableUserViewModel : ItemViewModelWrapper<UserViewModel>
    {
        private WebClient webClient;

        [Required(ErrorMessage = "User name is required")]
        public string Username
        {
            set
            {
                InternalViewModel.Username = value;

                if (Email == null || Email == string.Empty)
                {
                    Email = string.Format("{0}{1}", value,
                        (value == string.Empty) ? string.Empty :
                            value.Contains("@") ? string.Empty : "@");
                }
            }
            get { return InternalViewModel.Username; }
        }

        [Required(ErrorMessage = "First name is required")]
        public string Firstname
        {
            set
            {
                InternalViewModel.Firstname = value;
            }
            get{ return InternalViewModel.Firstname;}
        }

        public string Middlename
        {
            set{ InternalViewModel.Middlename = value; }
            get{ return InternalViewModel.Middlename; }
        }

        public string Surname
        {
            set { InternalViewModel.Surname = value; }
            get { return InternalViewModel.Surname; }
        }

        public string ImageUrl
        {
            set
            {
                CheckIfWellFormedUrl(value);
                
                //Can't do this. Must be a async call
                //CheckIfUrlExists(value);

                InternalViewModel.ImageUrl = value;
            }
            get { return InternalViewModel.ImageUrl; }
        }

        private void CheckIfWellFormedUrl(string url)
        {
            if (!WellFormattedUrl(url))
                throw new ValidationException("Not well formatted URL");
        }

        private bool WellFormattedUrl(string url)
        {
            if (url == null)
                return false;

            var lowerUrl = url.ToLower();            
            
            if (!lowerUrl.StartsWith("http://") &&
                !lowerUrl.StartsWith("https://"))
                return false;

            return true;
        }

        private void CheckIfUrlExists(string url)
        {
            if (webClient == null)
                webClient = new WebClient();

            var resetEvent = new ManualResetEvent(false);
            WebException exception = null;
            
            webClient.DownloadStringCompleted += (o, e) =>
            {
                if (e.Error != null)
                    exception = e.Error as WebException;
                resetEvent.Set();
            };
            webClient.DownloadStringAsync(new Uri(url));
            resetEvent.WaitOne();
            if (exception != null)
                throw new ValidationException("", exception);
            
        }

        [Required(ErrorMessage =  "E-mail is required")]
        public string Email
        {
            set{ InternalViewModel.Email = value;}
            get { return InternalViewModel.Email; }
        }

        public EditableUserViewModel() :
            this(Activator.CreateInstance(typeof(UserViewModel)) as UserViewModel)
        {
        }

        public EditableUserViewModel(UserViewModel viewModel) :
            base(viewModel)
        {

        }
    }
}
