using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.Integration.Database.DomainModel.Repositories;

namespace Smeedee.Client.Web.MobileServices.Auth
{
    public partial class Default : System.Web.UI.Page
    {
        private IRepository<Configuration> configRepo = new ConfigurationDatabaseRepository();

        protected void Page_Load(object sender, EventArgs e)
        {
            var apiKey = Request.QueryString["apiKey"] ?? "";
            Response.Write(IsApiKeyValid(apiKey));
        }

        private bool IsApiKeyValid(string apiKey)
        {
            var config = configRepo.Get(new ConfigurationByName("MobileServices"));
            if (config != null && config.Count() > 0)
            {
                if (config.First().ContainsSetting("ApiKey"))
                {
                    Response.Write("Actual: " + config.First().GetSetting("ApiKey").Value + " Sent in: " + apiKey);
                    return apiKey == config.First().GetSetting("ApiKey").Value;
                    
                }
            }
            return false;
        }
    }
}