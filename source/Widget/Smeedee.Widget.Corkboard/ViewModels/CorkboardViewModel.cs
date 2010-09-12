using System.Collections.ObjectModel;
using System.Linq;
using Smeedee.DomainModel.Corkboard;

namespace Smeedee.Widget.Corkboard.ViewModels
{
    public partial class CorkboardViewModel : INoteCollection
    {
        public void OnInitialize()
        {
            PositiveNotes = new ObservableCollection<NoteViewModel>();
            NegativeNotes = new ObservableCollection<NoteViewModel>();
        }

        public void AddNote(NoteViewModel note)
        {
            var notes = note.Type == NoteType.Positive ? PositiveNotes : NegativeNotes;

            if (NoteDoesNotAlreadyExist(note))
                notes.Add(note);
        }

        private bool NoteDoesNotAlreadyExist(NoteViewModel note)
        {
            var notes = note.Type == NoteType.Positive ? PositiveNotes : NegativeNotes;

            return notes.Where(t => t.Description == note.Description).Count() < 1;
        }

        private bool isSaving;

        public bool IsSaving
        {
            get { return isSaving; }
            set
            {
                if (value != isSaving)
                {
                    isSaving = value;
                    TriggerPropertyChanged<CorkboardViewModel>(t => t.IsSaving);
                }
            }
        }

        private bool isLoading;

        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                if (value != isLoading)
                {
                    isLoading = value;
                    TriggerPropertyChanged<CorkboardViewModel>(t => t.IsLoading);
                }
            }
        }
    }
}
