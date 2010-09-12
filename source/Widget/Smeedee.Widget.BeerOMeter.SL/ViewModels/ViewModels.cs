
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Conventions;
using TinyMVVM.Framework;
using Smeedee.Client.Framework.ViewModel;
namespace Smeedee.Widget.BeerOMeter.SL.ViewModels
{
	public partial class BeerOMeterViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual int NumberOfBeers
		{
			get 
			{
				OnGetNumberOfBeers(ref _NumberOfBeers);
				 
				return _NumberOfBeers; 
			}
			set
			{
				if (value != _NumberOfBeers)
				{
					OnSetNumberOfBeers(ref value); 
					_NumberOfBeers = value;
					TriggerPropertyChanged("NumberOfBeers");
				}
			}
		}
		private int _NumberOfBeers;

		partial void OnGetNumberOfBeers(ref int value);
		partial void OnSetNumberOfBeers(ref int value);

	
		
		//Commands
		public DelegateCommand Reset { get; set; }
		public DelegateCommand AnimationCompleted { get; set; }
		public DelegateCommand Save { get; set; }
		
		public BeerOMeterViewModel()
		{
			Reset = new DelegateCommand();
			AnimationCompleted = new DelegateCommand();
			Save = new DelegateCommand();
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

