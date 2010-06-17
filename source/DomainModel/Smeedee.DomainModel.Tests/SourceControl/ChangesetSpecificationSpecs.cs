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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Smeedee.DomainModel.SourceControl;
using TinyBDD.Specification.NUnit;

namespace Smeedee.DomainModel.SourceControlTests.ChangesetSpecificationSpecs
{
    public class Shared
    {
        protected void AssertIsValueObject(object a, object b)
        {
            (a.Equals(b)).ShouldBeTrue();
            (a.Equals(a)).ShouldBeTrue();
            (a.Equals(null)).ShouldBeFalse();
        }

        protected void AssertEqualOperatorIsOverloaded(object a, object b)
        {
            Assert.AreEqual(a, b);
            Assert.AreEqual(a, b);
            Assert.AreNotEqual(a, null);
            Assert.AreNotEqual(null, b);

            Assert.IsTrue(null != a);
            Assert.IsTrue(a != null);
        }

        protected void AssertItsHashable(object a, object b)
        {
            var dict = new Dictionary<object, object>();

            dict.Add(a, "hello");

            var cantAddToDictionary = false;
            try
            {
                dict.Add(b, "world");
            }
            catch (Exception)
            {
                cantAddToDictionary = true;
            }

            cantAddToDictionary.ShouldBeTrue();
            dict.ContainsKey(b).ShouldBeTrue();

        }
    }
        
    [TestFixture]
    public class AllChangesetsSpecificationSpecs : Shared
    {
        private AllChangesetsSpecification specification;
    
        [SetUp]
        public void Setup()
        {
            specification = new AllChangesetsSpecification();
            
        }

        [Test]
        public void Should_be_satisfied_by_all()
        {
            specification.IsSatisfiedBy(null).ShouldBeTrue();

            specification.IsSatisfiedBy(new Changeset()).ShouldBeTrue();
        }

        [Test]
        public void Should_be_able_to_parse()
        {
            var expression = specification.IsSatisfiedByExpression();
        }

        [Test]
        public void Assure_isa_value_object()
        {
            var allA = new AllChangesetsSpecification();
            var allB = new AllChangesetsSpecification();
            
            AssertIsValueObject(allA, allB);
        }

        [Test]
        public void Assure_equal_operator_is_overloaded()
        {
            var allA = new AllChangesetsSpecification();
            var allB = new AllChangesetsSpecification();

            AssertEqualOperatorIsOverloaded(allB, allB);
        }

        [Test]
        public void Assure_its_hashable()
        {
            var allA = new AllChangesetsSpecification();
            var allB = new AllChangesetsSpecification();

            AssertItsHashable(allA, allB);
        }
    }
       
    
    [TestFixture]
    public class ChangesetsForUserSpecificationSpecs 
    {
        private ChangesetsForUserSpecification specification;

        [SetUp]
        public void Setup()
        {
            specification = new ChangesetsForUserSpecification("Frank");
        }


        [Test]
        public void Should_have_default_constructor()
        {
            specification = new ChangesetsForUserSpecification();
            specification.ShouldNotBeNull();
        }

        [Test]
        public void Should_have_Username_property()
        {
            specification.Username.ShouldBe("Frank");
        }

        [Test]
        public void Should_be_satisfied_by_Changesets_with_same_Author_as_Username()
        {
            var changeset = new Changeset();
            changeset.Author = new Author();
            changeset.Author.Username = "Frank";

            specification.IsSatisfiedBy(changeset).ShouldBeTrue();

            changeset.Author.Username = "Truls";

            specification.IsSatisfiedBy(changeset).ShouldBeFalse();

        }


        [Test]
        public void Should_be_able_to_parse()
        {
            var expression = specification.IsSatisfiedByExpression();
        }
    }

    

    [TestFixture]
    public class ChangesetsAfterRevisionSpecificationSpecs : Shared
    {

        private ChangesetsAfterRevisionSpecification specification;

        [SetUp]
        public void Setup()
        {
            specification = new ChangesetsAfterRevisionSpecification(1000);
        }


        [Test]
        public void Should_have_default_constructor()
        {
            specification = new ChangesetsAfterRevisionSpecification();
            specification.ShouldNotBeNull();
        }

        [Test]
        public void Should_have_Revision_property()
        {
            specification.Revision.ShouldBe(1000);
        }


        [Test]
        public void Should_be_satisfied_by_Changesets_with_newer_Revision()
        {
            var changeset = new Changeset();
            changeset.Revision = 1001;

            specification.IsSatisfiedBy(changeset).ShouldBeTrue();

            changeset.Revision = 999;

            specification.IsSatisfiedBy(changeset).ShouldBeFalse();
        }


        [Test]
        public void Should_be_able_to_parse()
        {
            var expression = specification.IsSatisfiedByExpression();
        }

        [Test]
        public void Assure_isa_value_object()
        {
            var specA = new ChangesetsAfterRevisionSpecification(10);
            var specB = new ChangesetsAfterRevisionSpecification(10);

            AssertIsValueObject(specA, specB);
        }

        [Test]
        public void Assure_Equal_operator_is_overloaded()
        {
            var specA = new ChangesetsAfterRevisionSpecification(10);
            var specB = new ChangesetsAfterRevisionSpecification(10);

            AssertEqualOperatorIsOverloaded(specA, specB);
            Assert.IsFalse(specA != specB);
        }
        
        [Test]
        public void Assure_its_hashable()
        {
            var specA = new ChangesetsAfterRevisionSpecification(10);
            var specB = new ChangesetsAfterRevisionSpecification(10);

            AssertItsHashable(specA, specB);
        }
        
    }

    
   [TestFixture]
   public class ChangesetsNotMoreThanNumberSpecificationSpecs 
   {
       

       private ChangesetsNotMoreThanNumberSpecification specification;


       [SetUp]
       public void Setup()
       {
           specification = new ChangesetsNotMoreThanNumberSpecification(100);
       }


       [Test]
       public void Should_have_default_contrstructor()
       {
           specification = new ChangesetsNotMoreThanNumberSpecification();
           specification.ShouldNotBeNull();
       }


       [Test]
       public void Should_have_Amount_property()
       {
           specification.Amount.ShouldBe(100);
       }


       [Test]
       public void Should_have_AmountDone_property()
       {
           specification.Amount.ShouldBeInstanceOfType<int>();
       }

       [Test]
       public void Should_be_satisfied_by_all_Changesets_if_AmountDone_is_less_then_Amount()
       {
           specification.AmountDone = 50;
                 
           specification.IsSatisfiedBy(new EmptyChangeset()).ShouldBeTrue();

           specification.AmountDone = 500;

           specification.IsSatisfiedBy(new EmptyChangeset()).ShouldBeFalse();
       }

       [Test]
       public void Should_be_able_to_parse()
       {
           var expression = specification.IsSatisfiedByExpression();
       }

       
   }

       
}
