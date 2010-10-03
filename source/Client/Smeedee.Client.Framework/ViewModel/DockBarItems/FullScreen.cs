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
        private bool isInFullScreen;

        public FullScreen()
        {
            isInFullScreen = false;
            Description = "Show in fullscreen";
            Click = new DelegateCommand(() =>
            {
                var fullScreenService = this.GetDependency<IFullScreenService>();
                if (isInFullScreen)
                {
                    fullScreenService.GoOutFromFullScreen();
                    isInFullScreen = false;
                    Description = "Show in fullscreen";
                }
                else
                {
                    fullScreenService.GoIntoFullScreen();
                    isInFullScreen = true;
                    Description = "Exit fullscreen";
                }
            });
#if  SILVERLIGHT
            Icon = new FullScreenIcon();
#endif     
            
        }
    }
}
