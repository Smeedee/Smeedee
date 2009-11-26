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
using System.ComponentModel;
using System.Reflection;


namespace APD.Tests
{
    public class PropertyChangedRecorder
    {
        public List<string> ChangedProperties { get; set; }
        public Dictionary<string, List<object>> Changes { get; set; }

        INotifyPropertyChanged observable;

        public PropertyChangedRecorder(INotifyPropertyChanged observable)
        {
            this.observable = observable;

            ChangedProperties = new List<string>();
            Changes = new Dictionary<string, List<object>>();

            this.observable.PropertyChanged += new PropertyChangedEventHandler(observable_PropertyChanged);
        }

        void observable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ChangedProperties.Add(e.PropertyName.ToString());
            
            if (!Changes.ContainsKey(e.PropertyName))
                Changes.Add(e.PropertyName, new List<object>());

            var getVal = CallPropertyGetter(sender, e.PropertyName);
            if (getVal != null)
                Changes[e.PropertyName].Add(getVal);
        }

        private object CallPropertyGetter(object obj, string propertyName)
        {
            var prop = obj.GetType().GetProperty(propertyName,
                                      BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

            try
            {
                return prop.GetValue(obj, new object[] { });
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}