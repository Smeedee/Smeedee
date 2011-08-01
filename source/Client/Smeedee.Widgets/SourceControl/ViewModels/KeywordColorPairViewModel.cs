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

using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Widget.SourceControl.ViewModels
{
    public class KeywordColorPairViewModel : AbstractViewModel
    {
        public delegate void KeywordEvent(KeywordColorPairViewModel sender);

        public KeywordEvent KeywordChanged { get; set; }

        private string keyword;
        public string Keyword
        {
            get { return keyword; }
            set
            {
                if (value != keyword)
                {
                    keyword = value;
                    TriggerPropertyChanged<KeywordColorPairViewModel>(t => t.Keyword);
                    if (KeywordChanged != null) KeywordChanged(this);
                }
            }
        }

        private string colorName;
        public string ColorName
        {
            get { return colorName; }
            set
            {
                if (value != colorName)
                {
                    colorName = value;
                    TriggerPropertyChanged<KeywordColorPairViewModel>(t => t.ColorName);
                }
            }
        }

        public KeywordColorPairViewModel Clone()
        {
            return MemberwiseClone() as KeywordColorPairViewModel;
        }
    }
}
