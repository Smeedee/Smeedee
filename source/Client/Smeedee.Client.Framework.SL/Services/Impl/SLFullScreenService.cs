using System;
using System.Windows;
using Smeedee.Client.Framework.Services;

namespace Smeedee.Client.Framework.SL.Services.Impl
{
    public class SLFullScreenService : IFullScreenService
    {
        public void GoIntoFullScreen()
        {
            Application.Current.Host.Content.IsFullScreen = true;
        }

        public void GoOutFromFullScreen()
        {
            Application.Current.Host.Content.IsFullScreen = false;
        }
    }
}
