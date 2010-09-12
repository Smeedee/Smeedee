using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.DomainModel.Framework;
using TinyMVVM.Framework;

#if SILVERLIGHT
using Smeedee.Client.Framework.SL.Views.Dialogs;
#else
using Smeedee.Client.Framework.Views.Dialogs;
#endif

namespace Smeedee.Client.Framework.ViewModel.Dialogs
{
    public partial class EditSlideshowDialog
    {
        private int selectedSlideIndex;
        private Slide selectedSlide;

        partial void OnInitialize()
        {
            Title = "Edit slideshow";
            DisplayCancelButton = false;

            SetCommandsTextAndDescription();
            AddCommandsToButtonBar();

            View = new EditSlideshowDialogView() { DataContext = this };

            PropertyChanged += EditSlideshowDialog_PropertyChanged;
        }

        private void SetCommandsTextAndDescription()
        {
            Delete.Text = "Delete";
            Delete.Description = "Delete slide from Slideshow";

            MoveRight.Text = " -> ";
            MoveRight.Description = "Move the selected Slide one position to the right";

            MoveLeft.Text = " <- ";
            MoveLeft.Description = "Move the selected Slide one position to the left";
        }

        private void AddCommandsToButtonBar()
        {
            ButtonBarCommands = new ObservableCollection<DelegateCommand>();
            ButtonBarCommands.Add(MoveLeft);
            ButtonBarCommands.Add(MoveRight);
            ButtonBarCommands.Add(Delete);
        }

        void EditSlideshowDialog_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Slideshow" && 
                Slideshow != null)
            {
                Slideshow.PropertyChanged += Slideshow_PropertyChanged;
            }
        }

        void Slideshow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentSlide")
            {
                foreach (var buttonBarCommand in ButtonBarCommands)
                {
                    buttonBarCommand.TriggerCanExecuteChanged();
                }
            }
        }

        public bool CanDelete()
		{
			return SlideIsSelected();
		}

		private bool SlideIsSelected()
		{
			return Slideshow.CurrentSlide != null;
		}

		public void OnDelete()
		{
		    if (!SlideIsSelected()) return;
		    var index = Slideshow.Slides.IndexOf(Slideshow.CurrentSlide);
		    Slideshow.Slides.Remove(Slideshow.CurrentSlide);
		    Slideshow.CurrentSlide = Slideshow.Slides.Count > 0 ? Slideshow.Slides[Math.Min(index, Slideshow.Slides.Count - 1)] : null;
		}

		public bool CanMoveLeft()
		{
			return SlideIsSelected();
		}

		public bool CanMoveRight()
		{
			return SlideIsSelected();
		}

        public void OnMoveRight()
        {
            CopySelectedSlideFromSlideshow();
            RemoveSelectedSlideFromSlideshow();

            if (SelectedSlideIsNotTheLastSlideInSlideshow())
                InsertSlideInSlideshow(selectedSlideIndex += 1, selectedSlide);
            else
                InsertSlideInSlideshow(0, selectedSlide);

            SetSelectedSlideInSlideshow();
        }

        private void CopySelectedSlideFromSlideshow()
        {
            selectedSlideIndex = Slideshow.Slides.IndexOf(Slideshow.CurrentSlide);
            selectedSlide = Slideshow.CurrentSlide;
        }

        private void RemoveSelectedSlideFromSlideshow()
        {
            Slideshow.Slides.Remove(Slideshow.CurrentSlide);
        }

        private bool SelectedSlideIsNotTheLastSlideInSlideshow()
        {
            return selectedSlideIndex < Slideshow.Slides.Count;
        }

        private void InsertSlideInSlideshow(int index, Slide slide)
        {
            Slideshow.Slides.Insert(index, slide);
        }

        private void SetSelectedSlideInSlideshow()
        {
            Slideshow.CurrentSlide = selectedSlide;
        }

        public void OnMoveLeft()
        {
            CopySelectedSlideFromSlideshow();
            RemoveSelectedSlideFromSlideshow();

            if (SelectedSlideIsNotTheFirstSlideInSlideshow())
                InsertSlideInSlideshow(selectedSlideIndex -= 1, selectedSlide);
            else
                InsertSlideInSlideshow(Slideshow.Slides.Count, selectedSlide);

            SetSelectedSlideInSlideshow();
        }

        private bool SelectedSlideIsNotTheFirstSlideInSlideshow()
        {
            return selectedSlideIndex > 0;
        }
    }
}
