using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APD.DomainModel.Framework.Services
{
    public interface ICheckIfCredentialsIsValid
    {
        bool Check(string provider, string url, string username, string password);
    }
}
