using System.ComponentModel;

namespace Tweakster
{
    internal partial class Options : BaseOptionModel<Options>
    {
        [Category(Category.Editor)]
        [DisplayName("Copy empty lines")]
        [Description("Specifies if an empty lines should be copied to clipboard when the selection is empty")]
        [DefaultValue(false)]
        public bool CopyEmptyLines { get; set; }
    }
}
