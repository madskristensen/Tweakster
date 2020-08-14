using System.ComponentModel;

namespace Tweakster
{
    internal partial class Options : BaseOptionModel<Options>
    {
        [Category(_general)]
        [DisplayName("Enable Presentation Mode")]
        [Description("Determines if Presentation Mode should be added to the Windows Jump List. Requires Visual Studio restart.")]
        [DefaultValue(true)]
        public bool EnablePresentationMode { get; set; } = true;

        [Category(_general)]
        [DisplayName("Enable Safe Mode")]
        [Description("Determines if Safe Mode should be added to the Windows Jump List. Requires Visual Studio restart.")]
        [DefaultValue(true)]
        public bool EnableSafeMode { get; set; } = true;
    }
}
