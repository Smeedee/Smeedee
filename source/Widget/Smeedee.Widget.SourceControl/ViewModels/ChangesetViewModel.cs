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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using Smeedee.Client.Framework.ViewModel;


namespace Smeedee.Widget.SourceControl.ViewModels
{
    public class ChangesetViewModel : AbstractViewModel
    {
        public ChangesetViewModel()
        {
            developer = new Person();
            LightBackgroundColor = "#FF838383";
            DarkBackgroundColor = "#FF505050";
        }

        public bool ShouldBlink { get; set; }

        private Person developer;
        public Person Developer
        {
            get { return developer; }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                if (value != message)
                {
                    message = value;
                    TriggerPropertyChanged<ChangesetViewModel>(vm => vm.Message);
                }
            }
        }

        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set
            {
                if (value != date)
                {
                    date = value;
                    TriggerPropertyChanged<ChangesetViewModel>(vm => vm.Date);
                }
            }
        }

        private bool commentIsBad;
        public bool CommentIsBad
        {
            get{ return commentIsBad;}
            set
            {
                if(value!=commentIsBad)
                {
                    commentIsBad = value;
                    TriggerPropertyChanged<ChangesetViewModel>(vm => vm.CommentIsBad);
                }
            }
        }

        private long revision;
        public long Revision
        {
            get { return revision; }
            set {
                if (value != revision)
                {
                    revision = value;
                    TriggerPropertyChanged<ChangesetViewModel>(vm => vm.Revision);
                }
            }
        }

        private String _darkBackgroundColor;
        public String DarkBackgroundColor
        {
            get { return _darkBackgroundColor;  }
            set { 
               
                if (value != _darkBackgroundColor)
                {
                    _darkBackgroundColor = value;
                    TriggerPropertyChanged<ChangesetViewModel>(vm => vm.DarkBackgroundColor);
                }
            }
        }

        private String _lightBackgroundColor;
        public string LightBackgroundColor
        {
            get { return _lightBackgroundColor; }
            set
            {

                if (value != _lightBackgroundColor)
                {
                    _lightBackgroundColor = value;
                    TriggerPropertyChanged<ChangesetViewModel>(vm => vm.LightBackgroundColor);
                }
            }
        }
    }
}
