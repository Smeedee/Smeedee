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

using System;
using NUnit.Framework;
using Smeedee.Widgets.SourceControl.ViewModels;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Widgets.Tests.SourceControl.ViewModels
{
    class ChangesetSpecs
    {
        [TestFixture]
        public class When_ViewModel_is_spawned
        {
            private ChangesetViewModel viewModel;

            [SetUp]
            public void Setup()
            {
                viewModel = new ChangesetViewModel();
            }

            [Test]
            public void Should_have_a_Message_property()
            {
                viewModel.Message.ShouldBeNull();
            }

            [Test]
            public void Should_have_a_Date_property()
            {
                viewModel.Date.ShouldBe(DateTime.MinValue);
            }

            [Test]
            public void Should_have_Person_property()
            {
                viewModel.Developer.ShouldNotBeNull();
                viewModel.Developer.Name.ShouldBeNull();
                viewModel.Developer.Email.ShouldBeNull();
            }


            [Test]
            public void Should_have_CommentIsBad_property()
            {
                viewModel.CommentIsBad.ShouldNotBeNull();
                viewModel.CommentIsBad = true;
                viewModel.CommentIsBad.ShouldBe(true);
            }

            [Test]
            public void Should_have_BackgroundColor_property()
            {
                viewModel.BackgroundColor.ShouldBe(ChangesetViewModel.DEFAULT_BACKGROUND_COLOR);
            }
        }
    }
}
