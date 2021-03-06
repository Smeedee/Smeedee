﻿using System;
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
using Smeedee.Client.Widgets.SL.CI.Views;

namespace Smeedee.Client.Widgets.SL.CI
{
    //[Export(typeof(Slide))]
    public class BuildStatusSlide : Slide
    {
        public BuildStatusSlide()
        {
            Title = "Build status";
            View = new BuildStatusView();
            SettingsView = new BuildStatusSettingsView();
        }
    }
}
