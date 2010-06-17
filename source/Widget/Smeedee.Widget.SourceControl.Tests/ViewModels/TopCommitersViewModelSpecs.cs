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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using NUnit.Framework;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Widget.SourceControl.ViewModels;
using TinyBDD.Specification.NUnit;
using TinyBDD.Dsl.GivenWhenThen;
using Smeedee.Tests;

namespace Smeedee.Client.Widget.SourceControlTests.ViewModels.TopCommitersViewModelSpecs
{
    public class Shared
    {
        protected static TopCommitersViewModel viewModel;
        protected static PropertyChangedRecorder changeRecorder;
    }

    [TestFixture] 
    public class When_ViewModel_is_spawned : Shared
    {
        [SetUp]
        public void Setup()
        {
            viewModel = new TopCommitersViewModel();
        }

        [Test]
        public void Assure_ViewModel_is_a_AbstractViewModel()
        {
            (viewModel is AbstractViewModel).ShouldBeTrue();       
        }
        
        [Test]
        public void Assure_it_has_a_Developers_property()
        {
            viewModel.Developers.ShouldNotBeNull();       
        }
    }

    [TestFixture]
    public class When_properties_change : Shared
    {
        Context the_ViewModel_is_created = () =>
        {
            viewModel = new TopCommitersViewModel();
            changeRecorder = new PropertyChangedRecorder(viewModel.Developers);
        };

        [Test]
        public void Assure_observers_are_notified_when_content_in_Developer_property_changes()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_ViewModel_is_created);

                scenario.When("content in Developer property changes", () =>
                    viewModel.Developers.Add(new CodeCommiterViewModel()));

                scenario.Then("assure observers are notified", () =>
                    changeRecorder.ChangedProperties.Count.ShouldNotBe(0));
            });
        
        }
               
    }
        
}
