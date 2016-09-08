using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace SharpNjector.NjectClassifier
{
    public class NjectBlockTagger : ITagger<IClassificationTag>
    {
        private readonly ITagAggregator<IClassificationTag> _tagAggregator;
        private readonly ITagAggregator<IClassificationTag> _jsTagAggregator;
        private readonly ITextBuffer _buffer;
        private readonly ITextBuffer _shadowBuffer;

        public NjectBlockTagger(ITagAggregator<IClassificationTag> tagAggregator, ITagAggregator<IClassificationTag> jsTagAggregator, ITextBuffer buffer, ITextBuffer shadowBuffer)
        {
            _tagAggregator = tagAggregator;
            _jsTagAggregator = jsTagAggregator;
            _buffer = buffer;
            _shadowBuffer = shadowBuffer;
        }

        public IEnumerable<ITagSpan<IClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var newSpans = new NormalizedSnapshotSpanCollection(spans.Select(s => new SnapshotSpan(_shadowBuffer.CurrentSnapshot, s.Start.Position, s.End.Position)));

            var njectTags = _tagAggregator.GetTags(spans);
            var jsTags = _jsTagAggregator.GetTags(newSpans);

            var tagsToReturn =
                jsTags.Select(
                    t =>
                        new TagSpan<IClassificationTag>(
                            new SnapshotSpan(_buffer.CurrentSnapshot,
                                t.Span.Start.GetPoint(_buffer.CurrentSnapshot, PositionAffinity.Predecessor)
                                    .GetValueOrDefault()
                                    .Position,
                                t.Span.End.GetPoint(_buffer.CurrentSnapshot, PositionAffinity.Predecessor)
                                    .GetValueOrDefault()
                                    .Position), t.Tag)).ToList();

            return tagsToReturn;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}
