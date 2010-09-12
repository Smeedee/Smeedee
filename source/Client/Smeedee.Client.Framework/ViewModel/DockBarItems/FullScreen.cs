using Smeedee.Client.Framework.Services;
#if SILVERLIGHT
using Smeedee.Client.Framework.SL.Resources.Graphic.Icons;
#endif
using TinyMVVM.Framework;
using TinyMVVM.IoC;

namespace Smeedee.Client.Framework.ViewModel.DockBarItems
{
    public class FullScreen : DockBarItem
    {
        public FullScreen()
        {
            Description = "Show in fullscreen";
            Click = new DelegateCommand(() =>
            {
                var fullScreenService = this.GetDependency<IFullScreenService>();
                fullScreenService.GoIntoFullScreen();
            });
#if  SILVERLIGHT
            Icon = new FullScreenIcon();
#endif
        }
    }
}
