using System;
using System.Collections.Generic;

namespace Smeedee.Integration.CI.Hudson.DomainModel.Repositories.Data
{
    [Serializable]
    public class Root
    {
        #region Properties

        public List<Project> Jobs { get; set; }

        #endregion
    }
}
