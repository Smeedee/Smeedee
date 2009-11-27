using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
