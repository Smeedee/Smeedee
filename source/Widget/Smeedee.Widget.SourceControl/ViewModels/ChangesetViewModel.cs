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
using System.Collections.Generic;
using System.Linq;
using Smeedee.Client.Framework.ViewModel;


namespace Smeedee.Widget.SourceControl.ViewModels
{
    public class ChangesetViewModel : AbstractViewModel
    {
        public const string DEFAULT_BACKGROUND_COLOR = "GreyGradientBrush";

        public ChangesetViewModel()
        {
            developer = new Person();

            BackgroundColor = DEFAULT_BACKGROUND_COLOR;
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

        private String _backgroundColor;
        public string BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {

                if (value != _backgroundColor)
                {
                    _backgroundColor = value;
                    TriggerPropertyChanged<ChangesetViewModel>(vm => vm.BackgroundColor);
                }
            }
        }

    }

    public static class ChangesetBackgroundProvider
    {
        private static Dictionary<string, string> brushes = new Dictionary<string, string>
        {   
            {"grey", "DarkGreyGradientBrush"},
            {"darkGrey", "GreyGradientBrush"},
            {"lightGrey", "LightGreyGradientBrush"},

            {"lightBrown", "LightBrownGradientBrush"},
            {"brown", "BrownGradientBrush"},
            {"darkBrown", "DarkBrownGradientBrush"},

            {"red", "RedGradientBrush"},
            {"orange", "OrangeGradientBrush"},
            {"yellow", "YellowGradientBrush"},

            {"lightGreen", "LightGreenGradientBrush"},
            {"green", "MediumGreenGradientBrush"},
            {"darkGreen", "GreenGradientBrush"},

            {"lightBlue", "LightBlueGradientBrush"},
            {"blue", "BlueGradientBrush"},
            {"darkBlue", "DarkBlueGradientBrush"},
          
            {"pink", "PinkGradientBrush"},
            {"purple", "PurpleGradientBrush"},
            {"darkPurple", "DarkPurpleGradientBrush"}
          
        };

        public static string GetBrushName(string color)
        {
            return brushes.ContainsKey(color) ? brushes[color] : ChangesetViewModel.DEFAULT_BACKGROUND_COLOR;
        }

        public static string[] GetColors()
        {
            return brushes.Keys.ToArray();
        }
    }   
}
