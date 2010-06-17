using System.Windows;

namespace Smeedee.Client.Framework.SL.Controls
{
    public interface IDataTemplateSelector
    {
        DataTemplate SelectTemplate(object item, DependencyObject element);
    }
}
