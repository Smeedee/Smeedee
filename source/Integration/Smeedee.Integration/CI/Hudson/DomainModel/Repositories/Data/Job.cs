
using System.Collections.Generic;


namespace Smeedee.Integration.CI.Hudson.DomainModel.Repositories.Data
{
    public class Job
    {
        #region Properties

        public string Description { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool Buildable { get; set; }
        public List<Build> Builds { get; set; }

        #endregion
    }
}
