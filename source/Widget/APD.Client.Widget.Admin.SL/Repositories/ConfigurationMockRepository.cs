using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using APD.DomainModel.Config;
using APD.DomainModel.Framework;

namespace APD.Client.Widget.Admin.SL.Repositories
{
    public class ConfigurationMockRepository : IRepository<Configuration>, IPersistDomainModels<Configuration>
    {

        #region IRepository<Configuration> Members

        public IEnumerable<Configuration> Get(Specification<Configuration> specification)
        {
            Thread.Sleep(5000);
            var retValue = new List<Configuration>();

            return retValue;
        }

        #endregion

        public void Save(Configuration domainModel)
        {
            
        }

        public void Save(IEnumerable<Configuration> domainModels)
        {
            
        }
    }
}
