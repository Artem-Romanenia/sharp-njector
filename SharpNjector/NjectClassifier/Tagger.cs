using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;

namespace SharpNjector.NjectClassifier
{
    public class Tagger : ITagger<IClassificationTag>
    {
        private readonly ITagAggregator<IClassificationTag> _buffer;
        private readonly ITagAggregator<IClassificationTag> _view;

        public Tagger(ITagAggregator<IClassificationTag> buffer)
        {
            _buffer = buffer;
            //_view = view;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<IClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var u1 = _buffer.GetTags(spans);
            //var u2 = _view.GetTags(spans);

            yield break;
        }
    }
}
