using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Widgets.GenericCharting.ViewModels
{
    public partial class SeriesConfigViewModel : BasicViewModel
    {
        //State
        public virtual string Action
        {
            get
            {
                OnGetAction(ref _Action);

                return _Action;
            }
            set
            {
                if (value != _Action)
                {
                    OnSetAction(ref value);
                    _Action = value;
                    TriggerPropertyChanged("Action");
                }
            }
        }
        private string _Action;

        partial void OnGetAction(ref string value);
        partial void OnSetAction(ref string value);

        public virtual string Name
        {
            get
            {
                OnGetName(ref _Name);

                return _Name;
            }
            set
            {
                if (value != _Name)
                {
                    OnSetName(ref value);
                    _Name = value;
                    TriggerPropertyChanged("Name");
                }
            }
        }
        private string _Name;

        partial void OnGetName(ref string value);
        partial void OnSetName(ref string value);

        public virtual string Legend
        {
            get
            {
                OnGetLegend(ref _Legend);

                return _Legend;
            }
            set
            {
                if (value != _Legend)
                {
                    OnSetLegend(ref value);
                    _Legend = value;
                    TriggerPropertyChanged("Legend");
                }
            }
        }
        private string _Legend;

        partial void OnGetLegend(ref string value);
        partial void OnSetLegend(ref string value);

        public virtual string ChartType
        {
            get
            {
                OnGetChartType(ref _ChartType);

                return _ChartType;
            }
            set
            {
                if (value != _ChartType)
                {
                    OnSetChartType(ref value);
                    _ChartType = value;
                    TriggerPropertyChanged("ChartType");
                }
            }
        }
        private string _ChartType;

        partial void OnGetChartType(ref string value);
        partial void OnSetChartType(ref string value);

        public virtual string Brush
        {
            get
            {
                OnGetBrush(ref _Brush);

                return _Brush;
            }
            set
            {
                if (value != _Brush)
                {
                    OnSetBrush(ref value);
                    _Brush = value;
                    TriggerPropertyChanged("Brush");
                }
            }
        }
        private string _Brush;

        partial void OnGetBrush(ref string value);
        partial void OnSetBrush(ref string value);

        public virtual string Database
        {
            get
            {
                OnGetDatabase(ref _Database);

                return _Database;
            }
            set
            {
                if (value != _Database)
                {
                    OnSetDatabase(ref value);
                    _Database = value;
                    TriggerPropertyChanged("Database");
                }
            }
        }
        private string _Database;

        partial void OnGetDatabase(ref string value);
        partial void OnSetDatabase(ref string value);

        public virtual string Collection
        {
            get
            {
                OnGetCollection(ref _Collection);

                return _Collection;
            }
            set
            {
                if (value != _Collection)
                {
                    OnSetCollection(ref value);
                    _Collection = value;
                    TriggerPropertyChanged("Collection");
                }
            }
        }
        private string _Collection;

        partial void OnGetCollection(ref string value);
        partial void OnSetCollection(ref string value);

    }
}
