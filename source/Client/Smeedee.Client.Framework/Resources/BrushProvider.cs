using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smeedee.Client.Framework.Resources
{
    public class BrushProvider
    {
        public const string DEFAULT_BRUSH = "GreyGradientBrush";
       
        public List<BrushProviderItem> BrushList
        {
            get
            {
                var list = new List<BrushProviderItem>();
                foreach (var key in GetBrushKeys())
                {
                    list.Add(new BrushProviderItem(key, GetBrushName(key)));
                }
                return list;
            }
        }

        private static Dictionary<string, string> brushes = new Dictionary<string, string>
        {   
            {"grey", "DarkGreyGradientBrush"},
            {"darkGrey", "GreyGradientBrush"},
            {"lightGrey", "LightGreyGradientBrush"},

            {"lightBrown", "LightBrownGradientBrush"},
            {"brown", "BrownGradientBrush"},
            {"darkBrown", "DarkBrownGradientBrush"},

            {"red", "RedGradientBrush"},
            {"orange", "OrangeGradientBrush"},
            {"yellow", "YellowGradientBrush"},

            {"lightGreen", "LightGreenGradientBrush"},
            {"green", "MediumGreenGradientBrush"},
            {"darkGreen", "GreenGradientBrush"},

            {"lightBlue", "LightBlueGradientBrush"},
            {"blue", "BlueGradientBrush"},
            {"darkBlue", "DarkBlueGradientBrush"},
          
            {"pink", "PinkGradientBrush"},
            {"purple", "PurpleGradientBrush"},
            {"darkPurple", "DarkPurpleGradientBrush"}
          
        };

        public static string GetBrushName(string color)
        {
            return brushes.ContainsKey(color) ? brushes[color] : DEFAULT_BRUSH;
        }

        public static string[] GetBrushKeys()
        {
            return brushes.Keys.ToArray();
        }
    }

    public class BrushProviderItem
    {
        public BrushProviderItem(string name, string brush)
        {
            BrushKey = name;
            BrushName = brush;
        }
        public string BrushKey { get; set; }
        public string BrushName { get; set; }
    }

}
