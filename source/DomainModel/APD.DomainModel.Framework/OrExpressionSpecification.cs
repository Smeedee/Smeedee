using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APD.DomainModel.Framework
{
    public class OrExpressionSpecification<TDomainModel> : BinaryExpressionSpecification<TDomainModel>
    {
        public OrExpressionSpecification(Specification<TDomainModel> leftSpecification, Specification<TDomainModel> rightSpecificaiton) :
            base(leftSpecification, rightSpecificaiton)
        {
            
        }

        public override bool IsSatisfiedBy(TDomainModel domainObject)
        {
            return Left.IsSatisfiedBy(domainObject) || Right.IsSatisfiedBy(domainObject);
        }
    }
}
