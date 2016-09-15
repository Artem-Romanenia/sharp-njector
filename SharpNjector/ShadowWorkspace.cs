using System;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.LanguageServices;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;

namespace SharpNjector
{
    [Export]
    internal class ShadowWorkspaceWrapper
    {
        static readonly Type ISolutionCrawlerRegistrationService = Type.GetType("Microsoft.CodeAnalysis.SolutionCrawler.ISolutionCrawlerRegistrationService, Microsoft.CodeAnalysis.Workspaces");

        private readonly AdhocWorkspace _shadowWorkspace;

        private ShadowWorkspaceWrapper()
        {
            _shadowWorkspace = new AdhocWorkspace();

            var svc = typeof(HostWorkspaceServices)
                .GetMethod("GetService")
                .MakeGenericMethod(ISolutionCrawlerRegistrationService)
                .Invoke(_shadowWorkspace.Services, null);

            ISolutionCrawlerRegistrationService.GetMethod("Register")
                .Invoke(svc, new[] { _shadowWorkspace });

            var projInfo = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Default, "Default", "Default", "C#");
            var solution = _shadowWorkspace.CurrentSolution.AddProject(projInfo);

            _shadowWorkspace.TryApplyChanges(solution);

            //var solution = _shadowWorkspace.CurrentSolution;
            //foreach (var vsProject in _visualStudioWorkspace.CurrentSolution.Projects)
            //{
            //    solution = solution.AddProjectReference(proj.Id, new ProjectReference(vsProject.Id));
            //}

            var i = 0;
        }

        public void AddDocument(ITextBuffer buffer)
        {
            var proj = _shadowWorkspace.CurrentSolution.Projects.First();

            var docInfo = DocumentInfo.Create(DocumentId.CreateNewId(proj.Id), "valera.cs", loader: TextLoader.From(buffer.AsTextContainer(), VersionStamp.Default));
            var solution = _shadowWorkspace.CurrentSolution.AddDocument(docInfo);

            _shadowWorkspace.TryApplyChanges(solution);

            _shadowWorkspace.OpenDocument(docInfo.Id);
            //_shadowWorkspace.Register(buffer);
        }
    }

    //[Export]
    //public class ShadowWorkspace : Workspace
    //{
    //    static readonly Type ISolutionCrawlerRegistrationService = Type.GetType("Microsoft.CodeAnalysis.SolutionCrawler.ISolutionCrawlerRegistrationService, Microsoft.CodeAnalysis.Workspaces");

    //    public ShadowWorkspace() : base(MefHostServices.DefaultHost, "ShadowWorkspace")
    //    {
    //        var svc = typeof(HostWorkspaceServices)
    //            .GetMethod("GetService")
    //            .MakeGenericMethod(ISolutionCrawlerRegistrationService)
    //            .Invoke(Services, null);

    //        ISolutionCrawlerRegistrationService.GetMethod("Register")
    //            .Invoke(svc, new [] {this});
    //    }

    //    public ShadowWorkspace(HostServices host, string workspaceKind) : base(host, workspaceKind)
    //    {
    //    }

    //    public override bool CanApplyChange(ApplyChangesKind feature)
    //    {
    //        return true;
    //    }

    //    public void Register(ITextBuffer buffer)
    //    {
    //        this.RegisterText(buffer.AsTextContainer());
    //    }
    //}
}
