using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.ViewModel;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Conventions;

namespace Smeedee.Widgets.GenericCharting.ViewModels
{
    public class ChartViewModel : AbstractViewModel
    {

        public ObservableCollection<DataSetViewModel> Lines { get; private set; }
        public ObservableCollection<DataSetViewModel> Columns { get; private set; }
        public ObservableCollection<DataSetViewModel> Areas { get; private set; }

        //Commands
        public DelegateCommand Refresh { get; set; }

        public ChartViewModel()
        {
            Lines = new ObservableCollection<DataSetViewModel>();
            Columns = new ObservableCollection<DataSetViewModel>();
            Areas = new ObservableCollection<DataSetViewModel>();

            Refresh = new DelegateCommand();
            
            ApplyConvention(new BindCommandsDelegatesToMethods());

        }


        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                if (value != errorMessage)
                {
                    errorMessage = value;
                    TriggerPropertyChanged("ErrorMessage");
                }
            }
        }

        private bool showErrorMessageInsteadOfChart=false;

        public bool ShowErrorMessageInsteadOfChart
        {
            get { return showErrorMessageInsteadOfChart; }
            set
            {
                if (value != showErrorMessageInsteadOfChart)
                {
                    showErrorMessageInsteadOfChart = value;
                    TriggerPropertyChanged("ShowErrorMessageInsteadOfChart");
                }
            }
        }

    }
}
