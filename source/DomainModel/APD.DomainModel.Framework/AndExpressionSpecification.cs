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
            var paramExp = Expression.Parameter(typeof (TDomainModel), "c");

            Expression<Func<TDomainModel, bool>> leftExp = Left.IsSatisfiedByExpression();
            Expression<Func<TDomainModel, bool>> rightExp = Right.IsSatisfiedByExpression();

            var andExp = Expression.And(Expression.Invoke(leftExp, paramExp),
                Expression.Invoke(rightExp, paramExp));

            Expression<Func<TDomainModel, bool>> lambda = Expression.
                Lambda<Func<TDomainModel, bool>>(andExp, paramExp);

            return lambda;
        }
    }
}
