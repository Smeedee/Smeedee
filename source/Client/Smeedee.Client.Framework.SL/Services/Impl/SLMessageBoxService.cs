using System.Windows;
using Smeedee.Client.Framework.Services;

namespace Smeedee.Client.Framework.SL.Services.Impl
{
    public class SLMessageBoxService : IMessageBoxService
    {
        public MessageBoxResult Show(string message, string caption, MessageBoxButton buttons)
        {
            return MessageBox.Show(message, caption, buttons);
        }
    }
}
