using System.ComponentModel;

namespace Tweakster
{
    internal partial class Options : BaseOptionModel<Options>
    {
        [Category(Category.Editor)]
        [DisplayName("Code Cleanup on save")]
        [Description("Determines if Code Cleanup should execute when formatting the document")]
        [DefaultValue(true)]
        public bool RunCodeCleanupOnFormat { get; set; } = true;
    }
}
