using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Repositories;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.DSL.Specifications;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Client.Framework.Tests.Repositories
{
    public class WidgetConfigurationRepositorySpecs
    {
        public class context
        {
            protected Widget widgetToReturnConfigurationFrom;
            protected WidgetConfigurationRepository repo;
            protected Mock<IPersistDomainModelsAsync<Configuration>> persisterMock;

            [SetUp]
            public void Setup()
            {
                ViewModelBootstrapperForTests.Initialize();
                widgetToReturnConfigurationFrom = new Widget();
                persisterMock = new Mock<IPersistDomainModelsAsync<Configuration>>();
                repo = new WidgetConfigurationRepository(widgetToReturnConfigurationFrom, persisterMock.Object);
                before();
            }

            protected virtual void before()
            {}
        }

        [TestFixture]
        public class when_get_sync : context
        {
            [Test]
            public void Assure_widget_Configuration_is_returned()
            {
                var result = repo.Get(All.ItemsOf<Configuration>());

                result.Count().ShouldBe(1);
                result.ElementAt(0).ShouldBeSameAs(widgetToReturnConfigurationFrom.Configuration);
            }
        }

        [TestFixture]
        public class when_save : context
        {
            private Configuration newConfig;
            private bool saveCompletedWasFired = false;
            private Guid oldConfigId;

            protected override void before()
            {
                newConfig = new Configuration("new config");
                oldConfigId = widgetToReturnConfigurationFrom.Configuration.Id;
                repo.SaveCompleted += (o, e) => { saveCompletedWasFired = true; };
                repo.Save(newConfig);
            }

            [Test]
            public void Assure_widget_configuration_is_set()
            {
                widgetToReturnConfigurationFrom.Configuration.Name.ShouldBe(newConfig.Name);
            }

            [Test]
            public void Assure_ConfigurationId_is_retained()
            {
                newConfig.Id.ShouldBe(oldConfigId);
            }

            [Test]
            public void Assure_SaveCompleted_is_fired_when_persister_is_done()
            {
                saveCompletedWasFired.ShouldBeFalse();
                persisterMock.Raise(t => t.SaveCompleted += null, new SaveCompletedEventArgs());

                saveCompletedWasFired.ShouldBeTrue();
            }

            [Test]
            public void Assure_Configuration_was_saved_with_the_given_persister()
            {
                persisterMock.Verify(p=>p.Save(It.Is<Configuration>(config => config.Id == widgetToReturnConfigurationFrom.Configuration.Id)));
            }
        }


        [TestFixture]
        public class when_getting_async : context
        {
            private bool wasGetCompletedFired = false;

            protected override void before()
            {
                repo.GetCompleted += (o, e) => { wasGetCompletedFired = true; };
                repo.BeginGet(All.ItemsOf<Configuration>());
            }

            [Test]
            public void Assure_GetCompleted_is_fired()
            {
                wasGetCompletedFired.ShouldBeTrue();
            }

            [Test]
            public void Assure_WidgetConfig_is_returned_in_GetCompleted()
            {
                repo.GetCompleted += (o, e) =>
                {
                    e.Result.Count().ShouldBe(1);
                    e.Result.ElementAt(0).Id.ShouldBe(widgetToReturnConfigurationFrom.Configuration.Id);
                };

                repo.BeginGet(All.ItemsOf<Configuration>());
            }
        }

    }
}
