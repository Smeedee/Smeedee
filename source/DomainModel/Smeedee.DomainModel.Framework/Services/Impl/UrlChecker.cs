using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Smeedee.DomainModel.Framework.Services.Impl
{
    public class UrlChecker : ICheckIfResourceExists
    {
        #region ICheckIfResourceExists Members

        public bool Check(string url)
        {
            var uri = new Uri(url);
            var client = new TcpClient();
            try
            {
                client.Connect(uri.Host, uri.Port);
                client.Close();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        #endregion
    }
}
