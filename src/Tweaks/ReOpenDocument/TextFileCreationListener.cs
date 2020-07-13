using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Tweakster
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal class TextFileCreationListener : IWpfTextViewCreationListener
    {
        public static Dictionary<string, string> OpenFiles { get; } = new Dictionary<string, string>();

        [Import]
        internal ITextDocumentFactoryService _documentService = null;

        public void TextViewCreated(IWpfTextView textView)
        {
            if (_documentService.TryGetTextDocument(textView.TextBuffer, out ITextDocument doc))
            {
                var name = Path.GetFileName(doc.FilePath);
                OpenFiles[name] = doc.FilePath;

                textView.Properties.AddProperty("doc", name);
            }

            textView.Closed += TextView_Closed;
        }

        private void TextView_Closed(object sender, System.EventArgs e)
        {
            var textView = (IWpfTextView)sender;
            textView.Closed -= TextView_Closed;

            if (textView.Properties.TryGetProperty("doc", out string name) && OpenFiles.ContainsKey(name))
            {
                OpenFiles.Remove(name);
            }
        }
    }
}
