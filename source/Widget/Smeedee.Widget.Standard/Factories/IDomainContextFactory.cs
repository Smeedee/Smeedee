using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Client.Web.Services;

namespace Smeedee.Widget.Standard.Factories
{
    /// <summary>
    /// Resposible for spawning Domain Context (RIA Services) objects
    /// for accessing server.
    /// </summary>
    public interface IDomainContextFactory
    {
        SourceControlContext NewSourceControlContext();
    }
}
