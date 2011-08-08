using System.Collections.ObjectModel;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Widgets.GenericCharting.ViewModels
{
    public class DataSetViewModel : BasicViewModel
    {
        public DataSetViewModel()
        {
            Data = new ObservableCollection<DataPointViewModel>();

        }
        public virtual string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        private string _Name;

        public ObservableCollection<DataPointViewModel> Data
        {
            get
            {
                return _Data;
            }
            set
            {
                _Data = value;
            }
        }

        private ObservableCollection<DataPointViewModel> _Data;

        public virtual string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }

        private string _Type;
 
        public virtual string Brush
        {
            get
            {

                return _Brush;
            }
            set
            {
                if (value != _Brush)
                {
                    _Brush = value;
                    TriggerPropertyChanged("Brush");
                }
            }
        }
        private string _Brush;



 
    }
}
