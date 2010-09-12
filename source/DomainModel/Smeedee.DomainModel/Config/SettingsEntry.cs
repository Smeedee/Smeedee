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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


namespace Smeedee.DomainModel.Config
{
    [DataContract]
    public class SettingsEntry
    {
        [DataMember]
        public virtual string Name { get; set; }
        public virtual string Value
        {
            get
            {
                return Vals == null ? null : Vals.AsEnumerable().FirstOrDefault();
            }
        }

        [DataMember]
        public virtual IEnumerable<string> Vals { get; set; }

        [DataMember]
        public virtual Configuration Configuration { get; set; }

        public virtual bool HasMultipleValues
        {
            get
            {
                return Vals.AsQueryable().Count() > 1;
            }
        }

        public SettingsEntry()
        {
            Name = "";
            Vals = new string[0];
        }

        public SettingsEntry(string name, params string[] values)
        {
            Name = name;
            Vals = values ?? new string[0];
        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode().Equals(obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return string.Format("{0}.{1}", Name, Configuration.Name).GetHashCode();
        }
    }

    public class EntryNotFound : SettingsEntry
    {
        
    }

    public static class SettingsEntryEx
    {
        public static int ValueAsInt(this SettingsEntry settingsEntry)
        {
            try
            {
                return int.Parse(settingsEntry.Value);
            }
            catch (Exception exception)
            {
                throw  new ArgumentException("The value was not convertable to int: " + settingsEntry.Value, exception);
            }
        }
    }
}