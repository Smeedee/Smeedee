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
using NUnit.Framework;
using System.Linq.Expressions;
using APD.DomainModel.SourceControl;
using TinyBDD.Specification.NUnit;
using System.Reflection;

namespace APD.DomainModel.SourceControlTests.Learning.LinqExpressionSpecs
{
    public class Shared
    {
        protected Expression<Func<Changeset, bool>> expression;
        protected BinaryExpression binaryExpression;
    }

    [TestFixture]
    public class Simple_expression : Shared
    {

        [SetUp]
        public void Setup()
        {
            expression = (c => c.Revision > 10);
            binaryExpression = expression.Body as BinaryExpression;
        }

        [Test]
        public void Body_Should_be_a_binary_expression()
        {
            expression.Body.GetType().ShouldBe(typeof(BinaryExpression));
        }

        [Test]
        public void Body_Left_side_should_be_a_MemberExpression()
        {
            binaryExpression.Left.GetType().ShouldBe(typeof(MemberExpression));
        }

        [Test]
        public void Should_be_able_to_parse_Member_Name_from_MemberExpression()
        {
            var memberExpression = binaryExpression.Left as MemberExpression;

            memberExpression.Member.Name.ShouldBe("Revision");
            memberExpression.Expression.Type.ShouldBe(typeof(Changeset));
        }

        [Test]
        public void Body_Right_side_should_be_a_ConstantExpression()
        {
            binaryExpression.Right.GetType().ShouldBe(typeof(ConstantExpression));
        }

        [Test]
        public void Should_be_able_to_parse_value_from_ConstantExpression()
        {
            var constExpression = binaryExpression.Right as ConstantExpression;
            constExpression.Value.ShouldBe(10);
            constExpression.Type.ShouldBe(typeof(long));
        }
    }

    [TestFixture]
    public class Assotiated_object_member_expression : Shared
    {
        [SetUp]
        public void Setup()
        {
            expression = (c => c.Author.Username == "goeran");
            binaryExpression = expression.Body as BinaryExpression;
        }

        [Test]
        public void Body_Left_side_should_be_a_MemberExpression()
        {
            binaryExpression.Left.GetType().ShouldBe(typeof(MemberExpression));
        }

        [Test]
        public void Should_be_able_to_parse_MemberExpression()
        {
            var memberExpression = binaryExpression.Left as MemberExpression;

            memberExpression.Member.Name.ShouldBe("Username");
            memberExpression.Expression.Type.ShouldBe(typeof(Author));

            var mExp = memberExpression.Expression as MemberExpression;
            mExp.Member.Name.ShouldBe("Author");
            mExp.Expression.Type.ShouldBe(typeof(Changeset));
        }
    }

    [TestFixture]
    public class When_value_is_a_MemberExpression : Shared
    {
        DateTime today;
        [SetUp]
        public void Setup()
        {
            today = DateTime.Now;
            expression = (c => c.Time == today);
            binaryExpression = expression.Body as BinaryExpression;
        }

        [Test]
        public void Body_Right_side_should_be_a_memberExpression()
        {
            binaryExpression.Right.GetType().ShouldBe(typeof(MemberExpression));
            binaryExpression.Right.NodeType.ShouldBe(ExpressionType.MemberAccess);
        }

        [Test]
        public void Should_be_able_to_parse_right_MemberExpression()
        {
            var memberExpression = binaryExpression.Right as MemberExpression;

            memberExpression.Member.Name.ShouldBe("today");

            memberExpression.Expression.NodeType.ShouldBe(ExpressionType.Constant);
            var constantExpression = memberExpression.Expression as ConstantExpression;
            var obj = constantExpression.Value;
            obj.ShouldBe(this);
            var objType = obj.GetType();
            objType.GetField(memberExpression.Member.Name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj).ShouldBe(today);    
        

        }
    }
}
