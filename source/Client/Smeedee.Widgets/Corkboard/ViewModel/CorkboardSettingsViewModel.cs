using System;
using System.Collections.ObjectModel;
using System.Linq;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Corkboard;

namespace Smeedee.Widgets.Corkboard.ViewModel
{
    public partial class CorkboardSettingsViewModel : SettingsViewModelBase, INoteCollection
    {
        public const int MAXIMUM_NUMBER_OF_NOTES = 8;

        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                if (value != isLoading)
                {
                    isLoading = value;
                    TriggerPropertyChanged<CorkboardSettingsViewModel>(t => t.IsLoading);
                    CanAddNotes(null, null);
                }
            }
        }
        private bool isLoading;

        partial void OnInitialize()
        {
            PositiveNotes = new ObservableCollection<NoteViewModel>();
            NegativeNotes = new ObservableCollection<NoteViewModel>();

            SetEventHandlers();
            SetExecuteDelegates();
            MakeSaveAndReloadFollowHasChanges();
        }

        private void SetEventHandlers()
        {
            PositiveNotes.CollectionChanged += CanAddNotes;
            NegativeNotes.CollectionChanged += CanAddNotes;

            PositiveNotes.CollectionChanged += SetHasChanges;
            NegativeNotes.CollectionChanged += SetHasChanges;
        }

        private void SetExecuteDelegates()
        {
            Save.CanExecuteDelegate = () => HasChanges && !IsLoading;
            ReloadSettings.CanExecuteDelegate = () => HasChanges;
        }

        private void MakeSaveAndReloadFollowHasChanges()
        {
            PropertyChanged += (o, e) => { if (e.PropertyName.Equals("HasChanges")) Save.TriggerCanExecuteChanged(); };
            PropertyChanged += (o, e) => { if (e.PropertyName.Equals("HasChanges")) ReloadSettings.TriggerCanExecuteChanged(); };
        }

        private void SetHasChanges(object sender, EventArgs e)
        {
            HasChanges = true;
        }

        private void CanAddNotes(object sender, EventArgs e)
        {
            CanAddPositiveNote();
            CanAddNegativeNote();
        }

        public bool CanAddPositiveNote()
        {
            CanAddPositive = PositiveNotes.Count < MAXIMUM_NUMBER_OF_NOTES 
                          && PositiveNotes.Where(t => t.Description == "").Count() < 1
                          && !IsLoading;
            return CanAddPositive;
        }

        public bool CanAddNegativeNote()
        {
            CanAddNegative = NegativeNotes.Count < MAXIMUM_NUMBER_OF_NOTES 
                          && NegativeNotes.Where(t => t.Description == "").Count() < 1
                          && !IsLoading;
            return CanAddNegative;
        }

        public void OnAddPositiveNote()
        {
            AddNote(new NoteViewModel { Description = "", Type = NoteType.Positive });
        }

        public void OnAddNegativeNote()
        {
            AddNote(new NoteViewModel { Description = "", Type = NoteType.Negative });
        }

        public void AddNote(NoteViewModel note)
        {
            SubscribeToNoteEvents(note);

            var notes = note.Type == NoteType.Positive ? PositiveNotes : NegativeNotes;

            notes.Add(note);
        }

        private void SubscribeToNoteEvents(NoteViewModel note)
        {
            note.DeleteNote += DeleteNote;
            note.Move += MoveNote;
            note.PropertyChanged += CanAddNotes;
            note.PropertyChanged += SetHasChanges;
        }

        private void UnsubscribeToNoteEvents(NoteViewModel note)
        {
            note.DeleteNote -= DeleteNote;
            note.Move -= MoveNote;
            note.PropertyChanged -= CanAddNotes;
            note.PropertyChanged -= SetHasChanges;
        }

        private void MoveNote(object sender, MoveArgs args)
        {
            var note = (NoteViewModel)sender;
            var notes = note.Type == NoteType.Positive ? PositiveNotes : NegativeNotes;
            int index = notes.IndexOf(note);

            if (args.MoveDirection == MoveArgs.Direction.UP && index != 0)
                notes.UpdateNotePosition(index, --index);

            if (args.MoveDirection == MoveArgs.Direction.DOWN && index != notes.LastIndex())
                notes.UpdateNotePosition(index, ++index);
        }

        private void DeleteNote(object sender, EventArgs args)
        {
            var note = (NoteViewModel)sender;
            UnsubscribeToNoteEvents(note);
            var notes = note.Type == NoteType.Positive ? PositiveNotes : NegativeNotes;
            notes.Remove(note);
        }
    }

    public static class Extension
    {
        public static void UpdateNotePosition(this ObservableCollection<NoteViewModel> collection, int oldIndex, int newIndex)
        {
            var tmp = collection[oldIndex];
            collection[oldIndex] = collection[newIndex];
            collection[newIndex] = tmp;
        }

        public static int LastIndex(this ObservableCollection<NoteViewModel> collection)
        {
            return collection.Count - 1;
        }
    }
}