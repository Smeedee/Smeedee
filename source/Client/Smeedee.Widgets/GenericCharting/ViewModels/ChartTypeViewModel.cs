using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Widgets.GenericCharting.ViewModels
{
    public partial class ChartTypeViewModel : BasicViewModel
    {
        //State
        public virtual string ChartType
        {
            get
            {
                return _ChartType;
            }
            set
            {
                if (value != _ChartType)
                {
                    _ChartType = value;
                    TriggerPropertyChanged("ChartType");
                }
            }
        }
        private string _ChartType;

    }

}
