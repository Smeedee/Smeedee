using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using APD.DomainModel.Framework;


namespace APD.Integration.Framework.Atom.DomainModel.Repositories.AtomEntrySpecifications
{
    public class AtomEntriesNewerThanSpecification<T> : Specification<T> where T : AtomEntry
    {
        private readonly DateTime _time;

        public AtomEntriesNewerThanSpecification(DateTime time)
        {
            _time = time;
        }

        public override Expression<Func<T, bool>> IsSatisfiedByExpression()
        {
            return entry => entry.Updated >= _time;
        }
    }
}