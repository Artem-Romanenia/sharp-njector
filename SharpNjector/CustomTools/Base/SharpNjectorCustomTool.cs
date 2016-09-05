using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SharpNjector.CustomTools.Base
{
    public abstract class SharpNjectorCustomTool : IVsSingleFileGenerator
    {
        private const string Prefix = "#nject";
        private const string Code = @"
using System;

namespace SharpNjector
{{
    public class Njector
    {{
        public object Eval()
        {{
            return {0};
        }}
    }}
}}";

        public abstract string GetExtension();

        public int DefaultExtension(out string defaultExtension)
        {
            defaultExtension = GetExtension();
            return VSConstants.S_OK;
        }

        public int Generate(string inputFilePath, string inputFileContents, string defaultNamespace, IntPtr[] outputFileContents, out uint outputSize, IVsGeneratorProgress generateProgress)
        {
            var output = new StringBuilder();

            var injections = new List<object>();

            for (var i = 0; i < inputFileContents.Length; i++)
            {
                if (inputFileContents.Length >= i + Prefix.Length && inputFileContents.Substring(i, Prefix.Length) == Prefix)
                {
                    i += Prefix.Length;
                    var expressionString = new StringBuilder();

                    int depth = 0;
                    do
                    {
                        if (inputFileContents[i] == '(')
                            depth++;
                        else if (inputFileContents[i] == ')')
                            depth--;

                        expressionString.Append(inputFileContents[i]);

                        if (depth > 0)
                            i++;
                    } while (depth > 0 && i < inputFileContents.Length);

                    injections.Add(Eval(expressionString.ToString()));

                    output.AppendFormat("{{{0}}}", injections.Count - 1);
                }
                else
                {
                    output.Append(inputFileContents[i]);
                }
            }

            var outputBytes = Encoding.UTF8.GetBytes(string.Format(output.ToString(), injections.ToArray()));
            if (outputBytes.Length > 0)
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

            return VSConstants.S_OK;
        }

        private object Eval(string expression)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(string.Format(Code, expression));

            var assemblyName = Path.GetRandomFileName();
            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof (object).Assembly.Location),
            };

            var compilation = CSharpCompilation.Create(
                assemblyName,
                new[] {syntaxTree},
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (result.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    var assembly = Assembly.Load(ms.ToArray());

                    var type = assembly.GetType("SharpNjector.Njector");
                    var evaluator = Activator.CreateInstance(type);
                    return type.InvokeMember("Eval", BindingFlags.Default | BindingFlags.InvokeMethod, null, evaluator, null);
                }

                return null;
            }
        }
    }
}
