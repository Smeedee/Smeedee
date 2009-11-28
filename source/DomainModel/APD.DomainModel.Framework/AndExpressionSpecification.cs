using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace APD.DomainModel.Framework
{
    public class AndExpressionSpecification<TDomainModel> : BinaryExpressionSpecification<TDomainModel>
    {
        public AndExpressionSpecification(Specification<TDomainModel> leftSpecification,
            Specification<TDomainModel> rightSpecification) :
                base(leftSpecification, rightSpecification)
        {
            
        }

        public override bool IsSatisfiedBy(TDomainModel domainObject)
        {
            return Left.IsSatisfiedBy(domainObject) && Right.IsSatisfiedBy(domainObject);
        }
    }
}
