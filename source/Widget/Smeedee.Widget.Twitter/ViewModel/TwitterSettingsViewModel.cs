using System;
using System.ComponentModel;
using Smeedee.Client.Framework.ViewModel;
using TinyMVVM.Framework;

namespace Smeedee.Widget.Twitter.ViewModel
{
    public class TwitterSettingsViewModel : SettingsViewModelBase, IDataErrorInfo
    {
        public TwitterViewModel ViewModel{ get; set;}

        public DelegateCommand Search { get; set;}

        public const int MIN_SEARCH_STRING_LENGTH = 3;
        public const string VALIDATION_ERROR_STRING = "The search string is too short";
        private string validationString;

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                Search.TriggerCanExecuteChanged();
                Save.TriggerCanExecuteChanged();
                ReloadSettings.TriggerCanExecuteChanged();
            }
        }

        private string searchString;
        public string SearchString
        {
            get { return searchString; }
            set
            {
                if (value != searchString)
                {
                    searchString = value;
                    if (!IsSearchStringLongEnough())
                        SetValidationError();
                    else
                        ClearValidationError();

                    HasChanges = true;
                    TriggerPropertyChanged("SearchString");
                    Search.TriggerCanExecuteChanged();
                    Save.TriggerCanExecuteChanged();
                }
            }
        }

        private int numberOfTweetsToDisplay;
        public int NumberOfTweetsToDisplay
        {
            get { return numberOfTweetsToDisplay; }
            set
            {
                if (value != numberOfTweetsToDisplay)
                {
                    numberOfTweetsToDisplay = value;
                    HasChanges = true;
                    TriggerPropertyChanged("NumberOfTweetsToDisplay");
                }
            }
        }

        public int MaximumNumberOfTweets
        {
            get { return 25;}
        }

        public int MinimumNumberOfTweets
        {
            get { return 1; }
        }


        private TimeSpan refreshInterval;
        public TimeSpan RefreshInterval
        {
            get
            {
                return refreshInterval;
            }
            set
            {
                if (value != refreshInterval)
                {
                    refreshInterval = value;
                    HasChanges = true;
                    TriggerPropertyChanged("RefreshInterval");   
                }
            }
        }   

        public TimeSpan MaximumRefreshInterval
        {
            get { return new TimeSpan(0, 59, 59); }
        }

        public TimeSpan MinimumRefreshInterval
        {
            get { return new TimeSpan(0, 0, 20); }
        }

        public bool CanSearchOrSave()
        {
            return IsSearchStringLongEnough() && !IsLoading;
        }

        public bool CanReloadSettings()
        {
            return !IsLoading;
        }

        private bool IsSearchStringLongEnough()
        {
            return searchString.Length >= MIN_SEARCH_STRING_LENGTH;
        }

        private void ClearValidationError()
        {
            validationString = null;
        }

        private void SetValidationError()
        {
            validationString = VALIDATION_ERROR_STRING;
        }

        public string this[string columnName]
        {
            get { return validationString; }
        }

        string IDataErrorInfo.Error
        {
            get { throw new NotImplementedException(); }
        }

        public TwitterSettingsViewModel(TwitterViewModel viewModel)
        {
            SearchStringIsNotNull();
            ViewModel = viewModel;
            Search = new DelegateCommand {CanExecuteDelegate = CanSearchOrSave};
            Save.CanExecuteDelegate = CanSearchOrSave;
            ReloadSettings.CanExecuteDelegate = CanReloadSettings;
        }


        private void SearchStringIsNotNull()
        {
            searchString = "";
        }
    }
}
