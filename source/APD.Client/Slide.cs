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
using System.Collections.Generic;
using APD.Client.Framework;


namespace APD.Client
{
    public class Slide
    {
        public String Title { get; set; }
        public TimeSpan DisplayTime { get; set; }
        public Dictionary<SlideRegion, IVisibleModule> RegionMappings { get; set; }

        public Slide(String title, int secondsOnScreen)
        {
            Title = title;
            DisplayTime = new TimeSpan(0, 0, 0, secondsOnScreen);

            RegionMappings = new Dictionary<SlideRegion, IVisibleModule>();
        }

        public IVisibleModule MainContentModule
        {
            get
            {
                if (RegionMappings.ContainsKey(SlideRegion.MainContent))
                {
                    return RegionMappings[SlideRegion.MainContent];
                }
                return null;
            }
            set
            {
                RegionMappings[SlideRegion.MainContent] = value;
            }
        }

        public IVisibleModule BottomLeftModule
        {
            get
            {
                if (RegionMappings.ContainsKey(SlideRegion.BottomLeft))
                {
                    return RegionMappings[SlideRegion.BottomLeft];
                }
                return null;
            }
            set
            {
                RegionMappings[SlideRegion.BottomLeft] = value;
            }
        }

    }

    public enum SlideRegion
    {
        MainContent,
        BottomLeft
    }


}