using System;
using System.Collections.Generic;
using System.Linq;
using Smeedee.Client.Web.Framework;

namespace Smeedee.Client.Web.MobileServices.Auth
{
    public partial class Default : System.Web.UI.Page
    {
        private readonly MobileServicesAuthenticator authenticator = new MobileServicesAuthenticator();

        protected void Page_Load(object sender, EventArgs e)
        {
            var apiKey = Request.QueryString["apiKey"] ?? "";
            Response.Write(authenticator.IsApiKeyValid(apiKey));
        }
    }
}