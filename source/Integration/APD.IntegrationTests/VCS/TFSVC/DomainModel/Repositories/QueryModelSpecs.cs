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

using APD.Integration.VCS.TFSVC.DomainModel.Repositories;

using Microsoft.TeamFoundation.VersionControl.Client;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace APD.IntegrationTests.VCS.TFSVC.DomainModel.Repositories
{
    [TestFixture]
    public class QueryModelSpecs
    {
        protected static QueryModel qm;

        private readonly Context a_query_model_exists = () => { qm = new QueryModel(); };

        [Test]
        public void Assert_model_has_author_instantiated_to_fresh_author()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_query_model_exists);
                scenario.When("it is instantiated");
                scenario.Then("author should be a fresh author", () => qm.Author.Username.ShouldBe(null));
            });
        }

        [Test]
        public void Assert_model_has_end_revision_instantiated_to_latest()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_query_model_exists);
                scenario.When("it is instantiated");
                scenario.Then("end revision should be latest",
                              () => qm.RevisionTo.ShouldBe(VersionSpec.Latest));
            });
        }

        [Test]
        public void Assert_model_has_maximum_number_of_revisions_instantiated_to_int_max()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_query_model_exists);
                scenario.When("it is instantiated");
                scenario.Then("max amount of revisions should be int.MaxValue",
                              () => qm.MaxRevisions.ShouldBe(int.MaxValue));
            });
        }

        [Test]
        public void Assert_model_has_start_revision_instantiated_to_first()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_query_model_exists);
                scenario.When("it is instantiated");
                scenario.Then("start revision should be 1",
                              () => qm.RevisionFrom.ShouldBe(new ChangesetVersionSpec(1)));
            });
        }

        [Test]
        public void Assert_model_includes_changes()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_query_model_exists);
                scenario.When("it is instantiated");
                scenario.Then("include changes should be true",
                              () => qm.IncludeChanges.ShouldBeTrue());
            });
        }

        [Test]
        public void Assert_models_deletion_id_is_zero()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_query_model_exists);
                scenario.When("it is instantiated");
                scenario.Then("it's deletion id should be 0",
                              () => qm.DeletionId.ShouldBe(0));
            });
        }

        [Test]
        public void Assert_model_is_not_set_to_slot_mode()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_query_model_exists);
                scenario.When("it is instantiated");
                scenario.Then("it should not be set to slot mode",
                              () => qm.SlotMode.ShouldBeFalse());
            });
        }

        [Test]
        public void Assert_recursion_is_set_to_full()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_query_model_exists);
                scenario.When("it is instantiated");
                scenario.Then("it's recursion type should be full",
                              () => qm.RecursionType.ShouldBe(RecursionType.Full));
            });
        }
    }
}