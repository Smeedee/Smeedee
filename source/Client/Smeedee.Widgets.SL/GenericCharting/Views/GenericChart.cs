using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Client.Framework.SL.Converters;
using Smeedee.Widgets.GenericCharting.Controllers;
using Smeedee.Widgets.GenericCharting.ViewModels;

namespace Smeedee.Widgets.SL.GenericCharting.Views
{
    public class GenericChart : Chart
    {
        private static readonly StringToResourceConverter converter = new StringToResourceConverter();

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public DataTemplate LinearAxisTemplate
        {
            get { return (DataTemplate)GetValue(LinearAxisTemplateProperty); }
            set { SetValue(LinearAxisTemplateProperty, value); }
        }

        public DataTemplate CategoryAxisTemplate
        {
            get { return (DataTemplate)GetValue(CategoryAxisTemplateProperty); }
            set { SetValue(CategoryAxisTemplateProperty, value); }
        }

        public string XAxisType
        {
            get { return (string) GetValue(XAxisTypeProperty); }
            set { SetValue(XAxisTypeProperty, value); }
        }

        private void InitAxis(object s, EventArgs e)
        {
            if (LinearAxisTemplate != null && XAxisType == ChartConfig.LINEAR)
            {
                
                var linear = LinearAxisTemplate.LoadContent() as IAxis;
                ChangeXAxis(linear);
            }
            else
            {
                var category = CategoryAxisTemplate.LoadContent() as IAxis;
                ChangeXAxis(category);
            }
            InitSeries(null, EventArgs.Empty);
        }

        private void ChangeXAxis(IAxis axis)
        {
            if (axis == null || !(axis is FrameworkElement)) return;
            ((FrameworkElement) axis).DataContext = this.DataContext;
            for (var i=0;i<Axes.Count; i++)
            {
                if (Axes[i].Orientation == AxisOrientation.X)
                    Axes.RemoveAt(i);
            }
            
            Axes.Add(axis);
        }

        private void InitSeries(object s, EventArgs e)
        {
            Series.Clear();
            if (WeHaveItemsSource())
                AddSeries(ItemsSource);
        }

        private bool WeHaveItemsSource()
        {
            return ItemsSource != null;
        }

        private void AddSeries(IEnumerable source)
        {
            var series = from single in source.OfType<DataSetViewModel>()
                         select single;
            foreach (var s in series)
            {
                switch (s.Type)
                {
                    case ChartConfig.AREA:
                        Series.Add(CreateArea(s));
                        break;
                    case ChartConfig.COLUMNS:
                        Series.Add(CreateColumn(s));
                        break;
                    case ChartConfig.LINE:
                        Series.Add(CreateLine(s));
                        break;
                }
            }
        }

        private AreaSeries CreateArea(DataSetViewModel viewModel)
        {
            var series = new AreaSeries
            {
                ItemsSource = viewModel.Data,
                Title = viewModel.Name,
                DependentValuePath = "Y",
                IndependentValuePath = "X"
            };
            if (!string.IsNullOrEmpty(viewModel.Brush))
            {
                series.DataPointStyle = CreateStyle(viewModel, typeof(AreaDataPoint));
            }
            return series;

        }
        
        private LineSeries CreateLine(DataSetViewModel viewModel)
        {
            var series = new LineSeries
            {
                ItemsSource = viewModel.Data,
                Title = viewModel.Name,
                DependentValuePath = "Y",
                IndependentValuePath = "X"
            };
            if (!string.IsNullOrEmpty(viewModel.Brush))
            {
                series.DataPointStyle = CreateStyle(viewModel, typeof(LineDataPoint));
            }
            return series;
        }
        
        private ColumnSeries CreateColumn(DataSetViewModel viewModel)
        {
            var series = new ColumnSeries
            {
                ItemsSource = viewModel.Data,
                Title = viewModel.Name,
                DependentValuePath = "Y",
                IndependentValuePath = "X"
            };
            if (!string.IsNullOrEmpty(viewModel.Brush))
            {
                series.DataPointStyle = CreateStyle(viewModel, typeof(ColumnDataPoint));
            }
            return series;
        }

        private Style CreateStyle(DataSetViewModel viewModel, Type type)
        {
            var style = new Style(type);
            var setter = new Setter(BackgroundProperty, CreateBrush(viewModel));
            style.Setters.Add(setter);
            return style;
        }

        private object CreateBrush(DataSetViewModel viewModel)
        {
            return converter.Convert(viewModel.Brush, typeof (Brush), null, CultureInfo.CurrentUICulture);
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            MakeSourceDependencyProperty("ItemsSource");

        public static readonly DependencyProperty LinearAxisTemplateProperty =
            MakeDataTemplateDependencyProperty("LinearAxisTemplate");

        public static readonly DependencyProperty CategoryAxisTemplateProperty =
            MakeDataTemplateDependencyProperty("CategoryAxisTemplate");

        public static readonly DependencyProperty XAxisTypeProperty =
            DependencyProperty.Register("XAxisProperty", typeof(string), typeof(GenericChart),
                new PropertyMetadata(null, XAxisTypeChanged));


        private static DependencyProperty MakeSourceDependencyProperty(string name)
        {
            return DependencyProperty.Register(name, typeof(ObservableCollection<DataSetViewModel>), typeof(GenericChart),
                new PropertyMetadata(null, SeriesSourceChanged));
        }

        private static DependencyProperty MakeDataTemplateDependencyProperty(string name)
        {
            return DependencyProperty.Register(name, typeof(DataTemplate), typeof(GenericChart),
                                               new PropertyMetadata(null, DataTemplatesChanged));
        }

        private static void DataTemplatesChanged(DependencyObject d, DependencyPropertyChangedEventArgs eventArgs)
        {
            var chart = d as GenericChart;
            if (chart == null)
            {
                Debug.WriteLine("Something is wrong, dependency object is not a GenericChart");
                return;
            }
            chart.InitAxis(null, EventArgs.Empty);
        }

        private static void SeriesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs eventArgs)
        {
            var chart = d as GenericChart;
            if (chart == null)
            {
                Debug.WriteLine("Something is wrong, dependency object is not a GenericChart");
                return;
            }

            var old = eventArgs.OldValue as ObservableCollection<DataSetViewModel>;
            if (old != null)
                old.CollectionChanged -= chart.InitSeries;

            var newCollection = eventArgs.NewValue as ObservableCollection<DataSetViewModel>;
            if (newCollection == null)
            {
                Debug.WriteLine("Something is wrong, series source is not an ObservableCollection<DataSetViewModel>");
                return;
            }

            newCollection.CollectionChanged += chart.InitSeries;

            chart.InitSeries(newCollection, EventArgs.Empty);
        }
        
        private static void XAxisTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataTemplatesChanged(d, e);
        }

    }
}
