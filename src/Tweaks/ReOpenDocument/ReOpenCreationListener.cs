using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Tweakster
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal class ReOpenCreationListener : IWpfTextViewCreationListener
    {
        public static string LastClosed { get; private set; }

        [Import]
        internal ITextDocumentFactoryService _documentService = null;

        public void TextViewCreated(IWpfTextView textView)
        {
            if (_documentService.TryGetTextDocument(textView.TextBuffer, out ITextDocument doc))
            {
                textView.Properties.AddProperty("doc", doc.FilePath);
            }

            textView.Closed += TextView_Closed;
        }

        private void TextView_Closed(object sender, System.EventArgs e)
        {
            var textView = (IWpfTextView)sender;
            textView.Closed -= TextView_Closed;

            if (textView.Properties.TryGetProperty("doc", out string fileName))
            {
                LastClosed = fileName;
            }
        }
    }
}
