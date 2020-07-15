using System.ComponentModel;

namespace Tweakster
{
    internal partial class Options : BaseOptionModel<Options>
    {
        [Category(Category.Editor)]
        [DisplayName("Format on Move Line")]
        [Description("Calls 'Format Selection' when a line is moved up or down using Alt+UP/DOWN shortcuts.")]
        [DefaultValue(true)]
        public bool FormatOnMoveLine { get; set; } = true;
    }
}
