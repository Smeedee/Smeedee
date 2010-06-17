using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Smeedee.Client.Framework.SL.Controls
{
    public partial class DefaultImage : UserControl
    {
        private static DependencyProperty ImageUrlProperty = DependencyProperty.Register(
            "ImageUrl", typeof(string), typeof(FrameworkElement),
            new PropertyMetadata(new PropertyChangedCallback((o, e) =>
            { 
                var defaultImage = o as DefaultImage;
                var imageUrl = e.NewValue as string;
                
                //if (imageUrl.StartsWith("http://") ||  imageUrl.StartsWith("https://"))
                if (Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                    defaultImage.image.Source = new BitmapImage(new Uri(imageUrl));
            })));

        public string ImageUrl  
        {
            get 
            {
                var val = GetValue(ImageUrlProperty);
                return val as string;
            } 
            set
            {
                SetValue(ImageUrlProperty, value);
            }
        }

        public DefaultImage()
        {
            InitializeComponent();
        }
    }
}
