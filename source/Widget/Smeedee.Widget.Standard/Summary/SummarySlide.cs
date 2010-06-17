using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Client.Framework.SL.Views;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Widgets.SL.Summary.Views;

namespace Smeedee.Client.Widgets.SL.Summary
{
    //[Export(typeof(Slide))]
    public class SummarySlide : Slide
    {
        public SummarySlide()
        {
            Title = "Summary";
            View = new SummaryView();
        }
    }
}
