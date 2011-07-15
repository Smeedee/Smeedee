using System;
using System.Collections;
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

        private void InitSeries()
        {
            throw new NotImplementedException();
        }
    }
}
