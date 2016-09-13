using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace SharpNjector.NjectClassifier
{
    [Export(typeof(IClassifierProvider))]
    [ContentType("NjectorJsHost")]
    internal class ClassifierProvider : IClassifierProvider
    {

#pragma warning disable 649

        [Import]
        private IClassificationTypeRegistryService _classificationRegistry;

#pragma warning restore 649

        #region IClassifierProvider

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => new Classifier(_classificationRegistry));
        }

        #endregion
    }
}
