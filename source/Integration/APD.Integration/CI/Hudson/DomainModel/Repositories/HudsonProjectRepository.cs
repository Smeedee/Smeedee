using System;
using System.Collections.Generic;
using APD.DomainModel.CI;
using APD.DomainModel.Framework;
using APD.Integration.CI.Hudson.DomainModel.Repositories.Data;


namespace APD.Integration.CI.Hudson.DomainModel.Repositories
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
