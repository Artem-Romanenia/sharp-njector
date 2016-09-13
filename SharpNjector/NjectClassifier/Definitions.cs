using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace SharpNjector.NjectClassifier
{
    internal static class Definitions
    {

#pragma warning disable 169 // This disables "The field is never used" compiler's warning. Justification: the field is used by MEF.

        #region Content Types

        [Export]
        [Name("NjectorJsHost")]
        [BaseDefinition("code")]
        [BaseDefinition("projection")]
        private static ContentTypeDefinition _njectorJsHostContentTypeDefinition;

        [Export]
        [Name("NjectorJs")]
        [BaseDefinition("code")]
        private static ContentTypeDefinition _njectorJsContentTypeDefinition;

        #endregion

        #region File extension associations

        [Export]
        [FileExtension(".snjs")]
        [ContentType("NjectorJsHost")]
        private static FileExtensionToContentTypeDefinition _njectorFileExtensionDefinition;

        #endregion

        #region Classifications

        [Export]
        [Name("NjectBlock")]
        [BaseDefinition("text")]
        private static ClassificationTypeDefinition _njectBlockDefinition;

        [Export]
        [Name("NjectBlockContent")]
        [BaseDefinition("text")]
        private static ClassificationTypeDefinition _njectBlockContentDefinition;

        [Export]
        [Name("NjectUsingBlock")]
        [BaseDefinition("text")]
        private static ClassificationTypeDefinition _njectUsingBlockDefinition;

        [Export]
        [Name("NjectUsingBlockContent")]
        [BaseDefinition("text")]
        private static ClassificationTypeDefinition _njectUsingBlockContentDefinition;

        #endregion

#pragma warning restore 169

    }
}
