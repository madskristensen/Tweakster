using System.ComponentModel;

namespace Tweakster
{
    internal partial class Options : BaseOptionModel<Options>
    {
        [Category(Category.Build)]
        [DisplayName("Show build time statistics")]
        [Description("Shows the build time statistics in the Output Window after each build")]
        [DefaultValue(true)]
        public bool ShowBuildStats { get; set; } = true;
    }
}
