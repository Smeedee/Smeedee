using System;
using System.ComponentModel.Composition;

namespace Smeedee.Client.Framework.ViewModel
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class WidgetInfoAttribute : ExportAttribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }
        public string[] Tags { get; set; }
        public bool CanBeHostedInTraybar { get; set; }
        public bool CanBeHostedInSlide { get; set; }

		public WidgetInfoAttribute() : base(typeof(Widget))
		{
		}
    }
}
