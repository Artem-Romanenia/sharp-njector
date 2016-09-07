using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.Shell;

namespace SharpNjector.CustomTools.Base
{
    public abstract class SharpNjectorCustomTool : IVsSingleFileGenerator
    {
        private const string NjectKeyWord = "#nject";
        private const string NjectUsingKeyWord = "#nject-using";
        private const string ClassFormat = @"
{0}

namespace SharpNjector
{{
    public class ExpressionEvaluator : System.MarshalByRefObject, SharpNjector.CustomTools.Base.IExpressionEvaluator
    {{
        private readonly System.Func<object>[] _expressions;

        public ExpressionEvaluator()
        {{
            _expressions = new System.Func<object>[]
            {{
                {1}
            }};
        }}

        public string this[int i]
        {{
            get
            {{
                return _expressions[i]().ToString();
            }}
        }}
    }}
}}";
        private const string PropertyFormat = @"() => {{ return {0}; }},";
        private const string UsingFormat = "using {0};";

        private EnvDTE.DTE _dte;

        public SharpNjectorCustomTool()
        {
            _dte = (EnvDTE.DTE)Package.GetGlobalService(typeof(EnvDTE.DTE));
        }

        public abstract string GetExtension();

        public int DefaultExtension(out string defaultExtension)
        {
            defaultExtension = GetExtension();
            return VSConstants.S_OK;
        }

        public int Generate(string inputFilePath, string inputFileContents, string defaultNamespace, IntPtr[] outputFileContents, out uint outputSize, IVsGeneratorProgress generateProgress)
        {
            var assemblyPaths = GetAssemblyPaths();

            using (var domainWrapper = new DomainWrapper())
            {
                LoadAssembliesToDomain(domainWrapper, assemblyPaths);

                var metadataReferences = GetReferences(assemblyPaths);

                string outputFormat;
                IList<string> injectionExpressions;
                IList<string> usings;
                Parse(inputFileContents, out outputFormat, out injectionExpressions, out usings);

                var evaluatedExpressions = EvaluateExpressions(injectionExpressions, usings, metadataReferences, domainWrapper);

                var outputBytes = Encoding.UTF8.GetBytes(string.Format(outputFormat, evaluatedExpressions.ToArray()));
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
            }

            return VSConstants.S_OK;
        }

        private IList<string> GetAssemblyPaths()
        {
            var assemblyPaths = new List<string>();

            using (var projectCollection = new ProjectCollection())
            {
                var project = _dte.ActiveDocument.ProjectItem.ContainingProject;
                var projectBuildInfo = new Project(
                    project.FileName,
                    null,
                    null,
                    projectCollection,
                    ProjectLoadSettings.Default).Properties;

                var path = Path.Combine(projectBuildInfo.First(p => p.Name == "MSBuildProjectDirectory").EvaluatedValue,
                    projectBuildInfo.First(p => p.Name == "OutputPath").EvaluatedValue,
                    projectBuildInfo.First(p => p.Name == "TargetFileName").EvaluatedValue);

                assemblyPaths.Add(path);

                foreach (VSLangProj.Reference reference in (project.Object as VSLangProj.VSProject).References)
                {
                    if (reference.SourceProject == null && !assemblyPaths.Contains(reference.Path))
                    {
                        assemblyPaths.Add(reference.Path);
                    }
                    else
                    {
                        var innerProjectBuildInfo = new Project(
                            reference.SourceProject.FileName,
                            null,
                            null,
                            projectCollection,
                            ProjectLoadSettings.Default).Properties;

                        var innerPath = Path.Combine(innerProjectBuildInfo.First(p => p.Name == "MSBuildProjectDirectory").EvaluatedValue,
                            innerProjectBuildInfo.First(p => p.Name == "OutputPath").EvaluatedValue,
                            innerProjectBuildInfo.First(p => p.Name == "TargetFileName").EvaluatedValue);

                        assemblyPaths.Add(innerPath);
                    }
                }
            }

            return assemblyPaths;
        }

        private void LoadAssembliesToDomain(DomainWrapper domainWrapper, IList<string> assemblyPaths)
        {
            foreach (var assemblyPath in assemblyPaths)
                domainWrapper.LoadAssembly(assemblyPath);
        }

        private IList<MetadataReference> GetReferences(IList<string> assemblyPaths)
        {
            var references = assemblyPaths.Select(a => (MetadataReference)MetadataReference.CreateFromFile(a)).ToList();

            references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(IExpressionEvaluator).Assembly.Location));

            return references;
        }

        private void Parse(string input, out string outputFormat, out IList<string> injectionExpressions, out IList<string> usings)
        {
            injectionExpressions = new List<string>();
            usings = new List<string>();

            var output = new StringBuilder();

            for (var i = 0; i < input.Length; i++)
            {
                if (input.Length >= i + NjectUsingKeyWord.Length && input.Substring(i, NjectUsingKeyWord.Length) == NjectUsingKeyWord)
                {
                    i += NjectUsingKeyWord.Length;

                    var usingStr = new StringBuilder();

                    var depth = 0;
                    do
                    {
                        if (input[i] == '(')
                            depth++;
                        else if (input[i] == ')')
                            depth--;
                        else
                            usingStr.Append(input[i]);

                        if (depth > 0)
                            i++;
                    } while (depth > 0 && i < input.Length);

                    if (input.Substring(i + 1, Environment.NewLine.Length) == Environment.NewLine)
                        i += Environment.NewLine.Length;

                    usings.Add(usingStr.ToString());
                }
                else if (input.Length >= i + NjectKeyWord.Length && input.Substring(i, NjectKeyWord.Length) == NjectKeyWord)
                {
                    i += NjectKeyWord.Length;
                    var expression = new StringBuilder();

                    int depth = 0;
                    do
                    {
                        if (input[i] == '(')
                            depth++;
                        else if (input[i] == ')')
                            depth--;

                        expression.Append(input[i]);

                        if (depth > 0)
                            i++;
                    } while (depth > 0 && i < input.Length);

                    injectionExpressions.Add(expression.ToString());
                    output.AppendFormat("{{{0}}}", injectionExpressions.Count - 1);
                }
                else
                {
                    if (input[i] == '{')
                        output.Append("{{");
                    else if (input[i] == '}')
                        output.Append("}}");
                    else
                        output.Append(input[i]);
                }
            }

            outputFormat = output.ToString();
        }

        private IList<string> EvaluateExpressions(IList<string> expressions, IList<string> usings, IList<MetadataReference> references, DomainWrapper domainWrapper)
        {
            var propertiesFormat = new StringBuilder();

            foreach (var expression in expressions)
                propertiesFormat.AppendFormat(PropertyFormat, expression);

            var usingsString = new StringBuilder();

            foreach (var usingStr in usings)
                usingsString.AppendFormat(UsingFormat, usingStr).AppendLine();

            var syntaxTree = CSharpSyntaxTree.ParseText(string.Format(ClassFormat, usingsString, propertiesFormat));

            var assemblyName = Path.GetRandomFileName();

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

                    var assemblyTypeStr = domainWrapper.LoadAssembly(ms.ToArray());
                    var evaluator = domainWrapper.CreateObject<IExpressionEvaluator>(assemblyTypeStr, "SharpNjector.ExpressionEvaluator");

                    var evaluatedExpressions = new List<string>();

                    for (var i = 0; i < expressions.Count; i++)
                    {
                        var evaluatedExpression = evaluator[i];
                        evaluatedExpressions.Add(evaluatedExpression);
                    }

                    return evaluatedExpressions;
                }

                return null;
            }
        }
    }
}
