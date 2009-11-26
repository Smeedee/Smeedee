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
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Linq.Expressions;
using System.Reflection;

using SharpSvn;


namespace APD.Integration.VCS.SVN.DomainModel.Repositories
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

        public abstract void HandleRequest(SpecificationRequest request);

        private bool SuccessorSpecified()
        {
            return successor != null;
        }

        private void ThrowArgumentExceptionIfArgIsNull(Object handler, string argName)
        {
            if (handler == null)
            {
                throw new ArgumentException("Value can not be null", argName);
            }
        }
    }

    public class RevisionHandler : SpecificationHandler
    {
        // TODO: Improve readability
        public override void HandleRequest(SpecificationRequest request)
        {
            var expression = request.Specification.IsSatisfiedByExpression();
            
            // torstn - return home early, god damnit... 
            if (expression == null || !expression.Body.ToString().Contains("Revision"))
                return;

            var binaryExpression = expression.Body as BinaryExpression;

            if (binaryExpression != null &&
                binaryExpression.Right.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpression = binaryExpression.Right as MemberExpression;
                var constantExpression = memberExpression.Expression as ConstantExpression;
                var obj = constantExpression.Value;
                var objType = obj.GetType();
                var value = objType.GetProperty(memberExpression.Member.Name,
                                                ( BindingFlags.Instance | BindingFlags.Public )
                                    ).GetValue(obj, null);
                long val = Convert.ToInt64(value);
                request.SvnLogArgs.Range = new SvnRevisionRange(val, long.MaxValue);
            }
        }
    }
}