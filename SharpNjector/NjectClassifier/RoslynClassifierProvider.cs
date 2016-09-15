//using System.ComponentModel.Composition;
//using Microsoft.VisualStudio.Text;
//using Microsoft.VisualStudio.Text.Classification;
//using Microsoft.VisualStudio.Utilities;

//namespace SharpNjector.NjectClassifier
//{
//    [Export(typeof(IClassifierProvider))]
//    [ContentType("CSharp")]
//    public class RoslynClassifierProvider : IClassifierProvider
//    {
//        public IClassifier GetClassifier(ITextBuffer textBuffer)
//        {
//            return textBuffer.Properties.GetOrCreateSingletonProperty(() => new RoslynClassifier());
//        }
//    }
//}
