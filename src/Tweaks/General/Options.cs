using System.ComponentModel;

namespace Tweakster
{
    internal partial class Options : BaseOptionModel<Options>
    {
        private const string _general = "General";

        [Category(_general)]
        [DisplayName("Focus Solution Explorer")]
        [Description("Automatically focuses Solution Explorer on project load to make it visible in the IDE.")]
        [DefaultValue(true)]
        public bool FocusSolutionExplorerOnProjectLoad { get; set; } = true;

        [Category(_general)]
        [DisplayName("Auto save")]
        [Description("Automatically save documents and projects as they lose focus")]
        [DefaultValue(true)]
        public bool AutoSave { get; set; } = true;

        [Category(_build)]
        [DisplayName("Save solution on build")]
        [Description("Calls the 'Save All' command when a build begins")]
        [DefaultValue(true)]
        public bool SaveAllOnBuild { get; set; } = true;
    }
}
