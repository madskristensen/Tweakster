using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    internal sealed class DeleteOutputArtifacts : IOleCommandTarget
    {
        private readonly System.IServiceProvider _serviceProvider;
        private readonly IVsSolution _solution;
        private readonly DTE2 _dte;

        private DeleteOutputArtifacts(System.IServiceProvider serviceProvider, IVsSolution solution, DTE2 dte)
        {
            _serviceProvider = serviceProvider;
            _solution = solution;
            _dte = dte;
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Assumes.Present(commandService);

            var sln = await package.GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
            var dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;
            var pct = await package.GetServiceAsync(typeof(SVsRegisterPriorityCommandTarget)) as IVsRegisterPriorityCommandTarget;
            Assumes.Present(pct);

            var interceptor = new DeleteOutputArtifacts(package, sln, dte);
            pct.RegisterPriorityCommandTarget(0, interceptor, out _);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (pguidCmdGroup == typeof(VSConstants.VSStd97CmdID).GUID)
            {
                switch (nCmdID)
                {
                    case (uint)VSConstants.VSStd97CmdID.CleanSln:
                        if (InterceptCommand())
                        {
                            DeleteForSolution();
                        }

                        break;

                    case (uint)VSConstants.VSStd97CmdID.CleanSel:
                    case (uint)VSConstants.VSStd97CmdID.CleanCtx:
                        if (InterceptCommand())
                        {
                            DeleteForSelection();
                        }

                        break;
                }
            }

            return (int)Microsoft.VisualStudio.OLE.Interop.Constants.MSOCMDERR_E_FIRST;
        }

        private bool InterceptCommand()
        {
            return Options.Instance.DeleteOuputArtifactsOnClean &&
                   !VsShellUtilities.IsSolutionBuilding(_serviceProvider);
        }

        private void DeleteForSolution()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IEnumerable<string> projectFiles = GetProjectFiles(_solution);

            foreach (var projectFile in projectFiles)
            {
                DeleteArtifacts(projectFile);
            }
        }

        private void DeleteForSelection()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IEnumerable<Project> projects = from SelectedItem item in _dte.SelectedItems select item?.Project;

            foreach (Project project in projects)
            {
                DeleteArtifacts(project.GetRootFolder());
            }
        }

        private void DeleteArtifacts(string projectFile)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (string.IsNullOrEmpty(projectFile) || !Path.IsPathRooted(projectFile))
            {
                return;
            }

            string directory = null;

            if (File.Exists(projectFile))
            {
                directory = Path.GetDirectoryName(projectFile);
            }
            else if (Directory.Exists(projectFile))
            {
                directory = projectFile;
            }

            if (!string.IsNullOrEmpty(directory))
            {
                SafeDeleteDirectory(Path.Combine(directory, "bin"));
                SafeDeleteDirectory(Path.Combine(directory, "obj"));
            }
        }

        private void SafeDeleteDirectory(string path)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
        }

        private IEnumerable<string> GetProjectFiles(IVsSolution sln)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            sln.GetProjectFilesInSolution(0x1, 0, null, out var numberOfProjects);
            var projects = new string[numberOfProjects];
            sln.GetProjectFilesInSolution(0x1, numberOfProjects, projects, out _);

            return projects.Where(p => !string.IsNullOrEmpty(p) && File.Exists(p));
        }
    }
}
