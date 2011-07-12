using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.Widgets.GenericCharting.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Testing.Services;
using TinyMVVM.IoC;
using TinyMVVM.Repositories;

namespace Smeedee.Widgets.Tests.GenericCharting.ViewModels
{
    public class Shared:ScenarioClass
    {
        protected ChartViewModel chartViewModel;

        protected Mock<IRepository<ChartViewModel>> ChartRepositoryFake = new Mock<IRepository<ChartViewModel>>();
        protected Mock<IRepository<DataPointViewModel>> DataPointRepositoryFake = new Mock<IRepository<DataPointViewModel>>();
        protected Mock<IRepository<ChartSettingsViewModel>> ChartSettingsRepositoryFake = new Mock<IRepository<ChartSettingsViewModel>>();
        protected Mock<IRepository<DatabaseViewModel>> DataBaseRepositoryFake = new Mock<IRepository<DatabaseViewModel>>();
        protected Mock<IRepository<CollectionViewModel>> CollectionRepositoryFake = new Mock<IRepository<CollectionViewModel>>();

        [SetUp]
        public void Setup()
        {
            RemoveAllGlobalDependencies.ForAllViewModels();
            ConfigureGlobalDependencies.ForAllViewModels(config =>
                                                             {
                                                                 config.Bind<IUIInvoker>().To<UIInvokerForTesting>();

                                                                 config.Bind<IRepository<ChartViewModel>>().ToInstance(
                                                                     ChartRepositoryFake.Object);
                                                                 config.Bind<IRepository<DataPointViewModel>>().
                                                                     ToInstance(DataPointRepositoryFake.Object);
                                                                 config.Bind<IRepository<ChartSettingsViewModel>>().
                                                                     ToInstance(ChartSettingsRepositoryFake.Object);
                                                                 config.Bind<IRepository<DatabaseViewModel>>().
                                                                     ToInstance(DataBaseRepositoryFake.Object);
                                                                 config.Bind<IRepository<CollectionViewModel>>().
                                                                     ToInstance(CollectionRepositoryFake.Object);
                                                             });
        }

        [TearDown]
        public void TearDown()
        {
            RemoveAllGlobalDependencies.ForAllViewModels();
        }
    }
}
