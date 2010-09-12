using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Corkboard;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.Widget.Corkboard.ViewModels;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.Corkboard.Controllers
{
    public class CorkboardController
    {
        private readonly IRepository<RetrospectiveNote> _repository;
        private readonly IInvokeBackgroundWorker<IEnumerable<RetrospectiveNote>> _asyncClient;
        private readonly IPersistDomainModelsAsync<RetrospectiveNote> _persistRepository;
        private readonly IDeleteDomainModelsAsync<RetrospectiveNote> _deleteRepository;
        private readonly IProgressbar _progressBar;
        private readonly ILog _logger;
        private readonly IUIInvoker _uiInvoker;
        private readonly CorkboardViewModel _viewModel;
        private readonly CorkboardSettingsViewModel _settingsViewModel;
        private readonly ITimer _refreshNotifier;

        private Configuration _currentConfig;

        private const string SAVING_CONFIG_MESSAGE = "Saving notes";
        private const string LOADING_MESSAGE = "Retrospective notes are loading...";

        public CorkboardController(CorkboardViewModel viewModel,
                                   CorkboardSettingsViewModel settingsViewModel, 
                                   IRepository<RetrospectiveNote> retrospectiveNoteRepository,
                                   IPersistDomainModelsAsync<RetrospectiveNote> persistRetrospectiveNoteRepository, 
                                   IDeleteDomainModelsAsync<RetrospectiveNote> deleteRetrospectiveNoteRepository,
                                   ITimer timer, 
                                   IUIInvoker uiInvoker, 
                                   IInvokeBackgroundWorker<IEnumerable<RetrospectiveNote>> asyncClient, 
                                   ILog logger, 
                                   IProgressbar progressbar,
                                   Configuration config 
            )
        {
            _viewModel = viewModel;
            _settingsViewModel = settingsViewModel;
            _asyncClient = asyncClient;
            _repository = retrospectiveNoteRepository;
            _persistRepository = persistRetrospectiveNoteRepository;
            _uiInvoker = uiInvoker;
            _refreshNotifier = timer;
            _logger = logger;
            _progressBar = progressbar;
            _currentConfig = config;
            _deleteRepository = deleteRetrospectiveNoteRepository;
            
            _persistRepository.SaveCompleted += PersisterSaveCompleted;

            _refreshNotifier.Elapsed += (o, e) => UpdateViewModels();
            
            _settingsViewModel.Save.ExecuteDelegate = Save;
            _settingsViewModel.ReloadSettings.ExecuteDelegate = ReloadSettings;
            
            UpdateViewModels();
        }

        private void PersisterSaveCompleted(object sender, SaveCompletedEventArgs e)
        {
            SetIsNotSaving();
        }

        private void SetIsNotSaving()
        {
            _uiInvoker.Invoke(() => _viewModel.IsSaving = false); 
            _progressBar.HideInSettingsView(); 
        }

        private void UpdateViewModels()
        {
            _asyncClient.RunAsyncVoid(() =>
            {
                SetIsLoadingData();

                    try
                    {
                        var notes = GetAllRetrospectiveNotesInRepository();
                        notes = FilterNotesOnId(notes);
                        _uiInvoker.Invoke(() => AddNotesToViewModels(notes));
                    }
                    catch (Exception exception)
                    {
                        WriteLogError(exception);
                    }

                SetIsNotLoadingData();
            });
        }

        private IEnumerable<RetrospectiveNote> GetAllRetrospectiveNotesInRepository()
        {
            return _repository.Get(new AllSpecification<RetrospectiveNote>());
        }

        private IEnumerable<RetrospectiveNote> FilterNotesOnId(IEnumerable<RetrospectiveNote> allNotes)
        {
            return allNotes.Where(note => note.Id.Equals(_currentConfig.Id.ToString()));
        }

        private void AddNotesToViewModels(IEnumerable<RetrospectiveNote> retrospectiveNotes)
        {
            RemoveNotesFromViewModelsIfViewModelsHaveMoreNotesThanRepo(retrospectiveNotes);
            FillOldNotesOrAddNewNotes(retrospectiveNotes);
            _settingsViewModel.HasChanges = false; 
        }

        private void RemoveNotesFromViewModelsIfViewModelsHaveMoreNotesThanRepo(IEnumerable<RetrospectiveNote> retrospectiveNotes)
        {
            var noOfNegativeNotesFromRepo = FindNumberOfNotesOfGivenType(retrospectiveNotes, NoteType.Negative);
            var noOfPositiveNotesFromRepo = FindNumberOfNotesOfGivenType(retrospectiveNotes, NoteType.Positive);

            _uiInvoker.Invoke(() =>
            {
                while (ViewModelsHasMoreNotesThanRepo(_settingsViewModel.NegativeNotes,
                                                    noOfNegativeNotesFromRepo))
                {
                    RemoveNegativeNoteFromViewModels();
                }
            });

            _uiInvoker.Invoke(() =>
            {
                while (ViewModelsHasMoreNotesThanRepo(_settingsViewModel.PositiveNotes,
                noOfPositiveNotesFromRepo))
                {
                    RemovePositiveNoteFromViewModels();
                }
            });
        }

        private void RemovePositiveNoteFromViewModels()
        {
                _viewModel.PositiveNotes.RemoveAt(_viewModel.PositiveNotes.Count - 1);
                _settingsViewModel.PositiveNotes.RemoveAt(_settingsViewModel.PositiveNotes.Count - 1);
        }

        private void RemoveNegativeNoteFromViewModels()
        {
                _viewModel.NegativeNotes.RemoveAt(_viewModel.NegativeNotes.Count - 1);
                _settingsViewModel.NegativeNotes.RemoveAt(_settingsViewModel.NegativeNotes.Count - 1);
        }

        private static bool ViewModelsHasMoreNotesThanRepo(ObservableCollection<NoteViewModel> viewModelNotes, int noOfNotesFromRepo)
        {
            return viewModelNotes.Count > noOfNotesFromRepo;
        }

        private static int FindNumberOfNotesOfGivenType(IEnumerable<RetrospectiveNote> notes, NoteType type)
        {
            return notes.Count(note => note.Type == type);
        }

        private void FillOldNotesOrAddNewNotes(IEnumerable<RetrospectiveNote> retrospectiveNotes)
        {
            int positiveViewModelBuffer = _settingsViewModel.PositiveNotes.Count;
            int negativeViewModelBuffer = _settingsViewModel.NegativeNotes.Count;

                foreach (var note in retrospectiveNotes)
                {
                    if (CanFillOldNote(note, negativeViewModelBuffer, NoteType.Negative))
                    {
                        FillOldNegativeNote(note, negativeViewModelBuffer);
                        negativeViewModelBuffer--;
                    }

                    else if (CanFillOldNote(note, positiveViewModelBuffer, NoteType.Positive))
                    {
                        FillOldPositiveNote(note, positiveViewModelBuffer);
                        positiveViewModelBuffer--;
                    }

                    else
                    {
                      AddNewNote(note);
                    }
                }
        }

        private void AddNewNote(RetrospectiveNote note)
        {
            _uiInvoker.Invoke(() => 
            {
                _viewModel.AddNote(new NoteViewModel {Description = note.Description, Type = note.Type});
                _settingsViewModel.AddNote(new NoteViewModel{Description = note.Description, Type = note.Type});
            });
        }

        private void FillOldPositiveNote(RetrospectiveNote note, int positiveViewModelBuffer)
        {
            var nextOldNoteToFill = _settingsViewModel.PositiveNotes.Count - positiveViewModelBuffer;

            _uiInvoker.Invoke( () =>
            {
                _settingsViewModel.PositiveNotes[nextOldNoteToFill].Description = note.Description;
                UpdateViewModelFromSettingsViewModel();
            });
        }

        private void FillOldNegativeNote(RetrospectiveNote note, int negativeViewModelBuffer)
        {
            var nextOldNoteToFill = _settingsViewModel.NegativeNotes.Count - negativeViewModelBuffer;

            _uiInvoker.Invoke( () =>
            {
                _settingsViewModel.NegativeNotes[nextOldNoteToFill].Description = note.Description;
                UpdateViewModelFromSettingsViewModel();
            });
        }

        private static bool CanFillOldNote(RetrospectiveNote note, int positiveViewModelBuffer, NoteType type)
        {
            return positiveViewModelBuffer > 0 && note.Type == type;
        }

        private void ReloadSettings()
        {
            UpdateViewModelFromSettingsViewModel();
            UpdateViewModels();
        }

        public void Save()
        {
                SetIsSaving();
                UpdateViewModelFromSettingsViewModel();
                UpdateRepository();
                SetHasNoUnsavedChanges();
        }

        private void SetIsSaving()
        {
            _uiInvoker.Invoke(() =>
            {
                _viewModel.IsSaving = true; 
            });

            _progressBar.ShowInSettingsView(SAVING_CONFIG_MESSAGE);
        }

        private void UpdateViewModelFromSettingsViewModel()
        {
            _uiInvoker.Invoke(() =>
            {
                UpdateNotes(_settingsViewModel.NegativeNotes, _viewModel.NegativeNotes, _viewModel);
                UpdateNotes(_settingsViewModel.PositiveNotes, _viewModel.PositiveNotes, _viewModel);
            });
        }

        private void UpdateSettingsViewModelFromViewModel()
        {
            _uiInvoker.Invoke(() =>
            {
                UpdateNotes(_viewModel.NegativeNotes, _settingsViewModel.NegativeNotes, _settingsViewModel);
                UpdateNotes(_viewModel.PositiveNotes, _settingsViewModel.PositiveNotes, _settingsViewModel);
            });
        }

        private static void UpdateNotes(IEnumerable<NoteViewModel> from, ObservableCollection<NoteViewModel> targetCollection, INoteCollection targetViewModel)
        {

            targetCollection.Clear();
            foreach (var note in from)
            {
                targetViewModel.AddNote(new NoteViewModel { Description = note.Description, Type = note.Type });
            }
        }

        private void UpdateRepository()
        {
            try
            {
                var notes = ConvertToDistinctRetrospectiveNotes(_settingsViewModel.PositiveNotes.Union(_settingsViewModel.NegativeNotes));

                if (notes.Count() == 0)
                    _deleteRepository.Delete(new RetrospectiveNoteByIdSpecification(_currentConfig.Id.ToString()));
                else
                    _persistRepository.Save(notes);
            }
            catch (Exception exception)
            {
                WriteLogError(exception);
            }
        }

        private  IEnumerable<RetrospectiveNote> ConvertToDistinctRetrospectiveNotes(IEnumerable<NoteViewModel> notes)
        {
            var distinctNotes = new List<NoteViewModel>();

            foreach (var note in notes)
            {
                if (!distinctNotes.Select(t => t.Description).Contains(note.Description))
                    distinctNotes.Add(note);
            }

            return distinctNotes.Select(t => new RetrospectiveNote { Description = t.Description, Type = t.Type, Id = _currentConfig.Id.ToString() });
        }

        public void FlippedBackFromSettingsView(int time)
        {
            UpdateSettingsViewModelFromViewModel();
            SetHasNoUnsavedChanges();
            _refreshNotifier.Start(time);
        }

        private void SetHasNoUnsavedChanges()
        {
             _uiInvoker.Invoke( () => _settingsViewModel.HasChanges = false); 
        }

        public void Start(int time)
        {
            _refreshNotifier.Start(time);
        }

        public void Stop()
        {
            _refreshNotifier.Stop();
        }

        private void SetIsLoadingData()
        {
            _uiInvoker.Invoke(() =>
            {
                _viewModel.IsLoading = true;
                _settingsViewModel.IsLoading = true; ;
                                      
            });

            _progressBar.ShowInBothViews(LOADING_MESSAGE);
        }

        private void SetIsNotLoadingData()
        {
            _uiInvoker.Invoke(() =>
            {
                _viewModel.IsLoading = false;
                _settingsViewModel.IsLoading = false;  
            });

            _progressBar.HideInBothViews();
        }

        private void WriteLogError(Exception exception)
        {
            _logger.WriteEntry(new ErrorLogEntry
            {
                Message = exception.ToString(),
                Source = GetType().ToString(),
                TimeStamp = DateTime.Now
            });
        }

        public void ConfigurationChanged(Configuration newConfig)
        {
            _currentConfig = newConfig;
        }

        public void BeforeSaving()
        {
            _currentConfig.ChangeSetting("IsDefault", "No");
            _currentConfig.IsConfigured = true;
        }
    }
}