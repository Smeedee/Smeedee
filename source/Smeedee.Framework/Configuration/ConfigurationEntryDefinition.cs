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


namespace Smeedee.Framework.Configuration
{
    public class ConfigurationEntryDefinition
    {
        public string Type { get; set; }
        public object DefaultValue { get; set; }
        public string Name { get; set; }

        public ConfigurationEntryDefinition(){}

        public ConfigurationEntryDefinition(string name, Type type, object defaultValue)
        {
            ThrowIfNull(name, "The name cannot be null");
            ThrowIfNull(type, "The type cannot be null");

            ThrowIfNull(defaultValue, "The default value cannot be null.");


            Name = name;
            DefaultValue = defaultValue;
            Type = type.FullName;

            if( ! IsValidConfigurationValue(defaultValue) )
                throw new ArgumentException("The default value must be the same type as the configuration entry type.");
        }

        private void ThrowIfNull(object objectToCheckForNull, string message)
        {
            if (objectToCheckForNull == null)
                throw new ArgumentException(message);
        }

        public bool IsValidConfigurationValue(object value)
        {
            if( value == null )
                return false;

            bool typesMatch = value.GetType().FullName == this.Type;
            return typesMatch;
        }

        public bool isValidEntry(ConfigurationEntry entry)
        {
            bool isValueValid = IsValidConfigurationValue(entry.Value);
            bool areNamesMatching = entry.Name == this.Name;

            return isValueValid && areNamesMatching;
        }
    }
}
