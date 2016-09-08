using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using SharpNjector.Properties;

namespace SharpNjector.NjectClassifier
{
    internal class Classifier : IClassifier
    {
        private readonly IClassificationTypeRegistryService _registry;

        internal Classifier(IClassificationTypeRegistryService registry)
        {
            _registry = registry;
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var njectUsingKeywordLength = Resources.NjectUsingKeyWord.Length;
            var njectKeywordLength = Resources.NjectKeyWord.Length;

            var result = new List<ClassificationSpan>();

            var spanText = span.Snapshot.GetText();
            var njectBlockClassificationType = _registry.GetClassificationType("NjectBlock");

            for (var i = span.Start.Position; i < span.End.Position; i++)
            {
                if (span.End.Position >= i + njectUsingKeywordLength && spanText.Substring(i, njectUsingKeywordLength) == Resources.NjectUsingKeyWord)
                {
                    result.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(i, njectUsingKeywordLength)), njectBlockClassificationType));

                    //i += njectUsingKeywordLength;

                    //do
                    //{
                    //    if (i >= span.End.Position)
                    //        break;

                    //    if (spanText[i] == '(')
                    //} while (true);
                }
                else if (span.End.Position >= i + njectKeywordLength && spanText.Substring(i, njectKeywordLength) == Resources.NjectKeyWord)
                {
                    result.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(i, njectKeywordLength)), njectBlockClassificationType));
                }
            }

            return result;
        }
    }
}
