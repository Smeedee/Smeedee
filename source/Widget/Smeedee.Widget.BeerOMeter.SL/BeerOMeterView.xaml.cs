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

namespace Smeedee.Widget.BeerOMeter.SL
{
    public partial class BeerOMeterView : UserControl
    {
        public BeerOMeterView()
        {
            InitializeComponent();
            /*
            var timeSpan = new TimeSpan(0, 0, 0, 1, 0);
            EndPoint.KeyTime = new KeyTime().TimeSpan.Add(timeSpan);
             */
            Animation.Begin();

        }

    }
}
