using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.DomainModel.Framework;
using Smeedee.Integration.Database.DomainModel.Repositories;
using TinyBDD.Specification.NUnit;


namespace Smeedee.Integration.Tests.Database.DomainModel.Repositories.SlideConfigurationRepositorySpecs
{
    [TestFixture][Category("IntegrationTest")]
    public class when_saving : Smeedee.IntegrationTests.Database.DomainModel.Repositories.Shared
    {
        private SlideConfiguration slideConfiguration;

        [SetUp]
        public void Setup()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
            slideConfiguration = new SlideConfiguration()
            {
                Title = "Tittel",
                Duration = 300,
                SlideNumberInSlideshow = 20,
                WidgetType = "Smeedee.Widgets.Sourcecontrol",
                WidgetXapName = "Sourcecontrol.xap",
                WidgetConfigurationId = new Guid("76567898765435465718293847586946")
            };
        }

        [Test]
        public void Assure_SLideConfiguration_is_saved()
        {
            var repo = new SlideConfigurationRepository(sessionFactory);
            repo.Save(slideConfiguration);

            RecreateSessionFactory();

            var newRepo = new SlideConfigurationRepository(sessionFactory);

            var result = newRepo.Get(new AllSpecification<SlideConfiguration>());

            result.Count().ShouldBe(1);

            var slideFromDB = result.ElementAt(0);
            slideFromDB.Title.ShouldBe(slideConfiguration.Title);
            slideFromDB.Duration.ShouldBe(slideConfiguration.Duration);
            slideFromDB.SlideNumberInSlideshow.ShouldBe(slideConfiguration.SlideNumberInSlideshow);
            slideFromDB.WidgetType.ShouldBe(slideConfiguration.WidgetType);
            slideFromDB.WidgetXapName.ShouldBe(slideConfiguration.WidgetXapName);
            slideFromDB.WidgetConfigurationId.ShouldBe(slideConfiguration.WidgetConfigurationId);
        }

        [Test]
        public void Assure_saving_list_of_slides_replaces_all_slides()
        {
            var listOfSlides = new List<SlideConfiguration>
                                   {
                                       new SlideConfiguration() {Title = "nubmer one"},
                                       new SlideConfiguration() {Title = "number two"}
                                   };

            var repo = new SlideConfigurationRepository(sessionFactory);
            repo.Save(listOfSlides);

            var newListOfSlides = new List<SlideConfiguration>
                                      {
                                          new SlideConfiguration() {Title = "number three"}
                                      };

            repo.Save(newListOfSlides);

            RecreateSessionFactory();

            var newRepo = new SlideConfigurationRepository(sessionFactory);
            var result = newRepo.Get(new AllSpecification<SlideConfiguration>());

            result.Count().ShouldBe(1);
            result.ElementAt(0).Title.ShouldBe("number three");
        }

        [Test]
        public void Assure_deleteByTitle_deletes_slide()
        {
            var secondSlideConfiguration = new SlideConfiguration
            {
                Duration = 100,
                Title = "newSlide"
            };

            var repo = new SlideConfigurationRepository(sessionFactory);
            repo.Save(slideConfiguration);
            repo.Save(secondSlideConfiguration);
            repo.Delete(new SlideConfigurationByTitleSpecification(slideConfiguration.Title));

            RecreateSessionFactory();

            var newRepo = new SlideConfigurationRepository(sessionFactory);
            var result = newRepo.Get(new AllSpecification<SlideConfiguration>());
            result.Count().ShouldBe(1);
            result.ElementAt(0).Title.ShouldBe(secondSlideConfiguration.Title);
        }

        [Test]
        public void Assure_saving_one_slide_adds_the_slide()
        {
            var slide1 = new SlideConfiguration() { Title = "testing 1" };
            var slide2 = new SlideConfiguration() { Title = "testing 2" };
            var repo = new SlideConfigurationRepository(sessionFactory);
            repo.Save(slide1);

            RecreateSessionFactory();

            var newRepo = new SlideConfigurationRepository(sessionFactory);
            newRepo.Save(slide2);


            var result = newRepo.Get(new AllSpecification<SlideConfiguration>());
            result.Count().ShouldBe(2);
        }

        [Test]
        public void Assure_can_delete_by_id()
        {
            var secondSlideConfiguration = new SlideConfiguration
            {
                Duration = 100,
                Title = "newSlide"
            };

            var repo = new SlideConfigurationRepository(sessionFactory);
            repo.Save(slideConfiguration);
            repo.Save(secondSlideConfiguration);
            repo.Delete(new SlideConfigurationByIdSpecification(slideConfiguration.Id));

            RecreateSessionFactory();

            var newRepo = new SlideConfigurationRepository(sessionFactory);
            var result = newRepo.Get(new AllSpecification<SlideConfiguration>());
            result.Count().ShouldBe(1);
            result.ElementAt(0).Title.ShouldBe(secondSlideConfiguration.Title);
        }
    }
}
