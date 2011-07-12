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
    public partial class ChartViewModel : AbstractViewModel
    {
        //State
        public virtual ObservableCollection<DataPointViewModel> Data
        {
            get
            {
                OnGetData(ref _Data);

                return _Data;
            }
            set
            {
                OnSetData(ref value);
                _Data = value;
            }
        }

        private ObservableCollection<DataPointViewModel> _Data;
        partial void OnGetData(ref ObservableCollection<DataPointViewModel> value);
        partial void OnSetData(ref ObservableCollection<DataPointViewModel> value);


        //Commands
        public DelegateCommand Refresh { get; set; }

        public ChartViewModel()
        {
            Refresh = new DelegateCommand();

            OnInitialize();
            ApplyConvention(new BindCommandsDelegatesToMethods());
        }

        partial void OnInitialize();
    }
}
