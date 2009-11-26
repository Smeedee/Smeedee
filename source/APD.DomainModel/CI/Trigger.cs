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
using System.Runtime.Serialization;


namespace APD.DomainModel.CI
{
    [KnownType("GetKnownTypes")]
    [DataContract]
    public class Trigger
    {
        [DataMember]
        public virtual string Cause { get;  set; }

        [DataMember]
        public virtual string InvokedBy { get;  set; }

        public static Type[] GetKnownTypes()
        {
            return new []
                   {
                       typeof (UnknownTrigger), 
                       typeof (CodeModifiedTrigger), 
                       typeof (EventTrigger)
                   };
        }

        public override bool Equals(object obj)
        {
            Trigger other = obj as Trigger;
            if (other == null)
                return false;
            else
                return Cause.Equals(other.Cause) && InvokedBy.Equals(other.InvokedBy);
            
        }

        public override int GetHashCode()
        {
            return string.Format("{0}.{1}", Cause, InvokedBy).GetHashCode();
        }
    }
}