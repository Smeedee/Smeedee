using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Controls;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Client.Framework.Services.Impl
{
    [Export(typeof(IModuleLoader))]
    public class ModuleLoader : IModuleLoader
    {
        public void LoadTraybarWidgets(Traybar traybarViewModel)
        {
            CompositionInitializer.SatisfyImports(traybarViewModel);
        }

        public void LoadSlides(Slideshow slideshowViewModel)
        {
            CompositionInitializer.SatisfyImports(slideshowViewModel);
        }
    }
}
