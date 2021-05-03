using System.ComponentModel;
using Community.VisualStudio.Toolkit;

namespace Tweakster
{
    internal partial class Options : BaseOptionModel<Options>
    {
        private const string _debugger = "Debugger";

        [Category(_debugger)]
        [DisplayName("Start debugging on F10/F11")]
        [Description("Disables F10/F11 until debug mode is entered to stop accidental debugger starts.")]
        [DefaultValue(false)]
        public bool DebugOnF10F11 { get; set; }
    }
}
