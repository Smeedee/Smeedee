using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smeedee.Integration.Framework.Utils
{
    public interface IFileUtilities
    {
        IEnumerable<string> GetDLLs(string folderToSearch);

        IEnumerable<Type> FindImplementationsOrSubclassesOf<T>(string folderToSearch);
    }
}
