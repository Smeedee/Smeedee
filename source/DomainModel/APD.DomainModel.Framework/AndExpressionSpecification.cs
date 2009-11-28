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

        public override Expression<Func<TDomainModel, bool>> IsSatisfiedByExpression()
        {
            var andExp = Expression.And(Left.IsSatisfiedByExpression().Body,
                Right.IsSatisfiedByExpression().Body);

            Expression<Func<TDomainModel, bool>> lambda = Expression.
                Lambda<Func<TDomainModel, bool>>(andExp, Expression.Parameter(typeof(TDomainModel), "c"));

            return lambda;
        }
    }
}
