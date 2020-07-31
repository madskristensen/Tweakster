using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;

namespace Tweakster.Editor
{
    [Export(typeof(ICommandHandler))]
    [Name(nameof(DontCopyEmptySelection))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    public class DontCopyEmptySelection : ICommandHandler<CopyCommandArgs>
    {
        public string DisplayName => nameof(DontCopyEmptySelection);

        public bool ExecuteCommand(CopyCommandArgs args, CommandExecutionContext executionContext)
        {
            return !Options.Instance.CopyEmptySelection && args.TextView.Selection.IsEmpty;
        }

        public CommandState GetCommandState(CopyCommandArgs args)
        {
            // Unspecified means we can return either true or false in ExecuteCommand above
            return CommandState.Unspecified;
        }
    }
}
