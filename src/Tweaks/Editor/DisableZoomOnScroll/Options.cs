using System.ComponentModel;

namespace Tweakster
{
    internal partial class Options : BaseOptionModel<Options>
    {
        [Category(Category.Editor)]
        [DisplayName("Enable mouse wheel zoom")]
        [Description("Enable zooming in the editor when hitting Ctrl+MouseScroll")]
        [DefaultValue(false)]
        public bool EnableZoomOnScroll { get; set; }
    }
}
