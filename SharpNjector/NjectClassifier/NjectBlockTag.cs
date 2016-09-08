using Microsoft.VisualStudio.Text.Tagging;

namespace SharpNjector.NjectClassifier
{
    public class NjectBlockTag :  TextMarkerTag
    {
        public NjectBlockTag() : base("NjectBlockFormat")
        {
        }
    }
}
