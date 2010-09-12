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
using System.ComponentModel;

namespace Smeedee.DomainModel.Framework
{
    [Obsolete("This interface is obsolete in favor of the IDeleteDomainModelsAsync. Please use the new Async interface")]
    public interface IDeleteDomainModels<TDomainModel>
    {
        void Delete(Specification<TDomainModel> specification);
    }

    public interface IDeleteDomainModelsAsync<TDomainModel> : IDeleteDomainModels<TDomainModel>
    {
        event EventHandler<DeleteCompletedEventArgs<TDomainModel>> DeleteCompleted;
    }

    public class DeleteCompletedEventArgs<TDomainModel> : AsyncCompletedEventArgs
    {
        public Specification<TDomainModel> SpecificationUsed { get; private set; }

        public DeleteCompletedEventArgs(Specification<TDomainModel> specificationUsed)
            : this(null, specificationUsed)
        {
            SpecificationUsed = specificationUsed;
        }

        public DeleteCompletedEventArgs(Exception error, Specification<TDomainModel> specificationUsed) 
            : base(error, false, null)
        {
            SpecificationUsed = specificationUsed;
        }
    }
}