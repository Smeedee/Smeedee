using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using APD.DomainModel.CI;
using APD.DomainModel.Framework;


namespace APD.Integration.CI.TeamCity.DomainModel.Repositories
{
    public class NewestBuildSpecification : Specification<CIServer>
    {
        public override Expression<Func<CIServer, bool>> IsSatisfiedByExpression()
        {
            throw new NotImplementedException();
        }
    }
}