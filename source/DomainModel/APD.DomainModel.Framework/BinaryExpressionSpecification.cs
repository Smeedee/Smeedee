using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace APD.DomainModel.Framework
{
    public abstract class BinaryExpressionSpecification<TDomainModel> : Specification<TDomainModel>
    {
        public Specification<TDomainModel> Left { get; private set; }
        public Specification<TDomainModel> Right { get; private set; }

        public BinaryExpressionSpecification(Specification<TDomainModel> leftSpecification,
            Specification<TDomainModel> rightSpecification)
        {
            if (leftSpecification == null)
                throw new ArgumentNullException("leftSpecification");

            if (rightSpecification == null)
                throw new ArgumentNullException("rightSpecfication");

            Left = leftSpecification;
            Right = rightSpecification;
        }

        public override Expression<Func<TDomainModel, bool>> IsSatisfiedByExpression()
        {
            return (t) => true;
         }
    }
}
