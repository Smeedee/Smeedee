using System;
using System.Xml.Linq;
using Smeedee.Client.Framework.Services;

namespace Smeedee.Widgets.SL.Twitter.Controllers
{
    public interface IFetchXml
    {
        void BeginGetXml(string url);
        event EventHandler<AsyncResponseEventArgs<XDocument>>  GetXmlCompleted ;
    }
}