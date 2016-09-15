using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.CodeAnalysis;
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

        [Import]
        private ShadowWorkspaceWrapper _shadowWorkspace;

        public ITextViewModel CreateTextViewModel(ITextDataModel dataModel, ITextViewRoleSet roles)
        {
            var dataSnapshot = dataModel.DataBuffer.CurrentSnapshot;

            var jsProjectionBuffer = _projectionBufferFactoryService.CreateProjectionBuffer(
                null,
                new List<object>(), 
                ProjectionBufferOptions.None,
                _contentTypeRegistryService.GetContentType("JavaScript"));

            var njProjectionBuffer = _projectionBufferFactoryService.CreateProjectionBuffer(
                null,
                new List<object>(),
                ProjectionBufferOptions.None,
                _contentTypeRegistryService.GetContentType("NjectorJs"));

            var csProjectionBuffer = _projectionBufferFactoryService.CreateProjectionBuffer(
                null,
                new List<object>(),
                ProjectionBufferOptions.None,
                _contentTypeRegistryService.GetContentType("CSharp"));

            var projectionBuffer = _projectionBufferFactoryService.CreateProjectionBuffer(
                null,
                new List<object>(),
                ProjectionBufferOptions.None,
                _contentTypeRegistryService.GetContentType("NjectorJsHost"));

            var jsSpanNo = 0;
            var njSpanNo = 0;
            var csSpanNo = 0;
            var spanNo = 0;
            var jsAccumulatedLength = 0;
            var njAccumulatedLength = 0;
            var csAccumulatedLength = 0;

            Parser.Parse(dataSnapshot.GetText(), (blockType, start, length) =>
            {
                if (length == 0)
                    return;

                if (blockType == Parser.BlockType.Text)
                {
                    jsProjectionBuffer.InsertSpan(jsSpanNo++, dataSnapshot.CreateTrackingSpan(start, length, SpanTrackingMode.EdgeExclusive));
                    projectionBuffer.InsertSpan(spanNo++, jsProjectionBuffer.CurrentSnapshot.CreateTrackingSpan(jsAccumulatedLength, length, SpanTrackingMode.EdgeExclusive));
                    jsAccumulatedLength += length;
                }
                else if (blockType == Parser.BlockType.Code)
                {
                    csProjectionBuffer.InsertSpan(csSpanNo++, dataSnapshot.CreateTrackingSpan(start, length, SpanTrackingMode.EdgeExclusive));
                    projectionBuffer.InsertSpan(spanNo++, csProjectionBuffer.CurrentSnapshot.CreateTrackingSpan(csAccumulatedLength, length, SpanTrackingMode.EdgeExclusive));
                    csAccumulatedLength += length;
                }
                else
                {
                    njProjectionBuffer.InsertSpan(njSpanNo++, dataSnapshot.CreateTrackingSpan(start, length, SpanTrackingMode.EdgeExclusive));
                    projectionBuffer.InsertSpan(spanNo++, njProjectionBuffer.CurrentSnapshot.CreateTrackingSpan(njAccumulatedLength, length, SpanTrackingMode.EdgeExclusive));
                    njAccumulatedLength += length;
                }
            });

            _shadowWorkspace.AddDocument(csProjectionBuffer);

            return new ProjectionTextViewModel(dataModel, projectionBuffer);
        }
    }
}
