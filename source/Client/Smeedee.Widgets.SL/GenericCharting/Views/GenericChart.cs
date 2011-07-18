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

        public static readonly DependencyProperty LinesSourceProperty =
            DependencyProperty.Register("LinesSource", typeof (IEnumerable), typeof (GenericChart),
                new PropertyMetadata(null, (s, e) => ((GenericChart) s).InitSeries()));


        public DataTemplate LineTemplate
        {
            get { return (DataTemplate) GetValue(LineTemplateProperty); }
            set { SetValue(LineTemplateProperty, value); }
        }

        public static readonly DependencyProperty LineTemplateProperty =
            DependencyProperty.Register("LineTemplate", typeof (DataTemplate), typeof (GenericChart), 
                new PropertyMetadata(null, (s, e) => ((GenericChart)s).InitSeries()));

        private void InitSeries()
        {
            Series.Clear();
            if (LinesSource == null || LineTemplate == null)
                return;

            var series = from line in LinesSource.OfType<object>()
                         let seriesItem = LineTemplate.LoadContent() as ISeries
                         where seriesItem != null && seriesItem is FrameworkElement
                         let assignDataContext = ((FrameworkElement) seriesItem).DataContext = line
                         select seriesItem;

            series.ToList().ForEach(Series.Add);
        }
    }
}
