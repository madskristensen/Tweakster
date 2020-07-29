using System.ComponentModel;

namespace Tweakster
{
    internal partial class Options : BaseOptionModel<Options>
    {
        [Category(Category.General)]
        [DisplayName("Activate Solution Explorer")]
        [Description("Automatically activates Solution Explorer on project load to make it visible in the IDE.")]
        [DefaultValue(true)]
        public bool ActivateSolutionExplorerOnProjectLoad { get; set; } = true;
    }
}
