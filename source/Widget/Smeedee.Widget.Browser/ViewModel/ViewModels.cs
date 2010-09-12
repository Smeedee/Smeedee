
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Widget.Browser.ViewModel
{
	public partial class Browser : Slide
	{
		protected IUIInvoker UIInvoker { get; set; }

		//State
		public string Url
		{
			get { return _Url; }
			set
			{
				if (value != _Url)
				{
					_Url = value;
					TriggerPropertyChanged("Url");
				}
			}
		}
		private string _Url;

	
		
		//Commands
		
		public Browser()
		{
		
			ServiceLocator.SetLocatorIfNotSet(() => ServiceLocator.GetServiceLocator());
			UIInvoker = ServiceLocator.Instance.GetInstance<IUIInvoker>();
		
			ApplyDefaultConventions();
		}
	}
		
}