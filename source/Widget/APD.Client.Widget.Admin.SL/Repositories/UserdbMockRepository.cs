using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using APD.DomainModel.Users;
using APD.DomainModel.Framework;
using System.Collections.Generic;
using System.Threading;

namespace APD.Client.Widget.Admin.SL.Repositories
{
    public class UserdbMockRepository : IRepository<Userdb>, IPersistDomainModels<Userdb>
    {

        #region IRepository<Userdb> Members

        public IEnumerable<Userdb> Get(Specification<Userdb> specification)
        {
            Thread.Sleep(10000);
            var result = new List<Userdb>();
            result.Add(new Userdb("default"));
            return result;
        }

        #endregion

        #region IPersistDomainModels<Userdb> Members

        public void Save(Userdb domainModel)
        {
            
        }

        public void Save(IEnumerable<Userdb> domainModels)
        {
            
        }

        #endregion
    }
}
