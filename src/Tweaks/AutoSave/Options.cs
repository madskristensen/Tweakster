using System.ComponentModel;

namespace Tweakster
{
    internal partial class Options : BaseOptionModel<Options>
    {
        [Category(Category.General)]
        [DisplayName("Auto save")]
        [Description("Automatically save documents and projects as they lose focus")]
        [DefaultValue(true)]
        public bool AutoSave { get; set; } = true;

        [Category(Category.Build)]
        [DisplayName("Save solution on build")]
        [Description("Calls the 'Save All' command when a build begins")]
        [DefaultValue(true)]
        public bool SaveAllOnBuild { get; set; } = true;
    }
}
