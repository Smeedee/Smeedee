using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Widget.SourceControl.ViewModels
{
    public class RevisionCounterViewModel : AbstractViewModel
    {
        public TopCommitersViewModel topCommitersViewModel;

        private int revisionCount;
        
        public RevisionCounterViewModel(TopCommitersViewModel topCommitersViewModel)
        {
            this.topCommitersViewModel = topCommitersViewModel;
            topCommitersViewModel.Developers.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(DevelopersCollectionChanged);
        }

        public RevisionCounterViewModel()
        {

        }

        public void DevelopersCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var developer in topCommitersViewModel.Developers)
            {
                developer.PropertyChanged -= new PropertyChangedEventHandler(DeveloperListener);
                developer.PropertyChanged += new PropertyChangedEventHandler(DeveloperListener);
            }
            SetRevisionCount();
        }

        private void DeveloperListener(object sender, PropertyChangedEventArgs e)
        {
            SetRevisionCount();   
        }

        public int RevisionCount
        {
            get
            {
                return revisionCount;
            }
            set
            {
                if (value != revisionCount)
                {
                    revisionCount = value;
                    TriggerPropertyChanged<RevisionCounterViewModel>(vm => vm.RevisionCount);
                }
            }
        }

        private void SetRevisionCount()
        {
            var totalNumberOfRevisions = 0;

            foreach (var developer in topCommitersViewModel.Developers)
            {
                totalNumberOfRevisions += developer.NumberOfCommits;
            }
            RevisionCount = totalNumberOfRevisions;
        }
    }
}


