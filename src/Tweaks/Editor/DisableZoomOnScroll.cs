using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Tweakster
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType(ContentTypes.Text)]
    [TextViewRole(PredefinedTextViewRoles.Zoomable)]
    internal class ViewCreationListener : IWpfTextViewCreationListener
    {
        public void TextViewCreated(IWpfTextView textView)
        {
            var enabled = Options.Instance.EnableZoomOnScroll;
            textView.Options.SetOptionValue(DefaultWpfViewOptions.EnableMouseWheelZoomId, enabled);
        }
    }
}