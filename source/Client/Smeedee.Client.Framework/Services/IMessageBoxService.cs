using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Smeedee.Client.Framework.Services
{
    public interface IMessageBoxService
    {
        MessageBoxResult Show(string message, string caption, MessageBoxButton buttons);
    }
}
