using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using TinyMVVM.Framework;

namespace Smeedee.Client.Framework.Factories
{
    public class ModuleLoaderFactory
    {
        public IModuleLoader NewModuleLoader()
        {
            return ServiceLocator.Instance.GetInstance<IModuleLoader>();
        }
    }
}
