using System;
using System.Xml.Linq;
using Smeedee.Client.Framework.Services;

namespace Smeedee.Widget.Twitter.Controllers
{
    public interface IFetchXml
    {
        void BeginGetXml(string url);
        event EventHandler<AsyncResponseEventArgs<XDocument>>  GetXmlCompleted ;
    }
}