using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.DomainModel.Config.SlideConfig;
using TinyBDD.Specification.NUnit;

namespace Smeedee.DomainModel.Tests.Config.SlideConfig.WidgetDefinitionSpecs
{
    [TestFixture]
    public class when_spawned
    {
        [Test]
        public void Assure_ConfigKey_is_initialzed_with_a_GUID()
        {
            var widgetDefinition = new WidgetDefinition();
            var generatedGuid = Guid.Parse(widgetDefinition.ConfigKey);
            generatedGuid.ShouldNotBeNull();
        }
    }

    [TestFixture]
    public class when_creating_from_WidgetInfo
    {
        private string widgetName;
        private string widgetType;
        private string widgetXapName;

        [Test]
        public void Assure_information_is_set_correctly()
        {
            widgetName = "My Widget";
            widgetType = "MyNamespace.Widgets.MyWidget";
            widgetXapName = "MyWidgets_2010.xap";
            var widgetInfo = new WidgetInfo()
                                 {
                                     Name = widgetName,
                                     Type = widgetType,
                                     XAPName = widgetXapName
                                 };

            var widgetDefiniton = WidgetDefinition.FromWidgetInfo(widgetInfo);
            
            widgetDefiniton.XAPName.ShouldBe(widgetXapName);
            widgetDefiniton.Type.ShouldBe(widgetType);
        }
    }

}
