
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework;
using System.Collections.ObjectModel;
using Smeedee.DomainModel.Corkboard;

namespace Smeedee.Widget.Corkboard.ViewModels
{
	public partial class NoteViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public string Description
		{
			get { return _Description; }
			set
			{
				if (value != _Description)
				{
					_Description = value;
					TriggerPropertyChanged("Description");
				}
			}
		}
		private string _Description;

		public NoteType Type
		{
			get { return _Type; }
			set
			{
				if (value != _Type)
				{
					_Type = value;
					TriggerPropertyChanged("Type");
				}
			}
		}
		private NoteType _Type;

	
		
		//Commands
		public DelegateCommand Delete { get; set; }
		public DelegateCommand MoveUp { get; set; }
		public DelegateCommand MoveDown { get; set; }
		
		public NoteViewModel()
		{
			Delete = new DelegateCommand();
			MoveUp = new DelegateCommand();
			MoveDown = new DelegateCommand();
	
			ApplyDefaultConventions();
		}
	}
		
}