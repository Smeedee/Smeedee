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

using APD.Client.Widget.TrayBar.ViewModels;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.Client.Framework;


namespace APD.Client.Widget.TrayBarTests.ViewModels
{
    [TestFixture]
    public class TrayBarViewModelSpecs
    {
        [Test]
        public void Assert_view_model_has_list_of_items()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("A new viewmodel is instantiated");
                var viewModel = new TrayBarViewModel(new NoUIInvocation());

                scenario.When("it is instantiated");
                scenario.Then("it should have a list of TrayBarItemViewModels", () =>
                {
                    viewModel.ViewCollection.ShouldNotBeNull();
                });
            });
        }
    }
}