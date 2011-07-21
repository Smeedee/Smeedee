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
                            new DataSetViewModel {Name = "Radiation", Data = new ObservableCollection<DataPointViewModel> { new DataPointViewModel {X = 0, Y = 5}, new DataPointViewModel {X = 1, Y = 1}, new DataPointViewModel {X = 2, Y = 2}, new DataPointViewModel {X = 3, Y = 4}}},
                            //new DataSetViewModel {Name = "", Data = new ObservableCollection<DataPointViewModel> { new DataPointViewModel {X = 0, Y = 2}, new DataPointViewModel {X = 1, Y = 3}, new DataPointViewModel {X = 2, Y = 2}, new DataPointViewModel {X = 3, Y = 3}}}
                        };

            Columns = new ObservableCollection<DataSetViewModel>
                        {
                            new DataSetViewModel {Name = "Aliens", Data = new ObservableCollection<DataPointViewModel> { new DataPointViewModel {X = 1, Y = 1},  new DataPointViewModel {X = 2, Y = 4}}},
                            new DataSetViewModel {Name = "Monsters", Data = new ObservableCollection<DataPointViewModel> { new DataPointViewModel {X = 1, Y = 2},  new DataPointViewModel {X = 2, Y = 3}}}
                        };

            Areas = new ObservableCollection<DataSetViewModel>
                        {
                            new DataSetViewModel {Name = "Area 51", Data = new ObservableCollection<DataPointViewModel> { new DataPointViewModel {X = 0, Y = 2}, new DataPointViewModel {X = 1, Y = 6}, new DataPointViewModel {X = 2, Y = 4}, new DataPointViewModel {X = 3, Y = 4}}},
                            //new DataSetViewModel {Name = "", Data = new ObservableCollection<DataPointViewModel> { new DataPointViewModel {X = 0, Y = 0}, new DataPointViewModel {X = 1, Y = 5}, new DataPointViewModel {X = 2, Y = 2}, new DataPointViewModel {X = 3, Y = 1}}}
                        };

           
            Refresh = new DelegateCommand();
            
            ApplyConvention(new BindCommandsDelegatesToMethods());

        }

    }
}
