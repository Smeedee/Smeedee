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
using System.Collections.Generic;


namespace APD.DomainModel.Framework
{
    public abstract class RepositoryDecorator<TDomainModel> : IRepository<TDomainModel>
    {
        IRepository<TDomainModel> decoratedObject;
        private bool stop;
        IEnumerable<TDomainModel> repositoryRetValue;
        IEnumerable<TDomainModel> decoratorRetValue;

        public RepositoryDecorator(IRepository<TDomainModel> decoratedObject)
        {
            if (decoratedObject == null)
                throw new ArgumentException("Decorated Repository must be specified");

            this.decoratedObject = decoratedObject;
        }

        #region IRepository<TDomainModel> Members

        public virtual IEnumerable<TDomainModel> Get(Specification<TDomainModel> specification)
        {
            return decoratedObject.Get(specification); 
        }
        #endregion
    }
}