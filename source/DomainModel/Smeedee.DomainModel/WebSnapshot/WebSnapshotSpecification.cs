using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Smeedee.DomainModel.Framework;

namespace Smeedee.DomainModel.WebSnapshot
{
    public class WebSnapshotSpecification : Specification<WebSnapshot>
    {
        public override Expression<Func<WebSnapshot, bool>> IsSatisfiedByExpression()
        {
            return (t) => true;
        }
    }
}
