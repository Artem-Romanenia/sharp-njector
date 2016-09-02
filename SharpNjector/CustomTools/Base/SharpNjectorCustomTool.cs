using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpNjector.CustomTools.Base
{
    public abstract class SharpNjectorCustomTool : IVsSingleFileGenerator
    {
        public abstract string GetExtension();

        public int DefaultExtension(out string defaultExtension)
        {
            defaultExtension = GetExtension();
            return VSConstants.S_OK;
        }

        public int Generate(string inputFilePath, string inputFileContents, string defaultNamespace, IntPtr[] outputFileContents, out uint outputSize, IVsGeneratorProgress generateProgress)
        {
            byte[] outputBytes = Encoding.UTF8.GetBytes(inputFileContents);
            if (outputBytes != null)
            {
                outputSize = (uint)outputBytes.Length;
                outputFileContents[0] = Marshal.AllocCoTaskMem(outputBytes.Length);
                Marshal.Copy(outputBytes, 0, outputFileContents[0], outputBytes.Length);
            }
            else
            {
                outputFileContents[0] = IntPtr.Zero;
                outputSize = 0;
            }
            return 0;
        }
    }
}
