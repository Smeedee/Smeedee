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
using System.ComponentModel;


namespace APD.Framework
{
    public class RegisterMetadataType<Type, MetadataType>
    {
        public static void Register()
        {
            TypeDescriptor.AddProvider(new MetadataTypeDescriptorProvider<Type, MetadataType>(), typeof(Type));   
        }

    }

    internal class MetadataTypeDescriptorProvider<Type, MetadataType> : TypeDescriptionProvider
    {
        public override System.Type GetReflectionType(System.Type objectType, object instance)
        {
            return base.GetReflectionType(objectType, instance);
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(System.Type objectType, object instance)
        {
            if (objectType == typeof(Type))
                return new MetadataTypeDescriptor<MetadataType>();

            return base.GetTypeDescriptor(objectType, instance);
        }    
    }

    internal class MetadataTypeDescriptor<MetadataType> : ICustomTypeDescriptor
    {
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(typeof (MetadataType));
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(typeof(MetadataType));
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(typeof(MetadataType));
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(typeof(MetadataType));
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(typeof(MetadataType));
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(typeof(MetadataType));
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(typeof(MetadataType), typeof(Object));
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(typeof(MetadataType), attributes);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(typeof(MetadataType));
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(MetadataType), attributes);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(typeof(MetadataType));
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

    }
}
