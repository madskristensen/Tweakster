using System.ComponentModel;

namespace Tweakster
{
    internal partial class Options : BaseOptionModel<Options>
    {
        private const string _editor = "Editor";

        [Category(_editor)]
        [DisplayName("Close tab on Ctrl+W")]
        [Description("When true, Ctrl+W will close the current active document instead of executing Edit.SelectCurrentWord.")]
        [DefaultValue(true)]
        public bool CloseTabOnControlW { get; set; } = true;

        [Category(_editor)]
        [DisplayName("Code Cleanup on Format")]
        [Description("Determines if Code Cleanup should execute when formatting the document")]
        [DefaultValue(true)]
        public bool RunCodeCleanupOnFormat { get; set; } = true;

        [Category(_editor)]
        [DisplayName("Enable collapsing attributes")]
        [Description("It provides the possibility to collapse group of attributes into one collapsed line.")]
        [DefaultValue(true)]
        public bool CollapseMemberAttributes { get; set; } = true;

        [Category(_editor)]
        [DisplayName("Short form for attribute collapse")]
        [Description("Display short text (...) for the collapased form of attributes.")]
        [DefaultValue(true)]
        public bool CollapseMemberAttributesShortForm { get; set; } = true;

        [Category(_editor)]
        [DisplayName("Enable mouse wheel zoom")]
        [Description("Enable zooming in the editor when hitting Ctrl+MouseScroll")]
        [DefaultValue(false)]
        public bool EnableZoomOnScroll { get; set; }

        [Category(_editor)]
        [DisplayName("Copy empty selection")]
        [Description("Specifies if anything should be copied to the clipboard when no selection is made.")]
        [DefaultValue(true)]
        public bool CopyEmptySelection { get; set; } = true;

        [Category(_editor)]
        [DisplayName("Copy empty lines")]
        [Description("Specifies if the line should be copied to the clipboard when no selection is made. Requires re-opening of files to change.")]
        [DefaultValue(false)]
        public bool CopyEmptyLines { get; set; }

        [Category(_editor)]
        [DisplayName("Format on move line")]
        [Description("Calls 'Format Selection' when a line is moved up or down using Alt+UP/DOWN shortcuts.")]
        [DefaultValue(true)]
        public bool FormatOnMoveLine { get; set; } = true;

        [Category(_editor)]
        [DisplayName("Default zoom level")]
        [Description("The zoom level to apply when executing the Reset Zoom command using 'Ctrl+0'")]
        [DefaultValue(100)]
        public int DefaultZoomLevel { get; set; } = 100;

        [Category(_editor)]
        [DisplayName("Copy without indentation")]
        [Description("Determines if leading indentation should be removed when copying the selection.")]
        [DefaultValue(true)]
        public bool CopyWithoutIndentation { get; set; } = true;
    }
}
