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
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;


namespace Smeedee.Framework.Settings
{
    [XmlRoot("Settings")]
    public class Settings
    {

        public int Count
        {
            get { return Names.Count; }
        }

        [XmlElement("Names")]
        public List<string> Names { get; set; }


        [XmlElement("Values")]
        public List<object> Values { get; set; }

        public Settings()
        {
            Names = new List<string>();
            Values = new List<object>();
        }

        public object this[string name]
        {
            get { return Get(name); }
            set { Set(name, value); }
        }

        public virtual object Get(string name)
        {
            int index = Names.IndexOf(name);
            if( index < 0 )
            {
                return null;                
            }
            else
            {
                return Values[index];                
            }
        }

        public virtual void Set(string name, object value)
        {
            if (SettingExists(name))
            {
                int index = Names.IndexOf(name);
                Values[index] = value;
            }
            else
            {
                Names.Add(name);
                Values.Add(value);
            }
        }

        public virtual bool SettingExists(string settingName)
        {
            return Names.Contains(settingName);
        }

        public string Serialize()
        {
            var serializer = new XmlSerializer(typeof(Settings));
            using (var stream = new StringWriter())
            {
                try
                {
                    serializer.Serialize(stream, this);
                    return stream.ToString();
                }
                catch (Exception exception)
                {
                    throw new SettingsException("Could not serialize the settings. See inner exception for details.", exception);
                }
            }
        }

        public static Settings Deserialize(string serializedSettings)
        {
            var serializer = new XmlSerializer(typeof(Settings));
            using (var stream = new StringReader(serializedSettings))
            {
                try
                {
                    return serializer.Deserialize(stream) as Settings;
                }
                catch (Exception exception)
                {
                    throw new SettingsException("Could not deserialize the given input. See inner exception for details.", exception);
                }
            }
        }

        public virtual void Remove(string name)
        {
            if (SettingExists(name))
            {
                int index = Names.IndexOf(name);
                Names.RemoveAt(index);
                Values.RemoveAt(index);
            }
        }

        public virtual Dictionary<string, object>.Enumerator GetEnumerator()
        {
            var dictionary = new Dictionary<string, object>(Names.Count);
            for (int i = 0; i < Names.Count; i++)
            {
                dictionary.Add(Names[i], Values[i]);
            }

            return dictionary.GetEnumerator();
        }
    }
}