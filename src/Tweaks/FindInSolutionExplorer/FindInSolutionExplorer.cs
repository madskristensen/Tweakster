using System;
using System.ComponentModel.Design;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    internal sealed class FindInSolutionExplorer
    {
        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync<IMenuCommandService, OleMenuCommandService>();
            Assumes.Present(commandService);

            IVsUIShell uiShell = await package.GetServiceAsync<SVsUIShell, IVsUIShell>();

            var cmdId = new CommandID(PackageGuids.guidCommands, PackageIds.FindInSolutionExplorer);
            var menuItem = new OleMenuCommand((s, e) => Execute(uiShell), cmdId)
            {
                Supported = false
            };
            commandService.AddCommand(menuItem);
        }

        private static void Execute(IVsUIShell uiShell)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Guid guid = typeof(VSConstants.VSStd11CmdID).GUID;
            var id = (uint)VSConstants.VSStd11CmdID.SolutionExplorerSyncWithActiveDocument;

            uiShell.PostExecCommand(guid, id, 0, null);
        }
    }
}
