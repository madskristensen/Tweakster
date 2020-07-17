using System.ComponentModel;

namespace Tweakster
{
    internal partial class Options : BaseOptionModel<Options>
    {
        [Category(Category.Editor)]
        [DisplayName("Default zoom level")]
        [Description("The zoom level to apply when executing the Reset Zoom command using 'Ctrl+0'")]
        [DefaultValue(100)]
        public int DefaultZoomLevel { get; set; } = 100;
    }
}
