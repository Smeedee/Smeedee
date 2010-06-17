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
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;

namespace Smeedee.DomainModel.CI
{
    public class AllBuildsSpecification : Specification<Build>
    {
        public override Expression<Func<Build, bool>> IsSatisfiedByExpression()
        {
            return t => true;
        }
    }

    public class NewerThanBuildSpecification : Specification<Build>
    {
        private readonly DateTime startTime;

        public NewerThanBuildSpecification(DateTime startTime)
        {
            this.startTime = startTime;
        }

        public override Expression<Func<Build, bool>> IsSatisfiedByExpression()
        {
            return build => build.StartTime > startTime;
        }
    }

    public class OlderThanBuildSpecification : Specification<Build>
    {
        private readonly DateTime startTime;

        public OlderThanBuildSpecification(DateTime startTime)
        {
            this.startTime = startTime;
        }

        public override Expression<Func<Build, bool>> IsSatisfiedByExpression()
        {
            return build => build.StartTime < startTime;
        }
    }

    public class BuildInProjectSpecification : Specification<Build>
    {
        private readonly CIProject project;

        public BuildInProjectSpecification(CIProject project)
        {
            this.project = project;
        }

        public override Expression<Func<Build, bool>> IsSatisfiedByExpression()
        {
            return build => build.Project.SystemId.Equals(project.SystemId);
        }
    }
}