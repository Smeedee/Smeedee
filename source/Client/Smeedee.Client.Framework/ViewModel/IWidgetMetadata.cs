using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smeedee.Client.Framework.ViewModel
{
    public interface IWidgetMetadata
    {
        string Name { get; }
        string Description { get; }
        string Author { get; }
        string Version { get; }

        string[] Tags { get; }

    }
}
