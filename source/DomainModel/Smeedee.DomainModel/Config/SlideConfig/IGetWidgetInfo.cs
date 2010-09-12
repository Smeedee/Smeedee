using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Smeedee.DomainModel.Config.SlideConfig
{
    public interface IGetWidgetInfoFromXapFiles
    {
        IEnumerable<WidgetInfo> FromDirectory(DirectoryInfo directoryToSearch);
    }
}
