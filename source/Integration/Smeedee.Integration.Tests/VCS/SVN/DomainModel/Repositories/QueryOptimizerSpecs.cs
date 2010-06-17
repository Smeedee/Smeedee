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

using Smeedee.DomainModel.SourceControl;
using Smeedee.Integration.VCS.SVN.DomainModel.Repositories;

using NUnit.Framework;

using SharpSvn;

using TinyBDD.Specification.NUnit;
using Smeedee.DomainModel.Framework;


namespace Smeedee.IntegrationTests.VCS.SVN.DomainModel.Repositories.QueryOptimizerSpecs
{
    [TestFixture]
    public class When_query_changesets_after_a_given_revision
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            queryOptimizer = new SvnQueryOptimizer();
            specification = new ChangesetsAfterRevisionSpecification(10);
        }

        #endregion

        private SvnQueryOptimizer queryOptimizer;
        private Specification<Changeset> specification;

        [Test]
        public void Assure_StartRevision_is_specified_in_range()
        {
            SvnLogArgs queryArgs = queryOptimizer.Optimize(specification);

            queryArgs.Range.StartRevision.Revision.ShouldBe(10);
        }

        [Test]
        public void Assure_StartRevision_is_specified_in_range_when_using_AndExpressionSpec()
        {
            var andExpressionSpec = new AndExpressionSpecification<Changeset>(
                specification, new ChangesetsForUserSpecification("goeran"));

            var queryArgs = queryOptimizer.Optimize(andExpressionSpec);
            queryArgs.Range.StartRevision.Revision.ShouldBe(10);
        }
    }
}