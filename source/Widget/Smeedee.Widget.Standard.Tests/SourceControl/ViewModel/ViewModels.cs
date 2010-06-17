
using Smeedee.Widget.Standard.SourceControl.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyMVVM.Framework.Testing;
using TestContext = TinyMVVM.Framework.Testing.TestContext;
using System.ComponentModel.DataAnnotations;
using TinyMVVM.Framework;
using System.ServiceModel.DomainServices.Client;
using APD.DomainModel.SourceControl;

namespace Smeedee.Widget.Standard.Tests.SourceControl.ViewModel
{
	public abstract class VersionControlConfigurationContext : TestContext
	{
		protected VersionControlConfiguration viewModel;

		[TestInitialize]
		public void Setup()
		{
			ServiceLocator.SetLocator(ServiceLocatorForTesting.GetServiceLocator());
			
			Context();
		}

		public abstract void Context();

		public void Given_VersionControlConfiguration_is_created()
		{
			viewModel = new VersionControlConfiguration();
            viewModel.PropertyChangeRecorder.Start();
		}
		
		public void And_Url_is_set(string value)
		{
			viewModel.Url = value;
		}
		public void And_Username_is_set(string value)
		{
			viewModel.Username = value;
		}
		public void And_Password_is_set(string value)
		{
			viewModel.Password = value;
		}
		
	
		public void And_VersionControlConfiguration_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public void When_Url_is_set(string value)
		{
			viewModel.Url = value;
		}
		
		public void When_Username_is_set(string value)
		{
			viewModel.Username = value;
		}
		
		public void When_Password_is_set(string value)
		{
			viewModel.Password = value;
		}
		
		
		public void When_VersionControlConfiguration_is_spawned()
		{
			viewModel = new VersionControlConfiguration();
		} 
		
	}

	public abstract class SourceControlViewModelContext : TestContext
	{
		protected SourceControlViewModel viewModel;

		[TestInitialize]
		public void Setup()
		{
			ServiceLocator.SetLocator(ServiceLocatorForTesting.GetServiceLocator());
			
			Context();
		}

		public abstract void Context();

		public void Given_SourceControlViewModel_is_created()
		{
			viewModel = new SourceControlViewModel();
            viewModel.PropertyChangeRecorder.Start();
		}
		
		public void And_Changesets_is_set(EntitySet<Changeset> value)
		{
			viewModel.Changesets = value;
		}
		
	
		public void And_SourceControlViewModel_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public void When_Changesets_is_set(EntitySet<Changeset> value)
		{
			viewModel.Changesets = value;
		}
		
		
		public void When_SourceControlViewModel_is_spawned()
		{
			viewModel = new SourceControlViewModel();
		} 
		
	}

}

