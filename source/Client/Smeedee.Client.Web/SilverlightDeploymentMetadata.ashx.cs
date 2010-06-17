using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Smeedee.Client.Web
{
    /// <summary>
    /// Summary description for SilverlightDeploymentMetadata
    /// </summary>
    public class SilverlightDeploymentMetadata : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/xml";

            var output = new XDocument();
            var root = new XElement("SilverlightDeployment");
            output.Add(root);

            foreach (var file in Directory.GetFiles(context.Server.MapPath("ClientBin")))
            {
                root.Add(new XElement("XAP")
                {
                    Value = new FileInfo(file).Name
                });
            }

            context.Response.Write(output.ToString());
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}