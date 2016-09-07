using Microsoft.VisualStudio.Shell;
using SharpNjector.CustomTools.Base;
using System.Runtime.InteropServices;
using VSLangProj80;

namespace SharpNjector.CustomTools
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("c6f910ea-4b0d-41d4-94c2-eefafa80574e")]
    [CodeGeneratorRegistration(typeof(SharpNjectorJsCustomTool), "Evaluates injected expressions in SharpNjector js file.", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true, GeneratorRegKeyName = "SharpNjectorJsCustomTool")]
    [ProvideObject(typeof(SharpNjectorJsCustomTool))]
    public class SharpNjectorJsCustomTool : SharpNjectorCustomTool
    {
        public override string GetExtension()
        {
            return ".js";
        }
    }
}
