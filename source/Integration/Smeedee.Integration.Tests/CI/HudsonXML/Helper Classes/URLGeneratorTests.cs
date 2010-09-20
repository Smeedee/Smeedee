using System;
using NUnit.Framework;
using Smeedee.Integration.CI.HudsonXML.Helper_classes;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Integration.Tests.CI.HudsonXML
{
    public class URLGeneratorTests
    {
        private URLGenerator generator;
        private const string dummyURL = "http://example.com/";
        private const string projectName = "dummyProject";

        [SetUp]
        public void setUp()
        {
            generator = new URLGenerator(dummyURL);
        }

        [Test]
        public void AssureViewAllUrlIsCorrect()
        {
            generator.viewAll().Equals("http://example.com/view/All/api/xml/").ShouldBeTrue();
        }

        public void AssureXmlApiUrlIsCorrect(string projectName)
        {
            generator.xmlApi(projectName).Equals("http://example.com/job/dummyProject/api/xml/").ShouldBeTrue();            
        }

        public void AssureBuildUrlIsCorrect()
        {
            generator.urlForBuild(projectName, "42").Equals("http://example.com/job/dummyProject/42/api/xml/").ShouldBeTrue(); 
        }
    }
}
