using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace SharpNjector.NjectClassifier
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("NjectorJs")]
    [TagType(typeof(IClassificationTag))]
    public class NjectBlockTaggerProvider : IViewTaggerProvider
    {
        private bool _initialized;

#pragma warning disable 649

        [Import]
        private IContentTypeRegistryService _contentTypeRegistryService;

        [Import]
        private ITextBufferFactoryService _textBufferFactoryService;

        [Import]
        private IBufferTagAggregatorFactoryService _bufferTagAggregatorFactoryService;

#pragma warning restore 649

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (!_initialized)
            {
                _initialized = true;

                var shadowBuffer = _textBufferFactoryService.CreateTextBuffer(buffer.CurrentSnapshot.GetText(), _contentTypeRegistryService.GetContentType("JavaScript"));
                var jsTagAggregator = _bufferTagAggregatorFactoryService.CreateTagAggregator<IClassificationTag>(shadowBuffer);
                var tagAggregator = _bufferTagAggregatorFactoryService.CreateTagAggregator<IClassificationTag>(buffer);

                return buffer.Properties.GetOrCreateSingletonProperty(() => new NjectBlockTagger(tagAggregator, jsTagAggregator, buffer, shadowBuffer) as ITagger<T>);
            }

            return null;
        }
    }
}
