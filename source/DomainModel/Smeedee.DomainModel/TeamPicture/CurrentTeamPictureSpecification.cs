using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Smeedee.DomainModel.Framework;

namespace Smeedee.DomainModel.TeamPicture
{
    public class CurrentTeamPictureSpecification : Specification<TeamPicture>
    {
        public override Expression<Func<TeamPicture, bool>> IsSatisfiedByExpression()
        {
            return (t) => true;
        }
    }
}
