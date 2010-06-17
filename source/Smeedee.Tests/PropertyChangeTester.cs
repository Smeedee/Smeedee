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
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;


namespace Smeedee.Tests
{
    public class PropertyTester
    {
        private static int waitForEventHandlerTimeout = 50;
        public static int WaitForEventHandlerTimeout
        {
            get { return waitForEventHandlerTimeout; }
            set { waitForEventHandlerTimeout = value; }
        }

        public static bool WasNotified { get; private set; }
        private static string propertyName;

        public static void TestChange<T>(T testObject, Expression<Func<T, Object>> exp, object setToValue)
            where T : INotifyPropertyChanged
        {
            WasNotified = false;
            propertyName = GetPropertyName<T>(exp);

            SetValue(testObject, propertyName, setToValue);
        }

        private static string GetPropertyName<T>(Expression<Func<T, object>> exp)
        {
            string propName = "";
            if (exp.Body is UnaryExpression)
                propName = ((MemberExpression)((UnaryExpression)exp.Body).Operand).Member.Name;
            else
                propName = ((MemberExpression)exp.Body).Member.Name;

            return propName;
        }

        public static void TestForExistence<T>(Expression<Func<T, Object>> exp)
        {
        }

        public static bool PropertyIsType<T,P>(Expression<Func<T, Object>> exp)
        {
            string name = GetPropertyName<T>(exp);
            bool typesMatch = typeof (T).GetProperty(name).PropertyType == typeof (P);
            return typesMatch;
        }

        public static void TestChange<T>(T testObject, Expression<Func<T, Object>> exp)
            where T : INotifyPropertyChanged
        {
            WasNotified = false;

            if (exp.Body is UnaryExpression)
                propertyName = ((MemberExpression)((UnaryExpression)exp.Body).Operand).Member.Name;
            else
                propertyName = ((MemberExpression)exp.Body).Member.Name;

            PropertyInfo propertyInfo = typeof(T).GetProperty(propertyName);

            object oldValue = propertyInfo.GetValue(testObject, null);
            object newValue = ChangeValue(propertyInfo.PropertyType, oldValue);

            SetValue(testObject, propertyName, newValue);
        }

        private static void SetValue(INotifyPropertyChanged testObject, string propertyName, object newPropertyValue)
        {
            testObject.PropertyChanged += viewModel_PropertyChanged;
            PropertyInfo propertyInfo = testObject.GetType().GetProperty(propertyName);
            propertyInfo.SetValue(testObject, newPropertyValue, null);
            System.Threading.Thread.Sleep(WaitForEventHandlerTimeout);
            testObject.PropertyChanged -= viewModel_PropertyChanged;
        }

        private static void viewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            WasNotified = e.PropertyName == propertyName;
        }

        private static object ChangeValue(Type type, object oldValue)
        {
            if (type.IsEnum)
                return ChangeEnumValue(oldValue);


            switch (type.FullName)
            {
                case "System.String":
                    return oldValue == null ? "" : oldValue.ToString() + "change";

                case "System.Char":
                    return ((char)oldValue) == 'a' ? 'b' : 'a';

                case "System.Boolean":
                    return !((bool)oldValue);

                case "System.Byte":
                    return ((byte)oldValue) < byte.MaxValue ? byte.MaxValue : byte.MinValue;

                case "System.Decimal":
                    return ((decimal)oldValue) < decimal.MaxValue ? decimal.MaxValue : decimal.MinValue;

                case "System.Single":
                    return ((float)oldValue) < float.MaxValue ? float.MaxValue : float.MinValue;

                case "System.Double":
                    return ((double)oldValue) < double.MaxValue ? double.MaxValue : double.MinValue;

                case "System.Int16":
                    return ((short)oldValue) < short.MaxValue ? short.MaxValue : short.MinValue;

                case "System.Int32":
                    return ((int)oldValue) < int.MaxValue ? int.MaxValue : int.MinValue;

                case "System.Int64":
                    return ((long)oldValue) < long.MaxValue ? long.MaxValue : long.MinValue;

                case "System.DateTime":
                    return ((DateTime)oldValue) < DateTime.MaxValue ? DateTime.MaxValue : DateTime.MinValue;


                case "System.TimeSpan":
                    return ((TimeSpan)oldValue) < TimeSpan.MaxValue ? TimeSpan.MaxValue : TimeSpan.MinValue;

                default:
                    throw new ArgumentException("The PropertyChecker does not support the type " + type.FullName);
            }
        }

        private static object ChangeEnumValue(object oldValue)
        {
            if (oldValue == null || oldValue.GetType().IsEnum == false)
                throw new ArgumentException("The oldvalue needs to be an enum");

            Type enumType = oldValue.GetType();

            foreach (string enumName in Enum.GetNames(enumType))
            {
                if (enumName != Enum.GetName(enumType, oldValue))
                    return Enum.Parse(enumType, enumName);
            }

            throw new ArgumentException("You cannot change enums with only one value");
        }
    }
}
