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

using System;
using System.IO;
using System.Windows.Markup;
using APD.Client.Framework.ResourceDictionary;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using XamlReader=APD.Client.Framework.ResourceDictionary.XamlReader;


namespace APD.Client.FrameworkTests.XamlResourceDictionarySpecs
{
    public class Shared
    {
        protected const string VALID_RESOURCE_PATH = "APD.Client.Framework.Tests;component/StylesMock.xaml";
        protected const string INVALID_RESOURCE = "APD.Client.Framework.Tests;component/InvalidContent.xaml";
        protected const string INVALID_XAML_RESOURCE = "APD.Client.Framework.Tests;component/InvalidXAML.xaml";
        protected const string NON_EXISTING_RESOURCE = "I_DONT_EXIST.xaml";

        protected static XamlResourceDictionary instance;
        protected static string resourcePath;

        protected Context valid_resource_path = () => { resourcePath = VALID_RESOURCE_PATH; };
        protected Context invalid_resource_path = () => { resourcePath = "-->invalid_path<--"; };
        protected Context valid_resource_path_to_non_existing_content = () => { resourcePath = NON_EXISTING_RESOURCE; };
        protected Context valid_resource_path_to_invalid_content = () => { resourcePath = INVALID_RESOURCE; };
        protected Context valid_resource_path_to_wrong_xaml_type = () => { resourcePath = INVALID_XAML_RESOURCE; };

        protected When dictionary_is_created =
            () => { instance = new XamlResourceDictionary(resourcePath, new XamlReader()); };
    }

    [TestFixture]
    public class When_spawned : Shared
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void Assure_resource_exists()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(valid_resource_path);
                scenario.When(dictionary_is_created);
                scenario.Then("file stream is not null", () => instance.ResourceStream.ShouldNotBeNull());
            });
        }

        [Test]
        public void Assure_exception_is_thrown_on_wrong_resource_path()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(invalid_resource_path);
                scenario.When("the dictionary is created");
                scenario.Then("exception should be thrown", () =>
                {
                    this.ShouldThrowException<UriFormatException>(
                        () =>
                        dictionary_is_created(),
                        exception => { exception.ShouldNotBeNull(); }
                        );
                });
            });
        }

        [Test]
        public void Assure_exception_is_thrown_on_non_existing_resource()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(valid_resource_path_to_non_existing_content);
                scenario.When("the dictionary is created");
                scenario.Then("exception should be thrown", () =>
                {
                    this.ShouldThrowException<IOException>(
                        () =>
                        dictionary_is_created(),
                        exception => { exception.ShouldNotBeNull(); }
                        );
                });
            });
        }

        [Test]
        public void Assure_content_is_parsed()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(valid_resource_path);
                scenario.When(dictionary_is_created);
                scenario.Then("ResourceDictionary object should be created",
                              () => instance["FontFamily"].ToString().ShouldBe("Arial"));
            });
        }

        [Test]
        public void Assure_exception_is_thrown_on_wrong_xaml_type()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(valid_resource_path_to_wrong_xaml_type);
                scenario.When("the dictonary is created");
                scenario.Then("exception should be thrown", () =>
                {
                    this.ShouldThrowException<XamlParseException>(() =>
                        dictionary_is_created(),
                        exception => { exception.ShouldNotBeNull(); }
                    );
                });
            });
        }
    }
}