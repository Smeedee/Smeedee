﻿using System.Collections.ObjectModel;

namespace Smeedee.Widgets.WebSnapshot.ViewModel
{
    public partial class WebSnapshotSettingsViewModel

    {

        partial void OnInitialize()
        {
                       
        }


        public bool CanSave()
        {
            return true;
        }

        public bool IsSaving
        {
            get { return isSaving; }
            set
            {
                if (value != isSaving)
                {
                    isSaving = value;
                    TriggerPropertyChanged("IsSaving");
                }
            }
        }
        private bool isSaving;
    }
}
