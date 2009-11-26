using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace APD.Client.Framework.SL.Controls
{
    public class ItemsControlEx : ItemsControl
    {
        public static DependencyProperty ItemTemplateSelectorProperty = DependencyProperty.
            Register("ItemTemplateSelector", typeof(IDataTemplateSelector), typeof (ItemsControlEx),
                     new PropertyMetadata(null));

        public IDataTemplateSelector ItemTemplateSelector
        {
            get { return GetValue(ItemTemplateSelectorProperty) as IDataTemplateSelector; } 
            set { SetValue(ItemTemplateSelectorProperty, value);}
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            IDataTemplateSelector selector = this.ItemTemplateSelector;

            if (null != selector)
            {
                ((ContentPresenter)element).ContentTemplate = selector.SelectTemplate(item, element);
            }
        }
    }
}
