
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Conventions;
using TinyMVVM.Framework;
using System.Collections.ObjectModel;
using Smeedee.DomainModel.Corkboard;
using Smeedee.Client.Framework.ViewModel;
namespace Smeedee.Widgets.Corkboard.ViewModel
{
	public partial class CorkboardViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual ObservableCollection<NoteViewModel> PositiveNotes
		{
			get 
			{
				OnGetPositiveNotes(ref _PositiveNotes);
				 
				return _PositiveNotes; 
			}
			set
			{
				if (value != _PositiveNotes)
				{
					OnSetPositiveNotes(ref value); 
					_PositiveNotes = value;
					TriggerPropertyChanged("PositiveNotes");
				}
			}
		}
		private ObservableCollection<NoteViewModel> _PositiveNotes;

		partial void OnGetPositiveNotes(ref ObservableCollection<NoteViewModel> value);
		partial void OnSetPositiveNotes(ref ObservableCollection<NoteViewModel> value);

		public virtual ObservableCollection<NoteViewModel> NegativeNotes
		{
			get 
			{
				OnGetNegativeNotes(ref _NegativeNotes);
				 
				return _NegativeNotes; 
			}
			set
			{
				if (value != _NegativeNotes)
				{
					OnSetNegativeNotes(ref value); 
					_NegativeNotes = value;
					TriggerPropertyChanged("NegativeNotes");
				}
			}
		}
		private ObservableCollection<NoteViewModel> _NegativeNotes;

		partial void OnGetNegativeNotes(ref ObservableCollection<NoteViewModel> value);
		partial void OnSetNegativeNotes(ref ObservableCollection<NoteViewModel> value);

	
		
		//Commands
		
		public CorkboardViewModel()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Widgets.Corkboard.ViewModel
{
	public partial class CorkboardSettingsViewModel : SettingsViewModelBase
	{
		//State
		public virtual ObservableCollection<NoteViewModel> PositiveNotes
		{
			get 
			{
				OnGetPositiveNotes(ref _PositiveNotes);
				 
				return _PositiveNotes; 
			}
			set
			{
				if (value != _PositiveNotes)
				{
					OnSetPositiveNotes(ref value); 
					_PositiveNotes = value;
					TriggerPropertyChanged("PositiveNotes");
				}
			}
		}
		private ObservableCollection<NoteViewModel> _PositiveNotes;

		partial void OnGetPositiveNotes(ref ObservableCollection<NoteViewModel> value);
		partial void OnSetPositiveNotes(ref ObservableCollection<NoteViewModel> value);

		public virtual ObservableCollection<NoteViewModel> NegativeNotes
		{
			get 
			{
				OnGetNegativeNotes(ref _NegativeNotes);
				 
				return _NegativeNotes; 
			}
			set
			{
				if (value != _NegativeNotes)
				{
					OnSetNegativeNotes(ref value); 
					_NegativeNotes = value;
					TriggerPropertyChanged("NegativeNotes");
				}
			}
		}
		private ObservableCollection<NoteViewModel> _NegativeNotes;

		partial void OnGetNegativeNotes(ref ObservableCollection<NoteViewModel> value);
		partial void OnSetNegativeNotes(ref ObservableCollection<NoteViewModel> value);

		public virtual bool CanAddPositive
		{
			get 
			{
				OnGetCanAddPositive(ref _CanAddPositive);
				 
				return _CanAddPositive; 
			}
			set
			{
				if (value != _CanAddPositive)
				{
					OnSetCanAddPositive(ref value); 
					_CanAddPositive = value;
					TriggerPropertyChanged("CanAddPositive");
				}
			}
		}
		private bool _CanAddPositive;

		partial void OnGetCanAddPositive(ref bool value);
		partial void OnSetCanAddPositive(ref bool value);

		public virtual bool CanAddNegative
		{
			get 
			{
				OnGetCanAddNegative(ref _CanAddNegative);
				 
				return _CanAddNegative; 
			}
			set
			{
				if (value != _CanAddNegative)
				{
					OnSetCanAddNegative(ref value); 
					_CanAddNegative = value;
					TriggerPropertyChanged("CanAddNegative");
				}
			}
		}
		private bool _CanAddNegative;

		partial void OnGetCanAddNegative(ref bool value);
		partial void OnSetCanAddNegative(ref bool value);

	
		
		//Commands
		public DelegateCommand AddPositiveNote { get; set; }
		public DelegateCommand AddNegativeNote { get; set; }
		
		public CorkboardSettingsViewModel()
		{
			AddPositiveNote = new DelegateCommand();
			AddNegativeNote = new DelegateCommand();
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Widgets.Corkboard.ViewModel
{
	public partial class NoteViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual string Description
		{
			get 
			{
				OnGetDescription(ref _Description);
				 
				return _Description; 
			}
			set
			{
				if (value != _Description)
				{
					OnSetDescription(ref value); 
					_Description = value;
					TriggerPropertyChanged("Description");
				}
			}
		}
		private string _Description;

		partial void OnGetDescription(ref string value);
		partial void OnSetDescription(ref string value);

		public virtual NoteType Type
		{
			get 
			{
				OnGetType(ref _Type);
				 
				return _Type; 
			}
			set
			{
				if (value != _Type)
				{
					OnSetType(ref value); 
					_Type = value;
					TriggerPropertyChanged("Type");
				}
			}
		}
		private NoteType _Type;

		partial void OnGetType(ref NoteType value);
		partial void OnSetType(ref NoteType value);

	
		
		//Commands
		public DelegateCommand Delete { get; set; }
		public DelegateCommand MoveUp { get; set; }
		public DelegateCommand MoveDown { get; set; }
		
		public NoteViewModel()
		{
			Delete = new DelegateCommand();
			MoveUp = new DelegateCommand();
			MoveDown = new DelegateCommand();
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

