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
    public class ModuleLoader : IModuleLoader
    {
        private CompositionContainer compositionContainer;

        public ModuleLoader()
        {
            compositionContainer = new CompositionContainer(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
        }

        public void LoadTraybarWidgets(Traybar traybarViewModel)
        {
            compositionContainer.ComposeParts(traybarViewModel);
        }

        public void LoadSlides(Slideshow slideshowViewModel)
        {
            compositionContainer.ComposeParts(slideshowViewModel);
        }
    }
}
