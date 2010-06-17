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
using System.Reflection;

using Smeedee.DomainModel.Framework;

using SharpSvn;
using Smeedee.DomainModel.SourceControl;


namespace Smeedee.Integration.VCS.SVN.DomainModel.Repositories
{
    public abstract class SpecificationHandler
    {
        private SpecificationHandler successor;

        public void Successor(SpecificationHandler handler)
        {
            ThrowArgumentExceptionIfArgIsNull(handler, "handler");

            successor = handler;
        }

        public virtual void Request(SpecificationRequest request)
        {
            ThrowArgumentExceptionIfArgIsNull(request, "request");

            HandleRequest(request);

            if (SuccessorSpecified())
            {
                successor.Request(request);
            }
        }

        public abstract void HandleRequest(SpecificationRequest request);

        private bool SuccessorSpecified()
        {
            return successor != null;
        }

        private void ThrowArgumentExceptionIfArgIsNull(Object handler, string argName)
        {
            if (handler == null)
            {
                throw new ArgumentException("Value can not be null", argName);
            }
        }
    }

    public class RevisionHandler : SpecificationHandler
    {
        // TODO: Improve readability
        public override void HandleRequest(SpecificationRequest request)
        {
            var specification = request.Specification;

            if (specification is AndExpressionSpecification<Changeset>)
            {
                var andSpec = specification as AndExpressionSpecification<Changeset>;
                if (andSpec.Left is ChangesetsAfterRevisionSpecification)
                    specification = andSpec.Left;
                else if (andSpec.Right is ChangesetsAfterRevisionSpecification)
                    specification = andSpec.Right;
            }

            if (specification is ChangesetsAfterRevisionSpecification)
            {
                var revisionSpec = specification as ChangesetsAfterRevisionSpecification;
                request.SvnLogArgs.Range = new SvnRevisionRange(revisionSpec.Revision, long.MaxValue);
            }
        }
    }
}