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
using System.Linq;
using NUnit.Framework;
using System.ComponentModel;
using TinyBDD.Specification.NUnit;
using System.Runtime.Serialization;


namespace APD.Framework.Tests.RegisterMetadataTypeSpecs
{
    public static class ExtensionMethods
    {
        public static List<Attribute> ToList(this AttributeCollection attributes)
        {
            var retValue = new List<Attribute>();

            foreach (Attribute attr in attributes)
                retValue.Add(attr);

            return retValue;
        }

        public static List<PropertyDescriptor> ToList(this PropertyDescriptorCollection properties)
        {
            var retValue = new List<PropertyDescriptor>();

            foreach (PropertyDescriptor prop in properties)
                retValue.Add(prop);

            return retValue;
        }
    }

    [DataContract]
    public class MyCustomClassMetadata
    {
        [DataMember]
        public string Name { get; set; }
    }

    public class MyCustomClass
    {
        public string Name { get; set; }   
    }

    public class Shared
    {
        
    }

    [TestFixture]
    public class When_Metadatatype_is_registered
    {
        [Test]
        public void Assure_class_attributes_are_reflected_from_metadata_type()
        {
            RegisterMetadataType<MyCustomClass, MyCustomClassMetadata>.Register();

            var attributes = TypeDescriptor.GetAttributes(typeof (MyCustomClass)).ToList();

            attributes.Where(a => a is DataContractAttribute).
                Count().ShouldBe(1);
        }

        [Test]
        public void Assure_property_attribtues_are_reflected_from_metadata_type()
        {
            RegisterMetadataType<MyCustomClass, MyCustomClassMetadata>.Register();

            var properties = TypeDescriptor.GetProperties(typeof (MyCustomClass)).ToList();
            var nameProperty = properties.Where(p => p.Name == "Name").SingleOrDefault();
            nameProperty.ShouldNotBeNull();

            var attributes = nameProperty.Attributes.ToList();

            attributes.Where(a => a is DataMemberAttribute).
                Count().ShouldBe(1);
        }
    }

}
