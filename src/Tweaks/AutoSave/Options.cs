using System.ComponentModel;

namespace Tweakster
{
    internal partial class Options : BaseOptionModel<Options>
    {
        [Category(Category.General)]
        [DisplayName("Auto save")]
        [Description("Automatically save documents and projects")]
        [DefaultValue(true)]
        public bool AutoSave { get; set; } = true;
    }
}
