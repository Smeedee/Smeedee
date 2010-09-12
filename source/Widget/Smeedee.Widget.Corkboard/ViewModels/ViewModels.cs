
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework;
using System.Collections.ObjectModel;
using Smeedee.DomainModel.Corkboard;

namespace Smeedee.Widget.Corkboard.ViewModels
{
	public partial class CorkboardViewModel : TinyMVVM.Framework.ViewModelBase
	{
		protected IUIInvoker UIInvoker { get; set; }

		//State
		public ObservableCollection<NoteViewModel> PositiveNotes
		{
			get { return _PositiveNotes; }
			set
			{
				if (value != _PositiveNotes)
				{
					_PositiveNotes = value;
					TriggerPropertyChanged("PositiveNotes");
				}
			}
		}
		private ObservableCollection<NoteViewModel> _PositiveNotes;

		public ObservableCollection<NoteViewModel> NegativeNotes
		{
			get { return _NegativeNotes; }
			set
			{
				if (value != _NegativeNotes)
				{
					_NegativeNotes = value;
					TriggerPropertyChanged("NegativeNotes");
				}
			}
		}
		private ObservableCollection<NoteViewModel> _NegativeNotes;

		public bool CanAddPositive
		{
			get { return _CanAddPositive; }
			set
			{
				if (value != _CanAddPositive)
				{
					_CanAddPositive = value;
					TriggerPropertyChanged("CanAddPositive");
				}
			}
		}
		private bool _CanAddPositive;

		public bool CanAddNegative
		{
			get { return _CanAddNegative; }
			set
			{
				if (value != _CanAddNegative)
				{
					_CanAddNegative = value;
					TriggerPropertyChanged("CanAddNegative");
				}
			}
		}
		private bool _CanAddNegative;

	
		
		//Commands
		public DelegateCommand AddPositiveNote { get; set; }
		public DelegateCommand AddNegativeNote { get; set; }
		
		public CorkboardViewModel()
		{
			AddPositiveNote = new DelegateCommand();
			AddNegativeNote = new DelegateCommand();
		
			ServiceLocator.SetLocatorIfNotSet(() => ServiceLocator.GetServiceLocator());
			UIInvoker = ServiceLocator.Instance.GetInstance<IUIInvoker>();
		
			ApplyDefaultConventions();
		}
	}
}