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
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml;


namespace Smeedee.Client.Web.Serialization
{
public class UseReferenceTrackingSerializerAttribute : Attribute, IOperationBehavior
{
    public void AddBindingParameters(OperationDescription description, BindingParameterCollection parameters)
    {
    } 

    public void ApplyClientBehavior(OperationDescription description, ClientOperation proxy)
    {
        ReplaceDataContractSerializerOperationBehavior(description);
    } 

    public void ApplyDispatchBehavior(OperationDescription description, DispatchOperation dispatch)
    {
        ReplaceDataContractSerializerOperationBehavior(description);
    }
    public void Validate(OperationDescription description)
    {
    } 

    private static void ReplaceDataContractSerializerOperationBehavior (OperationDescription description)
    {
        DataContractSerializerOperationBehavior dcs = description.Behaviors.Find<DataContractSerializerOperationBehavior>(); 

        if (dcs != null)
            description.Behaviors.Remove(dcs); 

        description.Behaviors.Add(new ReferenceTrackingSerializerOperationBehavior(description));
    } 

    public class ReferenceTrackingSerializerOperationBehavior : DataContractSerializerOperationBehavior
    {

        public ReferenceTrackingSerializerOperationBehavior(OperationDescription operationDescription) : base(operationDescription) { } 

        public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
        {
            return new DataContractSerializer(type, name, ns, knownTypes, int.MaxValue, true, true, null);
        } 

        public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
        {
            return new DataContractSerializer(type,name, ns, knownTypes, int.MaxValue,true, true, null);
        }
    }
}


}
