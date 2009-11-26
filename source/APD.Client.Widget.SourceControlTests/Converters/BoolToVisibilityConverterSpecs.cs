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
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using System.Windows;
using APD.Client.Widget.SourceControl.Converters;
using NUnit.Framework;


namespace APD.Client.Widget.SourceControlTests.Converters
{
    [TestFixture]
    public class BoolToVisibilityConverterSpecs
    {
        [Test]
        public void Assure_collapsed_returns_false()
        {
            var bc = new BoolToVisibilityConverter();
            Assert.IsFalse((bool) bc.ConvertBack(Visibility.Collapsed, true.GetType(), null, null));
        }

        [Test]
        public void Assure_false_returns_colapsed()
        {
            var bc = new BoolToVisibilityConverter();
            Assert.IsTrue(Visibility.Collapsed==
                          (Visibility) bc.Convert(false, typeof (Visibility), null, null));
        }

        [Test]
        public void Assure_true_returns_visible()
        {
            var bc = new BoolToVisibilityConverter();
            Assert.IsTrue(Visibility.Visible==(Visibility) bc.Convert(true, typeof (Visibility), null, null));
        }

        [Test]
        public void Assure_visible_returns_true()
        {
            var bc = new BoolToVisibilityConverter();
            Assert.IsTrue((bool) bc.ConvertBack(Visibility.Visible, true.GetType(), null, null));
        }
        
        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void Assure_convert_throws_when_bool_not_supplied()
        {
            var bc = new BoolToVisibilityConverter();
            bc.Convert("foo", typeof (Visibility), null, null);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void Assure_convert_throws_when_visibility_not_supplied()
        {
            var bc = new BoolToVisibilityConverter();
            bc.Convert(true, typeof(string), null, null);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void Assure_convert_throws_when_both_not_supplied()
        {
            var bc = new BoolToVisibilityConverter();
            bc.Convert("lol", typeof(string), null, null);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void Assure_convBack_throws_when_visibility_not_supplied()
        {
            var bc = new BoolToVisibilityConverter();
            bc.ConvertBack("lol", typeof(bool), null, null);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void Assure_convBack_throws_when_bool_not_supplied()
        {
            var bc = new BoolToVisibilityConverter();
            bc.ConvertBack(Visibility.Visible, typeof(string), null, null);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void Assure_convBack_throws_when_both_not_supplied()
        {
            var bc = new BoolToVisibilityConverter();
            bc.ConvertBack("lol", typeof(string), null, null);
        }
    }
}