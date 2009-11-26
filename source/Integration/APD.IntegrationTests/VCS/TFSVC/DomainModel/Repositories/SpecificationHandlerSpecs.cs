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


namespace APD.IntegrationTests.VCS.TFSVC.DomainModel.SourceControl.Repositories
{
    [TestFixture]
    public class SpecificationHandlerSpecs
    {
        private static RevisionHandler handler;

        [Test]
        public void Assert_exception_not_thrown_when_calling_request_with_legal_argument_on_revision_handler()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("A RevisionHandler with a valid successor exists", () =>
                {
                    handler = new RevisionHandler();
                    handler.Successor(new RevisionHandler());
                });
                scenario.When("Request is called on it with a legal argument",
                              () =>
                              handler.Request(new SpecificationRequest(new AllChangesetsSpecification(),
                                                                       new QueryModel())));
                scenario.Then("An exception is not thrown");
            });
        }

        [Test]
        public void Assert_exception_not_thrown_when_calling_successor_with_legal_argument_on_revision_handler
            ()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("A RevisionHandler exists", () => handler = new RevisionHandler());
                scenario.When("Request is called on it with a legal argument",
                              () => handler.Successor(new RevisionHandler()));
                scenario.Then("An exception is not thrown");
            });
        }

        [Test]
        [ExpectedException("System.ArgumentException")]
        public void Assert_exception_thrown_when_calling_request_with_null_argument_on_revision_handler()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("A RevisionHandler exists", () => handler = new RevisionHandler());
                scenario.When("Request is called on it with an argument of null", () => handler.Request(null));
                scenario.Then("An exception is thrown");
            });
        }


        [Test]
        [ExpectedException("System.ArgumentException")]
        public void Assert_exception_thrown_when_calling_successor_with_null_argument_on_revision_handler()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("A RevisionHandler exists", () => handler = new RevisionHandler());
                scenario.When("Successor is called with null",
                              () => handler.Successor(null));
                scenario.Then("An exception is thrown");
            });
        }

        [Test]
        public void Assure_can_handle_revision_numbers_less_than_one()
        {
            QueryModel queryModel = null;
            RevisionHandler revisionHandler = null;
            SpecificationRequest specificationRequest = null;

            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("a query model and revision handler exist", () =>
                {
                    queryModel = new QueryModel();
                    revisionHandler = new RevisionHandler();
                    specificationRequest =
                        new SpecificationRequest(new ChangesetsAfterRevisionSpecification(0), queryModel);
                });

                scenario.When("all changesets are requested, including revision zero (domain-specific)",() =>
                {
                    revisionHandler.HandleRequest(specificationRequest);
                });

                scenario.Then("the query model should request changsets from revision 1 and up", () =>
                {
                    queryModel.RevisionFrom.ChangesetId.ShouldBe(1);
                });
            });
        }
    }
}