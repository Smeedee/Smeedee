using System;

namespace Smeedee.DomainModel.Config.SlideConfig
{
    public class WidgetDefinition   
    {
        public virtual string Type { get; set; }
        public virtual string XAPName { get; set; }
        public virtual string ConfigKey { get; set; }

        public WidgetDefinition()
        {
            ConfigKey = Guid.NewGuid().ToString();
        }

        public static WidgetDefinition FromWidgetInfo(WidgetInfo widgetInfo)
        {
            if (widgetInfo == null || string.IsNullOrEmpty(widgetInfo.Type) || string.IsNullOrEmpty(widgetInfo.XAPName))
                throw new InvalidOperationException("The widgetInfo needs to contain the type and the XAP name of the widget");

            return new WidgetDefinition()
                       {
                           Type = widgetInfo.Type,
                           XAPName = widgetInfo.XAPName
                       };
        }
    }
}