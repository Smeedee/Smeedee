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
	public abstract class GraphSettingsTestScenario<T> where T: class
	{
	    protected T Given { get { return this as T; } }
        protected T And { get { return this as T; } }
        protected T When { get { return this as T; } }
        protected T Then { get { return this as T; } }

		protected GraphSettings viewModel;

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
		public void GraphSettings_is_created()
		{
			viewModel = new GraphSettings();
		}

		//And
		public void Databases_is_set(ObservableCollection<DatabaseViewModel> value)
		{
			viewModel.Databases = value;
		}

		//When
	public void add_Database(Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
	
		public void SelectedDatabase_is_set(DatabaseViewModel value)
		{
			viewModel.SelectedDatabase = value;
		}

	
		public void SelectedCollection_is_set(CollectionViewModel value)
		{
			viewModel.SelectedCollection = value;
		}

		//When
	public void add_SelectedCollectio(Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
	
		public void AvailableProperties_is_set(ObservableCollection<string> value)
		{
			viewModel.AvailableProperties = value;
		}

		//When
	public void add_AvailablePropertie(Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
	
		public void SelectedPropertyForXAxis_is_set(string value)
		{
			viewModel.SelectedPropertyForXAxis = value;
		}

	
		public void SelectedPropertyForYAxis_is_set(string value)
		{
			viewModel.SelectedPropertyForYAxis = value;
		}

	
		public void Graph_is_set(Graph value)
		{
			viewModel.Graph = value;
		}

	
		

		//And
		public void GraphSettings_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		//When
		public void GraphSettings_is_spawned()
		{
			viewModel = new GraphSettings();
		} 
		
		//Whens
		public void Save_Command_is_executed()
		{
			viewModel.Save.Execute(null);
		}

		public void execute_Save_Command()
		{
			viewModel.Save.Execute(null);
		}
		public void Refresh_Command_is_executed()
		{
			viewModel.Refresh.Execute(null);
		}

		public void execute_Refresh_Command()
		{
			viewModel.Refresh.Execute(null);
		}
		public void Test_Command_is_executed()
		{
			viewModel.Test.Execute(null);
		}

		public void execute_Test_Command()
		{
			viewModel.Test.Execute(null);
		}
		public void Cancel_Command_is_executed()
		{
			viewModel.Cancel.Execute(null);
		}

		public void execute_Cancel_Command()
		{
			viewModel.Cancel.Execute(null);
		}
		}
}

