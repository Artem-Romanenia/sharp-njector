using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private const string Prefix = "#nject";
        private const string ClassFormat = @"
using System;
using SharpNjector.CustomTools.Base;

namespace SharpNjector
{{
    public class ExpressionEvaluator : MarshalByRefObject, IExpressionEvaluator
    {{
        private readonly Func<object>[] _expressions;

        public ExpressionEvaluator()
        {{
            _expressions = new Func<object>[]
            {{
                {0}
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
            var ads = new AppDomainSetup();
            ads.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            ads.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            var assemblyPaths = GetAssemblyPaths();

            using (var domainWrapper = new DomainWrapper())
            {
                LoadAssembliesToDomain(domainWrapper, assemblyPaths);

                var metadataReferences = GetReferences(assemblyPaths);

                string outputFormat;
                IList<string> injectionExpressions;
                Parse(inputFileContents, out outputFormat, out injectionExpressions);

                var evaluatedExpressions = EvaluateExpressions(injectionExpressions, metadataReferences, domainWrapper);

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

        private void Parse(string input, out string outputFormat, out IList<string> injectionExpressions)
        {
            injectionExpressions = new List<string>();
            var output = new StringBuilder();


            for (var i = 0; i < input.Length; i++)
            {
                if (input.Length >= i + Prefix.Length && input.Substring(i, Prefix.Length) == Prefix)
                {
                    i += Prefix.Length;
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

        private IList<string> EvaluateExpressions(IList<string> expressions, IList<MetadataReference> references, DomainWrapper domainWrapper)
        {
            var propertiesFormat = new StringBuilder();

            foreach (var expression in expressions)
                propertiesFormat.AppendFormat(PropertyFormat, expression);

            var syntaxTree = CSharpSyntaxTree.ParseText(string.Format(ClassFormat, propertiesFormat.ToString()));

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
