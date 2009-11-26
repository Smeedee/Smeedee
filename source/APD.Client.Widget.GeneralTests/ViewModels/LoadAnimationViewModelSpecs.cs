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

using NUnit.Framework;
using APD.Client.Widget.General.ViewModels;
using APD.Client.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.Client.Framework.ViewModels;
using APD.Tests;


namespace APD.Client.Widget.GeneralTests.ViewModels.LoadAnimationViewModelSpecs
{
    public class Shared
    {
        protected static LoadingAnimationViewModel viewModel;
        protected static PropertyChangedRecorder changeRecorder;

        protected static void SetupContext()
        {
            viewModel = new LoadingAnimationViewModel(new NoUIInvocation());
        }
    }

    [TestFixture]
    public class When_spawned : Shared
    {
        [SetUp]
        public void Setup()
        {
            SetupContext();
        }

        [Test]
        public void Assure_it_has_a_Display_property()
        {
            viewModel.Display.ShouldBeFalse();
        }

        [Test]
        public void Assure_it_has_a_Info_property()
        {
            viewModel.Info.ShouldBeNull();       
        }

        [Test]
        public void Assure_it_is_a_AbstractViewModel()
        {
            (viewModel is AbstractViewModel).ShouldBeTrue();       
        }
    }

    [TestFixture]
    public class When_Property_changes : Shared
    {
        Context viewModel_is_spawned = () =>
        {
            viewModel = new LoadingAnimationViewModel(new NoUIInvocation());
            changeRecorder = new PropertyChangedRecorder(viewModel);
        };


        [Test]
        public void Assure_observers_are_notified()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(viewModel_is_spawned);

                scenario.When("Display property is changed", () =>
                {
                    viewModel.Display = true;
                    viewModel.Info = "Loading...";
                });

                scenario.Then("assure observers are notified", () =>
                {
                    changeRecorder.ChangedProperties.ShouldContain("Display");
                    changeRecorder.ChangedProperties.ShouldContain("Info");
                });
            }).Execute();
        }
    }
        
}
