using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Smeedee.Client.Framework.ViewModel

{
    public abstract class BasicViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void TriggerPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void TriggerPropertyChanged<T>(Expression<Func<T, Object>> exp)
        {
            string propertyName;
            if (exp.Body is UnaryExpression)
                propertyName = ((MemberExpression)((UnaryExpression)exp.Body).Operand).Member.Name;
            else
                propertyName = ((MemberExpression)exp.Body).Member.Name;

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
