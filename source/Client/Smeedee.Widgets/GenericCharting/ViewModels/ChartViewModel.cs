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

        public ObservableCollection<DataSetViewModel> Lines { get; set; }
        public ObservableCollection<DataSetViewModel> Columns { get; set; }
        public ObservableCollection<DataSetViewModel> Areas { get; set; }

        //Commands
        public DelegateCommand Refresh { get; set; }

        public ChartViewModel()
        {
            // Fills collections with sample data, that should be removed when proper loading of data is implemented
            Lines = new ObservableCollection<DataSetViewModel>
                        {
                            new DataSetViewModel {Name = "Test", Data = new ObservableCollection<DataPointViewModel> { new DataPointViewModel {X = 1, Y = 1},  new DataPointViewModel {X = 2, Y = 2}}},
                            new DataSetViewModel {Name = "Test2", Data = new ObservableCollection<DataPointViewModel> { new DataPointViewModel {X = 1, Y = 3},  new DataPointViewModel {X = 2, Y = 1}}}
                        };

            Columns = new ObservableCollection<DataSetViewModel>
                        {
                            new DataSetViewModel {Name = "Test3", Data = new ObservableCollection<DataPointViewModel> { new DataPointViewModel {X = 1, Y = 3},  new DataPointViewModel {X = 2, Y = 1}}},
                            new DataSetViewModel {Name = "Test4", Data = new ObservableCollection<DataPointViewModel> { new DataPointViewModel {X = 1, Y = 5},  new DataPointViewModel {X = 2, Y = 2}}}
                        };

            Areas = new ObservableCollection<DataSetViewModel>
                        {
                            new DataSetViewModel {Name = "Test5", Data = new ObservableCollection<DataPointViewModel> { new DataPointViewModel {X = 1, Y = 6},  new DataPointViewModel {X = 2, Y = 4}}},
                            new DataSetViewModel {Name = "Test6", Data = new ObservableCollection<DataPointViewModel> { new DataPointViewModel {X = 1, Y = 5},  new DataPointViewModel {X = 2, Y = 2}}}
                        };

           
            Refresh = new DelegateCommand();
            
            ApplyConvention(new BindCommandsDelegatesToMethods());

        }

    }
}
