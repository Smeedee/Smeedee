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
using Smeedee.DomainModel.Framework;
using System.Linq.Expressions;

namespace Smeedee.DomainModel.Users
{
    public class DefaultUserdbSpecification : Specification<Userdb>
    {
        private const string DEFAULT_NAME = "default";

        public override Expression<Func<Userdb, bool>> IsSatisfiedByExpression()
        {
            return ( db => db.Name == "default" );
        }

        public override Userdb AssembleObject()
        {
            Userdb obj = base.AssembleObject();
            obj.Name = DEFAULT_NAME;
            return obj;
        }
    }

    public class UserdbNameSpecification : Specification<Userdb>
    {
        public string Name { get; set; }

        public UserdbNameSpecification()
        {
            
        }

        public UserdbNameSpecification(string name)
        {
            Name = name;
        }

        public override Expression<Func<Userdb, bool>> IsSatisfiedByExpression()
        {
            return ((db) => db.Name == Name);
        }
    }
}