using System.ComponentModel;

namespace Tweakster
{
    internal partial class Options : BaseOptionModel<Options>
    {
        [Category(Category.General)]
        [DisplayName("Focus Solution Explorer")]
        [Description("Automatically focuses Solution Explorer on project load to make it visible in the IDE.")]
        [DefaultValue(true)]
        public bool FocusSolutionExplorerOnProjectLoad { get; set; } = true;
    }
}
