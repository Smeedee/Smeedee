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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;


namespace APD.Client
{
    public class Animation
    {
        // this method will create a new storyboard and start the animation
        public static void PlayAnimation(string element, string property, TimeSpan time, double value)
        {
            // create a new storyboard for the animation and set the targetname and property
            var storyboard = new Storyboard();
            storyboard.SetValue(Storyboard.TargetNameProperty, element);
            storyboard.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath(property));

            // create a DoubleAnimation for the storyboard, set the properties and add it to the storyboard
            var animation = new DoubleAnimation
                            {
                                Duration = time, 
                                To = value
                            };
            storyboard.Children.Add(animation);

            // add the animation to the resources -> required to play the animation
            ( (UserControl) Application.Current.RootVisual ).Resources.Remove("sb");
            ( (UserControl) Application.Current.RootVisual ).Resources.Add("sb", storyboard);

            // finally start the animation
            storyboard.Begin();
        }
    }
}