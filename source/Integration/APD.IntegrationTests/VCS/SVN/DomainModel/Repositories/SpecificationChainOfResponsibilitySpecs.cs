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

using APD.DomainModel.SourceControl;
using APD.Integration.VCS.SVN.DomainModel.Repositories;

using Moq;

using NUnit.Framework;

using SharpSvn;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace APD.IntegrationTests.VCS.SVN.DomainModel.Repositories.SpecificationChainOfResponsibilitySpecss
{
    public class Shared
    {
        protected static SpecificationHandler handler;
        protected static SpecificationRequest request;
        protected static Mock<SpecificationHandler> successorMock;

        protected Context handler_is_created = () => { handler = new RevisionHandler(); };

        protected Context request_is_created =
            () => { request = new SpecificationRequest(new AllChangesetsSpecification(), new SvnLogArgs()); };

        protected Context successor_is_specified = () =>
        {
            successorMock = new Mock<SpecificationHandler>();
            handler.Successor(successorMock.Object);
        };
    }

    [TestFixture]
    public class When_specifying_successor : Shared
    {
        [Test]
        public void Assure_ArgumentException_is_thrown_if_Handler_is_Null()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(handler_is_created).
                    And("successor is null");

                scenario.When("successor is specified");

                scenario.Then("assure ArgumentException is thrown", () =>
                                                                    this.ShouldThrowException
                                                                        <ArgumentException>(() =>
                                                                                            handler.Successor(
                                                                                                null),
                                                                                            exception =>
                                                                                            exception.Message.
                                                                                                ShouldBe(
                                                                                                "Value can not be null\r\nParameter name: handler")));
            });
        }
    }

    [TestFixture]
    public class When_making_request : Shared
    {
        [Test]
        public void Assure_ArgumentException_is_thrown_if_request_is_null()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(handler_is_created).
                    And("request is null");

                scenario.When("making request");

                scenario.Then("assure ArgumentException is thrown", () =>
                                                                    this.ShouldThrowException
                                                                        <ArgumentException>(() =>
                                                                                            handler.Request(
                                                                                                null),
                                                                                            exception =>
                                                                                            exception.Message.
                                                                                                ShouldBe(
                                                                                                "Value can not be null\r\nParameter name: request")));
            });
        }
    }

    [TestFixture]
    public class When_making_request_and_successor_is_specified : Shared
    {
        [Test]
        public void Assure_request_is_forwarded_to_successor()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(handler_is_created).
                    And(successor_is_specified).
                    And(request_is_created);

                scenario.When("making request", () =>
                                                handler.Request(request));

                scenario.Then("assure request is forwarded to successor", () =>
                                                                          successorMock.Verify(
                                                                              h =>
                                                                              h.Request(
                                                                                  It.IsAny
                                                                                      <SpecificationRequest>()),
                                                                              Times.Once()));
            });
        }
    }
}