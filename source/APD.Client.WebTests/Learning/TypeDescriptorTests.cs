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
using System.Runtime.Serialization;
using TinyBDD.Specification.NUnit;


namespace APD.Client.WebTests.Learning
{
    public class CustomProvider : TypeDescriptionProvider
    {
        public CustomProvider()
        {
            
        }

        public override Type GetReflectionType(Type objectType, object instance)
        {
            return base.GetReflectionType(objectType, instance);
        }


        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            if (objectType == typeof(User))
                return new UserTypeDescriptor();
        
            return base.GetTypeDescriptor(objectType, instance);
        }
    }

    public class UserTypeDescriptor : ICustomTypeDescriptor
    {
        private Type userType;

        public UserTypeDescriptor()
        {
            userType = typeof (User);
        }

        public AttributeCollection GetAttributes()
        {
            var newAttributes = new AttributeCollection(new DataContractAttribute());
          
            return newAttributes;
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(typeof(User));
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(typeof (User));
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(typeof (User));
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(typeof (User));
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(typeof (User));
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(typeof (User), typeof (Object));
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(typeof (User), attributes);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(typeof (User));
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof (User), attributes);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(typeof (User));
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

    }

    public class User
    {
        public string Name { get; set; }
    }

    [TestFixture]
    public class TypeDescriptorTests
    {
        [Test]
        public void Verify_that_DataContractSerializer_uses_TypeDescriptor_to_reflect_on_a_types_metadata()
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(User));
         }

        [Test]
        public void How_to_hook_into_CLR_type_system()
        {
            TypeDescriptor.AddProvider(new CustomProvider(), typeof(User));
            
            var user = new User();
            var userType = user.GetType();

            var attributes = new List<Attribute>();
            foreach (Attribute attr in TypeDescriptor.GetAttributes(typeof (User)))
            {
                attributes.Add(attr);
            }

            //var dataContractAttr = userType.GetCustomAttributes(typeof(DataContractAttribute), true).ToList().SingleOrDefault();
            var dataContractAttr = attributes.Where(a => a.GetType() == typeof(DataContractAttribute)).SingleOrDefault();
            dataContractAttr.ShouldNotBeNull();
            dataContractAttr.ShouldBeInstanceOfType<DataContractAttribute>();
        }
    }
}
