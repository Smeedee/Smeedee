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
using System.Runtime.Serialization;


namespace Smeedee.DomainModel.SourceControl
{
    [DataContract(IsReference = true, Name = "Changeset", Namespace = "Smeedee.DomainModel.SourceControl")]
    public class Changeset
    {
        [DataMember(Order = 1)]
        public virtual long Revision { get; set; }
        [DataMember(Order = 2)]
        public virtual Author Author { get; set; }
        [DataMember(Order = 3)]
        public virtual DateTime Time { get; set; }
        [DataMember(Order = 4)]
        public virtual string Comment { get; set; }
        [DataMember(Order = 5)]
        public virtual ChangesetServer Server { get; set; }

        public virtual bool IsValidComment()
        {
            return !string.IsNullOrEmpty(Comment);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Changeset;
            return other != null &&
                   other.Revision == this.Revision &&
                   other.Server == this.Server;
        }

        public override int GetHashCode()
        {
            return string.Format("{0}.{1}", Server, Revision).GetHashCode();
        }
    }
}