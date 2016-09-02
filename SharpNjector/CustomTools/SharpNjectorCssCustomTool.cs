using Microsoft.VisualStudio.Shell;
using SharpNjector.CustomTools.Base;
using System;
using System.Runtime.InteropServices;
using VSLangProj80;

namespace SharpNjector.CustomTools
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("dc3b6976-85b8-4f5e-83f6-a8b70a3f39cb")]
    [CodeGeneratorRegistration(typeof(SharpNjectorCssCustomTool), "Generates css file", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true, GeneratorRegKeyName = "SharpNjectorCssCustomTool")]
    [ProvideObject(typeof(SharpNjectorCssCustomTool))]
    public class SharpNjectorCssCustomTool : SharpNjectorCustomTool
    {
        public override string GetExtension()
        {
            return ".css";
        }
    }
}
