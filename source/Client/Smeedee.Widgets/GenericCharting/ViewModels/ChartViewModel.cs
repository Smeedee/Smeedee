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

        public ObservableCollection<DataSetViewModel> Lines { get; set; }

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
            Lines = new ObservableCollection<DataSetViewModel>
                        {
                            new DataSetViewModel {Name = "Test", Data = new ObservableCollection<DataPointViewModel> { new DataPointViewModel {X = 1, Y = 1},  new DataPointViewModel {X = 2, Y = 2}}},
                            new DataSetViewModel {Name = "Test2", Data = new ObservableCollection<DataPointViewModel> { new DataPointViewModel {X = 1, Y = 3},  new DataPointViewModel {X = 2, Y = 1}}}
                        };
            Data = new ObservableCollection<DataPointViewModel>();
            
            Refresh = new DelegateCommand();

            OnInitialize();
            
            ApplyConvention(new BindCommandsDelegatesToMethods());
            
            
        }

        partial void OnInitialize();
    }
}
