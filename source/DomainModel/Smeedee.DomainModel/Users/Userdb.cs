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

using System.Collections.Generic;
using System.Linq;

using Smeedee.DomainModel.Framework.Factories;
using System.Runtime.Serialization;


namespace Smeedee.DomainModel.Users
{
    [DataContract(IsReference = true)]
    public class Userdb
    {
        private static GenericFactory<Userdb> factory = new GenericFactory<Userdb>();

        [DataMember]
        public virtual string Name { get; set; }

        private List<User> users;

        [DataMember]
        public virtual IEnumerable<User> Users
        {
            get { return users; } 
            set { users = value.ToList();}
        }

        public Userdb()
        {
            users = new List<User>();
        }

        public Userdb(string name) : this()
        {
            Name = name;
        }

        public static Userdb Default()
        {
            return factory.Assemble(new DefaultUserdbSpecification());    
        }

        public virtual void AddUser(User user)
        {
            user.Database = this;
            users.Add(user);
        }
    }
}