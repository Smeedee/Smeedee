
using TinyMVVM.Framework.Services;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using TinyMVVM.Framework;
using System.ServiceModel.DomainServices.Client;
using APD.DomainModel.SourceControl;
using Smeedee.DomainModel.Config;

namespace Smeedee.Widget.Standard.SourceControl.ViewModel
{
	public partial class ConfigurationViewModel : TinyMVVM.Framework.ViewModelBase
	{
		protected IUIInvoker UIInvoker { get; set; }

		//State
		public VCSConfiguration VCSConfiguration
		{
			get { return _VCSConfiguration; }
			set
			{
				if (value != _VCSConfiguration)
				{
					UIInvoker.Invoke(() =>
					{
						_VCSConfiguration = value;
						TriggerPropertyChanged("VCSConfiguration");
					});
				}
			}
		}
		private VCSConfiguration _VCSConfiguration;

	
		
		//Commands
		
		public ConfigurationViewModel()
		{
		
			ServiceLocator.SetLocatorIfNotSet(() => ServiceLocator.GetServiceLocator());
			UIInvoker = ServiceLocator.Instance.GetInstance<IUIInvoker>();
		
			ApplyDefaultConventions();
		}
	}
		
	public partial class SourceControlViewModel : TinyMVVM.Framework.ViewModelBase
	{
		protected IUIInvoker UIInvoker { get; set; }

		//State
		public ObservableCollection<ChangesetViewModel> Changesets { get; set; } 
	
		
		//Commands
		
		public SourceControlViewModel()
		{
		
			ServiceLocator.SetLocatorIfNotSet(() => ServiceLocator.GetServiceLocator());
			UIInvoker = ServiceLocator.Instance.GetInstance<IUIInvoker>();
		
			ApplyDefaultConventions();
		}
	}
		
	public partial class ChangesetViewModel : TinyMVVM.Framework.ViewModelBase
	{
		protected IUIInvoker UIInvoker { get; set; }

		//State
		public string Comment { get; set; } 
		public string Name { get; set; } 
		public string AvatarUrl { get; set; } 
		public string Revision { get; set; } 
		public string Time { get; set; } 
	
		
		//Commands
		
		public ChangesetViewModel()
		{
		
			ServiceLocator.SetLocatorIfNotSet(() => ServiceLocator.GetServiceLocator());
			UIInvoker = ServiceLocator.Instance.GetInstance<IUIInvoker>();
		
			ApplyDefaultConventions();
		}
	}
		
}