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

using APD.Client.Widget.Admin.ViewModels;


namespace APD.Client.Widget.Admin.SL.Views
{
    public partial class HolidaysView : UserControl
    {
        public HolidaysView()
        {
            InitializeComponent();
            gridHolidays.BeginEdit();
        }

        private void gridHolidays_KeyDown(object sender, KeyEventArgs e)
        {
            if( e.Key == Key.Delete )
                (DataContext as HolidaysDbViewModel).DeleteSelectedHolidayUICommand.Execute(null);
        }
    }
}
