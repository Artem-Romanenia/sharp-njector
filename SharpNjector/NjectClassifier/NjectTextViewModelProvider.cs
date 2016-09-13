using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Projection;
using Microsoft.VisualStudio.Utilities;

namespace SharpNjector.NjectClassifier
{
    [Export(typeof(ITextViewModelProvider))]
    [ContentType("NjectorJsHost")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    public class NjectTextViewModelProvider : ITextViewModelProvider
    {
        [Import]
        private IContentTypeRegistryService _contentTypeRegistryService;

        [Import]
        private IProjectionBufferFactoryService _projectionBufferFactoryService;

        public ITextViewModel CreateTextViewModel(ITextDataModel dataModel, ITextViewRoleSet roles)
        {
            var dataSnapshot = dataModel.DataBuffer.CurrentSnapshot;

            var text = dataSnapshot.GetText();

            var jsProjectionBuffer = _projectionBufferFactoryService.CreateProjectionBuffer(
                null,
                new List<object>(), 
                ProjectionBufferOptions.None,
                _contentTypeRegistryService.GetContentType("JavaScript"));

            var projectionBuffer = _projectionBufferFactoryService.CreateProjectionBuffer(
                null,
                new List<object>(), 
                ProjectionBufferOptions.None,
                _contentTypeRegistryService.GetContentType("NjectorJsHost"));

            var jsSpanNo = 0;
            var spanNo = 0;
            var accumulatedLength = 0;

            Parser.Parse(text, (blockType, start, length) =>
            {
                if (length == 0)
                    return;

                if (blockType == Parser.BlockType.Text)
                {
                    jsProjectionBuffer.InsertSpan(jsSpanNo++, dataSnapshot.CreateTrackingSpan(start, length, SpanTrackingMode.EdgeExclusive));
                    projectionBuffer.InsertSpan(spanNo++, jsProjectionBuffer.CurrentSnapshot.CreateTrackingSpan(accumulatedLength, length, SpanTrackingMode.EdgeExclusive));
                    accumulatedLength += length;
                }
                else
                {
                    projectionBuffer.InsertSpan(spanNo++, dataSnapshot.CreateTrackingSpan(start, length, SpanTrackingMode.EdgeExclusive));
                }
            });

            return new ProjectionTextViewModel(dataModel, projectionBuffer);
        }
    }
}
