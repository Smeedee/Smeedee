using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.DSL.Specifications;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Client.Framework.Tests.Services.Impl
{
    public class ConfigurationManagerTests 
    {
        public class context
        {
            protected Mock<IAsyncRepository<Configuration>> configRepoMock;
            protected Mock<ITimer> timerMock;

            protected Widget registeredWidget;
            protected ConfigurationManager manager;
            protected bool wasNotified;
            protected List<Configuration> configList;

            [SetUp]
            public void Setup()
            {
                ViewModelBootstrapperForTests.Initialize();
                timerMock = new Mock<ITimer>();
                configRepoMock = new Mock<IAsyncRepository<Configuration>>(); 
                wasNotified = false;

                registeredWidget = new Widget();
                registeredWidget.ConfigurationChanged += (o, e) => wasNotified = true;
                manager = new ConfigurationManager(timerMock.Object, configRepoMock.Object);

                before();
            }

            protected  virtual void before()
            {
                
            }

            protected void TimerElapses()
            {
                timerMock.Raise(t => t.Elapsed += null, new EventArgs());
            }
        }

        [TestFixture]
        public class when_spawned : context
        {
            [Test]
            public void Assure_timer_is_started()
            {
                timerMock.Verify(t => t.Start(ConfigurationManager.REFRESH_INTERVAL_SECONDS * 1000));
            }
        }




        [TestFixture]
        public class when_the_timer_elapses : context
        {
            protected override void before()
            {
                TimerElapses();
            }

            [Test]
            public void Assure_all_configurations_are_fetched()
            {
                configRepoMock.Verify(r => r.BeginGet(It.IsAny<AllSpecification<Configuration>>()));
            }
        }


        [TestFixture]
        public class when_a_widget_registers_and_there_is_a_config_for_it : context
        {
            protected override void before()
            {
                configRepoMock.Setup(r => r.BeginGet(It.IsAny<Specification<Configuration>>()))
                    .Raises(r => r.GetCompleted += null, new GetCompletedEventArgs<Configuration>(
                                                             new List<Configuration>
                                                             {
                                                                 new Configuration()
                                                                 {
                                                                     Id = registeredWidget.Configuration.Id, 
                                                                     Name = "changed name"}
                                                             },
                                                             All.ItemsOf<Configuration>()));
                manager = new ConfigurationManager(timerMock.Object, configRepoMock.Object);

                manager.RegisterForConfigurationUpdates(registeredWidget);

            }

            [Test]
            public void Assure_widget_is_notified_with_the_new_config()
            {
                wasNotified.ShouldBeTrue();
            }
        }



        [TestFixture]
        public class when_configuration_of_registered_widget_contains_changes : context
        {
            protected override void before()
            {
                configList = new List<Configuration>
                {
                    new Configuration()
                    {
                        Id = registeredWidget.Configuration.Id,
                        Name = registeredWidget.Configuration.Name + "Changed"
                    }
                };

                configRepoMock.Setup(t => t.BeginGet(It.IsAny<Specification<Configuration>>()))
                    .Raises(t => t.GetCompleted += null,
                            new GetCompletedEventArgs<Configuration>(configList, new AllSpecification<Configuration>()));

                manager.RegisterForConfigurationUpdates(registeredWidget);
                TimerElapses();
            }

            [Test]
            public void Assure_widget_is_notified()
            {
                wasNotified.ShouldBeTrue();
            }

            [Test]
            public void Assure_Value_can_be_null()
            {
                var configWithNullValue = new Configuration
                {
                    Id = registeredWidget.Configuration.Id,
                    Settings = new List<SettingsEntry>
                    {
                        new SettingsEntry("ValueIsNull", null)
                    }
                };

                registeredWidget.Configuration.NewSetting(new SettingsEntry("ValueIsNull", null));

                configList = new List<Configuration>{configWithNullValue};

                TimerElapses();
                Assert.IsTrue(true);
            }
        }


        [TestFixture]
        public class when_configuration_of_registered_widget_does_not_contain_changes : context
        {
            protected override void before()
            {
                configList = new List<Configuration> { registeredWidget.Configuration };
                manager.RegisterForConfigurationUpdates(registeredWidget);
                TimerElapses();
            }

            [Test]
            public void Assure_widget_is_not_notfied()
            {
                wasNotified.ShouldBeFalse();
            }
        }


    }
}
