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
        [Name("NjectorJs")]
        [BaseDefinition("JavaScript")]
        private static ContentTypeDefinition njectorContentTypeDefinition;

        #endregion

        #region File extension associations

        [Export]
        [FileExtension(".snjs")]
        [ContentType("NjectorJs")]
        private static FileExtensionToContentTypeDefinition njectorFileExtensionDefinition;

        #endregion

        #region Classifications

        [Export]
        [Name("NjectBlock")]
        [BaseDefinition("text")]
        private static ClassificationTypeDefinition njectBlockDefinition;

        [Export]
        [Name("NjectBlockContent")]
        [BaseDefinition("text")]
        private static ClassificationTypeDefinition njectBlockContentDefinition;

        [Export]
        [Name("NjectUsingBlock")]
        [BaseDefinition("text")]
        private static ClassificationTypeDefinition njectUsingBlockDefinition;

        [Export]
        [Name("NjectUsingBlockContent")]
        [BaseDefinition("text")]
        private static ClassificationTypeDefinition njectUsingBlockContentDefinition;

        #endregion

#pragma warning restore 169
    }
}
