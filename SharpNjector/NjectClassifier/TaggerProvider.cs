using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace SharpNjector.NjectClassifier
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("CSharp")]
    [TagType(typeof(IClassificationTag))]
    public class TaggerProvider : IViewTaggerProvider
    {
        [Import]
        private readonly IBufferTagAggregatorFactoryService _bufferTagAggregatorFactoryService;

        [Import]
        private readonly IViewTagAggregatorFactoryService _viewTagAggregatorFactoryService;

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            return new Tagger(_bufferTagAggregatorFactoryService.CreateTagAggregator<IClassificationTag>(buffer)) as ITagger<T>;
        }
    }
}
