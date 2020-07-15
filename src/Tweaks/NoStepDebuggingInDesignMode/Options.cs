using System.ComponentModel;

namespace Tweakster
{
    internal partial class Options : BaseOptionModel<Options>
    {
        [Category(Category.Build)]
        [DisplayName("No debug on F10/F11")]
        [Description("Disables F10/F11 until debug mode is entered to stop accidental debugger starts.")]
        [DefaultValue(false)]
        public bool DebugOnF10F11 { get; set; } = false;
    }
}
