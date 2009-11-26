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
using System.Linq.Expressions;
using APD.DomainModel.Framework;


namespace APD.DomainModel.SourceControl
{
    public class AllChangesetsSpecification : Specification<Changeset>
    {
        public override bool IsSatisfiedBy(Changeset domainObject)
        {
            return true;
        }

        public override Expression<Func<Changeset, bool>> IsSatisfiedByExpression()
        {
            return ( c => true );
        }

        public override bool Equals(object obj)
        {
            return obj is AllChangesetsSpecification;
        }

        public override int GetHashCode()
        {
            return this.GetType().GetHashCode();
        }

        public static bool operator == (AllChangesetsSpecification a, AllChangesetsSpecification b)
        {
            return a is AllChangesetsSpecification && b is AllChangesetsSpecification;
        }

        public static bool operator != (AllChangesetsSpecification a, AllChangesetsSpecification b)
        {
            if (a is AllChangesetsSpecification && b is AllChangesetsSpecification)
                return false;
            else
                return true;
        }
    }

    public class ChangesetsForUserSpecification : Specification<Changeset>
    {
        public ChangesetsForUserSpecification() {}

        public ChangesetsForUserSpecification(string username)
        {
            Username = username;
        }

        public string Username { get; set; }

        public override Expression<Func<Changeset, bool>> IsSatisfiedByExpression()
        {
            return ( c => c.Author.Username == Username );
        }
    }

    public class ChangesetsAfterRevisionSpecification : Specification<Changeset>
    {
        public ChangesetsAfterRevisionSpecification() {}

        public ChangesetsAfterRevisionSpecification(long revision)
        {
            Revision = revision;
        }

        public long Revision { get; set; }

        public override Expression<Func<Changeset, bool>> IsSatisfiedByExpression()
        {
            return ( c => c.Revision > Revision );
        }

        public override bool Equals(object obj)
        {
            if (obj is ChangesetsAfterRevisionSpecification)
            {
                var spec = obj as ChangesetsAfterRevisionSpecification;
                return Revision == spec.Revision;
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return Revision.GetHashCode();
        }

        public static bool operator == (ChangesetsAfterRevisionSpecification a, ChangesetsAfterRevisionSpecification b)
        {
            if ((object)a == null || (object)b == null)
                return false;

            return a.Revision == b.Revision;
        }

        public static bool operator != (ChangesetsAfterRevisionSpecification a, ChangesetsAfterRevisionSpecification b)
        {
            if ((object)a == null || (object)b == null)
                return true;

            return a.Revision != b.Revision;
        }

        
    }

    public class ChangesetsNotMoreThanNumberSpecification : Specification<Changeset>
    {
        public ChangesetsNotMoreThanNumberSpecification() {}

        public ChangesetsNotMoreThanNumberSpecification(int amount)
        {
            Amount = amount;
        }

        // TODO: State here to be used for post-query optimization if necessary.
        public int AmountDone { get; set; }
        public int Amount { get; set; }

        public override Expression<Func<Changeset, bool>> IsSatisfiedByExpression()
        {
            return ( c => AmountDone < Amount );
        }
    }
}