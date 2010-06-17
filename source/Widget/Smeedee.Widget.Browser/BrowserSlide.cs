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
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Widget.Browser.Views;

namespace Smeedee.Widget.Browser
{
    [Export(typeof(Slide))]
    public class BrowserSlide : ViewModel.Browser
    {
        public BrowserSlide()
        {
            Title = "Web browser slide:)";
            /*View = new BrowserView()
            {
                DataContext = this
            };*/
            SettingsView = new BrowserSettingsView()
            {
                DataContext = this
            };
        }
    }
}
