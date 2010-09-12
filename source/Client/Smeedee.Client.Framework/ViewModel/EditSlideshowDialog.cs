using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
#if SILVERLIGHT
using Smeedee.Client.Framework.SL.Views.Dialogs;
#else
using Smeedee.Client.Framework.Views.Dialogs;
#endif

namespace Smeedee.Client.Framework.ViewModel
{
    public partial class EditSlideshowDialog
    {
        public void OnInitialize()
        {
            Title = "Edit slideshow";
			
			ButtonBarCommands = new ObservableCollection<ICommand>();
			ButtonBarCommands.Add(MoveLeft);
			ButtonBarCommands.Add(MoveRight);
			ButtonBarCommands.Add(Delete);

            View = new EditSlideshowDialogView() { DataContext = this };
        }

		public bool CanDelete()
		{
			return SlideIsSelected();
		}

		private bool SlideIsSelected()
		{
			return SelectedSlide != null;
		}

		public void OnDelete()
		{
			if (SlideIsSelected())
			{
				Slideshow.Slides.Remove(SelectedSlide);
			}
		}

		public bool CanMoveLeft()
		{
			return SlideIsSelected();
		}

		public bool CanMoveRight()
		{
			return SlideIsSelected();
		}
    }
}
