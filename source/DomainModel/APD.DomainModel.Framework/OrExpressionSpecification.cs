using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace APD.DomainModel.Framework
{
    public class OrExpressionSpecification<TDomainModel> : BinaryExpressionSpecification<TDomainModel>
    {
        public OrExpressionSpecification(Specification<TDomainModel> leftSpecification, Specification<TDomainModel> rightSpecificaiton) :
            base(leftSpecification, rightSpecificaiton)
        {
            
        }

        public override Expression<Func<TDomainModel, bool>> IsSatisfiedByExpression()
        {
            var orExp = Expression.Or(Left.IsSatisfiedByExpression().Body,
                Right.IsSatisfiedByExpression().Body);

            Expression<Func<TDomainModel, bool>> lambda = Expression.
                Lambda<Func<TDomainModel, bool>>(orExp, Expression.Parameter(typeof(TDomainModel), "c"));

            return lambda;
        }
    }
}
