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

using APD.DomainModel.SourceControl;
using APD.Integration.VCS.TFSVC.DomainModel.Repositories;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace APD.IntegrationTests.VCS.TFSVC.DomainModel.Repositories
{
    [TestFixture]
    public class When_pre_optimizing_fields_in_query_models
    {
        protected static QueryModel query;
        protected static TfsVcQueryOptimizer optimizer;

        private readonly Context a_query_model_exists = () => { query = new QueryModel(); };
        private readonly Context a_query_optimizer_exists = () => { optimizer = new TfsVcQueryOptimizer(); };

        [Test]
        public void Assert_optimizer_is_instantiated_with_specification_handler()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_query_optimizer_exists);
                scenario.When("it is instantiated");
                scenario.Then("it's specification handler should not be null.",
                              () => optimizer.Handler.ShouldNotBeNull());
            });
        }


        [Test]
        public void Assert_optimizer_will_change_author()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_query_model_exists).
                    And(a_query_optimizer_exists);
                scenario.When("the query is optimized with username dagolap",
                              () => optimizer.Optimize(new ChangesetsForUserSpecification("dagolap"), query));
                scenario.Then(
                    "the query model should have it's 'author' set to a new Author containing dagolap",
                    () => query.Author.Username.ShouldBe("dagolap"));
            });
        }

        [Test]
        public void Assert_optimizer_will_change_maximum_number_of_changesets_to_retrieve()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_query_model_exists).
                    And(a_query_optimizer_exists);
                scenario.When("the query is optimized with maximum number of revisions to retrieve set to 2",
                              () => optimizer.Optimize(new ChangesetsNotMoreThanNumberSpecification(2), query));
                scenario.Then("the query model should have it's maximum amount set to 2",
                              () => query.MaxRevisions.ShouldBe(2));
            });
        }

        [Test]
        public void Assert_optimizer_will_change_revisionFrom()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_query_model_exists).
                    And(a_query_optimizer_exists);
                scenario.When("the query is optimized with a specification of revision 3 or newer",
                              () => optimizer.Optimize(new ChangesetsAfterRevisionSpecification(3), query));
                scenario.Then("the query model should have it's 'revision from' set to 3",
                              () => query.RevisionFrom.ChangesetId.ShouldBe(3));
            });
        }
    }
}