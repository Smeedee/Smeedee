#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Linq.Expressions;
using System.Reflection;

using Microsoft.TeamFoundation.VersionControl.Client;

using Changeset=Smeedee.DomainModel.SourceControl.Changeset;


namespace Smeedee.Integration.VCS.TFSVC.DomainModel.Repositories
{
    public abstract class SpecificationHandler
    {
        private SpecificationHandler successor;

        public void Successor(SpecificationHandler handler)
        {
            ThrowArgumentExceptionIfArgIsNull(handler, "handler");

            successor = handler;
        }

        public virtual void Request(SpecificationRequest request)
        {
            ThrowArgumentExceptionIfArgIsNull(request, "request");

            HandleRequest(request);

            if (SuccessorSpecified())
            {
                successor.Request(request);
            }
        }

        private bool SuccessorSpecified()
        {
            return successor != null;
        }

        private static void ThrowArgumentExceptionIfArgIsNull(Object handler, string argName)
        {
            if (handler == null)
            {
                throw new ArgumentException("Value can not be null", argName);
            }
        }

        public virtual object GetValueFromExpressionBody(Expression<Func<Changeset, bool>> expression)
        {
            var binaryExpression = expression.Body as BinaryExpression;

            if (binaryExpression != null &&
                binaryExpression.Right.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpression = binaryExpression.Right as MemberExpression;
                var constantExpression = memberExpression.Expression as ConstantExpression;
                var obj = constantExpression.Value;
                var objType = obj.GetType();
                var value = objType.GetProperty(memberExpression.Member.Name,
                                                BindingFlags.Instance | BindingFlags.Public).GetValue(obj,
                                                                                                      null);
                return value;
            }
            return null;
        }

        public abstract void HandleRequest(SpecificationRequest request);
    }

    public class RevisionHandler : SpecificationHandler
    {
        public override void HandleRequest(SpecificationRequest request)
        {
            Expression<Func<Changeset, bool>> expression = request.Specification.IsSatisfiedByExpression();

            object value = GetValueFromExpressionBody(expression);

            if (expression.Body.ToString().Contains("Revision"))
            {
                var val = Convert.ToInt32(value);
                if (val < 1)
                {
                    val = 1;
                }
                request.Query.RevisionFrom = new ChangesetVersionSpec(val);
                request.Query.RevisionTo = VersionSpec.Latest;
            }

            if (expression.Body.ToString().Contains("Username"))
            {
                var val = Convert.ToString(value);
                request.Query.Author.Username = val;
            }

            if (expression.Body.ToString().Contains("Amount"))
            {
                var val = Convert.ToInt32(value);
                request.Query.MaxRevisions = val;
            }
        }
    }
}