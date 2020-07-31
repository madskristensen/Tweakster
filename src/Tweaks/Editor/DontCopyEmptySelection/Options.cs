using System.ComponentModel;

namespace Tweakster
{
    internal partial class Options : BaseOptionModel<Options>
    {
        [Category(Category.Editor)]
        [DisplayName("Copy empty selection")]
        [Description("Specifies if anything should be copied to the clipboard when no selection is made.")]
        [DefaultValue(true)]
        public bool CopyEmptySelection { get; set; } = true;

        [Category(Category.Editor)]
        [DisplayName("Copy empty lines")]
        [Description("Specifies if the line should be copied to the clipboard when no selection is made. Requires re-opening of files to change.")]
        [DefaultValue(false)]
        public bool CopyEmptyLines { get; set; }
    }
}
