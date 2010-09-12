using System;
using System.Collections.Generic;

namespace Smeedee.DomainModel.Config.SlideConfig
{
    public class SlideConfiguration
    {
        public const string DEFAULT_SLIDE_TITLE = "Slide title";
        public const int DEFAULT_DURATION = 30;

        public virtual Guid Id { get; set; }
        public virtual string Title { get; set; }
        public virtual int Duration { get; set; }
        public virtual int SlideNumberInSlideshow { get; set; }

        public virtual string WidgetType { get; set; }
        public virtual string WidgetXapName { get; set; }
        public virtual Guid WidgetConfigurationId { get; set; }

        public SlideConfiguration()
        {
            Title = DEFAULT_SLIDE_TITLE;
            Duration = DEFAULT_DURATION;
            Id = Guid.NewGuid();
        }
    }
}