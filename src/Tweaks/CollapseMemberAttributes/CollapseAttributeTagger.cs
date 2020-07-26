using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Tagging;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    public class CollapseAttributeTagger : ITagger<IStructureTag>
    {
        private readonly ITextBuffer _buffer;
        private readonly bool _isShortCollapsedForm;
        private readonly string _shortCollapsedForm = "...";
        private List<Span> _spans = new List<Span>();
        private bool _isParsing;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public CollapseAttributeTagger(ITextBuffer buffer, bool isShortCollapsedForm)
        {
            _buffer = buffer;
            _isShortCollapsedForm = isShortCollapsedForm;
            _buffer.ChangedHighPriority += OnBufferChanged;
            Parse(_buffer.CurrentSnapshot);
        }

        private void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            if (e.After == _buffer.CurrentSnapshot)
            {
                Parse(_buffer.CurrentSnapshot);
            }
        }

        public IEnumerable<ITagSpan<IStructureTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (SnapshotSpan span in spans)
            {
                IEnumerable<Span> outlines = _spans.Where(s => s.IntersectsWith(span));

                foreach (Span outline in outlines)
                {
                    var snapshotSpan = new SnapshotSpan(_buffer.CurrentSnapshot, outline);
                    StructureTag tag = CreateTag(outline);

                    yield return new TagSpan<IStructureTag>(snapshotSpan, tag);
                }
            }
        }

        private StructureTag CreateTag(Span outline)
        {
            ITextSnapshotLine line = _buffer.CurrentSnapshot.GetLineFromPosition(outline.Start);
            var firstLineText = line.GetText().Trim();

            return new StructureTag(
                _buffer.CurrentSnapshot,
                outliningSpan: outline,
                headerSpan: line.Extent,
                guideLineSpan: outline,
                guideLineHorizontalAnchor: outline.Start,
                type: PredefinedStructureTagTypes.Nonstructural,
                isCollapsible: true,
                collapsedForm: _isShortCollapsedForm ? _shortCollapsedForm : firstLineText,
                collapsedHintForm: firstLineText,
                isDefaultCollapsed: true
            );
        }

        private void Parse(ITextSnapshot snapshot)
        {
            if (_isParsing)
            {
                return;
            }

            _isParsing = true;

            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await Task.Delay(200);

                var list = new List<Span>();
                var start = 0;
                var end = 0;
                var numberOfLines = 0;

                try
                {
                    foreach (ITextSnapshotLine line in snapshot.Lines)
                    {
                        if (IsAttributeLine(line, out Span attributeSpan))
                        {
                            if (start == 0)
                            {
                                start = attributeSpan.Start;
                            }

                            end = attributeSpan.End;
                            numberOfLines++;
                        }
                        else
                        {
                            if (start > 0 && (numberOfLines > 1 || (numberOfLines > 0 && _isShortCollapsedForm)))
                            {
                                list.Add(Span.FromBounds(start, end));
                            }

                            numberOfLines = 0;
                            start = 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.Write(ex);
                }

                _spans = list;
                _isParsing = false;
                var args = new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, 0, snapshot.Length));

                TagsChanged?.Invoke(this, args);

            }).FileAndForget(nameof(CollapseAttributeTagger));
        }

        private static bool IsAttributeLine(ITextSnapshotLine line, out Span span)
        {
            var text = line.GetText();
            var clean = text.TrimStart();

            span = Span.FromBounds(line.Start + (text.Length - clean.Length), line.End);

            clean = clean.TrimEnd();

            return !string.IsNullOrEmpty(clean) && clean.StartsWith("[") && clean.EndsWith("]");
        }
    }
}
