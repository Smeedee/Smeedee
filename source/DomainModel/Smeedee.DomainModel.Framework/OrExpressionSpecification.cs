using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Smeedee.DomainModel.Framework
{
    public class OrExpressionSpecification<TDomainModel> : BinaryExpressionSpecification<TDomainModel>
    {
        public OrExpressionSpecification(Specification<TDomainModel> leftSpecification, Specification<TDomainModel> rightSpecificaiton) :
            base(leftSpecification, rightSpecificaiton)
        {
            
        }

        public override Expression<Func<TDomainModel, bool>> IsSatisfiedByExpression()
        {
            var paramExp = Expression.Parameter(typeof(TDomainModel), "c");

            Expression<Func<TDomainModel, bool>> leftExp = Left.IsSatisfiedByExpression();
            Expression<Func<TDomainModel, bool>> rightExp = Right.IsSatisfiedByExpression();

            var orExp = Expression.Or(Expression.Invoke(leftExp, paramExp),
                Expression.Invoke(rightExp, paramExp));

            Expression<Func<TDomainModel, bool>> lambda = Expression.
                Lambda<Func<TDomainModel, bool>>(orExp, paramExp);

            return lambda;
        }
    }
}
