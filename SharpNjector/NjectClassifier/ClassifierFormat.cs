using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace SharpNjector.NjectClassifier
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "NjectBlock")]
    [Name("NjectBlock")]
    [UserVisible(true)]
    [Order(After = Priority.Default)]
    internal sealed class ClassifierFormat : ClassificationFormatDefinition
    {
        public ClassifierFormat()
        {
            DisplayName = "Nject Block";
            BackgroundColor = Colors.Blue;
            ForegroundColor = Colors.White;
        }
    }
}
