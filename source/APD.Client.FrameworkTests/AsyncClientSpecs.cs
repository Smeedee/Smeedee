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
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using System.Collections.Generic;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.Client.Framework;
using System.Threading;


namespace APD.Client.FrameworkTests.AsyncSpecs
{
    public class Shared
    {
        protected static AsyncClient<string> client;

        protected Context client_is_created = () =>
        {
            client = new AsyncClient<string>();
        };

        protected When run_async_void = () =>
        {
        };

        protected When run_async = () =>
        {
        };

        protected string FourSecondsMethod()
        {
            Thread.Sleep(4000);

            return "goeran";
        }


    }

    [TestFixture] 
    public class When_RunAsync : Shared
    {
        [Test]
        public void Assure_Action_is_executed_on_a_background_thread()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(client_is_created);

                scenario.When(run_async);

                scenario.Then("assure it's executed on a background thread", () =>
                {
                    var start = DateTime.Now;
                    int thisThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                    int requestThreadId = -1;
                    client.RunAsync(() =>
                    {
                        requestThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                        return FourSecondsMethod();
                    });
                    var end = DateTime.Now;
                    var span = end.Subtract(start).TotalMilliseconds;
                    span.ShouldBeLessThan(500);
                    thisThreadId.ShouldNotBe(requestThreadId);
                });
            });
        }

        [Test]
        public void Assure_Lambda_Action_is_invoked()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(client_is_created);

                scenario.When(run_async);

                scenario.Then("assure Lambda action is invoked", () =>
                {
                    var invoked = false;
                    client.RunAsync(() => 
                    {
                        invoked = true;
                        return "haldis";
                    });
                    Thread.Sleep(50);
                    invoked.ShouldBeTrue();
                });
            });
        }

        [Test]
        public void Assure_Delegate_Action_is_invoked()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(client_is_created);

                scenario.When(run_async_void);

                scenario.Then("assure delegate action is invoked", () =>
                {
                    var manualResetEvent = new ManualResetEvent(false);
                    var executed = false;

                    client.RunAsyncCompleted += (o, e) =>
                    {
                        executed = true;
                        manualResetEvent.Set();
                    };

                    client.RunAsync(ResponseMethod);

                    manualResetEvent.Reset();
                    manualResetEvent.WaitOne(1000);
                });
            });
        
        }

        [Test]
        public void Assure_RunAsyncCompleted_is_triggered()
        {
            Scenario.StartNew(this, scenario =>
            {
                var resetEvent = new ManualResetEvent(false);

                scenario.Given(client_is_created);

                scenario.When(run_async);

                scenario.Then("assure RunAsyncCompleted is triggered", () =>
                {
                    string returnValue = null;

                    client.RunAsyncCompleted += (o, e) =>
                    {
                        returnValue = e.ReturnValue;
                        resetEvent.Set();
                    };

                    client.RunAsync(() =>
                    {
                        Thread.Sleep(1000);
                        return "haldis";
                    });

                    resetEvent.Reset();
                    resetEvent.WaitOne(2000);

                    returnValue.ShouldBe("haldis");

                    client.RunAsync(new Func<string>(ResponseMethod));

                    resetEvent.Reset();
                    resetEvent.WaitOne(2000);

                    returnValue.ShouldBe("jalmar");



                });
            });
        }

        string ResponseMethod()
        {
            return "jalmar";
        }


        [Test]
        [Ignore]
        public void Assure_it_can_handle_heavy_load()
        {
            Scenario.StartNew(this, scenario =>
            {
                var returnValues = new List<string>();

                scenario.
                    Given(client_is_created).
                    And("response handler is specified", () =>
                    {
                        client.RunAsyncCompleted += (o, e) =>
                        {
                            returnValues.Add(e.ReturnValue);
                        };
                    });

                scenario.When("1000 requests are made", () =>
                {
                    for (int i = 0; i < 1000; i++)
                        client.RunAsync(() => 
                        {
                            return "goeran";
                        });
                });

                scenario.Then("assure all responses are received", () =>
                {
                    Thread.Sleep(2000);
                    returnValues.ShouldHave(1000);
                    returnValues.ForEach(rv => rv.ShouldBe("goeran"));
                });
            });
        }
    }

    [TestFixture]
    public class When_RunAsyncVoid : Shared
    {

        bool delegateActionMethodInvoked;

        [Test]
        public void Assure_its_executed_on_a_background_thread()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(client_is_created);

                scenario.When(run_async_void);

                scenario.Then("assure it's executed on a background thread", () =>
                {
                    int thisThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                    int messageThreadId = -1;
                    var start = DateTime.Now;
                    client.RunAsyncVoid(() =>
                    {
                        messageThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                        FourSecondsMethod();
                    });
                    var end = DateTime.Now;
                    var span = end.Subtract(start).TotalMilliseconds;
                    span.ShouldBeLessThan(500);
                    thisThreadId.ShouldNotBe(messageThreadId);
                });
            });
        }

        [Test]
        public void Assure_Lambda_action_is_invoked()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(client_is_created);

                scenario.When(run_async_void);

                scenario.Then("assure lambda action is invoked", () =>
                {
                    var invoked = false;
                    client.RunAsyncVoid(() => invoked = true);
                    Thread.Sleep(1000);
                    invoked.ShouldBeTrue();
                });
            });
        }

        [Test]
        public void Assure_delegate_action_is_invoked()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(client_is_created);

                scenario.When(run_async_void);

                scenario.Then("assure delegate action is invoked", () =>
                {
                    client.RunAsyncVoid(DelegateActionMethod);
                    Thread.Sleep(1000);
                    delegateActionMethodInvoked.ShouldBeTrue();
                });
            });
        }

        void DelegateActionMethod()
        {
            delegateActionMethodInvoked = true;
        }
    }
}
