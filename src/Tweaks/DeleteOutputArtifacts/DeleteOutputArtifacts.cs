using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    internal sealed class DeleteOutputArtifacts
    {
        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Assumes.Present(commandService);

            var sln = await package.GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
            Assumes.Present(sln);

            var outWindow = await package.GetServiceAsync(typeof(SVsOutputWindow)) as IVsOutputWindow;
            Assumes.Present(outWindow);

            outWindow.GetPane(VSConstants.GUID_BuildOutputWindowPane, out IVsOutputWindowPane buildPane);

            var cmdId = new CommandID(PackageGuids.guidCommands, PackageIds.DeleteOutputArtifacts);
            var menuItem = new OleMenuCommand((s, e) => Execute(sln, buildPane), cmdId);
            menuItem.BeforeQueryStatus += (s, e) =>
            {
                menuItem.Enabled = !VsShellUtilities.IsSolutionBuilding(package);
            };
            commandService.AddCommand(menuItem);
        }

        private static void Execute(IVsSolution sln, IVsOutputWindowPane buildPane)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var item = ProjectHelpers.GetSelectedItem();
            buildPane.Clear();
            buildPane.Activate();

            if (item is Project project)
            {
                WriteToOutput($"Start deleting 2 directories...", buildPane);
                DeleteArtifacts(project.GetFullPath(), buildPane);
                WriteToOutput($"Done", buildPane);
            }
            else if (item == null) // It's a solution
            {
                IEnumerable<string> projectFiles = GetProjectFiles(sln);

                WriteToOutput($"Start deleting {projectFiles.Count() * 2} directories...", buildPane);

                foreach (var projectFile in projectFiles)
                {
                    DeleteArtifacts(projectFile, buildPane);
                }

                WriteToOutput($"Done", buildPane);

            }
        }

        private static void DeleteArtifacts(string projectFile, IVsOutputWindowPane buildPane)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
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
                SafeDeleteDirectory(Path.Combine(directory, "bin"), buildPane);
                SafeDeleteDirectory(Path.Combine(directory, "obj"), buildPane);
            }
        }

        private static void SafeDeleteDirectory(string path, IVsOutputWindowPane buildPane)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                WriteToOutput($"  Deleting {path}", buildPane);

                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch (Exception)
            {
                WriteToOutput($"Failed to delete {path}", buildPane);
            }
        }

        private static IEnumerable<string> GetProjectFiles(IVsSolution sln)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            sln.GetProjectFilesInSolution(0x1, 0, null, out var numberOfProjects);
            var projects = new string[numberOfProjects];
            sln.GetProjectFilesInSolution(0x1, numberOfProjects, projects, out _);

            return projects.Where(p => !string.IsNullOrEmpty(p) && File.Exists(p));
        }

        public static void WriteToOutput(string message, IVsOutputWindowPane buildPane)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            ErrorHandler.ThrowOnFailure(buildPane.OutputStringThreadSafe(message + Environment.NewLine));

        }
    }
}
