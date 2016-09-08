using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace SharpNjector.NjectClassifier
{
    [Export(typeof(IClassifierProvider))]
    [ContentType("NjectorJs")]
    internal class ClassifierProvider : IClassifierProvider
    {

#pragma warning disable 649

        [Import]
        private IClassificationTypeRegistryService _classificationRegistry;

#pragma warning restore 649

        #region IClassifierProvider

        /// <summary>
        /// Gets a classifier for the given text buffer.
        /// </summary>
        /// <param name="buffer">The <see cref="ITextBuffer"/> to classify.</param>
        /// <returns>A classifier for the text buffer, or null if the provider cannot do so in its current state.</returns>
        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => new Classifier(_classificationRegistry));
        }

        #endregion
    }
}
