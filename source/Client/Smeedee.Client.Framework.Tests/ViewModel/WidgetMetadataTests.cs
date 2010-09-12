using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Tests.ViewModel;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Client.Framework.Tests.ViewModel
{
    public class WidgetMetadataTests 
    {

        [TestFixture]
        public class On_spawn : WidgetMetadataTestContext
        {
            [Test]
            public void Assert_SecondsOnScreen_is_set()
            {
                viewModel.SecondsOnScreen.ShouldBe(Slide.DEFAULT_SECONDSONSCREEN);
            }

            public override void Context()
            {
                Given_WidgetMetadata_is_created();
            }
        }



        [TestFixture]
        public class when_IsDescriptionCapped_is_set : WidgetMetadataTestContext
        {
            private string aLongDescription = "dette er en lang beskrivelse av noe som egentlig ikke trenger en lang beskrivelse men det er fordi jeg ksal teste om denne klassen klarer å cappe denne teksten på ca 100 chars";

            public override void Context()
            {
                Given_WidgetMetadata_is_created();
                And_PropertychangedRecorder_is_started();
                And_Description_is_set(
                    aLongDescription);
                When_IsDescriptionCapped_is_set(true);
            }

            [Test]
            public void Assure_null_description_returns_empty_string()
            {
                viewModel.Description = null;
                viewModel.Description.ShouldBe("");
            }

            [Test]
            public void Assure_last_three_chars_of_capped_description_are_dots()
            {
                viewModel.Description.EndsWith("...").ShouldBeTrue();
            }
            

            [Test]
            public void Assure_description_is_capped_at_100_chars_when_true()
            {
                viewModel.Description.Length.ShouldBe(100);
            }

            [Test]
            public void Assure_propertyChanged_for_Description_is_triggered()
            {
                viewModel.PropertyChangeRecorder.Data.Any( r=>r.PropertyName == "Description" ).ShouldBeTrue();
            }

            [Test]
            public void Assure_description_is_only_capped_when_IsCapped_is_true()
            {
                And_Description_is_set(aLongDescription);
                When_IsDescriptionCapped_is_set(false);
                (viewModel.Description.Length > 100).ShouldBeTrue();
            }
        }
        
    }
}
