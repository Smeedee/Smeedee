using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Smeedee.Widgets.SL.SourceControl.Views
{
    public partial class TopCommitersSettings : UserControl
    {
        public TopCommitersSettings()
        {
            InitializeComponent();
            SetBlackOutDates();
        }

        private void SetBlackOutDates()
        {
            try
            {
                var tomorrow = DateTime.Now.Date.AddDays(1);
                var farInTheFuture = DateTime.Now.Date.AddYears(500);
                var futureDateRange = new CalendarDateRange(tomorrow, farInTheFuture);
                SinceDatePicker.BlackoutDates.Add(futureDateRange);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
