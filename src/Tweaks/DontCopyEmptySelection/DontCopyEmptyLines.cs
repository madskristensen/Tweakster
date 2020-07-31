using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;

namespace Tweakster.Editor
{
    [Export(typeof(ICommandHandler))]
    [Name(nameof(DontCopyEmptyLines))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    public class DontCopyEmptyLines : ICommandHandler<CopyCommandArgs>
    {
        public string DisplayName => nameof(DontCopyEmptyLines);

        public bool ExecuteCommand(CopyCommandArgs args, CommandExecutionContext executionContext)
        {
            if (Options.Instance.CopyEmptyLines || !args.TextView.Selection.IsEmpty)
            {
                return false;
            }

            SnapshotPoint position = args.TextView.Selection.Start.Position;
            ITextSnapshotLine line = args.TextView.TextBuffer.CurrentSnapshot.GetLineFromPosition(position);
            var text = line.Extent.GetText();

            return string.IsNullOrWhiteSpace(text);
        }

        public CommandState GetCommandState(CopyCommandArgs args)
        {
            // Unspecified means we can return either true or false in ExecuteCommand above
            return CommandState.Unspecified;
        }
    }
}
