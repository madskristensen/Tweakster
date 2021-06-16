using System;
using System.ComponentModel.Design;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    internal sealed class DuplicateWindow
    {
        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            IMenuCommandService commandService = await package.GetServiceAsync<IMenuCommandService, IMenuCommandService>();
            Assumes.Present(commandService);

            IVsUIShell uiShell = await package.GetServiceAsync<SVsUIShell, IVsUIShell>();

            var cmdId = new CommandID(PackageGuids.guidCommands, PackageIds.DuplicateWindow);
            var menuItem = new OleMenuCommand((s, e) => Execute(uiShell), cmdId);
            commandService.AddCommand(menuItem);
        }

        private static void Execute(IVsUIShell uiShell)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Guid guid = typeof(VSConstants.VSStd97CmdID).GUID;
            var id = (uint)VSConstants.VSStd97CmdID.NewWindow;

            uiShell.PostExecCommand(guid, id, 0, null);
        }
    }
}
