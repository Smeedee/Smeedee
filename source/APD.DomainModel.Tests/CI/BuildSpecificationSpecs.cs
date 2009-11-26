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

using APD.DomainModel.Framework;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;


namespace APD.DomainModel.CI.BuildSpecificationSpecs
{
    [TestFixture]
    public class latestBuildsSpecification
    {
        [Test]
        public void IsSatisfiedBy_should_always_return_true()
        {
            var spec = new AllSpecification<CIServer>();

            spec.IsSatisfiedBy(null).ShouldBeTrue();
        }
    }

    [TestFixture]
    public class allBuildsByUserSpecification
    {
        [Test]
        public void should_return_true_when_users_match()
        {
            var build = new Build
                        {
                            Trigger = new CodeModifiedTrigger("tuxbear")
                        };
            var spec = new AllBuildsByUser("tuxbear");

            spec.IsSatisfiedBy(build).ShouldBeTrue();
        }

        [Test]
        public void should_return_false_when_users_dont_match()
        {
            var build = new Build
            {
                Trigger = new CodeModifiedTrigger("torstn")
            };
            var spec = new AllBuildsByUser("tuxbear");

            spec.IsSatisfiedBy(build).ShouldBeFalse();
        }

        [Test]
        public void should_return_false_when_trigger_is_not_codemodifiedtrigger()
        {
            var build = new Build
            {
                Trigger = new UnknownTrigger()
            };
            var spec = new AllBuildsByUser("tuxbear");

            spec.IsSatisfiedBy(build).ShouldBeFalse();
        }
    }
}