using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smeedee.DomainModel.Framework.Services
{
    public interface ICheckIfResourceExists
    {
        bool Check(string url);
    }
}
