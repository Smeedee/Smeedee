using System;
using System.Collections.Generic;

namespace APD.Integration.CI.Hudson.DomainModel.Repositories.Data
{
    [Serializable]
    public class Root
    {
        #region Properties

        public List<Project> Jobs { get; set; }

        #endregion
    }
}
