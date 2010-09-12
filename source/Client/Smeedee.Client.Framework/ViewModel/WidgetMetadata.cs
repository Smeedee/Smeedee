using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smeedee.Client.Framework.ViewModel
{
    public partial class WidgetMetadata : IWidgetMetadata
    {
        partial void OnInitialize()
        {
            SecondsOnScreen = Slide.DEFAULT_SECONDSONSCREEN;
            PropertyChanged += (o, e) => { if (e.PropertyName == "IsDescriptionCapped") TriggerPropertyChanged("Description"); };
        }

        partial void OnGetDescription(ref string value)
        {
            if (value == null)
                value = "";
            if (value.Length > 100 && IsDescriptionCapped)
                value = new string(value.Take(97).ToArray()) + "...";
        }
    }
}
