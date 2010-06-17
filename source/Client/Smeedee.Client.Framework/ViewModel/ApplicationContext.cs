using System;
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace Smeedee.Client.Framework.ViewModel
{
    public partial class ApplicationContext
    {
        public void OnInitialize()
        {
            Traybar = new Traybar();
            Slideshow = new Slideshow();
            Slideshow.PropertyChanged += Slideshow_PropertyChanged;

            if (Slideshow.CurrentSlide != null)
                Title = Slideshow.CurrentSlide.Title;
        }

       void Slideshow_PropertyChanged(object sender, PropertyChangedEventArgs e)
       {
           if (Slideshow.CurrentSlide != null)
           {
               if (e.PropertyName == "CurrentSlide")
               {
                   Title = Slideshow.CurrentSlide.Title;
               }
               else if (e.PropertyName == "SlideshowInfo")
               {
                   Subtitle = Slideshow.SlideshowInfo;
               }
           }
       }
    }
}
