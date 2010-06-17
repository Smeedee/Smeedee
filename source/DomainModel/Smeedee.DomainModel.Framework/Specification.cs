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
using System.Linq.Expressions;


namespace Smeedee.DomainModel.Framework
{
    public abstract class Specification<TDomainModel>
    {
        public virtual bool IsSatisfiedBy(TDomainModel domainObject)
        {
            return IsSatisfiedByExpression().Compile().Invoke(domainObject);
        }

        public virtual TDomainModel AssembleObject()
        {
            return Activator.CreateInstance<TDomainModel>();
        }

        public abstract Expression<Func<TDomainModel, bool>> IsSatisfiedByExpression();
    }

    public class AllSpecification<TDomainModel> : Specification<TDomainModel>
    {
        public override Expression<Func<TDomainModel, bool>> IsSatisfiedByExpression()
        {
            return t => true;
        }

        public override bool Equals(object other)
        {
            var otherSpec = other as AllSpecification<TDomainModel>;
            return otherSpec != null;
        }
    }

    public class LinqSpecification<TDomainModel> : Specification<TDomainModel>
    {
        private Expression<Func<TDomainModel, bool>> linqSpecification;

        public LinqSpecification(Expression<Func<TDomainModel, bool>> linqSpecification)
        {
            this.linqSpecification = linqSpecification;
        }

        public override Expression<Func<TDomainModel, bool>> IsSatisfiedByExpression()
        {
            return linqSpecification;
        }
    }
}