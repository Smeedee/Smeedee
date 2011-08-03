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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Smeedee.Widget.DeveloperInfo.SL.Views
{
    public partial class UserInfo : UserControl
    {
        public Brush TextBrush
        {
            get { return (Brush)GetValue(TextBrushProperty); }
            set { SetValue(TextBrushProperty, value); }
        }

        public UserInfo()
        {
            // Required to initialize variables
            InitializeComponent();
            UpdateTextBrush();
        }

        public void UpdateTextBrush()
        {
            var brush = TextBrush;
            if (brush != null)
            {
                Name.Foreground = brush;
            }
            else
            {
                Name.Foreground = (Brush)Application.Current.Resources["FontBrushBright"];
            }
        }

        public static readonly DependencyProperty TextBrushProperty =
            DependencyProperty.Register("TextBrush", typeof(Brush), typeof(UserInfo),
                new PropertyMetadata(null, TextBrushChanged));

        private static void TextBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var userInfo = d as UserInfo;
            if (userInfo == null) return;
            userInfo.UpdateTextBrush();
        }
    }

}