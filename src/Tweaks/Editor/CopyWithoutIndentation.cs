using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;

namespace Tweakster.Tweaks.Editor
{
    [Export(typeof(ICommandHandler))]
    [Name(nameof(CopyWithoutIndentation))]
    [ContentType(ContentTypes.Text)]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    public class CopyWithoutIndentation : ICommandHandler<CopyCommandArgs>
    {
        public string DisplayName => nameof(CopyWithoutIndentation);

        public bool ExecuteCommand(CopyCommandArgs args, CommandExecutionContext executionContext)
        {
            ITextSelection selection = args.TextView.Selection;

            if (selection.SelectedSpans.Count > 1 || !Options.Instance.CopyWithoutIndentation)
            {
                return false;
            }

            if (args.TextView.TryGetTextViewLineContainingBufferPosition(selection.Start.Position, out var viewLine))
            {
                if (viewLine.Start.Position == selection.Start.Position)
                {
                    return false;
                }
            }

            var lines = args.SubjectBuffer.CurrentSnapshot.GetText(selection.SelectedSpans[0]).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            SnapshotPoint indentation = selection.Start.Position - viewLine.Start.Position;

            var sb = new StringBuilder();
            sb.AppendLine(lines[0]);

            foreach (var line in lines.Skip(1))
            {
                if (line.Length > indentation)
                {
                    var indentText = line.Substring(0, indentation);

                    if (!string.IsNullOrWhiteSpace(indentText))
                    {
                        return false;
                    }

                    sb.AppendLine(line.Substring(indentation));
                }
                else
                {
                    sb.AppendLine(line);
                }
            }

            Clipboard.SetText(sb.ToString());
            return true;
        }

        public CommandState GetCommandState(CopyCommandArgs args)
        {
            return CommandState.Unspecified;
        }
    }
}