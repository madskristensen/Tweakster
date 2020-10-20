using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Text.Formatting;
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

        [Import]
        private IRtfBuilderService _rtfService { get; set; }

        public bool ExecuteCommand(CopyCommandArgs args, CommandExecutionContext executionContext)
        {
            ITextSelection selection = args.TextView.Selection;

            if (selection.SelectedSpans.Count != 1 // Only handle single selections
                || selection.Start.Position == selection.End.Position // Don't handle zero-width selections
                || !Options.Instance.CopyWithoutIndentation)
            {
                return false;
            }

            ITextSnapshot snapshot = args.TextView.TextBuffer.CurrentSnapshot;

            // Only handle selections that starts with indented
            if (args.TextView.TryGetTextViewLineContainingBufferPosition(selection.Start.Position, out ITextViewLine viewLine))
            {
                if (viewLine.Start.Position == selection.Start.Position)
                {
                    return false;
                }
            }

            IEnumerable<ITextViewLine> lines = from line in args.TextView.TextViewLines
                                               where line.IntersectsBufferSpan(selection.SelectedSpans[0])
                                               select line;

            // Only handle when multiple lines are selected
            if (lines.Count() == 1)
            {
                return false;
            }

            SnapshotPoint indentation = selection.Start.Position - viewLine.Start.Position;
            var spans = new List<SnapshotSpan>();
            var sb = new StringBuilder();

            foreach (ITextViewLine line in lines)
            {
                if (line.Extent.IsEmpty)
                {
                    spans.Add(line.Extent);
                    sb.AppendLine();
                }
                else
                {
                    var end = line.Length - indentation;
                    if (selection.End.Position.Position < line.End.Position)
                    {
                        end -= (line.End.Position - selection.End.Position.Position);
                    }

                    var span = new SnapshotSpan(snapshot, line.Start + indentation, end);
                    spans.Add(span);
                    sb.AppendLine(span.GetText());
                }
            }

            var rtf = _rtfService.GenerateRtf(new NormalizedSnapshotSpanCollection(spans), args.TextView);

            var data = new DataObject();
            data.SetText(rtf.TrimEnd(), TextDataFormat.Rtf);
            data.SetText(sb.ToString().TrimEnd(), TextDataFormat.Text);
            Clipboard.SetDataObject(data, false);

            return true;
        }

        public CommandState GetCommandState(CopyCommandArgs args)
        {
            return CommandState.Unspecified;
        }
    }
}