using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.LanguageServices;
using System.ComponentModel.Composition;
using System.Linq;

namespace SharpNjector
{
    [Export]
    internal class ShadowWorkspace
    {
        private readonly VisualStudioWorkspace _visualStudioWorkspace;

        private readonly AdhocWorkspace _shadowWorkspace;

        [ImportingConstructor]
        private ShadowWorkspace(VisualStudioWorkspace visualStudioWorkspace)
        {
            _visualStudioWorkspace = visualStudioWorkspace;
            _shadowWorkspace = new AdhocWorkspace();

            var projInfo = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Default, "Default", "Default", "C#");
            var proj = _shadowWorkspace.AddProject(projInfo);

            var solution = _shadowWorkspace.CurrentSolution;
            foreach (var vsProject in _visualStudioWorkspace.CurrentSolution.Projects)
            {
                solution = solution.AddProjectReference(proj.Id, new ProjectReference(vsProject.Id));
            }

            var i = 0;
        }
    }
}
