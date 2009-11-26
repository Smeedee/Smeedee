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
using System.Threading;
using System.Windows.Controls;
using APD.Client.Framework;
using APD.Client.Widget.TrayBar.Controllers;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace APD.Client.Widget.TrayBarTests.Controllers
{
    // TODO: These tests return ok no matter what, so the output must be examined. Better solution for this?
    [TestFixture]
    public class TrayBarControllerSpecs
    {
        private static TrayBarController controller;

        private Context a_controller_is_instantiated =
            () => { controller = new TrayBarController(new NeverNotifyRefresh(), new NoUIInvocation()); };

        [Test]
        [STAThread]
        public void Assert_view_can_be_added()
        {
            var t = new Thread(new ThreadStart(delegate()
            {
                Scenario.StartNew(this, scenario =>
                {
                    scenario.Given(a_controller_is_instantiated);
                    scenario.When("the controller is told to add a view to its view model",
                                  () => { controller.AddView(new UserControl()); });
                    scenario.Then("the new view should have been added to the controllers view model",
                                  () => { 0.ShouldBeLessThan(controller.ViewModel.ViewCollection.Count); });
                });
            })) {ApartmentState = ApartmentState.STA};

            t.Start();
            t.Join();
        }

        [Test]
        [STAThread]
        public void Assert_view_can_be_removed()
        {
            var t = new Thread(new ThreadStart(delegate()
            {
                Scenario.StartNew(this, scenario =>
                {
                    scenario.Given(a_controller_is_instantiated);
                    scenario.When("the controller is told to add, and then remove a view from its list", () =>
                    {
                        var view = new UserControl();
                        var view2 = new UserControl();
                        controller.AddView(view);
                        controller.AddView(view2);
                        1.ShouldBeLessThan(controller.ViewModel.ViewCollection.Count);
                        controller.RemoveView(view2);
                        controller.RemoveView(view);
                        controller.ViewModel.ViewCollection.Count.ShouldBe(0);
                    });

                    scenario.Then("the view should have been added, and then deleted.", () => { });
                });
            })) {ApartmentState = ApartmentState.STA};

            t.Start();
            t.Join();
        }
    }
}