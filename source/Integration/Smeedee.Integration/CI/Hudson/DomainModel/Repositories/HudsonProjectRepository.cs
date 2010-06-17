using System;
using System.Collections.Generic;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;
using Smeedee.Integration.CI.Hudson.DomainModel.Repositories.Data;


namespace Smeedee.Integration.CI.Hudson.DomainModel.Repositories
{
    class HudsonProjectRepository : IRepository<CIServer>
    {
        #region Private Members

        private HudsonApi hudsonApi = null;

        #endregion

        #region Constructors

		public HudsonProjectRepository()
		{
			hudsonApi = new HudsonApi();
		}

		public HudsonProjectRepository(string serverUrl)
		{
			hudsonApi = new HudsonApi(serverUrl);
		}

        #endregion

        #region IRepository Members

        public IEnumerable<CIServer> Get(Specification<CIServer> specification)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
