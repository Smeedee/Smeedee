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


namespace APD.DomainModel.Users
{
    [DataContract]
    public class User
    {
        [DataMember]
        public virtual string Username { get; set; }
        
        [DataMember]
        public virtual string Email { get; set; }
        [DataMember]
        public virtual string ImageUrl { get; set; }
        [DataMember]
        public virtual Userdb Database { get; set; }

        [DataMember]
        public virtual string Firstname { get; set; }
        [DataMember]
        public virtual string Middlename { get; set; }
        [DataMember]
        public virtual string Surname { get; set; }

        /// <summary>
        /// TODO: This may not be necessary. This is really a GUI consern, and not a job for the DomainModel
        /// </summary>
        public virtual string Name 
        { 
            get 
            {
                string combinedName = Firstname + 
                                        (Middlename != null ? " " + Middlename : string.Empty) +
                                        (Surname != null ? " " + Surname : string.Empty);
                
                if (string.IsNullOrEmpty(combinedName))
                    return Username;
                else
                    return combinedName;
            } 
        }

        public User()
        {
            Database = new Userdb("default");
        }

        public User(string username) : this()
        {
            Username = username;
        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode().Equals(obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return string.Format("{0}.{1}", Username, Database.Name).GetHashCode();
        }

        public static readonly User defaultUser = new User
        {
            Username = "default",
            Firstname = "Joe Doe",
            Email = "joe@doe.no",
            ImageUrl =
                "UserImages/default_user.jpg"
        };

        public static readonly User systemUser = new User
        {
            Username = "system",
            Firstname = "System user",
            Email = "",
            ImageUrl = "UserImages/system_user.jpg"
        };

        public static readonly User unknownUser = new User
        {
            Username = "unknown",
            Firstname = "Unknown user",
            Email = "",
            ImageUrl = "UserImages/unknown_user.jpg"
        };
    }
}