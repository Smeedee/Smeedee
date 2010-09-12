using System;
using System.Net;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Tasks.Framework.Tests
{
    [TestFixture]
    public class ServiceLocatorForSchedulerSpecs
    {
        private IocContainerForScheduler container;

        [SetUp]
        public void Setup()
        {
            container = new IocContainerForScheduler();
        }

        [Test]
        public void Should_be_able_to_bind_type_to_constant()
        {
            container.BindToConstant(10);
            container.Get<int>().ShouldBe(10);
        }

        [Test]
        public void Should_be_able_to_bind_type_to_type()
        {
            container.BindTo<ICredentials>(typeof(NetworkCredential));
            container.Get<ICredentials>().ShouldBeInstanceOfType<NetworkCredential>();
        }

        [Test]
        public void Should_not_return_same_object_twice()
        {
            container.BindTo<DateTime>(typeof(DateTime));
            
            container.BindToConstant<long>(1234567890);
            var firstResult = container.Get<DateTime>();

            container.BindToConstant<long>(987654321);
            var secondResult = container.Get<DateTime>();

            firstResult.ShouldNotBe(secondResult);
        }
    }


}
