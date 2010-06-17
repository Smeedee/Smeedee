#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System.Net;
using System.Xml;

using Smeedee.Integration.PMT.RallyDev.DomainModel.Repositories;

using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.IntegrationTests.PMT.RallyDev.XmlDownloaderSpecs
{
    [TestFixture]
    public class When_downloading_xml
    {
        protected static string webServiceUrl;
        protected const string VALID_USERNAME = "d.nyvik@gmail.com";
        protected const string VALID_PASSWORD = "haldis07";
        protected const string INVALID_USERNAME = "dyptvann@snorkel.org";
        protected const string INVALID_PASSWORD = "Diiiirk for 3";
        protected const string VALID_URL = "https://community.rallydev.com/slm/webservice/1.13/project";
        protected const string INVALID_URL = "badUrl";
        protected const string VALID_URL_WITH_HTML_TAGS = "https://community.rallydev.com/slm/webservice/1.13/task/512446089";

        protected static XmlDownloader XmlDownloader;

        protected Context xmlDownloader_is_created_with_valid_credentials = () =>
        {
            XmlDownloader = new XmlDownloader(VALID_USERNAME, VALID_PASSWORD);
        };

        protected Context xmlDownloader_is_created_with_invalid_credentials = () =>
        {
            XmlDownloader = new XmlDownloader(INVALID_USERNAME, INVALID_PASSWORD);
        };


        [Test]
        public void should_be_able_to_login_and_download_xml()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(xmlDownloader_is_created_with_valid_credentials);
                scenario.When("getting xml document with valid credentials");
                scenario.Then("the server should return a default reply", ()=>
                {
                    string result = XmlDownloader.GetXmlDocumentString(VALID_URL);
                    result.ShouldNotBeNull();
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(result);

                    xmlDocument.ShouldNotBeNull();
                });
            });
        }

        [Test]
        [ExpectedException(typeof(WebException))]
        public void should_not_be_authorized_to_download_with_wrong_username_and_password()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(xmlDownloader_is_created_with_invalid_credentials);
                scenario.When("getting xml document with invalid credentials");
                scenario.Then("XmlDownloader should throw an exception", ()=>
                {
                    XmlDownloader.GetXmlDocumentString(VALID_URL);
                });
            });
        }

        [Test]
        [ExpectedException(typeof(WebException))]
        public void should_throw_exception_when_wbservice_url_is_invalid()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(xmlDownloader_is_created_with_valid_credentials);
                scenario.When("getting xml document from invalid url");
                scenario.Then("XmlDownloader should throw an exception", ()=>
                {
                    XmlDownloader.GetXmlDocumentString(INVALID_URL);
                });
            });
        }

        [Test]
        public void should_remove_html_tags()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(xmlDownloader_is_created_with_valid_credentials);
                scenario.When("getting xml that contains html tags from valid url");
                scenario.Then("XmlDownloader should remove the html tags", ()=>
                {
                    string result = XmlDownloader.GetXmlDocumentString(VALID_URL_WITH_HTML_TAGS);
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(result);
                    xmlDocument.ShouldNotBeNull();
                });
            });
        }
    }
}