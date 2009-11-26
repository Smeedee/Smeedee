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
using APD.Client.Widget.TrayBar.ViewModels;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace APD.Client.Widget.TrayBarTests.ViewModels
{
    [TestFixture]
    public class TrayBarItemViewModelSpecs
    {
        [Test]
        [STAThread]
        public void Assert_view_model_has_item()
        {
            var t = new Thread(new ThreadStart(delegate()
            {
                Scenario.StartNew(this, scenario =>
                {
                    scenario.Given("A new itemviewmodel is instantiated");
                    var viewModel = new TrayBarItemViewModel();

                    scenario.When("a user control is added to it");
                    viewModel.Control = new UserControl();

                    scenario.Then("it should have a field for a UserControl to contain a view",
                                  () => { viewModel.Control.ShouldNotBeNull(); });
                });
            })) {ApartmentState = ApartmentState.STA};

            t.Start();
            t.Join();
        }
    }
}