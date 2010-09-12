using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.Client.Web.Tests.SlideConfigurationService;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.DomainModel.Framework.DSL.Specifications;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Client.Web.Tests.Services.Integration
{
    [TestFixture][Category("IntegrationTest")]
    public class SlideConfigurationRepositoryServiceSpecs
    {
        private SlideConfiguration slideConfiguration;
        private SlideConfigurationRepositoryServiceClient client;

        [SetUp]
        public void Setup()
        {
            slideConfiguration = new SlideConfiguration()
            {
                Duration = 200,
                Title = "hello!",
                SlideNumberInSlideshow = 30,
                WidgetConfigurationId = Guid.NewGuid(),
                WidgetType = "Smeedee.Widget",
                WidgetXapName = "Smeedee.Widgets.xap"
            };


            client = new SlideConfigurationRepositoryServiceClient();

            client.Save(slideConfiguration);
        }

        [Test]
        public void Assure_service_can_save_data()
        {
            var result = client.Get(All.ItemsOf<SlideConfiguration>());

            var resultConfig = result.Last();

            AssertAreEqual(resultConfig, slideConfiguration);
        }

        private void AssertAreEqual(SlideConfiguration resultConfig, SlideConfiguration expectedConfig)
        {
            resultConfig.Title.ShouldBe(expectedConfig.Title);
            resultConfig.Duration.ShouldBe(expectedConfig.Duration);
            resultConfig.SlideNumberInSlideshow.ShouldBe(expectedConfig.SlideNumberInSlideshow);
            resultConfig.WidgetType.ShouldBe(expectedConfig.WidgetType);
            resultConfig.WidgetConfigurationId.ShouldBe(expectedConfig.WidgetConfigurationId);
            resultConfig.WidgetXapName.ShouldBe(expectedConfig.WidgetXapName);
        }

        [Test]
        public void Assure_delete_removes_the_slideConfig()
        {
            var result = client.Get(All.ItemsOf<SlideConfiguration>());
            if( result.Count() == 0 )
                client.Save(slideConfiguration);

            client.Delete(All.ItemsOf<SlideConfiguration>());

            var resultAfterDelete = client.Get(All.ItemsOf<SlideConfiguration>());

            resultAfterDelete.Count.ShouldBe(0);
        }

        [Test]
        public void Assure_ByIdSpecification_can_be_serialized()
        {
            client.Save(slideConfiguration);
            var result = client.Get(new SlideConfigurationByIdSpecification(slideConfiguration.Id));
            
            result.Count.ShouldBe(1);
            AssertAreEqual(result.Single(), slideConfiguration );
        }
    }
}
