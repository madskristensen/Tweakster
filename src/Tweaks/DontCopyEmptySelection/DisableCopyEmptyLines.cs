using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Tweakster
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType(ContentTypes.Text)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal class DisableCopyEmptyLines : IWpfTextViewCreationListener
    {
        public void TextViewCreated(IWpfTextView textView)
        {
            var enabled = Options.Instance.CopyEmptyLines;
            textView.Options.SetOptionValue(DefaultTextViewOptions.CutOrCopyBlankLineIfNoSelectionId, enabled);
        }
    }
}