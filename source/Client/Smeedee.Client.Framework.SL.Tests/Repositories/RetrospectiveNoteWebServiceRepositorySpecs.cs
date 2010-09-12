using System.Collections.Generic;
using System.Linq;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Smeedee.Client.Framework.SL.Repositories;
using Smeedee.DomainModel.Corkboard;
using Smeedee.DomainModel.Framework;

namespace Smeedee.Client.Framework.SL.Tests.Repositories
{
    public class RetrospectiveNoteWebServiceRepositorySpecs : SilverlightTest
    {
        public void Assure_can_send_and_retrieve()
        {
            var repo = new RetrospectiveNoteWebServiceRepository();
            var toSave = new List<RetrospectiveNote>
                             {
                                 new RetrospectiveNote() { Description = "Heia smeedee"}
                             };
            repo.Save(toSave);

            var result = repo.Get(new AllSpecification<RetrospectiveNote>());

            Assert.IsTrue(result.Count() > 0);
            Assert.AreEqual("Heia smeedee", result.First().Description);
        }

        public static void Main()
        {
            new RetrospectiveNoteWebServiceRepositorySpecs().Assure_can_send_and_retrieve();
        }
    }
}
