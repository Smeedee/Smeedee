using System;
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace Smeedee.Client.Framework.ViewModel
{
    public partial class ApplicationContext
    {
        partial void OnInitialize()
        {
            Traybar = new Traybar();
            Slideshow = new Slideshow();
            DockBar = new DockBar();
        }
    }
}
