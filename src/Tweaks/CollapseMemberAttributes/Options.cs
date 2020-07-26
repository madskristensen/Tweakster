using System.ComponentModel;

namespace Tweakster
{
    internal partial class Options : BaseOptionModel<Options>
    {
        [Category(Category.Editor)]
        [DisplayName("Enable collapsing attributes")]
        [Description("It provides the possibility to collapse group of attributes into one collapsed line.")]
        [DefaultValue(true)]
        public bool CollapseMemberAttributes { get; set; } = true;

        [Category(Category.Editor)]
        [DisplayName("Short form for attribute collapse")]
        [Description("Display short text (...) for the collapased form of attributes.")]
        [DefaultValue(true)]
        public bool CollapseMemberAttributesShortForm { get; set; } = true;
    }
}
