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
using NUnit.Framework;
using TinyBDD.Specification.NUnit;


namespace Smeedee.DomainModel.CI.TriggerSpecs
{
    [TestFixture]
    public class trigger
    {
        private Type[] derivingTypes;

        [SetUp]
        public void Setup()
        {
            derivingTypes = Trigger.GetKnownTypes();
        }

        [Test]
        public void should_know_about_deriving_types()
        {
            foreach (var type in derivingTypes)
            {
                type.BaseType.ShouldBe( typeof(Trigger) );
            }
        }

        [Test]
        public void deriving_types_should_have_parameterless_contructor()
        {
            foreach (var type in derivingTypes)
            {
                object o = Activator.CreateInstance(type);
            }
        }
    }

    [TestFixture]
    public class code_modified_trigger
    {
        [Test]
        public void assure_code_modified_trigger_exists()
        {
            var username = "torstn";
            var trigger = new CodeModifiedTrigger(username);
            var abstractTrigger = trigger as Trigger;
            
            abstractTrigger.ShouldNotBeNull();
        }

        [Test]
        public void should_have_username()
        {
            var username = "torstn";
            var trigger = new CodeModifiedTrigger(username);

            trigger.InvokedBy.ShouldBe(username);
        }

        [Test]
        public void Assure_Cause_is_set()
        {
            var trigger = new CodeModifiedTrigger("haldis");

            trigger.Cause.ShouldBe("Code Modified");
        }
    }


    [TestFixture]
    public class unknown_trigger
    {
        [Test]
        public void assure_unknown_trigger_exists()
        {
            var trigger = new UnknownTrigger();
            var abstractTrigger = trigger as Trigger;
            abstractTrigger.ShouldNotBeNull();
        }

        [Test]
        public void Assure_Cause_is_set()
        {
            var trigger = new UnknownTrigger();
            trigger.Cause.ShouldBe("Unknown");
        }

        [Test]
        public void Assure_By_is_set()
        {
            var trigger = new UnknownTrigger();
            trigger.InvokedBy.ShouldBe("unknown");
        }
        
        
    }


    [TestFixture]
    public class event_trigger
    {
        private const string EVENT_NAME = "Nighty Build";

        [Test]
        public void assure_event_trigger_exists()
        {
            var trigger = new EventTrigger(EVENT_NAME);
            var abstractTrigger = trigger as Trigger;
            
            abstractTrigger.ShouldNotBeNull();
        }

        [Test]
        public void should_have_event_name_property()
        {
            var trigger = new EventTrigger(EVENT_NAME);
            
            trigger.Cause.ShouldBe(EVENT_NAME);
        }

        [Test]
        public void Assure_Cause_is_NOT_set()
        {
            var trigger = new EventTrigger();

            trigger.Cause.ShouldBeNull();
        }

        [Test]
        public void Assure_Cause_is_set()
        {
            var trigger = new EventTrigger(EVENT_NAME);

            trigger.Cause.ShouldBe(EVENT_NAME);
        }
        

        [Test]
        public void Assure_By_is_set()
        {
            var trigger = new EventTrigger();
            trigger.InvokedBy.ShouldBe("system");

            trigger = new EventTrigger(EVENT_NAME);
            trigger.InvokedBy.ShouldBe("system");

        }
        
        
    }
}