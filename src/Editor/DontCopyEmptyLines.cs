using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Commanding;
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
            ITextView view = args.TextView;

            if (!view.Selection.IsEmpty || !view.Caret.ContainingTextViewLine.Extent.IsEmpty)
            {
                // Pass it on to the next command handler
                return false;
            }

            // We handled it. Don't pass it on to the next command handler
            return true;
        }

        public CommandState GetCommandState(CopyCommandArgs args)
        {
            // Unspecified means we can return either true or false in ExecuteCommand above
            return CommandState.Unspecified;
        }
    }
}
