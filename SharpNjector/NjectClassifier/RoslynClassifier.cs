//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.CodeAnalysis.Classification;
//using Microsoft.CodeAnalysis.Text;
//using Microsoft.VisualStudio.Text;
//using Microsoft.VisualStudio.Text.Classification;

//namespace SharpNjector.NjectClassifier
//{
//    public class RoslynClassifier : IClassifier
//    {
//        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
//        {
//            var container = span.Snapshot.TextBuffer.AsTextContainer();

//            IList<ClassificationSpan> u = Microsoft.CodeAnalysis.Classification.Classifier.GetClassifiedSpans()
//            return u;
//        }

//        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
//    }
//}
