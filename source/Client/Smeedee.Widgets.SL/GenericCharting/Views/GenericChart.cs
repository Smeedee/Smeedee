using System;
using System.Collections;
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

namespace Smeedee.Widgets.SL.GenericCharting.Views
{
    public class GenericChart : Chart
    {
        public IEnumerable LinesSource
        {
            get { return (IEnumerable) GetValue(LinesSourceProperty); }
            set { SetValue(LinesSourceProperty, value); }
        }

        public DataTemplate LineTemplate
        {
            get { return (DataTemplate)GetValue(LineTemplateProperty); }
            set { SetValue(LineTemplateProperty, value); }
        }

        public IEnumerable ColumnsSource
        {
            get { return (IEnumerable) GetValue(ColumnsSourceProperty); }
            set { SetValue(ColumnsSourceProperty, value); }
        }

        public DataTemplate ColumnTemplate
        {
            get { return (DataTemplate) GetValue(ColumnTemplateProperty); }
            set { SetValue(ColumnTemplateProperty, value); }
        }

        public IEnumerable AreasSource
        {
            get { return (IEnumerable) GetValue(AreasSourceProperty); }
            set { SetValue(AreasSourceProperty, value); }
        }

        public DataTemplate AreaTemplate
        {
            get { return (DataTemplate) GetValue(AreaTemplateProperty); }
            set { SetValue(AreaTemplateProperty, value); }
        }

        private void InitSeries()
        {
            Series.Clear();
            if (WeHaveAreaTemplateAndAreasSource())
                AddSeriesFrom(AreasSource, AreaTemplate);

            if (WeHaveColumnTemplateAndColumnsSource())
                AddSeriesFrom(ColumnsSource, ColumnTemplate);

            if (WeHaveLineTemplateAndLinesSource())
                AddSeriesFrom(LinesSource, LineTemplate);
        }

        private bool WeHaveAreaTemplateAndAreasSource()
        {
            return AreaTemplate != null && AreasSource != null;
        }

        private bool WeHaveColumnTemplateAndColumnsSource()
        {
            return ColumnTemplate != null && ColumnsSource != null;
        }

        private bool WeHaveLineTemplateAndLinesSource()
        {
            return LineTemplate != null && LinesSource != null;
        }

        private void AddSeriesFrom(IEnumerable source, DataTemplate template)
        {
            var series = from line in source.OfType<object>()
                         let seriesItem = template.LoadContent() as ISeries
                         where seriesItem != null && seriesItem is FrameworkElement
                         let assignDataContext = ((FrameworkElement)seriesItem).DataContext = line
                         select seriesItem;

            series.ToList().ForEach(Series.Add);
        }

        public static readonly DependencyProperty LinesSourceProperty =
            MakeSourceDependencyProperty("LinesSource");

        public static readonly DependencyProperty LineTemplateProperty =
            MakeDataTemplateDependencyProperty("LineTemplate");

        public static readonly DependencyProperty ColumnsSourceProperty =
            MakeSourceDependencyProperty("ColumnsSource");

        public static readonly DependencyProperty ColumnTemplateProperty =
            MakeDataTemplateDependencyProperty("ColumnTemplate");

        public static readonly DependencyProperty AreasSourceProperty =
            MakeSourceDependencyProperty("AreasSource");

        public static readonly DependencyProperty AreaTemplateProperty =
            MakeDataTemplateDependencyProperty("AreaTemplate");

        private static DependencyProperty MakeSourceDependencyProperty(string name)
        {
            return DependencyProperty.Register(name, typeof(IEnumerable), typeof(GenericChart),
                new PropertyMetadata(null, (s, e) => ((GenericChart)s).InitSeries()));
        }

        private static DependencyProperty MakeDataTemplateDependencyProperty(string name)
        {
            return DependencyProperty.Register(name, typeof (DataTemplate), typeof (GenericChart),
                                               new PropertyMetadata(null, (s, e) => ((GenericChart) s).InitSeries()));
        }

    }
}
