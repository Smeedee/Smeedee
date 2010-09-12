using System;
using NUnit.Framework;
using Moq;
//TODO: This should be resolved based on namespace dec in model specification
using Tskjortebutikken.Widgets.ViewModel;
using TinyMVVM.Framework;
using TinyMVVM.Repositories;
using TinyMVVM.Framework.Testing;
using TinyMVVM.Framework.Testing.Services;
using TinyMVVM.Framework.Services;
using TinyMVVM.IoC;
using System;
using System.Collections.ObjectModel;
using Tskjortebutikken.Widgets.ViewModel;

namespace Tskjortebutikken.Widgets.Tests.ViewModel.TestScenarios
{
	public abstract class CollectionViewModelTestScenario<T> where T: class
	{
	    protected T Given { get { return this as T; } }
        protected T And { get { return this as T; } }
        protected T When { get { return this as T; } }
        protected T Then { get { return this as T; } }

		protected CollectionViewModel viewModel;

		protected Mock<IRepository<Graph>> GraphRepositoryFake = new Mock<IRepository<Graph>>();
		protected Mock<IRepository<DataPoint>> DataPointRepositoryFake = new Mock<IRepository<DataPoint>>();
		protected Mock<IRepository<GraphSettings>> GraphSettingsRepositoryFake = new Mock<IRepository<GraphSettings>>();
		protected Mock<IRepository<DatabaseViewModel>> DatabaseViewModelRepositoryFake = new Mock<IRepository<DatabaseViewModel>>();
		protected Mock<IRepository<CollectionViewModel>> CollectionViewModelRepositoryFake = new Mock<IRepository<CollectionViewModel>>();
	
		[SetUp]
		public void Setup()
		{
			RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
			{
				config.Bind<IUIInvoker>().To<UIInvokerForTesting>();

				config.Bind<IRepository<Graph>>().ToInstance(GraphRepositoryFake.Object);
				config.Bind<IRepository<DataPoint>>().ToInstance(DataPointRepositoryFake.Object);
				config.Bind<IRepository<GraphSettings>>().ToInstance(GraphSettingsRepositoryFake.Object);
				config.Bind<IRepository<DatabaseViewModel>>().ToInstance(DatabaseViewModelRepositoryFake.Object);
				config.Bind<IRepository<CollectionViewModel>>().ToInstance(CollectionViewModelRepositoryFake.Object);
		
			});

			Before();
		}

		protected virtual void Before()
		{
		}

		[TearDown]
		public void TearDown()
		{
			After();
			RemoveAllGlobalDependencies.ForAllViewModels();
		}

		protected virtual void After()
		{
		}

		//Given
		public void dependencies_are_configured(Action<DependencyConfigSemantics> configAction)
		{
			ConfigureGlobalDependencies.ForAllViewModels(configAction);
		}

		//Given & And
		public void CollectionViewModel_is_created()
		{
			viewModel = new CollectionViewModel();
		}

		//And
		public void Name_is_set(string value)
		{
			viewModel.Name = value;
		}

	
		

		//And
		public void CollectionViewModel_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		//When
		public void CollectionViewModel_is_spawned()
		{
			viewModel = new CollectionViewModel();
		} 
		
		//Whens
		}
}

