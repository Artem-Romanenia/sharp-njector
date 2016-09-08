using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace SharpNjector.NjectClassifier
{
    [Export(typeof(EditorFormatDefinition))]
    [Name("NjectBlockFormat")]
    [UserVisible(true)]
    public class NjectBlockFormatDefinition : MarkerFormatDefinition
    {
        public NjectBlockFormatDefinition()
        {
            DisplayName = "Nject Block";
            BackgroundColor = Colors.Blue;
            ForegroundColor = Colors.White;
        }
    }
}
