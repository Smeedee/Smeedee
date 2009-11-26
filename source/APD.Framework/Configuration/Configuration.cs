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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using APD.Framework.Settings;


namespace APD.Framework.Configuration
{
    public class Configuration : IEnumerable<ConfigurationEntry>
    {
        public string Owner { get; set; }
        List<ConfigurationEntry> entries = new List<ConfigurationEntry>();

        public void Add(ConfigurationEntry entry)
        {
            entries.Add(entry);
        }

        public bool EntryExists(string name)
        {
            return entries.Any(e => e.Name == name);
        }

        public object ReadEntryValue(string name)
        {
            try
            {
                return entries.Single(e => e.Name == name).Value;
            }
            catch(Exception exception)
            {
                throw new SettingsException("A configuration entry with this name does not exist", exception);
            }
        }

        #region Implementation of IEnumerable

        public IEnumerator<ConfigurationEntry> GetEnumerator()
        {
            return entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
