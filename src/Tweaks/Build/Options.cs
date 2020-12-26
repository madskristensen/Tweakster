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

        /// <summary>   If Enabled, the Build Output Windows will output the reason why a project is
        ///             being built.</summary>
        /// <remarks>   Ultimately, this will set the DWORD registry value <c>U2DCheckVerbosity</c> in the 
        ///             Visual Studio registry hive under the <c>General</c> key. DO NOT alter the
        ///             property name as it reflects the registry value that is set. Note the
        ///             <see cref="OverrideCollectionNameAttribute"/> and <see cref="OverrideDataTypeAttribute"/>
        ///             have been applied so the value is loaded/saved in the appropriate place and is of
        ///             the appropriate type. <a href="https://github.com/madskristensen/Tweakster/issues/60">
        ///             Issue documentation</a> </remarks>
        [Category(_build)]
        [DisplayName("Up-To-Date Check Verbose")]
        [Description("If Enabled, the Build Output Windows will output the reason why a project is being built.")]
        [DefaultValue(false)]
        [OverrideCollectionName("General")]
        [OverrideDataType(SettingDataType.Bool)]
        public bool U2DCheckVerbosity { get; set; } = false;

        [Category(_build)]
        [DisplayName("Show ordered build output")]
        [Description("Activate the Build Order tab in the Output Window after each build")]
        [DefaultValue(false)]
        public bool ShowBuildOrder { get; set; } = false;
    }
}
