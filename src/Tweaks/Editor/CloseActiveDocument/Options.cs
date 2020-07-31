using System.ComponentModel;

namespace Tweakster
{
    internal partial class Options : BaseOptionModel<Options>
    {
        [Category(Category.General)]
        [DisplayName("Close tab on Ctrl+W")]
        [Description("When true, Ctrl+W will close the current active document instead of executing Edit.SelectCurrentWord.")]
        [DefaultValue(true)]
        public bool CloseTabOnControlW { get; set; } = true;
    }
}
