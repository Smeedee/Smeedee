using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using TinyBDD.Specification.NUnit;


namespace APD.DomainModel.CI.Test.CIServerSpecs
{
    public class Shared
    {
        protected CIServer server;

        public Shared()
        {
            server = new CIServer("CC.net", "http://smeedee.org/ccnet");   
        }
    }

    [TestFixture]
    public class When_spawned_with_params : Shared
    {
        [Test]
        public void Assure_name_is_set()
        {
            server.Name.ShouldBe("CC.net");
        }

        [Test]
        public void Assure_url_is_set()
        {
            server.Url.ShouldBe("http://smeedee.org/ccnet");
        }

        [Test]
        public void Assure_it_have_projects()
        {
            server.Projects.ShouldNotBeNull();
        }
    }

    [TestFixture]
    public class When_adding_projects : Shared
    {
        [Test]
        public void Assure_its_possible_to_add_Project()
        {
            server.AddProject(new CIProject("development"));
        }

        [Test]
        public void Assure_its_possible_to_add_multiple_Projects()
        {
            for (int i = 0; i < 10; i++)
                server.AddProject(new CIProject("development" + i));
        }

        [Test]
        public void Assure_Project_argument_is_validated()
        {
            this.ShouldThrowException<ArgumentNullException>(() =>
                server.AddProject(null), exception =>
                    exception.Message.ShouldBe("Value cannot be null.\r\nParameter name: newProject"));
        }
    }

    [TestFixture]
    public class When_projects_is_added : Shared
    {
        [SetUp]
        public void Setup()
        {
            server.AddProject(new CIProject("development"));
            server.AddProject(new CIProject("main"));
        }

        [Test]
        public void Assure_projects_been_added()
        {
            server.Projects.Count().ShouldBe(2);
            server.Projects.Where(p => p.ProjectName == "development").Count().ShouldBe(1);
            server.Projects.Where(p => p.ProjectName == "main").Count().ShouldBe(1);
        }
    }

    [TestFixture]
    public class When_projects_added_to_multiple_servers : Shared
    {
        private CIServer anotherServer;

        [SetUp]
        public void Setup()
        {
            anotherServer = new CIServer("Hudson", "http://smeedee.org/hudson");

            server.AddProject(new CIProject("development"));
            server.AddProject(new CIProject("main"));

            anotherServer.AddProject(new CIProject("development"));
            anotherServer.AddProject(new CIProject("main"));
        }

        [Test]
        public void Assure_Servers_are_self_contained()
        {
            server.Projects.Count().ShouldBe(2);
            server.Projects.Where(p => p.ProjectName == "development").Count().ShouldBe(1);
            server.Projects.Where(p => p.ProjectName == "main").Count().ShouldBe(1);

            anotherServer.Projects.Count().ShouldBe(2);
            anotherServer.Projects.Where(p => p.ProjectName == "development").Count().ShouldBe(1);
            anotherServer.Projects.Where(p => p.ProjectName == "main").Count().ShouldBe(1);
        }
    }
}
