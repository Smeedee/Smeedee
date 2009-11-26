using System;
using System.Collections.Generic;

using APD.DomainModel.Framework;
using APD.DomainModel.SourceControl;


namespace APD.Integration.ClearCase.DomainModel.Repositories
{
    public class ChangesetRepository : IRepository<Changeset>
    {
        IEnumerable<Changeset> IRepository<Changeset>.Get(Specification<Changeset> specification)
        {
            return Get(specification);
        }

        public IEnumerable<Changeset> Get(Specification<Changeset> specification)
        {
            throw new NotImplementedException();
        }
    }
}