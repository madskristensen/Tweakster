using System.ComponentModel;

namespace Tweakster
{
    internal partial class Options : BaseOptionModel<Options>
    {
        [Category(Category.Build)]
        [DisplayName("Delete obj and bin on clean")]
        [Description("Specifies the obj and bin folders should be deleted when running the Clean command.")]
        [DefaultValue(true)]
        public bool DeleteOuputArtifactsOnClean { get; set; } = true;
    }
}
