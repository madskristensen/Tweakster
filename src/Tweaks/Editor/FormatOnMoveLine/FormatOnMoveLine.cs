using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Utilities;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    [Export(typeof(ICommandHandler))]
    [Name(nameof(FormatOnMoveLine))]
    [ContentType(ContentTypes.CSharp)]
    [ContentType(ContentTypes.VisualBasic)]
    [ContentType(ContentTypes.HTML)]
    // C++:  Haven't tried it. Help wanted to test
    // CSS:  Doesn't work well
    // JSON: Doesn't support Format Selection command
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    public class FormatOnMoveLine : ICommandHandler<MoveSelectedLinesUpCommandArgs>, ICommandHandler<MoveSelectedLinesDownCommandArgs>
    {
        private bool _formatQueued;

        [Import]
        private readonly IEditorCommandHandlerServiceFactory _commandService = default;

        public string DisplayName => nameof(FormatOnMoveLine);

        public CommandState GetCommandState(MoveSelectedLinesUpCommandArgs args)
        {
            return CommandState.Available;
        }

        public bool ExecuteCommand(MoveSelectedLinesUpCommandArgs args, CommandExecutionContext executionContext)
        {
            FormatSelection(args.TextView, args.SubjectBuffer);

            return false;
        }

        public CommandState GetCommandState(MoveSelectedLinesDownCommandArgs args)
        {
            return CommandState.Available;
        }

        public bool ExecuteCommand(MoveSelectedLinesDownCommandArgs args, CommandExecutionContext executionContext)
        {
            FormatSelection(args.TextView, args.SubjectBuffer);

            return false;
        }

        private void FormatSelection(ITextView textView, ITextBuffer buffer)
        {
            if (!Options.Instance.FormatOnMoveLine)
            {
                return;
            }

            MoveCaretIfNeeded(textView);

            if (!_formatQueued)
            {
                _formatQueued = true;

                ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
                {
                    await Task.Delay(1);

                    IEditorCommandHandlerService service = _commandService.GetService(textView);
                    var cmd = new FormatSelectionCommandArgs(textView, buffer);
                    service.Execute((v, b) => cmd, null);

                    _formatQueued = false;

                }).FileAndForget(nameof(FormatOnMoveLine));
            }
        }

        private static void MoveCaretIfNeeded(ITextView textView)
        {
            if (textView.Selection.IsEmpty)
            {
                if (textView.TryGetTextViewLineContainingBufferPosition(textView.Selection.Start.Position, out ITextViewLine line))
                {
                    var text = line.Extent.GetText();
                    var length = text.Length - text.TrimStart().Length;

                    textView.Caret.MoveTo(line.Extent.Start + length);
                }
            }
        }
    }
}
