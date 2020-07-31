using System.ComponentModel;

namespace Tweakster
{
    internal partial class Options : BaseOptionModel<Options>
    {
        private const string _build = "Build";

        [Category(_build)]
        [DisplayName("Show build time statistics")]
        [Description("Shows the build time statistics in the Output Window after each build")]
        [DefaultValue(true)]
        public bool ShowBuildStats { get; set; } = true;

        [Category(_build)]
        [DisplayName("Delete obj and bin on clean")]
        [Description("Specifies the obj and bin folders should be deleted when running the Clean command.")]
        [DefaultValue(true)]
        public bool DeleteOuputArtifactsOnClean { get; set; } = true;
    }
}
